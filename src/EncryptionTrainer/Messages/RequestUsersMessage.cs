using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging.Messages;
using EncryptionTrainer.Models;

namespace EncryptionTrainer.Messages;

public class RequestUsersMessage : RequestMessage<List<User>>;