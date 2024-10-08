using System;

namespace EncryptionTrainer.Loaders;

public interface IImageLoader : IDisposable
{
    event EventHandler<ImageCapturedEventArgs> ImageCaptured;
    
    void Load();
}