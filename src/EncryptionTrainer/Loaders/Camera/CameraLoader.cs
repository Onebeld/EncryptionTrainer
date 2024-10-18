using System.Collections.Generic;
using System.Collections.Immutable;
using Emgu.CV;
using Emgu.CV.Util;
using EncryptionTrainer.Models;

namespace EncryptionTrainer.Loaders.Camera;

public class CameraLoader : IImageProvider
{
    private readonly List<IImageListener> _listeners = new();
    
    private readonly Mat _frame;
    private readonly VideoCapture _videoCapture;

    private int _width;
    private int _height;

    public CameraLoader()
    {
        _frame = new Mat();
        _videoCapture = new VideoCapture();
    }

    public void Attach(IImageListener listener)
    {
        _listeners.Add(listener);
    }

    public void Detach(IImageListener listener)
    { 
        _listeners.Remove(listener);
    }

    public void Load()
    {
        _videoCapture.Read(_frame);

        _width = _frame.Width;
        _height = _frame.Height;

        VectorOfByte vectorOfByte = new();
        CvInvoke.Imencode(".jpg", _frame, vectorOfByte);

        PixelBufferArrived(vectorOfByte.ToArray());
    }
    
    private void PixelBufferArrived(byte[] bufferScope)
    {
        ImageData imageData = new(_width, _height, bufferScope);
            
        NotifyListeners(imageData);
    }
    
    private void NotifyListeners(ImageData imageData)
    {
        ImmutableList<IImageListener> list = _listeners.ToImmutableList();
        
        foreach (IImageListener imageListener in list)
        {
            imageListener.OnImageCaptured(imageData);
        }
    }

    public void Dispose()
    {
        _frame.Dispose();
        _videoCapture.Dispose();
    }
}