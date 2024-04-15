// Implemented by Garrett Slattengren

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text turnDisplay;
    [SerializeField] private TMP_Text goalDisplay;
    private bool temporaryTextActive = false;
    private bool navyHasOre = false;
    private bool pirateHasOre = false;

    private bool navyTurn = true;
    private bool gameOver = false;

    [SerializeField] private GameObject animTextObj;   
    [SerializeField] private TMP_Text animText;
    [SerializeField] private GameObject animBackground;

    [SerializeField] private GameObject pieceDisplay;
    [SerializeField] private TMP_Text pieceDisplayName;
    [SerializeField] private TMP_Text pieceDescription;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject particles;

    private Coroutine a, b;

    private void Update()
    {
        if (navyTurn && !gameOver) { 
        
            turnDisplay.SetText("NAVY'S TURN");
            turnDisplay.color = new UnityEngine.Color(0, 0.03921569f, 0.6666667f, 1f);
        }
        else if(!gameOver)
        {
            turnDisplay.SetText("PIRATE'S TURN");
            turnDisplay.color = new UnityEngine.Color(0.4588234f, 0f, 0f, 1f);
        }
    }

    public void GameWon(bool navyWon, bool stalemate = false)
    {
        gameOver = true;

        GoalText("THE GAME IS OVER");
        if(navyWon && stalemate)
        {
            turnDisplay.SetText("STALEMATE! NAVY WINS!");
            turnDisplay.color = new UnityEngine.Color(0, 0.03921569f, 0.6666667f, 1);
        }else if (stalemate)
        {
            turnDisplay.SetText("STALEMATE! PIRATES WIN!");
            turnDisplay.color = new UnityEngine.Color(0.4588234f, 0f, 0f, 1f);
        }
        else if (navyWon)
        {
            turnDisplay.SetText("NAVY WINS!");
            turnDisplay.color = new UnityEngine.Color(0, 0.03921569f, 0.6666667f, 1);
        }
        else
        {
            turnDisplay.SetText("PIRATES WIN!");
            turnDisplay.color = new UnityEngine.Color(0.4588234f, 0f, 0f, 1f);
        }
    }

    public void UpdateGoal(bool navyTurn, bool hasOre)
    {
        if(navyTurn)
        {
            navyHasOre = hasOre;
        }
        else
        {
            pirateHasOre = hasOre;
        }
    }

    public void UpdateTurn(bool currentTurn)
    {
        navyTurn = currentTurn;
    }

    public void DisplayTempText(string text, float delay = 1f)
    {
        //StopAllCoroutines();
        temporaryTextActive = true;
        StartCoroutine(TemporaryText(text, delay));
    }

    // Sets the goal text to the provided text, or appends the provided text onto an existing goal text
    public void GoalText(string text, bool append = false)
    {
        temporaryTextActive = false;
        //StopAllCoroutines();

        if (!append)
        {
            goalDisplay.SetText(text);
        }
        else
        {
            string currentText = goalDisplay.text;

            if(currentText == "")
            {
                currentText = text;
            }
            else
            {
                currentText = currentText + "\n" + text;
            }

            goalDisplay.SetText(currentText);
        }
    }

    IEnumerator TemporaryText(string temporaryText, float duration = 1f)
    {
        string currentText = goalDisplay.text;

        goalDisplay.SetText(temporaryText);
        yield return new WaitForSeconds(duration);
        goalDisplay.SetText(currentText);
        temporaryTextActive = false;
    }

    public void PlayTurnAnim(bool turn)
    {
        if(a != null)
        {
            StopCoroutine(a);
            StopCoroutine(b);
        }
        

        if (turn)
        {

            animText.SetText("NAVY'S TURN");
            animText.color = new UnityEngine.Color(0, 0.03921569f, 0.6666667f, 1);
        }
        else
        {
            animText.SetText("PIRATE'S TURN");
            animText.color = new UnityEngine.Color(0.4588234f, 0f, 0f, 1f);
        }
        a = StartCoroutine(AnimBackground());
        b = StartCoroutine(AnimText());
    }

    IEnumerator AnimBackground()
    {
        animBackground.SetActive(true);
        float timeElapsed = 0;
        float startHeight = animBackground.GetComponent<RectTransform>().rect.height;

        while (timeElapsed < 0.25f)
        {
            float val = Mathf.SmoothStep(startHeight, 250, timeElapsed / 0.25f);
            animBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, val);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        animBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, 250);
        yield return new WaitForSeconds(0.5f);

        timeElapsed = 0;
        while(timeElapsed < 0.25f)
        {
            float val = Mathf.SmoothStep(250, 0, timeElapsed / 0.25f);
            animBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, val);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        animBackground.SetActive(false);
    }

    IEnumerator AnimText()
    {
        animTextObj.SetActive(true);
        float timeElapsed = 0;
        float red = animText.color.r;
        float green = animText.color.g;
        float blue = animText.color.b;
        //float alpha = animText.color.a;
        float alpha = 0;

        while (timeElapsed < 0.25f)
        {
            float val = Mathf.Lerp(alpha, 1, timeElapsed / 0.25f);
            animText.color = new UnityEngine.Color(red, green, blue, val);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        animText.color = new UnityEngine.Color(red, green, blue, 1);
        yield return new WaitForSeconds(0.5f);

        timeElapsed = 0;
        while (timeElapsed < 0.25f)
        {
            float val = Mathf.Lerp(1, 0, timeElapsed / 0.25f);
            animText.color = new UnityEngine.Color(red, green, blue, val);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        animTextObj.SetActive(false);
    }

    public IEnumerator AnimGunner(Vector3 startPos, Vector3 targetPos, bool isNavy)
    {
        particles.SetActive(true);
        var particleCom = particles.GetComponent<ParticleSystem>().main;
        
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, targetPos);
        particles.GetComponent<Transform>().transform.position = targetPos;
        UnityEngine.Color col;
        if(isNavy)
            col = new UnityEngine.Color(0, 0, 1, 1);
        else 
            col = new UnityEngine.Color(1, 0, 1, 1);

        particleCom.startColor = col;
        float timeElapsed = 0;
        while (timeElapsed < 0.5f)
        {
            float val = Mathf.Lerp(1, 0, timeElapsed / 0.5f);
            col.a = val;
            lineRenderer.startColor = col;
            lineRenderer.endColor = col;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void UpdateSelectedPiece(PieceType piece, bool isNavy)
    {
        if (piece == PieceType.Ore || piece == PieceType.LandMine)
            return;

        pieceDisplay.SetActive(true);
        switch (piece)
        {
            case PieceType.Royal1:
                if (isNavy)
                {
                    pieceDisplayName.SetText("Admiral");
                    pieceDescription.SetText("Moves any unblocked distance in any direction, captures by replacement.");
                }
                else
                {
                    pieceDisplayName.SetText("Captain");
                    pieceDescription.SetText("Moves exactly five squares in any orthogonal direction, and can change direction mid-move. Jumps over blockers, and captures any enemy piece in the fifth square by replacement.");
                }
                break;
            case PieceType.Royal2:
                if (isNavy)
                {
                    pieceDisplayName.SetText("Tactician");
                    pieceDescription.SetText("Moves up to two unblocked squares orthogonally, captures by replacement. Can also use the moveset of any enemy piece within whatever zone the Tactician is in (the 3 rows on each player's side of the board, and the 4 rows in the middle).");
                }
                else
                {
                    pieceDisplayName.SetText("Corsair");
                    pieceDescription.SetText("Moves one space diagonally, captures by replacement, or can jump to any open square on the board. If the corsair jumps this way, the corsair cannot move on the following turn.");
                }
                break;
            case PieceType.Mate:
                pieceDisplayName.SetText("Mate");
                pieceDescription.SetText("Moves one square in any direction, but cannot move backwards unless the Mate has captured an enemy piece. Captures by replacement.");
                break;
            case PieceType.Quartermaster:
                pieceDisplayName.SetText("Quartermaster");
                pieceDescription.SetText("Moves two spaces orthogonally and one space perpendicularly. Jumps over blockers and captures by replacement.");
                break;
            case PieceType.Cannon:
                pieceDisplayName.SetText("Cannon");
                pieceDescription.SetText("Can move one unblocked space in any direction, captures by jumping any unblocked distance orthogonally and must land on the opposite adjacent square to the captured piece. An Energy Shield can be jumped over in this way, but it won’t be captured.");
                break;
            case PieceType.Bomber:
                pieceDisplayName.SetText("Engineer");
                pieceDescription.SetText("Moves up to two unblocked squares in any direction. The only piece that can capture Energy Shields by replacement, but cannot capture any other piece besides the flag. Can return one Energy Shield from the Jail Zone to the game board in any open square adjacent to the Miner. Cannot move if a Jail Zone has been returned to the game board this turn.");
                break;
            case PieceType.Vanguard:
                pieceDisplayName.SetText("Vanguard");
                pieceDescription.SetText("Moves one square forward or backward, both orthogonally and diagonally, but can move any unblocked distance sideways. Captures by replacement.");
                break;
            case PieceType.Navigator:
                pieceDisplayName.SetText("Navigator");
                pieceDescription.SetText("Moves one square sideways, both orthogonally and diagonally, but can move any unblocked distance forwards and backwards.");
                break;
            case PieceType.Gunner:
                pieceDisplayName.SetText("Gunner");
                pieceDescription.SetText("Moves one unblocked space in any direction, captures by shooting a piece up to 4 unblocked spaces away in any direction. Cannot capture by replacement and must move after capturing a piece before the Gunner can capture another piece.");
                break;
        }
    }

    public void HideSelectedPiece()
    {
        pieceDisplay.SetActive(false);
    }
}
