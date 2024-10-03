using EncryptionTrainer.Biometry;
using EncryptionTrainer.Enums;
using EncryptionTrainer.Loaders.Camera;
using EncryptionTrainer.ViewModels.Windows;
using PleasantUI.Controls;

namespace EncryptionTrainer.Windows;

public partial class CameraCaptureWindow : ContentDialog
{
    public CameraCaptureWindow()
    {
        InitializeComponent();

        DataContext = new CameraCaptureViewModel(this, null, null, CameraCaptureType.Registration);
    }

    public CameraCaptureWindow(ImageBiometry imageBiometry, CameraLoader cameraLoader, CameraCaptureType cameraCaptureType, byte[]? referenceFaceData = null)
    {
        InitializeComponent();

        DataContext = new CameraCaptureViewModel(this, imageBiometry, cameraLoader, cameraCaptureType, referenceFaceData);
    }
}