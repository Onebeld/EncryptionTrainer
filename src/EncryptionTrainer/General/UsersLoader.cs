using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EncryptionTrainer.Models;

namespace EncryptionTrainer.General;

public class UsersLoader
{
    private const uint MagicNumber = 0x4f424c44;

    private const byte MajorVersion = 1;
    private const byte MinorVersion = 0;

    private static readonly string Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users.bin");

    public static void Save(List<User> users)
    {
        using FileStream fileStream = new(Path, FileMode.Create, FileAccess.Write);
        using BinaryWriter writer = new(fileStream, Encoding.ASCII);
        
        WriteVersion(writer);

        foreach (User user in users)
        {
            WriteUsername(writer, user);

            WritePassword(writer, user);

            WriteFaceData(writer, user);

            WritePasswordEntryCharacteristic(user, writer);
        }
    }

    public static List<User> Load()
    {
        if (!File.Exists(Path))
            return [];
        
        using FileStream fileStream = new(Path, FileMode.Open, FileAccess.Read);
        using BinaryReader reader = new(fileStream, Encoding.ASCII);

        uint magicNumber = reader.ReadUInt32();
        if (magicNumber != MagicNumber)
            throw new InvalidDataException("Invalid users.bin file");

        Version version = ReadVersion(reader);

        if (version < new Version(MajorVersion, MinorVersion))
            throw new InvalidDataException("Invalid users.bin file");

        List<User> users = new();
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            string username = ReadUsername(reader);

            string password = ReadPassword(reader);

            byte[]? faceData = ReadFaceData(reader);

            PasswordEntryCharacteristic passwordEntryCharacteristic = ReadPasswordEntryCharacteristic(reader);

            users.Add(new User(username, password, passwordEntryCharacteristic, faceData));
        }
        
        return users;
    }
    
    private static Version ReadVersion(BinaryReader reader)
    {
        byte majorVersion = reader.ReadByte();
        byte minorVersion = reader.ReadByte();

        return new Version(majorVersion, minorVersion);
    }

    private static string ReadUsername(BinaryReader reader)
    {
        int usernameLength = reader.ReadInt32();
        byte[] usernameBytes = reader.ReadBytes(usernameLength);
        string username = Encoding.ASCII.GetString(usernameBytes);
        return username;
    }

    private static string ReadPassword(BinaryReader reader)
    {
        int passwordLength = reader.ReadInt32();

        StringBuilder stringBuilder = new();

        for (int i = 0; i < passwordLength; i++)
            stringBuilder.Append((char)reader.ReadInt16());
        
        return stringBuilder.ToString();
    }

    private static byte[]? ReadFaceData(BinaryReader writer)
    {
        int faceDataLength = writer.ReadInt32();
        byte[]? faceData = null;
        if (faceDataLength > 0)
            faceData = writer.ReadBytes(faceDataLength);
        return faceData;
    }

    private static PasswordEntryCharacteristic ReadPasswordEntryCharacteristic(BinaryReader writer)
    {
        int keyDownTimesCount = writer.ReadInt32();
        int keyUpTimesCount = writer.ReadInt32();
        int interKeyTimesCount = writer.ReadInt32();
            
        PasswordEntryCharacteristic passwordEntryCharacteristic = new();
            
        for (int i = 0; i < keyDownTimesCount; i++)
            passwordEntryCharacteristic.KeyDownTimes.Add(writer.ReadInt64());
        for (int i = 0; i < keyUpTimesCount; i++)
            passwordEntryCharacteristic.KeyUpTimes.Add(writer.ReadInt64());
        for (int i = 0; i < interKeyTimesCount; i++)
            passwordEntryCharacteristic.InterKeyTimes.Add(writer.ReadInt64());
        return passwordEntryCharacteristic;
    }

    private static void WriteVersion(BinaryWriter writer)
    {
        writer.Write(MagicNumber);
        writer.Write(MajorVersion);
        writer.Write(MinorVersion);
    }
    
    private static void WriteUsername(BinaryWriter writer, User user)
    {
        writer.Write(user.Username.Length);
        writer.Write(Encoding.ASCII.GetBytes(user.Username));
    }
    
    private static void WritePassword(BinaryWriter writer, User user)
    {
        writer.Write(user.Password.Length);
        writer.Write(Encoding.Unicode.GetBytes(user.Password));
    }
    
    private static void WritePasswordEntryCharacteristic(User user, BinaryWriter writer)
    {
        int keyDownTimesCount = user.PasswordEntryCharacteristic.KeyDownTimes.Count;
        int keyUpTimesCount = user.PasswordEntryCharacteristic.KeyUpTimes.Count;
        int interKeyTimesCount = user.PasswordEntryCharacteristic.InterKeyTimes.Count;
            
        writer.Write(keyDownTimesCount);
        writer.Write(keyUpTimesCount);
        writer.Write(interKeyTimesCount);

        foreach (long keyDownTime in user.PasswordEntryCharacteristic.KeyDownTimes)
            writer.Write(keyDownTime);
        foreach (long keyUpTime in user.PasswordEntryCharacteristic.KeyUpTimes)
            writer.Write(keyUpTime);
        foreach (long interKeyTime in user.PasswordEntryCharacteristic.InterKeyTimes)
            writer.Write(interKeyTime);
    }

    private static void WriteFaceData(BinaryWriter writer, User user)
    {
        writer.Write(user.FaceData?.Length ?? -1);

        if (user.FaceData is not null)
            writer.Write(user.FaceData);
    }
}