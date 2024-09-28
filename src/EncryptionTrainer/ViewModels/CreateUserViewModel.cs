using CommunityToolkit.Mvvm.Messaging;
using EncryptionTrainer.Messages;
using EncryptionTrainer.Models;
using PleasantUI;

namespace EncryptionTrainer.ViewModels;

public class CreateUserViewModel : ViewModelBase
{
    private string _username;
    private string _password;
    private string _confirmPassword;

    private byte[] _faceData;
    

    public void AddUser()
    {
        User user = new User(_username, _password, _faceData);

        WeakReferenceMessenger.Default.Send(user, "AddUser");
    }
    
    public void GoBack()
    {
        WeakReferenceMessenger.Default.Send(new GoBackMessage());
    }
}