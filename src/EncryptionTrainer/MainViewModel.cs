using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EncryptionTrainer.General;
using EncryptionTrainer.Messages;
using EncryptionTrainer.Models;
using EncryptionTrainer.Pages;
using PleasantUI.Controls;
using PleasantUI.Core.Localization;

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

    public void OpenSettingsPage()
    {
        if (App.MainWindow.ModalWindows.Any())
            return;
        
        if (Page is not SettingsPage)
            ChangePage(new SettingsPage());
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
            
            PleasantSnackbar.Show(App.MainWindow, Localizer.Instance["UserAdded"], icon: (Geometry)Application.Current.FindResource("AccountBoxRegular"), notificationType: NotificationType.Success);
        });
        
        WeakReferenceMessenger.Default.Register<RequestFaceDataListMessage>(this, (recipient, message) =>
        {
            List<byte[]> list = _userDatabase.Users.Select(user => user.FaceData).OfType<byte[]>().ToList();

            message.Reply(list);
        });
    }

    private void ChangePage(Control? page, bool forward = true)
    {
        _previousPages.Push(Page);
        IsForwardAnimation = forward;

        Page = page;
    }

    private void GoBack()
    {
        IsForwardAnimation = false;
        Page = _previousPages.Pop();
    }
}