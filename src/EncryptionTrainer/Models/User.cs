namespace EncryptionTrainer.Models;

public class User(string username, string password, PasswordEntryCharacteristic passwordEntryCharacteristic, byte[]? faceData)
{
    public string Username { get; init; } = username;

    public string Password { get; init; } = password;

    public byte[]? FaceData { get; init; } = faceData;

    public PasswordEntryCharacteristic PasswordEntryCharacteristic { get; init; } = passwordEntryCharacteristic;
}