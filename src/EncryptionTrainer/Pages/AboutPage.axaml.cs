using Avalonia.Controls;
using EncryptionTrainer.ViewModels;

namespace EncryptionTrainer.Pages;

public partial class AboutPage : UserControl
{
    public AboutPage()
    {
        InitializeComponent();

        DataContext = new AboutViewModel();
    }
}