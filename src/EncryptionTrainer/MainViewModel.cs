using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using EncryptionTrainer.General;
using EncryptionTrainer.Messages;
using EncryptionTrainer.Models;
using EncryptionTrainer.Pages;
using PleasantUI;

namespace EncryptionTrainer;

public class MainViewModel : ViewModelBase
{
    private readonly Stack<Control?> _previousPages = new();
    
    private bool _isForwardAnimation = true;
    private Control? _page;

    private UserDatabase _userDatabase;

    public bool IsForwardAnimation
    {
        get => _isForwardAnimation;
        set => RaiseAndSet(ref _isForwardAnimation, value);
    }

    public Control? Page
    {
        get => _page;
        set => RaiseAndSet(ref _page, value);
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
        if (Page is not SettingsPage)
            ChangePage(new SettingsPage());
    }
    
    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<GoBackMessage>(this, (_, _) =>
        {
            GoBack();
        });
        
        WeakReferenceMessenger.Default.Register<User, string>(this, "AddUser", (_, message) =>
        {
            _userDatabase.Users.Add(message);
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