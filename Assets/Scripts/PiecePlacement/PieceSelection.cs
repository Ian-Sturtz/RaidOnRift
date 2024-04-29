using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class PieceSelection : MonoBehaviour
{
    private bool p1Navy;
    private bool navySelecting;

    #region Piece Tally
    
    private int royal1 = 0;
    private int royal2 = 0;
    private int mate = 5;
    private int quartermaster = 0;
    private int cannon = 0;
    private int bomber = 0;
    private int vanguard = 0;
    private int navigator = 0;
    private int gunner = 0;

    private int totalPoints = 5;

    [SerializeField] private int navyTotal = 0;
    [SerializeField] private int pirateTotal = 0;
    
    #endregion

    #region PS Menus

    [SerializeField] private GameObject FactionSelectMenu;
    [SerializeField] private GameObject NavySelectMenu;
    [SerializeField] private GameObject PirateSelectMenu;
    [SerializeField] private GameObject WaitingForOpponentMenu;

    #endregion

    #region PS Timer
    [SerializeField] private TimerController timer;
    
    #endregion

    #region UI
    
    [SerializeField] private GameObject navyUI;
    [SerializeField] private GameObject pirateUI;

    [SerializeField] private TMP_Text navyPoints;
    [SerializeField] private TMP_Text piratePoints;

    private TMP_Text pointsText;
    private VideoPlayer videoPlayer;
    private VideoManager videoManager;
    
    [SerializeField] private TMP_Text navyPlayer;
    [SerializeField] private TMP_Text piratePlayer;

    [SerializeField] private TMP_Text navyInfoText;
    [SerializeField] private TMP_Text pirateInfoText;
    
    [SerializeField] private VideoPlayer navyVideo;
    [SerializeField] private VideoPlayer pirateVideo;
    #endregion

    #region Point Values/Info
    [SerializeField] private int royal1Points = 22;
    [SerializeField] private int royal2Points = 16;
    [SerializeField] private int matePoints = 1;
    [SerializeField] private int quartermasterPoints = 7;
    [SerializeField] private int cannonPoints = 7;
    [SerializeField] private int bomberPoints = 3;
    [SerializeField] private int vanguardPoints = 6;
    [SerializeField] private int navigatorPoints = 5;
    [SerializeField] private int gunnerPoints = 7;

    [SerializeField] private int maxPoints = 100;
    [SerializeField] private int maxRoyals = 1;
    [SerializeField] private int maxArmy = 2;
    [SerializeField] private int maxPeasants = 10;
    [SerializeField] private int minPeasants = 5;
    #endregion


    private void Start()
    {
        videoManager = this.GetComponent<VideoManager>();

        if(MultiplayerController.Instance != null)
        {
            if(Server.Instance.isActive || Client.Instance.isActive)
                PieceManager.instance.onlineMultiplayer = true;

            if(PieceManager.instance.onlineMultiplayer)
            {
                timer.gameObject.SetActive(true);
                timer.startTimer();

                if(MultiplayerController.Instance.currentTeam == 0)
                {
                    OnNavyChosen();
                }
                else
                {
                    OnPiratesChosen();
                }

                RegisterEvents();
            }
        }
    }

    private void OnDestroy()
    {
        UnRegisterEvents();
    }

    private void Update()
    {
        // An online game is running and both teams have confirmed their pieces
        // It's time to move onto the next scene
        if(PieceManager.instance.onlineMultiplayer && navyTotal > 0 && pirateTotal > 0)
        {
            timer.stopTimer();

            if (navyTotal < pirateTotal)
                PieceManager.instance.navyFirst = true;
            else if (navyTotal > pirateTotal)
                PieceManager.instance.navyFirst = false;
            else
            {
                if (Server.Instance.isActive)
                {
                    Debug.Log("This is a multiplayer game, the host is going first");
                    PieceManager.instance.navyFirst = MultiplayerController.Instance.currentTeam == 0;
                }
                else
                {
                    Debug.Log("This is a multiplayer game, the client is not going first");
                    PieceManager.instance.navyFirst = MultiplayerController.Instance.currentTeam == 1;
                }
            }

            Debug.Log("Navy first: " + PieceManager.instance.navyFirst);

            MultiplayerController.Instance.gameWon = -1;

            SceneManager.LoadScene("Piece Placement");
        }
    }

    public void OnNavyChosen()
    {
        FactionSelectMenu.SetActive(false);
        NavySelectMenu.SetActive(true);
        chosenFaction(true);
    }

    public void OnPiratesChosen()
    {
        FactionSelectMenu.SetActive(false);
        PirateSelectMenu.SetActive(true);
        chosenFaction(false);
    }

    public void chosenFaction(bool choseNavy)
    {
        p1Navy = choseNavy;
        navySelecting = choseNavy;
        if(navySelecting)
        {
            pointsText = navyPoints;
            videoPlayer = navyVideo;
        }
        else
        {
            pointsText = piratePoints;
            videoPlayer = pirateVideo;
        }
        videoManager.Setup(navySelecting);
    }

    public void IncreaseCount(GameObject parent)
    {
        string name = parent.name;
        TMP_Text amount = parent.transform.Find("Amount").GetComponent<TMP_Text>();

        if (name == "Select Royal1" && royal1 < maxRoyals && totalPoints + royal1Points <= maxPoints)
        {
            royal1++;
            amount.SetText(royal1 + "/" + maxRoyals);
            totalPoints += royal1Points;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Royal2" && royal2 < maxRoyals && totalPoints + royal2Points <= maxPoints)
        {
            royal2++;
            amount.SetText(royal2 + "/" + maxRoyals);
            totalPoints += royal2Points;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Mate" && mate < maxPeasants && totalPoints + matePoints <= maxPoints)
        {
            mate++;
            amount.SetText(mate + "/" + maxPeasants);
            totalPoints += matePoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Quartermaster" && quartermaster < maxArmy && totalPoints + quartermasterPoints <= maxPoints)
        {
            quartermaster++;
            amount.SetText(quartermaster + "/" + maxArmy);
            totalPoints += quartermasterPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Cannon" && cannon < maxArmy && totalPoints + cannonPoints <= maxPoints)
        {
            cannon++;
            amount.SetText(cannon + "/" + maxArmy);
            totalPoints += cannonPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Engineer" && bomber < maxArmy && totalPoints + bomberPoints <= maxPoints)
        {
            bomber++;
            amount.SetText(bomber + "/" + maxArmy);
            totalPoints += bomberPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Vanguard" && vanguard < maxArmy && totalPoints + vanguardPoints <= maxPoints)
        {
            vanguard++;
            amount.SetText(vanguard + "/" + maxArmy);
            totalPoints += vanguardPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Navigator" && navigator < maxArmy && totalPoints + navigatorPoints <= maxPoints)
        {
            navigator++;
            amount.SetText(navigator + "/" + maxArmy);
            totalPoints += navigatorPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Gunner" && gunner < maxArmy && totalPoints + gunnerPoints <= maxPoints)
        {
            gunner++;
            amount.SetText(gunner + "/" + maxArmy);
            totalPoints += gunnerPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }

        displayInfo(parent);
    }
    public void DecreaseCount(GameObject parent)
    {
        string name = parent.name;
        TMP_Text amount = parent.transform.Find("Amount").GetComponent<TMP_Text>();

        if (name == "Select Royal1" && royal1 > 0)
        {
            royal1--;
            amount.SetText(royal1 + "/" + maxRoyals);
            totalPoints -= royal1Points;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Royal2" && royal2 > 0)
        {
            royal2--;
            amount.SetText(royal2 + "/" + maxRoyals);
            totalPoints -= royal2Points;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Mate" && mate > minPeasants)
        {
            mate--;
            amount.SetText(mate + "/" + maxPeasants);
            totalPoints -= matePoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Quartermaster" && quartermaster > 0)
        {
            quartermaster--;
            amount.SetText(quartermaster + "/" + maxArmy);
            totalPoints -= quartermasterPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Cannon" && cannon > 0)
        {
            cannon--;
            amount.SetText(cannon + "/" + maxArmy);
            totalPoints -= cannonPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Engineer" && bomber > 0)
        {
            bomber--;
            amount.SetText(bomber + "/" + maxArmy);
            totalPoints -= bomberPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Vanguard" && vanguard > 0)
        {
            vanguard--;
            amount.SetText(vanguard + "/" + maxArmy);
            totalPoints -= vanguardPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Navigator" && navigator > 0)
        {
            navigator--;
            amount.SetText(navigator + "/" + maxArmy);
            totalPoints -= navigatorPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Gunner" && gunner > 0)
        {
            gunner--;
            amount.SetText(gunner + "/" + maxArmy);
            totalPoints -= gunnerPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }

        displayInfo(parent);
    }

    public void ConfirmSelection()
    {
        if (PieceManager.instance.onlineMultiplayer)
        {
            WaitForOpponent();

            NetIdentifyTeam idTeam = new NetIdentifyTeam();

            idTeam.totalPoints = totalPoints;
            idTeam.Mate_Count = mate;
            idTeam.Bomber_Count = bomber;
            idTeam.Vanguard_Count = vanguard;
            idTeam.Navigator_Count = navigator;
            idTeam.Gunner_Count = gunner;
            idTeam.Cannon_Count = cannon;
            idTeam.Quartermaster_Count = quartermaster;
            idTeam.Royal2_Count = royal2;
            idTeam.Royal1_Count = royal1;

            if (navySelecting)
                idTeam.teamID = 0;
            else if (!navySelecting)
            {
                idTeam.teamID = 1;
            }
            MultiplayerController.Instance.gameWon = 1;

            Client.Instance.SendToServer(idTeam);

            timer.piecesSent = true;
        }
        else
        {
            if (navySelecting)
            {
                PieceManager.instance.navyRoyal1 = royal1;
                PieceManager.instance.navyRoyal2 = royal2;
                PieceManager.instance.navyQuartermaster = quartermaster;
                PieceManager.instance.navyCannon = cannon;
                PieceManager.instance.navyBomber = bomber;
                PieceManager.instance.navyVanguard = vanguard;
                PieceManager.instance.navyNavigator = navigator;
                PieceManager.instance.navyGunner = gunner;
                PieceManager.instance.navyMate = mate;
                navyTotal = totalPoints;
            }
            else
            {
                PieceManager.instance.pirateRoyal1 = royal1;
                PieceManager.instance.pirateRoyal2 = royal2;
                PieceManager.instance.pirateQuartermaster = quartermaster;
                PieceManager.instance.pirateCannon = cannon;
                PieceManager.instance.pirateBomber = bomber;
                PieceManager.instance.pirateVanguard = vanguard;
                PieceManager.instance.pirateNavigator = navigator;
                PieceManager.instance.pirateGunner = gunner;
                PieceManager.instance.pirateMate = mate;
                pirateTotal = totalPoints;
            }

            if (navySelecting == p1Navy)
            {
                royal1 = 0;
                royal2 = 0;
                mate = 5;
                quartermaster = 0;
                cannon = 0;
                bomber = 0;
                vanguard = 0;
                navigator = 0;
                gunner = 0;

                totalPoints = 5;

                navySelecting = !navySelecting;

                if (navySelecting)
                {
                    pointsText = navyPoints;
                    videoPlayer = pirateVideo;

                    navyPlayer.SetText("Player 2: Select Crew");
                }
                else
                {
                    pointsText = piratePoints;
                    videoPlayer = pirateVideo;

                    piratePlayer.SetText("Player 2: Select Crew");
                }
                videoManager.Setup(navySelecting);

                navyUI.SetActive(navySelecting);
                pirateUI.SetActive(!navySelecting);
            }
            else
            {
                if (navyTotal < pirateTotal)
                    PieceManager.instance.navyFirst = true;
                else if (navyTotal > pirateTotal)
                    PieceManager.instance.navyFirst = false;
                else
                    PieceManager.instance.navyFirst = p1Navy;   // Player 1 will go first in the event of a tie

                Debug.Log("Navy first: " + PieceManager.instance.navyFirst);

                SceneManager.LoadScene("Piece Placement");
            }
        }
    }

    public void displayInfo(GameObject parent)
    {
        string name = parent.name;
        TMP_Text topText;
        TMP_Text infoText;
        if(navySelecting)
        {
            topText = navyPlayer;
            infoText = navyInfoText;
        }
        else
        {
            topText = piratePlayer;
            infoText = pirateInfoText;
        }

        if (name == "Select Royal1")
        {
            if(navySelecting)
            {
                topText.SetText("Admiral [" + royal1Points + " points]");
                infoText.SetText("A Navy-exclusive crewmate. Moves any unblocked distance in any direction. She captures by landing on an enemy.");
                
            }
            else
            {
                topText.SetText("Captain [" + royal1Points + " points]");
                infoText.SetText("A Pirate-exclusive crewmate. Moves exactly five squares in any up/down or left/right direction, and can change direction while moving. He jumps over blockers, and captures enemies in the fifth square by landing on them.");
            }
            videoPlayer.clip = videoManager.royal1;
        }
        if (name == "Select Royal2")
        {
            if(navySelecting)
            {
                topText.SetText("Tactician [" + royal2Points + " points]");
                infoText.SetText("A Navy-exclusive crewmate. Moves up to two open squares up/down or left/right, and captures by landing on an enemy. It can also use the moveset of any enemy within whatever zone the it's in (the 3 rows on each player's side of the board, or the 4 rows in the middle).");
            }
            else
            {
                topText.SetText("Corsair [" + royal2Points + " points]");
                infoText.SetText("A Pirate-exclusive crewmate. Moves up to two open squares diagonally, and captures by landing on an enemy. She can also jump to any open square on the board, but if she does, she can't jump on her next turn.");
            }
            videoPlayer.clip = videoManager.royal2;
        }
        if (name == "Select Mate")
        {
            topText.SetText("Mate [" + matePoints + " point]");
            infoText.SetText("Moves one square in any direction, but cannot move backwards unless he's captured an enemy piece. He captures by landing on an enemy. Get him across the board and see how strong this unassuming crewmate can become...");
            videoPlayer.clip = videoManager.mate;
        }
        if (name == "Select Quartermaster")
        {
            topText.SetText("Quartermaster [" + quartermasterPoints + " points]");
            infoText.SetText("Moves two spaces up/down or left/right and one space perpendicular. He jumps over enemies in his way and captures by landing on an enemy.");
            videoPlayer.clip = videoManager.quartermaster;
        }
        if (name == "Select Cannon")
        {
            topText.SetText("Cannon [" + cannonPoints + " points]");
            infoText.SetText("Moves one open space in any direction, or captures by jumping over enemies. He can cross any open distance while jumping, but he needs space on the other side to land or else he can't make the jump. He can jump over Energy Shields like this, but can't capture them.");
            videoPlayer.clip = videoManager.cannon;
        }
        if (name == "Select Engineer")
        {
            topText.SetText("Engineer [" + bomberPoints + " points]");
            infoText.SetText("Moves up to two open squares in any direction. He's the only piece that can capture Energy Shields by landing on them and can redeploy spare Energy Shields back to the board, but he can't capture any enemies unless he's armed with a spare Energy Shield.");
            videoPlayer.clip = videoManager.engineer;
        }
        if (name == "Select Vanguard")
        {
            topText.SetText("Vanguard [" + vanguardPoints + " points]");
            infoText.SetText("Moves one square forwards or backwards, both up/down or left/right and diagonally, but can move any open distance sideways. He captures by landing on an enemy.");
            videoPlayer.clip = videoManager.vanguard;
        }
        if (name == "Select Navigator")
        {
            topText.SetText("Navigator [" + navigatorPoints + " points]");
            infoText.SetText("Moves one square sideways, both up/down or left/right and diagonally, but can move any open distance forwards and backwards. He captures by landing on an enemy.");
            videoPlayer.clip = videoManager.navigator;
        }
        if (name == "Select Gunner")
        {
            topText.SetText("Gunner [" + gunnerPoints + " points]");
            infoText.SetText("Moves one open space in any direction, or he captures by shooting an enemy up to 3 unblocked spaces away in any direction. He has to move somewhere to reload his weapon before he can capture again. Be warned, he can't shoot the enemy Ore or he might destroy it!");
            videoPlayer.clip = videoManager.gunner;
        }
    }

    public void WaitForOpponent()
    {
        if (PirateSelectMenu.activeInHierarchy)
            PirateSelectMenu.SetActive(false);
        if(NavySelectMenu.activeInHierarchy)
            NavySelectMenu.SetActive(false);

        WaitingForOpponentMenu.SetActive(true);
    }

    #region Events

    private void RegisterEvents()
    {
        NetUtility.S_IDENTIFY_TEAM += OnIdentifyTeamServer;

        NetUtility.C_IDENTIFY_TEAM += OnIdentifyTeamClient;
    }

    private void UnRegisterEvents()
    {
        NetUtility.S_IDENTIFY_TEAM -= OnIdentifyTeamServer;

        NetUtility.C_IDENTIFY_TEAM -= OnIdentifyTeamClient;
    }

    // Server
    private void OnIdentifyTeamServer(NetMessage msg, NetworkConnection cnn)
    {
        NetIdentifyTeam idTeam = msg as NetIdentifyTeam;
        
        Server.Instance.Broadcast(idTeam);
    }

    // Client
    private void OnIdentifyTeamClient(NetMessage msg)
    {
        NetIdentifyTeam idTeam = msg as NetIdentifyTeam;

        Debug.Log($"Mates: {idTeam.Mate_Count}");
        Debug.Log($"Engineers: {idTeam.Bomber_Count}");
        Debug.Log($"Vanguards: {idTeam.Vanguard_Count}");
        Debug.Log($"Navigators: {idTeam.Navigator_Count}");
        Debug.Log($"Gunners: {idTeam.Gunner_Count}");
        Debug.Log($"Cannons: {idTeam.Cannon_Count}");
        Debug.Log($"Quartermasters: {idTeam.Quartermaster_Count}");
        Debug.Log($"Royal 2s: {idTeam.Royal2_Count}");
        Debug.Log($"Royal 1s: {idTeam.Royal1_Count}");
        Debug.Log($"Total Points: {idTeam.totalPoints}");

        // Message contains the Navy's team and the active player is the Pirates
        if (idTeam.teamID == 0)
        {
            PieceManager.instance.navyMate = idTeam.Mate_Count;
            PieceManager.instance.navyBomber = idTeam.Bomber_Count;
            PieceManager.instance.navyVanguard = idTeam.Vanguard_Count;
            PieceManager.instance.navyNavigator = idTeam.Navigator_Count;
            PieceManager.instance.navyGunner = idTeam.Gunner_Count;
            PieceManager.instance.navyCannon = idTeam.Cannon_Count;
            PieceManager.instance.navyQuartermaster = idTeam.Quartermaster_Count;
            PieceManager.instance.navyRoyal2 = idTeam.Royal2_Count;
            PieceManager.instance.navyRoyal1 = idTeam.Royal1_Count;

            navyTotal = idTeam.totalPoints;
        }
        // Message contains the Pirate's team and the active player is the Navy
        else if(idTeam.teamID == 1)
        {
            PieceManager.instance.pirateMate = idTeam.Mate_Count;
            PieceManager.instance.pirateBomber = idTeam.Bomber_Count;
            PieceManager.instance.pirateVanguard = idTeam.Vanguard_Count;
            PieceManager.instance.pirateNavigator = idTeam.Navigator_Count;
            PieceManager.instance.pirateGunner = idTeam.Gunner_Count;
            PieceManager.instance.pirateCannon = idTeam.Cannon_Count;
            PieceManager.instance.pirateQuartermaster = idTeam.Quartermaster_Count;
            PieceManager.instance.pirateRoyal2 = idTeam.Royal2_Count;
            PieceManager.instance.pirateRoyal1 = idTeam.Royal1_Count;

            pirateTotal = idTeam.totalPoints;
        }
    }
    #endregion
}
