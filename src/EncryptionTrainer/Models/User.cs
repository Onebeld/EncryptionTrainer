using System.Collections.Generic;
using EncryptionTrainer.Biometry.KeystrokeDynamics;

namespace EncryptionTrainer.Models;

public class User(string username, string password, List<List<Keystroke>> keystrokeTemplates, byte[]? faceData)
{
    public string Username { get; init; } = username;

    public string Password { get; init; } = password;

    public byte[]? FaceData { get; init; } = faceData;
    
    public List<List<Keystroke>> KeystrokeTemplates { get; init; } = keystrokeTemplates;
}