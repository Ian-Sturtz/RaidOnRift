using Unity.Networking.Transport;
using Unity.Collections;
using UnityEngine;

public class NetWelcome : NetMessage
{
    public int AssignedTeam { set; get; }
    public double VersionNumber { set; get; }

    public NetWelcome()
    {
        Code = OpCode.WELCOME;
        double version;
        if (double.TryParse(Application.version, out version))
        {
            Debug.Log(version);
            VersionNumber = version;
        }
        else
        {
            Debug.Log("Version could not be found");
        }
    }
    public NetWelcome(DataStreamReader reader)
    {
        
        Code = OpCode.WELCOME;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(AssignedTeam);
        writer.WriteDouble(VersionNumber);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        AssignedTeam = reader.ReadInt();
        VersionNumber = reader.ReadDouble();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_WELCOME?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_WELCOME?.Invoke(this, cnn);
    }
}
