using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EncryptionTrainer.ViewModels;

namespace EncryptionTrainer.Pages;

public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();

        DataContext = new SettingsViewModel();
    }
}