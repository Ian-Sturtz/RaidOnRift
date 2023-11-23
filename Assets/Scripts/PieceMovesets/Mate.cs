using UnityEngine;

public class Mate : Piece
{
    public int[,] GetValidMoves(GameObject[,] tiles)
    {
        int[,] moveAssessment;
        
        moveAssessment = new int[10, 10];

        for (int x = 0; x < 10; x++)
            for (int y = 0; y < 10; y++)
                moveAssessment[x, y] = -1;

        GameObject currentSquare = tiles[currentX, currentY];

        // For all squares +/- 1 away from current position
        for (int x_change = -1; x_change < 2; x_change++)
        {
            for (int y_change = -1; y_change < 2; y_change++)
            {
                if (y_change >= 0 || hasCaptured)
                {
                    if (IsSquareOnBoard(currentX + x_change, currentY + y_change))
                    {
                        moveAssessment[currentX + x_change, currentY + y_change] = 1;
                    }
                }
            }
        }

        moveAssessment[currentX, currentY] = 0;

        return moveAssessment;
    }
}
