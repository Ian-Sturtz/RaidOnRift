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
    CAPTURE_PIECE = 7,
    CANNON_CAPTURE = 8,
    RESPAWN = 9,
    GAME_WON = 10,
    REMATCH = 11
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
            case OpCode.CAPTURE_PIECE: msg = new NetCapturePiece(stream); break;
            case OpCode.CANNON_CAPTURE: msg = new NetCannonCapture(stream); break;
            case OpCode.RESPAWN: msg = new NetRespawn(stream); break;
            case OpCode.GAME_WON: msg = new NetGameWon(stream); break;
            case OpCode.REMATCH: msg = new NetRematch(stream); break;

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
    public static Action<NetMessage> C_CAPTURE_PIECE;
    public static Action<NetMessage> C_CANNON_CAPTURE;
    public static Action<NetMessage> C_RESPAWN;
    public static Action<NetMessage> C_GAME_WON;
    public static Action<NetMessage> C_REMATCH;

    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_START_GAME;
    public static Action<NetMessage, NetworkConnection> S_IDENTIFY_TEAM;
    public static Action<NetMessage, NetworkConnection> S_POSITION_PIECE;
    public static Action<NetMessage, NetworkConnection> S_MOVE_PIECE;
    public static Action<NetMessage, NetworkConnection> S_CAPTURE_PIECE;
    public static Action<NetMessage, NetworkConnection> S_CANNON_CAPTURE;
    public static Action<NetMessage, NetworkConnection> S_RESPAWN;
    public static Action<NetMessage, NetworkConnection> S_GAME_WON;
    public static Action<NetMessage, NetworkConnection> S_REMATCH;

}
