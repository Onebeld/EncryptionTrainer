using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using EncryptionTrainer.Biometry.KeystrokeDynamics;
using EncryptionTrainer.General;
using EncryptionTrainer.ViewModels;

namespace EncryptionTrainer.Pages;

public partial class CreateUserPage : UserControl
{
    private readonly CreateUserViewModel _viewModel;
    private readonly Stopwatch _stopwatch = new();

    private List<Keystroke> _keystrokes = new();
    
    private double _keyDownTime = 0;
    private double _elapsedMilliseconds = 0;
    
    public CreateUserPage()
    {
        InitializeComponent();

        _viewModel = new CreateUserViewModel();
        DataContext = _viewModel;
    }

    private void PasswordTextBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_viewModel.Attempt > AppSettings.Instance.NumberOfAttempts)
        {
            (sender as TextBox).Text = string.Empty;
            _viewModel.Password = string.Empty;
        }
        
        if (e.Key == Key.Enter)
        {
            _viewModel.PasswordIsEntered = true;
            _viewModel.ConfirmPassword = _viewModel.Password;
        }
        else if (e.Key == Key.Escape)
        {
            _keystrokes.Clear();
            _viewModel.Password = string.Empty;
            
            _stopwatch.Stop();
            _stopwatch.Reset();
            _elapsedMilliseconds = 0;
            _viewModel.Attempt = 0;
        }
        else if (e.Key == Key.Back)
        {
            _keystrokes.Clear();
            _viewModel.Password = string.Empty;
            
            _stopwatch.Stop();
            _stopwatch.Reset();
        }
        else
        {
            _keyDownTime = _stopwatch.ElapsedMilliseconds;
        }
    }

    private void PasswordTextBox_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter && e.Key != Key.Escape && e.Key != Key.Back)
        {
            _elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
            _keystrokes.Add(new Keystroke(TimeSpan.FromMilliseconds(_keyDownTime), TimeSpan.FromMilliseconds(_elapsedMilliseconds)));
            
            _stopwatch.Reset();
            _viewModel.Attempt++;
        }
    }
}