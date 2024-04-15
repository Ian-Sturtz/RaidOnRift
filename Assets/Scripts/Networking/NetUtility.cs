using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public enum OpCode
{
    KEEP_ALIVE = 1,
    WELCOME = 2,
    START_GAME = 3,
    IDENTIFY_TEAM = 4,
    POSITION_PIECE = 5,
    MOVE_PIECE = 6,
    REMATCH = 7
}

public static class NetUtility
{
    public static void OnData(DataStreamReader stream, NetworkConnection cnn, Server server = null)
    {
        NetMessage msg = null;
        var opCode = (OpCode)stream.ReadByte();
        switch (opCode)
        {
            case OpCode.KEEP_ALIVE: msg = new NetKeepAlive(stream); break;
            case OpCode.WELCOME: msg = new NetWelcome(stream); break;
            case OpCode.START_GAME: msg = new NetStartGame(stream); break;
            case OpCode.IDENTIFY_TEAM: msg = new NetIdentifyTeam(stream); break;
            case OpCode.POSITION_PIECE: msg = new NetPositionPiece(stream); break;
            case OpCode.MOVE_PIECE: msg = new NetMovePiece(stream); break;
            //case OpCode.REMATCH: msg = new NetRematch(stream); break;

            default:
                Debug.LogError("Message received had no OpCode");
                break;
        }

        if (server != null)
        {
            msg.ReceivedOnServer(cnn);
        }
        else
        {
            msg.ReceivedOnClient();
        }
    }

    // Net messages
    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_IDENTIFY_TEAM;
    public static Action<NetMessage> C_POSITION_PIECE;
    public static Action<NetMessage> C_MOVE_PIECE;
    public static Action<NetMessage> C_REMATCH;

    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_START_GAME;
    public static Action<NetMessage, NetworkConnection> S_IDENTIFY_TEAM;
    public static Action<NetMessage, NetworkConnection> S_POSITION_PIECE;
    public static Action<NetMessage, NetworkConnection> S_MOVE_PIECE;
    public static Action<NetMessage, NetworkConnection> S_REMATCH;

}
