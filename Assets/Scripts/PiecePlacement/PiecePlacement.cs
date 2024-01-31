using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePlacement : MonoBehaviour
{
    public PPJailBoard jail;
    public PPGameBoard board;

    // Start is called before the first frame update
    void Start()
    {
            Piece piece;
            if (PieceManager.instance != null)
            {
                Debug.Log(PieceManager.instance.navyFirst);


                for (int i = PieceManager.instance.navyRoyal1; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Royal1, true, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.navyRoyal2; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Royal2, true, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.navyQuartermaster; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Quartermaster, true, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.navyCannon; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Cannon, true, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.navyBomber; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Bomber, true, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.navyVanguard; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Vanguard, true, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.navyNavigator; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Navigator, true, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.navyGunner; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Gunner, true, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.navyMate; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Mate, true, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }

                for (int i = PieceManager.instance.pirateRoyal1; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Royal1, false, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.pirateRoyal2; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Royal2, false, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.pirateQuartermaster; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Quartermaster, false, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.pirateCannon; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Cannon, false, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.pirateBomber; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Bomber, false, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.pirateVanguard; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Vanguard, false, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.pirateNavigator; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Navigator, false, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.pirateGunner; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Gunner, false, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
                for (int i = PieceManager.instance.pirateMate; i > 0; i--)
                {
                    piece = board.SpawnPiece(PieceType.Mate, false, 0, 0);
                    jail.InsertAPiece(piece);
                    piece.destroyPiece();
                }
            }
            else
            {
                Debug.Log("Piece manager not loaded, using default loadout");

                // Default selection, used for testing
                piece = board.SpawnPiece(PieceType.Royal1, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Royal2, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Quartermaster, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Cannon, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Cannon, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Bomber, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Bomber, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Vanguard, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Vanguard, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Navigator, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Navigator, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Gunner, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Gunner, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Mate, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Mate, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Mate, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Mate, true, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();

                piece = board.SpawnPiece(PieceType.Royal1, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Royal2, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Quartermaster, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Cannon, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Cannon, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Bomber, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Bomber, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Vanguard, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Vanguard, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Navigator, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Navigator, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Gunner, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Gunner, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Mate, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Mate, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Mate, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
                piece = board.SpawnPiece(PieceType.Mate, false, 0, 0);
                jail.InsertAPiece(piece);
                piece.destroyPiece();
            }

            piece = board.SpawnPiece(PieceType.Ore, true, 0, 0);
            jail.InsertAPiece(piece);
            piece.destroyPiece();
            piece = board.SpawnPiece(PieceType.LandMine, true, 0, 0);
            jail.InsertAPiece(piece);
            piece.destroyPiece();
            piece = board.SpawnPiece(PieceType.LandMine, true, 0, 0);
            jail.InsertAPiece(piece);
            piece.destroyPiece();
            piece = board.SpawnPiece(PieceType.LandMine, true, 0, 0);
            jail.InsertAPiece(piece);
            piece.destroyPiece();
            piece = board.SpawnPiece(PieceType.LandMine, true, 0, 0);
            jail.InsertAPiece(piece);
            piece.destroyPiece();

            piece = board.SpawnPiece(PieceType.Ore, false, 0, 0);
            jail.InsertAPiece(piece);
            piece.destroyPiece();
            piece = board.SpawnPiece(PieceType.LandMine, false, 0, 0);
            jail.InsertAPiece(piece);
            piece.destroyPiece();
            piece = board.SpawnPiece(PieceType.LandMine, false, 0, 0);
            jail.InsertAPiece(piece);
            piece.destroyPiece();
            piece = board.SpawnPiece(PieceType.LandMine, false, 0, 0);
            jail.InsertAPiece(piece);
            piece.destroyPiece();
            piece = board.SpawnPiece(PieceType.LandMine, false, 0, 0);
            jail.InsertAPiece(piece);
            piece.destroyPiece();
        
    }
}
