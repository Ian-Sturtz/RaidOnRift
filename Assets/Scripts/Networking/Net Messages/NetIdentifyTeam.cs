using Unity.Networking.Transport;
using Unity.Collections;
using UnityEngine;

public class NetIdentifyTeam : NetMessage
{
    public int Mate_Count { set; get; }
    public int Bomber_Count { set; get; }
    public int Vanguard_Count { set; get; }
    public int Navigator_Count { set; get; }
    public int Gunner_Count { set; get; }
    public int Cannon_Count { set; get; }
    public int Quartermaster_Count { set; get; }
    public int Royal2_Count { set; get; }
    public int Royal1_Count { set; get; }


    public NetIdentifyTeam()
    {
        Code = OpCode.IDENTIFY_TEAM;
    }
    public NetIdentifyTeam(DataStreamReader reader)
    {
        Code = OpCode.IDENTIFY_TEAM;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);

        writer.WriteInt(Mate_Count);
        writer.WriteInt(Bomber_Count);
        writer.WriteInt(Vanguard_Count);
        writer.WriteInt(Navigator_Count);
        writer.WriteInt(Gunner_Count);
        writer.WriteInt(Cannon_Count);
        writer.WriteInt(Quartermaster_Count);
        writer.WriteInt(Royal2_Count);
        writer.WriteInt(Royal1_Count);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        Mate_Count = reader.ReadInt();
        Bomber_Count = reader.ReadInt();
        Vanguard_Count = reader.ReadInt();
        Navigator_Count = reader.ReadInt();
        Gunner_Count = reader.ReadInt();
        Cannon_Count = reader.ReadInt();
        Quartermaster_Count = reader.ReadInt();
        Royal2_Count = reader.ReadInt();
        Royal1_Count = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_IDENTIFY_TEAM?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_IDENTIFY_TEAM?.Invoke(this, cnn);
    }
}
