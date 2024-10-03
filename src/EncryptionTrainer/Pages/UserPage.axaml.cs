using Avalonia.Controls;
using EncryptionTrainer.Models;
using EncryptionTrainer.ViewModels;

namespace EncryptionTrainer.Pages;

public partial class UserPage : UserControl
{
    private UserViewModel _viewModel;
    
    public UserPage()
    {
        InitializeComponent();
    }
    
    public UserPage(User user)
    {
        InitializeComponent();

        _viewModel = new UserViewModel(user);
        DataContext = _viewModel;
    }
}