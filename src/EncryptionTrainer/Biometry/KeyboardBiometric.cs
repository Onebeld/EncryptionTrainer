using System;

namespace EncryptionTrainer.Biometry;

public class KeyboardBiometric
{
    public struct Keystroke
    {
        public char Key;
        public DateTime TimeDown;
        public DateTime TimeUp;

        public bool IsError;
    }
}