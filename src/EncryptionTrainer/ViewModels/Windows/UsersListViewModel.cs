using System.Collections.Generic;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EncryptionTrainer.Messages;
using EncryptionTrainer.Models;
using EncryptionTrainer.Windows;

namespace EncryptionTrainer.ViewModels.Windows;

public class UsersListViewModel : ObservableObject
{
    private readonly UsersListWindow _window;
    
    public AvaloniaList<User> Users { get; } = new();
    public List<User> DeletedUsers { get; } = new();

    public UsersListViewModel(UsersListWindow window)
    {
        _window = window;
        
        List<User> users = WeakReferenceMessenger.Default.Send(new RequestUsersMessage());
        Users.AddRange(users);
    }

    public void DeleteUser(User user)
    {
        Users.Remove(user);
        DeletedUsers.Add(user);
    }

    public void CloseWithDeleteUsers()
    {
        WeakReferenceMessenger.Default.Send(DeletedUsers, "DeleteUsers");
        
        _window.Close();
    }

    public void Close()
    {
        _window.Close();
    }
}