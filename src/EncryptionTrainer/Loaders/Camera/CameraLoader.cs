using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlashCap;
using SkiaSharp;

namespace EncryptionTrainer.Loaders.Camera;

public class CameraLoader : IImageLoader
{
    private CaptureDevice? _captureDevice;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public event EventHandler<ImageCapturedEventArgs>? ImageCaptured;

    public event EventHandler? Initialized;

    public CameraLoader()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        _ = InitializeCaptureDeviceAsync();
    }

    public void Load()
    {
        _captureDevice?.StartAsync();
    }
    
    protected virtual void OnImageCaptured(byte[] bytes)
    {
        ImageCaptured?.Invoke(this, new ImageCapturedEventArgs(bytes));
    }

    private async Task InitializeCaptureDeviceAsync()
    {
        CaptureDevices captureDevices = new();
        
        IEnumerable<CaptureDeviceDescriptor> descriptions = captureDevices.EnumerateDescriptors().ToList();

        if (!descriptions.Any())
            throw new SystemException("No cameras found");

        CaptureDeviceDescriptor descriptor = descriptions.ElementAt(0);

        _captureDevice = await descriptor.OpenAsync(descriptor.Characteristics[0], PixelBufferArrived,
            ct: _cancellationTokenSource.Token);
        
        descriptions.GetEnumerator().Dispose();
        
        Initialized?.Invoke(this, EventArgs.Empty);
    }
    
    private void PixelBufferArrived(PixelBufferScope bufferScope)
    {
        byte[] image = bufferScope.Buffer.ExtractImage();
        bufferScope.ReleaseNow();
            
        OnImageCaptured(image);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        
        _captureDevice?.Dispose();
        _cancellationTokenSource.Dispose();
    }
}