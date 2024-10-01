namespace EncryptionTrainer.Models;

public class ImageData(int width, int height, byte[] data)
{
    public int Width { get; init; } = width;
    public int Height { get; init; } = height;

    public byte[] Data { get; init; } = data;
}