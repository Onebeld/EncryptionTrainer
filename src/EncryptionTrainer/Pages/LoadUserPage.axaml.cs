using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EncryptionTrainer.ViewModels;

namespace EncryptionTrainer.Pages;

public partial class LoadUserPage : UserControl
{
    public LoadUserPage()
    {
        InitializeComponent();

        DataContext = new LoadUserViewModel();
    }
}