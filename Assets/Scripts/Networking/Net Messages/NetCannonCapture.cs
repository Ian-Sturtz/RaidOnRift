using Unity.Collections;
using Unity.Networking.Transport;

public class NetCannonCapture : NetMessage
{
    public int teamID { set; get; }         // 0: Navy, 1: Pirates
    public int originalX { set; get; }      // X pos that piece started in
    public int originalY { set; get; }      // Y pos that piece started in
    public int targetX { set; get; }        // X pos to move piece to
    public int targetY { set; get; }        // Y pos to move piece to
    public int turnOver { set; get; }       // 1: the turn is now over, 0: the turn is not over (replace ore or orebearer second move)
    public int captureDir { set; get; }     // -1: No capture, 0: Capturing Y+1, 1: Capturing X+1, 2: Capturing Y-1, 3: Capturing X-1
                                            // Captures the piece on the corresponding square, never eat soggy waffles

    public NetCannonCapture()
    {
        Code = OpCode.CANNON_CAPTURE;
    }
    public NetCannonCapture(DataStreamReader reader)
    {
        Code = OpCode.CANNON_CAPTURE;
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
        writer.WriteInt(captureDir);
        writer.WriteInt(turnOver);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        teamID = reader.ReadInt();
        originalX = reader.ReadInt();
        originalY = reader.ReadInt();
        targetX = reader.ReadInt();
        targetY = reader.ReadInt();
        captureDir = reader.ReadInt();
        turnOver = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_CANNON_CAPTURE?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_CANNON_CAPTURE?.Invoke(this, cnn);
    }
}
