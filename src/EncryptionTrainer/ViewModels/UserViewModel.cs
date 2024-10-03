using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EncryptionTrainer.Messages;
using EncryptionTrainer.Models;

namespace EncryptionTrainer.ViewModels;

public class UserViewModel : ObservableObject
{
    public UserViewModel(User user)
    {
        
    }
    
    public void GoBack()
    {
        WeakReferenceMessenger.Default.Send(new GoBackMessage());
    }
}