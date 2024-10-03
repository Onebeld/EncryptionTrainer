using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using EncryptionTrainer.Biometry;
using EncryptionTrainer.Enums;
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
    
    private CameraCaptureType _cameraCaptureType;

    private byte[]? _referenceFaceData;

    private CancellationTokenSource _cancellationTokenSource;
    
    public SKBitmap? CameraBitmap
    {
        get => _cameraBitmap;
        set => SetProperty(ref _cameraBitmap, value);
    }

    public CameraCaptureViewModel(CameraCaptureWindow window, ImageBiometry imageBiometry, CameraLoader cameraLoader, CameraCaptureType cameraCaptureType, byte[]? referenceFaceData = null)
    {
        _window = window;

        if (Design.IsDesignMode)
            return;

        _cancellationTokenSource = new CancellationTokenSource();
        
        _cameraLoader = cameraLoader;
        _imageBiometry = imageBiometry;

        _referenceFaceData = referenceFaceData;

        _cameraCaptureType = cameraCaptureType;
        
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

    public async Task ReceivePredictionResultAsync(bool result)
    {
        await DisposeAsync();
        
        _window.Close(result);
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

            if (_cameraCaptureType == CameraCaptureType.Registration)
            {
                byte[]? faceData = _imageBiometry.GetFaceData();

                if (faceData is not null)
                {
                    _cancellationTokenSource.Cancel();
                    Dispatcher.UIThread.Post(() => { _ = ReceiveFaceDataAsync(faceData); });
                }
            }
            else if (_cameraCaptureType == CameraCaptureType.Identification)
            {
                if (_referenceFaceData is null)
                    return;

                bool? result = _imageBiometry.CompareFaces(_referenceFaceData);

                if (result is not null)
                {
                    _cancellationTokenSource.Cancel();
                    Dispatcher.UIThread.Post(() => { _ = ReceivePredictionResultAsync(result.Value); });
                }
            }
        }, ct: _cancellationTokenSource.Token);

        await _captureDevice.StartAsync();
        
        descriptions.GetEnumerator().Dispose();
    }

    private async Task DisposeAsync()
    {
        await _captureDevice.StopAsync();
        _imageBiometry.Dispose();
        CameraBitmap?.Dispose();
        _cancellationTokenSource.Dispose();
    }
}