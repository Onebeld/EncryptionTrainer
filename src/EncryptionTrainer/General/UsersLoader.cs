using System;
using System.IO;
using System.Text;

namespace EncryptionTrainer.General;

public class UsersLoader
{
    private const uint MagicNumber = 0x5f5f5f5f;

    private const byte MajorVersion = 1;
    private const byte MinorVersion = 0;

    private static readonly string Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.bin");

    public static void Save()
    {
        using FileStream fileStream = new(Path, FileMode.Create, FileAccess.Write);
        using BinaryWriter writer = new(fileStream, Encoding.ASCII);
        
        WriteVersion(writer);
    }
    
    private static void WriteVersion(BinaryWriter writer)
    {
        writer.Write(MagicNumber);
        writer.Write(MajorVersion);
        writer.Write(MinorVersion);
    }
}