using EncryptionTrainer.Biometry;
using EncryptionTrainer.Loaders.Camera;
using EncryptionTrainer.ViewModels.Windows;
using PleasantUI.Controls;

namespace EncryptionTrainer.Windows;

public partial class CameraCaptureWindow : ContentDialog
{
    public CameraCaptureWindow()
    {
        InitializeComponent();

        DataContext = new CameraCaptureViewModel(this, null, null);
    }

    public CameraCaptureWindow(ImageBiometry imageBiometry, CameraLoader cameraLoader)
    {
        InitializeComponent();

        DataContext = new CameraCaptureViewModel(this, imageBiometry, cameraLoader);
    }
}