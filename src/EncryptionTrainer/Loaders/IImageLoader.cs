using EncryptionTrainer.Models;

namespace EncryptionTrainer.Loaders;

public interface IImageLoader
{
    ImageData Load();
}