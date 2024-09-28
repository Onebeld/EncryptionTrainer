using System.Collections.Generic;
using EncryptionTrainer.Models;

namespace EncryptionTrainer.General;

public class UserDatabase
{
    public List<User> Users { get; } = new();
}