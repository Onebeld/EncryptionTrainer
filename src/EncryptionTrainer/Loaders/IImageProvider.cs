using System;

namespace EncryptionTrainer.Loaders;

public interface IImageProvider : IDisposable
{
    void Attach(IImageListener listener);
    void Detach(IImageListener listener);

    void Load();
}