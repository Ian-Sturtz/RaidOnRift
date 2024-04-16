using Unity.Collections;
using Unity.Networking.Transport;

public class NetCapturePiece : NetMessage
{
    public int teamID { set; get; }         // 0: Navy, 1: Pirates
    public int originalX { set; get; }      // X pos that piece started in
    public int originalY { set; get; }      // Y pos that piece started in
    public int targetX { set; get; }        // X pos to move piece to
    public int targetY { set; get; }        // Y pos to move piece to
    public int gunnerCapture { set; get; }  // 0: move to take that square (regular capture), 1: don't move to take that square (gunner capture)
    public int turnOver { set; get; }       // 1: the turn is now over, 0: the turn is not over (replace ore or orebearer second move)

    public NetCapturePiece()
    {
        Code = OpCode.CAPTURE_PIECE;
    }
    public NetCapturePiece(DataStreamReader reader)
    {
        Code = OpCode.CAPTURE_PIECE;
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
        writer.WriteInt(gunnerCapture);
        writer.WriteInt(turnOver);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        teamID = reader.ReadInt();
        originalX = reader.ReadInt();
        originalY = reader.ReadInt();
        targetX = reader.ReadInt();
        targetY = reader.ReadInt();
        gunnerCapture = reader.ReadInt();
        turnOver = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_CAPTURE_PIECE?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_CAPTURE_PIECE?.Invoke(this, cnn);
    }
}
