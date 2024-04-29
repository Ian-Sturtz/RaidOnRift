using Unity.Networking.Transport;
using Unity.Collections;
using UnityEngine;

public class NetIdentifyTeam : NetMessage
{
    public int teamID { set; get; }         // 0: Navy, 1: Pirates
    public int totalPoints { set; get; }    // Total points of a team
    public int Mate_Count { set; get; }     // How many Mates?
    public int Engineer_Count { set; get; }   // How many Engineers?
    public int Vanguard_Count { set; get; } // How many Vanguard?
    public int Navigator_Count { set; get; }// How many Navigators?
    public int Gunner_Count { set; get; }   // How many Gunners?
    public int Cannon_Count { set; get; }   // How many Cannons?
    public int Quartermaster_Count { set; get; }    // How many Quartermasters
    public int Royal2_Count { set; get; }   // How many Royal 2s?
    public int Royal1_Count { set; get; }   // How many Royal 1s?


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

        writer.WriteInt(teamID);
        writer.WriteInt(totalPoints);
        writer.WriteInt(Mate_Count);
        writer.WriteInt(Engineer_Count);
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
        teamID = reader.ReadInt();
        totalPoints = reader.ReadInt();
        Mate_Count = reader.ReadInt();
        Engineer_Count = reader.ReadInt();
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
