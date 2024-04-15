using Unity.Collections;
using Unity.Networking.Transport;

public class NetMovePiece : NetMessage
{
    public int teamID { set; get; }         // 0: Navy, 1: Pirates
    public int originalX { set; get; }      // X pos that piece started in
    public int originalY { set; get; }      // Y pos that piece started in
    public int targetX { set; get; }        // X pos to move piece to
    public int targetY { set; get; }        // Y pos to move piece to
    public int corsairJump { set; get; }    // 0: not a corsairJump, 1: corsairJump

    public NetMovePiece()
    {
        Code = OpCode.MOVE_PIECE;
    }
    public NetMovePiece(DataStreamReader reader)
    {
        Code = OpCode.MOVE_PIECE;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(teamID);
        writer.WriteInt(originalX);
        writer.WriteInt(originalY);
        writer.WriteInt(targetX);
        writer.WriteInt(targetY);
        writer.WriteInt(corsairJump);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        teamID = reader.ReadInt();
        originalX = reader.ReadInt();
        originalY = reader.ReadInt();
        targetX = reader.ReadInt();
        targetY = reader.ReadInt();
        corsairJump = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_MOVE_PIECE?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_MOVE_PIECE?.Invoke(this, cnn);
    }
}
