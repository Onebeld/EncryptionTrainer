using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EncryptionTrainer.Biometry;
using EncryptionTrainer.Enums;
using EncryptionTrainer.Loaders;
using EncryptionTrainer.Loaders.Camera;
using EncryptionTrainer.Loaders.ImageFile;
using EncryptionTrainer.Messages;
using EncryptionTrainer.Models;
using EncryptionTrainer.Windows;
using PleasantUI.Controls;
using PleasantUI.Core.Localization;

namespace EncryptionTrainer.ViewModels;

public class CreateUserViewModel : ObservableObject
{
    private readonly List<byte[]> _faceDataList;
    private PasswordEntryCharacteristic _passwordEntryCharacteristic = new();
    
    private string _username;
    private string _password;
    private byte[]? _faceData;
    
    private string _confirmPassword;

    private bool _passwordIsEntered;
    private bool _passwordIsConfirmed;
    
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }

    public bool PasswordIsEntered
    {
        get => _passwordIsEntered;
        set => SetProperty(ref _passwordIsEntered, value);
    }

    public bool PasswordIsConfirmed
    {
        get => _passwordIsConfirmed;
        set => SetProperty(ref _passwordIsConfirmed, value);
    }

    public bool FaceDataIsAdded
    {
        get => _faceData is not null;
    }

    public PasswordEntryCharacteristic PasswordEntryCharacteristic
    {
        get => _passwordEntryCharacteristic;
        set => SetProperty(ref _passwordEntryCharacteristic, value);
    }

    public CreateUserViewModel()
    {
        _faceDataList = WeakReferenceMessenger.Default.Send(new RequestFaceDataListMessage());
    }
    
    public void AddUser()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            PleasantSnackbar.Show(App.MainWindow, Localizer.Instance["PleaseEnterUsername"], icon: (Geometry)Application.Current.FindResource("AccountBoxRegular"), notificationType: NotificationType.Error);

            return;
        }

        Regex regex = new("^[a-zA-Z0-9]*$");

        if (!regex.IsMatch(Username))
        {
            PleasantSnackbar.Show(App.MainWindow, Localizer.Instance["UsernameOnlyLettersNumbers"], icon: (Geometry)Application.Current.FindResource("AccountBoxRegular"), notificationType: NotificationType.Error);

            return;
        }
        
        if (!PasswordIsEntered)
        {
            PleasantSnackbar.Show(App.MainWindow, Localizer.Instance["PleaseEnterPassword"], icon: (Geometry)Application.Current.FindResource("AccountBoxRegular"), notificationType: NotificationType.Error);
            
            return;
        }

        if (!PasswordIsConfirmed)
        {
            PleasantSnackbar.Show(App.MainWindow, Localizer.Instance["PleaseConfirmPassword"], icon: (Geometry)Application.Current.FindResource("AccountBoxRegular"), notificationType: NotificationType.Error);

            return;
        }
        
        User user = new(_username, _password, _passwordEntryCharacteristic, _faceData);

        WeakReferenceMessenger.Default.Send(user, "AddUser");
    }

    public async Task LoadFaceFromImage()
    {
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
            return;
        
        IImageLoader imageLoader = new ImageFileLoader(files[0].Path.AbsolutePath);

        ImageBiometry imageBiometry = new(imageLoader, _faceDataList);

        byte[]? faceData = imageBiometry.GetFaceData();

        if (faceData is not null)
        {
            _faceData = faceData;
            OnPropertyChanged(nameof(FaceDataIsAdded));
            
            PleasantSnackbar.Show(App.MainWindow, Localizer.Instance["BiometricsAdded"], icon: (Geometry)Application.Current.FindResource("AccountBoxRegular"), notificationType: NotificationType.Success);
        }
        else
        {
            PleasantSnackbar.Show(App.MainWindow, Localizer.Instance["BiometricsNotAdded"], icon: (Geometry)Application.Current.FindResource("AccountBoxRegular"), notificationType: NotificationType.Error);
        }
    }

    public async Task LoadFaceFromCamera()
    {
        CameraLoader cameraLoader = new();
        ImageBiometry imageBiometry = new(cameraLoader, _faceDataList);

        CameraCaptureWindow captureWindow = new(imageBiometry, cameraLoader, CameraCaptureType.Registration);

        byte[]? faceData = await captureWindow.Show<byte[]?>(App.MainWindow);
        
        if (faceData is not null)
        {
            _faceData = faceData;
            OnPropertyChanged(nameof(FaceDataIsAdded));
            
            PleasantSnackbar.Show(App.MainWindow, Localizer.Instance["BiometricsAdded"], icon: (Geometry)Application.Current.FindResource("AccountBoxRegular"), notificationType: NotificationType.Success);
        }
    }
    
    public void GoBack()
    {
        WeakReferenceMessenger.Default.Send(new GoBackMessage());
    }
}