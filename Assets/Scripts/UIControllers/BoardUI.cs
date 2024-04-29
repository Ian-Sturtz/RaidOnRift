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
    [SerializeField] private GameObject viewBoard;
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
            viewBoard.SetActive(true);

            Image image1 = rematchButton.GetComponent<Image>();
            Image image2 = quitButton.GetComponent<Image>();
            Image image3 = viewBoard.GetComponent<Image>();
            TMP_Text text1 = rematchButton.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text text2 = quitButton.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text text3 = rematchButton.transform.GetChild(0).GetComponent<TMP_Text>();
            timeElapsed = 0;
            while(timeElapsed < 0.5f)
            {
                float val = Mathf.SmoothStep(0, 1, timeElapsed / 0.5f);
                image1.color = new UnityEngine.Color(0, 0, 0, val);
                image2.color = new UnityEngine.Color(0, 0, 0, val);
                image3.color = new UnityEngine.Color(0, 0, 0, val);
                text1.color = new UnityEngine.Color(1, 1, 1, val);
                text2.color = new UnityEngine.Color(1, 1, 1, val);
                text3.color = new UnityEngine.Color(1, 1, 1, val);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            image1.color = new UnityEngine.Color(0, 0, 0, 1);
            image2.color = new UnityEngine.Color(0, 0, 0, 1);
            image3.color = new UnityEngine.Color(0, 0, 0, 1);
            text1.color = new UnityEngine.Color(1, 1, 1, 1);
            text2.color = new UnityEngine.Color(1, 1, 1, 1);
            text3.color = new UnityEngine.Color(1, 1, 1, 1);
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

    public void UpdateSelectedPiece(PieceType piece, bool isNavy, bool hasOre = false, bool specialProperty = false)
    {
        pieceDisplay.SetActive(true);

        if ((piece == PieceType.Ore && navyTurn == isNavy) || (piece == PieceType.EnergyShield && navyTurn == isNavy))
            if (!specialProperty) return;

        if (hasOre)
        {
            pieceDisplayName.SetText("Orebearer");
            pieceDescription.SetText("The Orebearer can move one square in any direction, but can move a second time if it captures an enemy.");
            return;
        }

        switch (piece)
        {
            case PieceType.Royal1:
                if (isNavy)
                {
                    pieceDisplayName.SetText("Admiral");
                    pieceDescription.SetText("Moves any unblocked distance in any direction. She captures by landing on an enemy.");
                }
                else
                {
                    pieceDisplayName.SetText("Captain");
                    pieceDescription.SetText("Moves exactly five squares in any up/down or left/right direction, and can change direction while moving. He jumps over blockers, and captures enemies in the fifth square by landing on them.");
                }
                break;
            case PieceType.Royal2:
                if (isNavy)
                {
                    pieceDisplayName.SetText("Tactician");
                    pieceDescription.SetText("Moves up to two open squares up/down or left/right, and captures by landing on an enemy. It can also use the moveset of any enemy within whatever zone the it's in (the 3 rows on each player's side of the board, or the 4 rows in the middle).");
                }
                else
                {
                    pieceDisplayName.SetText("Corsair");
                    pieceDescription.SetText("Moves any open distance diagonally, and captures by landing on an enemy while moving this way. She can also jump to any open square on the board, but if she does, she can't jump on her next turn.");
                }
                break;
            case PieceType.Mate:
                pieceDisplayName.SetText("Mate");
                pieceDescription.SetText("Moves one square in any direction, but cannot move backwards unless he's captured an enemy piece. He captures by landing on an enemy.");
                break;
            case PieceType.Quartermaster:
                pieceDisplayName.SetText("Quartermaster");
                pieceDescription.SetText("Moves two spaces up/down or left/right and one space perpendicular. He jumps over enemies in his way and captures by landing on an enemy.");
                break;
            case PieceType.Cannon:
                pieceDisplayName.SetText("Cannon");
                pieceDescription.SetText("Moves one open square in any direction, or captures by jumping over enemies. He can cross any open distance while jumping, but he needs space on the other side to land or else he can't make the jump. He can jump over Energy Shields and allies like this, but doesn't capture them.");
                break;
            case PieceType.Engineer:
                pieceDisplayName.SetText("Engineer");
                pieceDescription.SetText("Moves up to two open squares in any direction. He's the only piece that can capture Energy Shields by landing on them and can redeploy spare Energy Shields back to the board, but he can't capture any enemies unless he's armed with a spare Energy Shield.");
                break;
            case PieceType.Vanguard:
                pieceDisplayName.SetText("Vanguard");
                pieceDescription.SetText("Moves one square forwards or backwards, both up/down or left/right and diagonally, but can move any open distance sideways. He captures by landing on an enemy.");
                break;
            case PieceType.Navigator:
                pieceDisplayName.SetText("Navigator");
                pieceDescription.SetText("Moves one square sideways, both up/down or left/right and diagonally, but can move any open distance forwards and backwards. He captures by landing on an enemy.");
                break;
            case PieceType.Gunner:
                pieceDisplayName.SetText("Gunner");
                pieceDescription.SetText("Moves one open space in any direction, or he captures by shooting an enemy up to 3 unblocked spaces away in any direction. He has to move somewhere to reload his weapon before he can capture again. Be warned, he can't shoot the enemy Ore or he might destroy it!");
                break;
            case PieceType.Ore:
                pieceDisplayName.SetText("Ore");
                pieceDescription.SetText("The most concentrated energy supply in the cosmos. Protect it with your life and get your enemy's supply as quickly as you can!");
                break;
            case PieceType.EnergyShield:
                pieceDisplayName.SetText("Energy Shield");
                pieceDescription.SetText("Defenses made from pure energy. No force can get through them, but some are able to get over top of them. Engineers are skilled at building and dismantling them.");
                break;
        }
    }

    public void SetPieceDisplay(string pieceNameText, string pieceDescriptionText)
    {
        pieceDisplay.SetActive(true);
        pieceDisplayName.SetText(pieceNameText);
        PieceDisplayDescription(pieceDescriptionText);
    }

    public void PieceDisplayDescription(string pieceDescriptionText, bool append = false)
    {
        if (append)
        {
            string currentText = pieceDescription.text;

            pieceDescription.SetText(currentText + "\n" + pieceDescriptionText);
        }
        else
            pieceDescription.SetText(pieceDescriptionText);
    }
    public void TTUpdateSelectedPiece(TTPieceType piece, bool isNavy)
    {
        if (piece == TTPieceType.Ore || piece == TTPieceType.LandMine)
            return;

        pieceDisplay.SetActive(true);
        switch (piece)
        {
            case TTPieceType.Royal1:
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
            case TTPieceType.Royal2:
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
            case TTPieceType.Mate:
                pieceDisplayName.SetText("Mate");
                pieceDescription.SetText("Moves one square in any direction, but cannot move backwards unless the Mate has captured an enemy piece. Captures by replacement.");
                break;
            case TTPieceType.Quartermaster:
                pieceDisplayName.SetText("Quartermaster");
                pieceDescription.SetText("Moves two spaces orthogonally and one space perpendicularly. Jumps over blockers and captures by replacement.");
                break;
            case TTPieceType.Cannon:
                pieceDisplayName.SetText("Cannon");
                pieceDescription.SetText("Can move one unblocked space in any direction, captures by jumping any unblocked distance orthogonally and must land on the opposite adjacent square to the captured piece. An Energy Shield can be jumped over in this way, but it won�t be captured.");
                break;
            case TTPieceType.Bomber:
                pieceDisplayName.SetText("Engineer");
                pieceDescription.SetText("Moves up to two unblocked squares in any direction. The only piece that can capture Energy Shields by replacement, but cannot capture any other piece besides the flag. Can return one Energy Shield from the Jail Zone to the game board in any open square adjacent to the Miner. Cannot move if a Jail Zone has been returned to the game board this turn.");
                break;
            case TTPieceType.Vanguard:
                pieceDisplayName.SetText("Vanguard");
                pieceDescription.SetText("Moves one square forward or backward, both orthogonally and diagonally, but can move any unblocked distance sideways. Captures by replacement.");
                break;
            case TTPieceType.Navigator:
                pieceDisplayName.SetText("Navigator");
                pieceDescription.SetText("Moves one square sideways, both orthogonally and diagonally, but can move any unblocked distance forwards and backwards.");
                break;
            case TTPieceType.Gunner:
                pieceDisplayName.SetText("Gunner");
                pieceDescription.SetText("Moves one unblocked space in any direction, captures by shooting a piece up to 4 unblocked spaces away in any direction. Cannot capture by replacement and must move after capturing a piece before the Gunner can capture another piece.");
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
            rematchButton.SetActive(false);
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
