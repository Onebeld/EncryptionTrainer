using EncryptionTrainer.ViewModels.Windows;
using PleasantUI.Controls;

namespace EncryptionTrainer.Windows;

public partial class UsersListWindow : ContentDialog
{
    public UsersListWindow()
    {
        InitializeComponent();

        DataContext = new UsersListViewModel(this);
    }
}