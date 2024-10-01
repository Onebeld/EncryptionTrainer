using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using EncryptionTrainer.Biometry;
using EncryptionTrainer.Loaders.Camera;
using EncryptionTrainer.Windows;
using FlashCap;
using SkiaSharp;

namespace EncryptionTrainer.ViewModels.Windows;

public class CameraCaptureViewModel : ObservableObject
{
    private readonly CameraCaptureWindow _window;
    
    private readonly CameraLoader _cameraLoader;
    private readonly ImageBiometry _imageBiometry;

    private SKBitmap? _cameraBitmap;
    private CaptureDevice _captureDevice;
    
    public SKBitmap? CameraBitmap
    {
        get => _cameraBitmap;
        set => SetProperty(ref _cameraBitmap, value);
    }

    public CameraCaptureViewModel(CameraCaptureWindow window,ImageBiometry imageBiometry, CameraLoader cameraLoader)
    {
        _window = window;

        if (Design.IsDesignMode)
            return;
        
        _cameraLoader = cameraLoader;
        _imageBiometry = imageBiometry;
        
        _ = RunCaptureAsync();
    }

    public async Task CancelAsync()
    {
        await DisposeAsync();
        
        _window.Close(null);
    }

    public async Task ReceiveFaceDataAsync(byte[] faceData)
    {
        await DisposeAsync();
        
        _window.Close(faceData);
    }

    private async Task RunCaptureAsync()
    {
        CaptureDevices devices = new();

        IEnumerable<CaptureDeviceDescriptor> descriptions = devices.EnumerateDescriptors();

        if (!descriptions.Any())
            return;

        CaptureDeviceDescriptor descriptor = descriptions.ElementAt(0);

        _captureDevice = await descriptor.OpenAsync(descriptor.Characteristics[1], bufferScope =>
        {
            byte[] image = bufferScope.Buffer.ExtractImage();
            var bitmap = SKBitmap.Decode(image);
            
            bufferScope.ReleaseNow();

            Dispatcher.UIThread.Post(() => { CameraBitmap = bitmap; });

            _cameraLoader.Load(bitmap.Width, bitmap.Height, image);

            byte[]? faceData = _imageBiometry.GetFaceData();

            if (faceData is not null)
                Dispatcher.UIThread.Post(() =>
                {
                    _ = ReceiveFaceDataAsync(faceData);
                });
        });

        await _captureDevice.StartAsync();
        
        descriptions.GetEnumerator().Dispose();
    }

    private async Task DisposeAsync()
    {
        await _captureDevice.StopAsync();
        _imageBiometry.Dispose();
        CameraBitmap?.Dispose();
    }
}