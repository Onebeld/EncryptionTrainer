using EncryptionTrainer.Biometry;
using EncryptionTrainer.ViewModels.Windows;
using PleasantUI.Controls;

namespace EncryptionTrainer.Windows;

public partial class CameraCaptureWindow : ContentDialog
{
    public CameraCaptureWindow()
    {
        InitializeComponent();

        DataContext = new CameraCaptureViewModel(this, FaceBiometric.Mode.Determination);
    }

    public CameraCaptureWindow(FaceBiometric.Mode mode, byte[]? referenceFaceData = null)
    {
        InitializeComponent();

        DataContext = new CameraCaptureViewModel(this, mode, referenceFaceData);
    }
}