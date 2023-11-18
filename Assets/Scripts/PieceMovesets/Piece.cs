using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    Ore = 0,
    Mate = 1,
}

public class Piece : MonoBehaviour
{
    public PieceType type;
    public bool isNavy;
    public bool hasCaptured;
    public bool hasOre;
    public int currentX;
    public int currentY;

    private Vector3 desiredPosition;
    private Vector3 desiredScale;
}
