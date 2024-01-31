using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [SerializeField] private TMP_Text navyPoints;
    [SerializeField] private TMP_Text piratePoints;

    private TMP_Text pointsText;

    [SerializeField] private TMP_Text navyPlayer;
    [SerializeField] private TMP_Text piratePlayer;

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

    public void chosenFaction(bool choseNavy)
    {
        p1Navy = choseNavy;
        navySelecting = choseNavy;
        if(navySelecting)
        {
            pointsText = navyPoints;
        }
        else
        {
            pointsText = piratePoints;
        }
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
            amount.SetText(quartermaster + "/2");
            totalPoints += quartermasterPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Cannon" && cannon < maxArmy && totalPoints + cannonPoints <= maxPoints)
        {
            cannon++;
            amount.SetText(cannon + "/2");
            totalPoints += cannonPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Bomber" && bomber < maxArmy && totalPoints + bomberPoints <= maxPoints)
        {
            bomber++;
            amount.SetText(bomber + "/2");
            totalPoints += bomberPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Vanguard" && vanguard < maxArmy && totalPoints + vanguardPoints <= maxPoints)
        {
            vanguard++;
            amount.SetText(vanguard + "/2");
            totalPoints += vanguardPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Navigator" && navigator < maxArmy && totalPoints + navigatorPoints <= maxPoints)
        {
            navigator++;
            amount.SetText(navigator + "/2");
            totalPoints += navigatorPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Gunner" && gunner < maxArmy && totalPoints + gunnerPoints <= maxPoints)
        {
            gunner++;
            amount.SetText(gunner + "/2");
            totalPoints += gunnerPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
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
            amount.SetText(quartermaster + "/2");
            totalPoints -= quartermasterPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Cannon" && cannon > 0)
        {
            cannon--;
            amount.SetText(cannon + "/2");
            totalPoints -= cannonPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Bomber" && bomber > 0)
        {
            bomber--;
            amount.SetText(bomber + "/2");
            totalPoints -= bomberPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Vanguard" && vanguard > 0)
        {
            vanguard--;
            amount.SetText(vanguard + "/2");
            totalPoints -= vanguardPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Navigator" && navigator > 0)
        {
            navigator--;
            amount.SetText(navigator + "/2");
            totalPoints -= navigatorPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Gunner" && gunner > 0)
        {
            gunner--;
            amount.SetText(gunner + "/2");
            totalPoints -= gunnerPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
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

                navyPlayer.SetText("Player 2: Select Crew");
            }
            else
            {
                pointsText = piratePoints;

                piratePlayer.SetText("Player 2: Select Crew");
            }
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
}
