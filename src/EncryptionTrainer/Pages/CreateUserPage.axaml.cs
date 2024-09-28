using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EncryptionTrainer.ViewModels;

namespace EncryptionTrainer.Pages;

public partial class CreateUserPage : UserControl
{
    public CreateUserPage()
    {
        InitializeComponent();

        DataContext = new CreateUserViewModel();
    }
}