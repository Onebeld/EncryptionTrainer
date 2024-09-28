namespace EncryptionTrainer.Models;

public class User(string username, string password, byte[]? faceData)
{
    public string Username { get; set; } = username;

    public string Password { get; set; } = password;

    public byte[]? FaceData { get; set; } = faceData;
}