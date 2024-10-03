using CommunityToolkit.Mvvm.Messaging.Messages;
using EncryptionTrainer.Models;

namespace EncryptionTrainer.Messages;

public class SignInUserMessage : AsyncRequestMessage<bool>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public PasswordEntryCharacteristic PasswordEntryCharacteristic { get; set; }

    public SignInUserMessage(string username, string password, PasswordEntryCharacteristic passwordEntryCharacteristic)
    {
        Username = username;
        Password = password;
        PasswordEntryCharacteristic = passwordEntryCharacteristic;
    }
}