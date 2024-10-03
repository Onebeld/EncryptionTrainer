using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EncryptionTrainer.Messages;
using EncryptionTrainer.Models;
using PleasantUI.Controls;
using PleasantUI.Core.Localization;

namespace EncryptionTrainer.ViewModels;

public class LoadUserViewModel : ObservableObject
{
    private PasswordEntryCharacteristic _passwordEntryCharacteristic = new();
    
    private string _username = string.Empty;
    private string _password = string.Empty;

    private Stopwatch _stopwatch;

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
    
    public PasswordEntryCharacteristic PasswordEntryCharacteristic
    {
        get => _passwordEntryCharacteristic;
        set => SetProperty(ref _passwordEntryCharacteristic, value);
    }

    public LoadUserViewModel(Stopwatch stopwatch)
    {
        _stopwatch = stopwatch;
    }

    public async Task SingIn()
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

        if (string.IsNullOrWhiteSpace(Password))
        {
            PleasantSnackbar.Show(App.MainWindow, Localizer.Instance["PleaseEnterPassword"], icon: (Geometry)Application.Current.FindResource("AccountBoxRegular"), notificationType: NotificationType.Error);
            
            return;
        }
        
        ResetStopwatch();

        bool result =
            await WeakReferenceMessenger.Default.Send(new SignInUserMessage(Username, Password,
                _passwordEntryCharacteristic));

        if (!result)
        {
            Password = string.Empty;
            _passwordEntryCharacteristic.ClearAllTimes();
        }
    }
    
    public void GoBack()
    {
        WeakReferenceMessenger.Default.Send(new GoBackMessage());
    }
    
    private void ResetStopwatch()
    {
        _stopwatch.Stop();
        _stopwatch.Reset();
    }
}