using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class PieceSelection : MonoBehaviour
{
    private bool p1Navy;
    private bool navySelecting;

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

    private int navyTotal;
    private int pirateTotal;

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

    [SerializeField] private VideoPlayer navyVideo;
    [SerializeField] private VideoPlayer pirateVideo;

    private void Start()
    {
        videoManager = this.GetComponent<VideoManager>();
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

    public void confirmSelection()
    {
        if(navySelecting)
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

        if(navySelecting == p1Navy)
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
                PieceManager.instance.navyFirst = p1Navy;
            SceneManager.LoadScene("Piece Placement");
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
                infoText.SetText("A navy exclusive crewmate. Moves any unblocked distance in any direction, captures by replacement.");
                
            }
            else
            {
                topText.SetText("Captain [" + royal1Points + " points]");
                infoText.SetText("A pirate exclusive crewmate. Moves exactly five squares in any orthogonal direction, and can change direction mid-move. Cannot visit the same square twice in a move. Jumps over blockers, and captures any enemy piece in the fifth square by replacement.");
            }
            videoPlayer.clip = videoManager.royal1;
        }
        if (name == "Select Royal2")
        {
            if(navySelecting)
            {
                topText.SetText("Tactician [" + royal2Points + " points]");
                infoText.SetText("A navy exclusive crewmate. Moves up to two unblocked squares orthogonally, captures by replacement. Can also use the moveset of any enemy piece within whatever zone the Tactician is in (Allied Start Zone, Neutral Zone, or Enemy Start Zone).");
            }
            else
            {
                topText.SetText("Corsair [" + royal2Points + " points]");
                infoText.SetText("A pirate exclusive crewmate. Moves one space diagonally, captures by replacement, or can jump to any open square on the board. If the corsair jumps this way, the corsair cannot move on player 2’s following turn.");
            }
            videoPlayer.clip = videoManager.royal2;
        }
        if (name == "Select Mate")
        {
            topText.SetText("Mate [" + matePoints + " point]");
            infoText.SetText("Moves one square in any direction, but cannot move backwards unless the Mate has captured an enemy piece. Captures by replacement. Can perform Jailbreaks after reaching the opponent’s Royal Zone.");
        }
        if (name == "Select Quartermaster")
        {
            topText.SetText("Quartermaster [" + quartermasterPoints + " points]");
            infoText.SetText("Moves two spaces orthogonally and one space perpendicularly. Jumps over blockers and captures by replacement.");
        }
        if (name == "Select Cannon")
        {
            topText.SetText("Cannon [" + cannonPoints + " points]");
            infoText.SetText("Can move one unblocked space in any direction, captures by jumping any unblocked distance orthogonally and must land on the opposite adjacent square to the captured piece. A Land Mine can be jumped over in this way, but the Land Mine won’t be captured.");
        }
        if (name == "Select Engineer")
        {
            topText.SetText("Engineer [" + bomberPoints + " points]");
            infoText.SetText("Moves up to two unblocked squares in any direction. The only piece that can capture Land Mines by replacement, but cannot capture any other piece besides the flag. Can return one Land Mine from the Jail Zone to the game board in any open square adjacent to the Miner. Cannot move if a Jail Zone has been returned to the game board this turn.");
        }
        if (name == "Select Vanguard")
        {
            topText.SetText("Vanguard [" + vanguardPoints + " points]");
            infoText.SetText("Moves one square forward or backward, both orthogonally and diagonally, but can move any unblocked distance sideways. Captures by replacement.");
        }
        if (name == "Select Navigator")
        {
            topText.SetText("Navigator [" + navigatorPoints + " points]");
            infoText.SetText("Moves one square sideways, both orthogonally and diagonally, but can move any unblocked distance forwards and backwards.");
        }
        if (name == "Select Gunner")
        {
            topText.SetText("Gunner [" + gunnerPoints + " points]");
            infoText.SetText("Moves one unblocked space in any direction, captures by shooting a piece up to 4 unblocked spaces away in any direction. Cannot capture by replacement and must move after capturing a piece before the Archer can capture another piece.");
            videoPlayer.clip = videoManager.gunner;
        }
    }
}
