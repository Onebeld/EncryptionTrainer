using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EncryptionTrainer.Biometry;
using EncryptionTrainer.General;
using EncryptionTrainer.Loaders;
using EncryptionTrainer.Loaders.ImageFile;
using EncryptionTrainer.Messages;
using EncryptionTrainer.Models;
using EncryptionTrainer.Pages;
using EncryptionTrainer.Windows;
using PleasantUI.Controls;
using PleasantUI.Core.Localization;
using PleasantUI.Core.Structures;
using PleasantUI.ToolKit;

namespace EncryptionTrainer;

public class MainViewModel : ObservableObject
{
    private readonly Stack<Control?> _previousPages = new();
    
    private bool _isForwardAnimation = true;
    private Control? _page;

    private readonly UserDatabase _userDatabase = new();

    public bool IsForwardAnimation
    {
        get => _isForwardAnimation;
        set => SetProperty(ref _isForwardAnimation, value);
    }
    
    public Control? Page
    {
        get => _page;
        set => SetProperty(ref _page, value);
    }

    public MainViewModel()
    {
        List<User> users = UsersLoader.Load();
        _userDatabase.Users.AddRange(users);
        
        Page = new HomePage();
        
        RegisterMessages();
    }

    public void OpenCreateUserPage()
    {
        ChangePage(new CreateUserPage());
    }

    public void OpenLoadUserPage()
    {
        ChangePage(new LoadUserPage());
    }

    public void OpenAboutPage()
    {
        ChangePage(new AboutPage());
    }

    public void OpenSettingsPage()
    {
        if (App.MainWindow.ModalWindows.Any())
            return;
        
        if (Page is not SettingsPage)
            ChangePage(new SettingsPage());
    }

    public void OpenUsersListWindow()
    {
        UsersListWindow window = new();
        window.Show(App.MainWindow);
    }

    public void SaveUsers()
    {
        UsersLoader.Save(_userDatabase.Users);
    }

    private void ChangePage(Control? page, bool forward = true, bool back = true)
    {
        if (back)
            _previousPages.Push(Page);
        
        IsForwardAnimation = forward;

        Page = page;
    }

    private void GoBack()
    {
        IsForwardAnimation = false;
        Page = _previousPages.Pop();
    }
    
    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<GoBackMessage>(this, (_, _) =>
        {
            GoBack();
        });
        
        WeakReferenceMessenger.Default.Register<User, string>(this, "AddUser", (_, user) =>
        {
            _userDatabase.Users.Add(user);
            GoBack();
            
            PleasantSnackbar.Show(App.MainWindow, Localizer.Instance["UserAdded"], icon: (Geometry)Application.Current.FindResource("AccountBoxRegular"), notificationType: NotificationType.Success);
        });
        
        WeakReferenceMessenger.Default.Register<RequestUsersMessage>(this, (_, message) =>
        {
            message.Reply(_userDatabase.Users);
        });
        
        WeakReferenceMessenger.Default.Register<List<User>, string>(this, "DeleteUsers", (_, users) =>
        {
            foreach (User user in users)
                _userDatabase.Users.Remove(user);
        });
        
        WeakReferenceMessenger.Default.Register<MainViewModel, SignInUserMessage>(this, (recipient, message) =>
        {
            async Task<bool> SignIn(MainViewModel viewModel, SignInUserMessage message)
            {
                User? user = _userDatabase.Users.FirstOrDefault(user => user.Username == message.Username);
                
                if (user is null)
                {
                    PleasantSnackbar.Show(App.MainWindow, "Такого пользователя не существует", icon: (Geometry)Application.Current.FindResource("AccountRegular"), notificationType: NotificationType.Error);
                    
                    return false;
                }
                
                if (user.Password != message.Password)
                {
                    PleasantSnackbar.Show(App.MainWindow, "Неверный пароль", icon: (Geometry)Application.Current.FindResource("AccountRegular"), notificationType: NotificationType.Error);
                    
                    return false;
                }

                (double avgHoldTimeDiff, double avgInterkeyTimeDiff) =
                    message.PasswordEntryCharacteristic.CompareCharacteristic(user.PasswordEntryCharacteristic);

                if (avgHoldTimeDiff < 0.4)
                {
                    PleasantSnackbar.Show(App.MainWindow, "Доступ запрещен " + avgHoldTimeDiff + " " + avgInterkeyTimeDiff, icon: (Geometry)Application.Current.FindResource("AccountRegular"), notificationType: NotificationType.Error);
                    
                    return false;
                }

                if (user.FaceData is null)
                {
                    ChangePage(new UserPage(user), back: false);
                    return true;
                }
                
                string result = await MessageBox.Show(App.MainWindow,
                    "Нам нужно убедиться, что это действительно вы",
                    "Выберите способ для идентификации личности", [
                        new MessageBoxButton
                        {
                            Result = "camera",
                            Text = "Использовать камеру"
                        },
                        new MessageBoxButton
                        {
                            Result = "file",
                            Text = "Загрузить изображение"
                        },
                        new MessageBoxButton
                        {
                            Result = "cancel",
                            Text = "Отмена"
                        }
                    ]);

                switch (result)
                {
                    case "cancel":
                        return false;
                    case "camera":
                    {
                        bool? predictionResult = await viewModel.CheckFaceFromCamera(user);

                        if (predictionResult is null)
                            return false;
                        
                        if (!predictionResult.Value)
                        {
                            PleasantSnackbar.Show(App.MainWindow, "Доступ запрещен", icon: (Geometry)Application.Current.FindResource("AccountRegular"), notificationType: NotificationType.Error);
                            
                            return false;
                        }

                        break;
                    }
                    case "file":
                    {
                        bool? predictionResult = await viewModel.CheckFaceFromImage(user);
                        
                        if (predictionResult is null)
                            return false;

                        if (!predictionResult.Value)
                        {
                            PleasantSnackbar.Show(App.MainWindow, "Доступ запрещен", icon: (Geometry)Application.Current.FindResource("AccountRegular"), notificationType: NotificationType.Error);
                            
                            return false;
                        }

                        break;
                    }
                }

                ChangePage(new UserPage(user), back: false);
                return true;
            }
            
            message.Reply(SignIn(recipient, message));
        });
    }

    private async Task<bool?> CheckFaceFromCamera(User user)
    {
        CameraCaptureWindow captureWindow = new(FaceBiometric.Mode.Comparison, user.FaceData);
        
        bool? result = await captureWindow.Show<bool?>(App.MainWindow);
        return result;
    }

    private async Task<bool?> CheckFaceFromImage(User user)
    {
        if (user.FaceData is null)
            return null;
        
        TopLevel? topLevel = TopLevel.GetTopLevel(App.MainWindow);

        IReadOnlyList<IStorageFile> files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Localizer.Instance["SelectImage"],
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType(Localizer.Instance["ImageFiles"])
                {
                    Patterns = ["*.jpg", "*.jpeg", "*.png"]
                }
            ]
        });
        
        if (files.Count == 0)
            return null;
        
        IImageLoader imageLoader = new ImageFileLoader(files[0].Path.AbsolutePath);
        FaceBiometric faceBiometric = new(imageLoader, user.FaceData);
        
        imageLoader.Load();

        return faceBiometric.FaceMatches;
    }
}