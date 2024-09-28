using CommunityToolkit.Mvvm.Messaging;
using EncryptionTrainer.Messages;
using PleasantUI;

namespace EncryptionTrainer.ViewModels;

public class LoadUserViewModel : ViewModelBase
{
    public void GoBack()
    {
        WeakReferenceMessenger.Default.Send(new GoBackMessage());
    }
}