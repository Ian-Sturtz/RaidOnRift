using Unity.Collections;
using Unity.Networking.Transport;

public class NetPositionPiece : NetMessage
{
    public int teamID { set; get; }         // 0: Navy, 1: Pirates
    public int jailIndex { set; get; }      // What index is the piece in jail?
    public int targetX { set; get; }        // X pos to spawn piece
    public int targetY { set; get; }        // Y pos to spawn piece

    public NetPositionPiece()
    {
        Code = OpCode.POSITION_PIECE;
    }
    public NetPositionPiece(DataStreamReader reader)
    {
        Code = OpCode.POSITION_PIECE;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(teamID);
        writer.WriteInt(jailIndex);
        writer.WriteInt(targetX);
        writer.WriteInt(targetY);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        teamID = reader.ReadInt();
        jailIndex = reader.ReadInt();
        targetX = reader.ReadInt();
        targetY = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_POSITION_PIECE?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_POSITION_PIECE?.Invoke(this, cnn);
    }
}
