using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace EncryptionTrainer.Messages;

public class RequestFaceDataListMessage : RequestMessage<List<byte[]>>;