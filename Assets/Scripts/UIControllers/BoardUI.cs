// Implemented by Garrett Slattengren

using System.Collections;
using TMPro;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text turnDisplay;
    [SerializeField] private TMP_Text goalDisplay;
    private bool temporaryTextActive = false;
    private bool navyHasOre = false;
    private bool pirateHasOre = false;

    private bool navyTurn = true;
    public bool gameOver = false;

    [SerializeField] private GameObject animTextObj;   
    [SerializeField] private TMP_Text animText;
    [SerializeField] private GameObject animBackground;

    [SerializeField] private GameObject pieceDisplay;
    [SerializeField] private TMP_Text pieceDisplayName;
    [SerializeField] private TMP_Text pieceDescription;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject particles;

    [SerializeField] private GameObject rematchButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject rematchPrompt;

    private GameObject gameBoardObject;
    private GameBoard board;

    private bool[] playerRematch = new bool[2];

    private Coroutine a, b;

    private void Awake()
    {
        RegisterToEvents();

        gameBoardObject = GameObject.FindGameObjectWithTag("GameBoard");
        board = gameBoardObject.GetComponent<GameBoard>();
    }

    private void OnDestroy()
    {
        UnRegisterToEvents();
    }

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

        PlayVictory(navyWon);
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

    IEnumerator AnimBackground(bool gameOver = false)
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
        if(!gameOver) { 
            while(timeElapsed < 0.25f)
            {
                float val = Mathf.SmoothStep(250, 0, timeElapsed / 0.25f);
                animBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, val);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            animBackground.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            rematchButton.SetActive(true);
            quitButton.SetActive(true);

            Image image1 = rematchButton.GetComponent<Image>();
            Image image2 = quitButton.GetComponent<Image>();
            TMP_Text text1 = rematchButton.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text text2 = quitButton.transform.GetChild(0).GetComponent<TMP_Text>();
            timeElapsed = 0;
            while(timeElapsed < 0.5f)
            {
                float val = Mathf.SmoothStep(0, 1, timeElapsed / 0.5f);
                image1.color = new UnityEngine.Color(0, 0, 0, val);
                image2.color = new UnityEngine.Color(0, 0, 0, val);
                text1.color = new UnityEngine.Color(1, 1, 1, val);
                text2.color = new UnityEngine.Color(1, 1, 1, val);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            image1.color = new UnityEngine.Color(0, 0, 0, 1);
            image2.color = new UnityEngine.Color(0, 0, 0, 1);
            text1.color = new UnityEngine.Color(1, 1, 1, 1);
            text2.color = new UnityEngine.Color(1, 1, 1, 1);
        }
    }

    IEnumerator AnimText(bool gameOver = false)
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

        if (!gameOver)
        {
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
    }

    public void PlayVictory(bool winner)
    {
        if (a != null)
        {
            StopCoroutine(a);
            StopCoroutine(b);
        }


        if (winner)
        {

            animText.SetText("NAVY WINS");
            animText.color = new UnityEngine.Color(0, 0.03921569f, 0.6666667f, 1);
        }
        else
        {
            animText.SetText("PIRATES WIN");
            animText.color = new UnityEngine.Color(0.4588234f, 0f, 0f, 1f);
        }
        a = StartCoroutine(AnimBackground(true));
        b = StartCoroutine(AnimText(true));
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
                    pieceDescription.SetText("Moves any unblocked distance in any direction, captures by landing on an enemy.");
                }
                else
                {
                    pieceDisplayName.SetText("Captain");
                    pieceDescription.SetText("Moves exactly five squares in any up/down or left/right direction, and can change direction mid-move. Jumps over blockers, and captures any enemy piece in the fifth square by landing on an enemy.");
                }
                break;
            case PieceType.Royal2:
                if (isNavy)
                {
                    pieceDisplayName.SetText("Tactician");
                    pieceDescription.SetText("Moves up to two unblocked squares up/down or left/right, captures by landing on an enemy. Can also use the moveset of any enemy piece within whatever zone the Tactician is in (the 3 rows on each player's side of the board, and the 4 rows in the middle).");
                }
                else
                {
                    pieceDisplayName.SetText("Corsair");
                    pieceDescription.SetText("Moves one space diagonally, captures by landing on an enemy, or can jump to any open square on the board. If the corsair jumps this way, the corsair cannot move on the following turn.");
                }
                break;
            case PieceType.Mate:
                pieceDisplayName.SetText("Mate");
                pieceDescription.SetText("Moves one square in any direction, but cannot move backwards unless the Mate has captured an enemy piece. captures by landing on an enemy.");
                break;
            case PieceType.Quartermaster:
                pieceDisplayName.SetText("Quartermaster");
                pieceDescription.SetText("Moves two spaces up/down or left/right and one space perpendicularly. Jumps over blockers and captures by landing on an enemy.");
                break;
            case PieceType.Cannon:
                pieceDisplayName.SetText("Cannon");
                pieceDescription.SetText("Can move one unblocked space in any direction, captures by jumping any unblocked distance up/down or left/right and must land on the opposite next to square to the captured piece. An Energy Shield can be jumped over in this way, but it won’t be captured.");
                break;
            case PieceType.Bomber:
                pieceDisplayName.SetText("Engineer");
                pieceDescription.SetText("Moves up to two unblocked squares in any direction. The only piece that can capture Energy Shields by landing on them, but cannot capture any other piece besides the flag. Can return one Energy Shield from the Jail Zone to the game board in any open square next to to the Miner. Cannot move if a Jail Zone has been returned to the game board this turn.");
                break;
            case PieceType.Vanguard:
                pieceDisplayName.SetText("Vanguard");
                pieceDescription.SetText("Moves one square forward or backward, both up/down or left/right and diagonally, but can move any unblocked distance sideways. captures by landing on an enemy.");
                break;
            case PieceType.Navigator:
                pieceDisplayName.SetText("Navigator");
                pieceDescription.SetText("Moves one square sideways, both up/down or left/right and diagonally, but can move any unblocked distance forwards and backwards.");
                break;
            case PieceType.Gunner:
                pieceDisplayName.SetText("Gunner");
                pieceDescription.SetText("Moves one unblocked space in any direction, captures by shooting a piece up to 4 unblocked spaces away in any direction. Cannot capture by landing on enemy and must move after capturing a piece before the Gunner can capture another piece.");
                break;
        }
    }

    public void HideSelectedPiece()
    {
        pieceDisplay.SetActive(false);
    }

    public void Rematch()
    {
        // Set up rematch for online here
        if (PieceManager.instance.onlineMultiplayer)
        {
            NetRematch rm = new NetRematch();

            rm.teamID = MultiplayerController.Instance.currentTeam;
            rm.wantRematch = 1;

            Client.Instance.SendToServer(rm);
        }
        else
            SceneManager.LoadScene("Piece Selection");
    }

    public void QuitToMenu()
    {
        if (PieceManager.instance.onlineMultiplayer)
        {
            MultiplayerController.Instance.gameWon = board.gameWon ? 1 : 0;
            SceneManager.LoadScene("Connection Dropped");
        }

        else
            SceneManager.LoadScene("Main Menu");
    }

    private void RegisterToEvents()
    {
        NetUtility.S_REMATCH += OnRematchServer;

        NetUtility.C_REMATCH += OnRematchClient;
    }
    private void UnRegisterToEvents()
    {
        NetUtility.S_REMATCH -= OnRematchServer;

        NetUtility.C_REMATCH -= OnRematchClient;
    }

    // Server
    private void OnRematchServer(NetMessage msg, NetworkConnection cnn)
    {
        Server.Instance.Broadcast(msg);
    }

    // Client
    private void OnRematchClient(NetMessage msg)
    {
        NetRematch rm = msg as NetRematch;

        // Set the boolean for rematch
        playerRematch[rm.teamID] = rm.wantRematch == 1;

        // Activate UI to inform player that opponent wants a rematch
        if (MultiplayerController.Instance.currentTeam != rm.teamID)
        {
            Debug.Log("Opponent wants a rematch");
            rematchPrompt.SetActive(true);
        }

        // If both players want a rematch
        if (playerRematch[0] && playerRematch[1])
        {
            SceneManager.LoadScene("Board");
        }
    }
}
