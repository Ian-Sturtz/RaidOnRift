using Unity.Collections;
using Unity.Networking.Transport;

public class NetGameWon : NetMessage
{
    public int teamID { set; get; }         // 0: Navy won, 1: Pirates won

    public NetGameWon()
    {
        Code = OpCode.GAME_WON;
    }
    public NetGameWon(DataStreamReader reader)
    {
        Code = OpCode.GAME_WON;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(teamID);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        teamID = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_GAME_WON?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_GAME_WON?.Invoke(this, cnn);
    }
}
