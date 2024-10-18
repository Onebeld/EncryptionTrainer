using EncryptionTrainer.Models;

namespace EncryptionTrainer.Loaders;

public interface IImageListener
{
    void OnImageCaptured(ImageData imageData);
}