using Avalonia.Controls;
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