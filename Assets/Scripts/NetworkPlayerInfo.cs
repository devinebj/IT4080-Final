using Unity.Netcode;
using Unity.Collections;

public struct NetworkPlayerInfo : INetworkSerializable, System.IEquatable<NetworkPlayerInfo> {
    public ulong ClientId;
    public bool Ready;
    public FixedString32Bytes PlayerName;

    public NetworkPlayerInfo(ulong id) {
        ClientId = id;
        Ready = false;
        PlayerName = "Not Set";
    }
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref Ready);
        serializer.SerializeValue(ref PlayerName);
    }

    
    public bool Equals(NetworkPlayerInfo other) {
        return false;
    }
}
