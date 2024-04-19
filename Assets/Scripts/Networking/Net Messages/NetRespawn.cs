using Unity.Collections;
using Unity.Networking.Transport;

public class NetRespawn : NetMessage
{
    public int teamID { set; get; }         // 0: Navy, 1: Pirates
    public int jailIndex { set; get; }      // What index is the piece in jail?
    public int targetX { set; get; }        // X pos to spawn piece
    public int targetY { set; get; }        // Y pos to spawn piece
    public int specialPiece { set; get; }   // -1: nothing; 0: Shield Deployed, 1: Ore deployed + turn over, 2: Ore deployed + turn not over

    public NetRespawn()
    {
        Code = OpCode.RESPAWN;
    }
    public NetRespawn(DataStreamReader reader)
    {
        Code = OpCode.RESPAWN;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(teamID);
        writer.WriteInt(jailIndex);
        writer.WriteInt(targetX);
        writer.WriteInt(targetY);
        writer.WriteInt(specialPiece);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        teamID = reader.ReadInt();
        jailIndex = reader.ReadInt();
        targetX = reader.ReadInt();
        targetY = reader.ReadInt();
        specialPiece = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_RESPAWN?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_RESPAWN?.Invoke(this, cnn);
    }
}
