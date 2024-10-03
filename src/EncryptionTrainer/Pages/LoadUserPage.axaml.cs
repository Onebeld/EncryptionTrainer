using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using EncryptionTrainer.ViewModels;

namespace EncryptionTrainer.Pages;

public partial class LoadUserPage : UserControl
{
    private readonly LoadUserViewModel _viewModel;
    private readonly Stopwatch _stopwatch = new();
    
    public LoadUserPage()
    {
        InitializeComponent();

        _viewModel = new LoadUserViewModel(_stopwatch);
        DataContext = _viewModel;
    }

    private void PasswordTextBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            _ = _viewModel.SingIn();
            
            return;
        }
        
        if (e.Key != Key.Enter && e.Key != Key.Escape && e.Key != Key.Back)
        {
            if (!_stopwatch.IsRunning)
                _stopwatch.Start();
            
            _viewModel.PasswordEntryCharacteristic.KeyDownTimes.Add(_stopwatch.ElapsedMilliseconds);
            _viewModel.PasswordEntryCharacteristic.AddInterKeyTime();
        }

        if (sender is TextBox { Text.Length: 0 })
            ResetStopwatch();
    }

    private void PasswordTextBox_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter && e.Key != Key.Escape && e.Key != Key.Back)
            _viewModel.PasswordEntryCharacteristic.KeyUpTimes.Add(_stopwatch.ElapsedMilliseconds);
    }
    
    private void ResetStopwatch()
    {
        _stopwatch.Stop();
        _stopwatch.Reset();
    }
}