using Avalonia.Controls;
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