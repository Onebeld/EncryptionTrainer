using System.IO;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EncryptionTrainer.Messages;
using EncryptionTrainer.Models;

namespace EncryptionTrainer.ViewModels;

public class UserViewModel : ObservableObject
{
    private User _user;

    public Bitmap? Bitmap
    {
        get
        {
            if (_user.FaceData is null)
                return null;
                
            MemoryStream ms = new(_user.FaceData);
            return new Bitmap(ms);
        }
    }
    
    public UserViewModel(User user)
    {
        _user = user;
    }
    
    public void GoBack()
    {
        WeakReferenceMessenger.Default.Send(new GoBackMessage());
    }
}