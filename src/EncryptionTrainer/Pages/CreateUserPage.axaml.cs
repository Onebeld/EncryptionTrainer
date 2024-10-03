using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Media;
using EncryptionTrainer.ViewModels;
using PleasantUI.Controls;
using PleasantUI.Core.Localization;

namespace EncryptionTrainer.Pages;

public partial class CreateUserPage : UserControl
{
    private readonly CreateUserViewModel _viewModel;
    private readonly Stopwatch _stopwatch = new();
    
    public CreateUserPage()
    {
        InitializeComponent();

        _viewModel = new CreateUserViewModel();
        DataContext = _viewModel;
    }

    private void PasswordTextBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            _viewModel.PasswordIsEntered = true;
            _viewModel.ConfirmPassword = string.Empty;
            ConfirmPasswordTextBox.Focus();
        }
    }

    private void ConfirmPasswordTextBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            _viewModel.PasswordIsEntered = false;
            _viewModel.PasswordIsConfirmed = false;
            _viewModel.Password = string.Empty;
            _viewModel.PasswordEntryCharacteristic.ClearAllTimes();
            
            ResetStopwatch();

            PasswordTextBox.Focus();
        }
        
        if (e.Key != Key.Enter)
        {
            if (!_stopwatch.IsRunning)
                _stopwatch.Start();
            
            _viewModel.PasswordEntryCharacteristic.KeyDownTimes.Add(_stopwatch.ElapsedMilliseconds);
            _viewModel.PasswordEntryCharacteristic.AddInterKeyTime();
        }
        else
        {
            if (_viewModel.Password != _viewModel.ConfirmPassword)
            {
                ResetStopwatch();
                
                _viewModel.ConfirmPassword = string.Empty;
                
                PleasantSnackbar.Show(App.MainWindow, Localizer.Instance["PasswordNotMatch"], icon: (Geometry)Application.Current.FindResource("AccountBoxRegular"), notificationType: NotificationType.Error);
                return;
            }
            
            _viewModel.PasswordIsConfirmed = true;
            
            ResetStopwatch();
        }
    }

    private void ResetStopwatch()
    {
        _stopwatch.Stop();
        _stopwatch.Reset();
    }

    private void ConfirmPasswordTextBox_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter && e.Key != Key.Escape)
        {
            _viewModel.PasswordEntryCharacteristic.KeyUpTimes.Add(_stopwatch.ElapsedMilliseconds);
        }
    }
}