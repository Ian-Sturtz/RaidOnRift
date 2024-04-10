using Unity.Networking.Transport;
using Unity.Collections;
using UnityEngine;

public class NetStartGame : NetMessage
{
    public int Start_Game { set; get; }

    public NetStartGame()
    {
        Code = OpCode.START_GAME;
    }
    public NetStartGame(DataStreamReader reader)
    {
        Code = OpCode.START_GAME;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(Start_Game);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        Start_Game = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_START_GAME?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_START_GAME?.Invoke(this, cnn);
    }
}
