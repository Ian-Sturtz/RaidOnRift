using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PieceSelection : MonoBehaviour
{
    private bool p1Navy;
    private bool navySelecting;
    private int[] navyLoadout = new int[9];
    private int[] pirateLoadout = new int[9];

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
    private int totalRoyals = 0;
    private int totalArmy = 0;

    [SerializeField] private TMP_Text navyPoints;
    [SerializeField] private TMP_Text navyRoyals;
    [SerializeField] private TMP_Text navyArmy;
    [SerializeField] private TMP_Text piratePoints;
    [SerializeField] private TMP_Text pirateRoyals;
    [SerializeField] private TMP_Text pirateArmy;

    private TMP_Text pointsText;
    private TMP_Text royalsText;
    private TMP_Text armyText;

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
    [SerializeField] private int maxRoyals = 2;
    [SerializeField] private int maxArmy = 8;

    public void chosenFaction(bool choseNavy)
    {
        p1Navy = choseNavy;
        navySelecting = choseNavy;
        if(navySelecting)
        {
            pointsText = navyPoints;
            royalsText = navyRoyals;
            armyText = navyArmy;
        }
        else
        {
            pointsText = piratePoints;
            royalsText = pirateRoyals;
            armyText = pirateArmy;
        }
    }

    public void IncreaseCount(GameObject parent)
    {
        string name = parent.name;
        TMP_Text amount = parent.transform.Find("Amount").GetComponent<TMP_Text>();

        if (name == "Select Royal1" && royal1 < 2 && totalRoyals < maxRoyals && totalPoints + royal1Points <= maxPoints)
        {
            royal1++;
            amount.SetText(royal1 + "/2");
            totalRoyals++;
            royalsText.SetText(totalRoyals + "/" + maxRoyals);
            totalPoints += royal1Points;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Royal2" && royal2 < 2 && totalRoyals < maxRoyals && totalPoints + royal2Points <= maxPoints)
        {
            royal2++;
            amount.SetText(royal2 + "/2");
            totalRoyals++;
            royalsText.SetText(totalRoyals + "/" + maxRoyals);
            totalPoints += royal2Points;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Mate" && mate < 10 && totalPoints + matePoints <= maxPoints)
        {
            mate++;
            amount.SetText(mate + "/10");
            totalPoints += matePoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Quartermaster" && quartermaster < 2 && totalArmy < maxArmy && totalPoints + quartermasterPoints <= maxPoints)
        {
            quartermaster++;
            amount.SetText(quartermaster + "/2");
            totalArmy++;
            armyText.SetText(totalArmy + "/" + maxArmy);
            totalPoints += quartermasterPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Cannon" && cannon < 2 && totalArmy < maxArmy && totalPoints + cannonPoints <= maxPoints)
        {
            cannon++;
            amount.SetText(cannon + "/2");
            totalArmy++;
            armyText.SetText(totalArmy + "/" + maxArmy);
            totalPoints += cannonPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Bomber" && bomber < 2 && totalArmy < maxArmy && totalPoints + bomberPoints <= maxPoints)
        {
            bomber++;
            amount.SetText(bomber + "/2");
            totalArmy++;
            armyText.SetText(totalArmy + "/" + maxArmy);
            totalPoints += bomberPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Vanguard" && vanguard < 2 && totalArmy < maxArmy && totalPoints + vanguardPoints <= maxPoints)
        {
            vanguard++;
            amount.SetText(vanguard + "/2");
            totalArmy++;
            armyText.SetText(totalArmy + "/" + maxArmy);
            totalPoints += vanguardPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Navigator" && navigator < 2 && totalArmy < maxArmy && totalPoints + navigatorPoints <= maxPoints)
        {
            navigator++;
            amount.SetText(navigator + "/2");
            totalArmy++;
            armyText.SetText(totalArmy + "/" + maxArmy);
            totalPoints += navigatorPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Gunner" && gunner < 2 && totalArmy < maxArmy && totalPoints + gunnerPoints <= maxPoints)
        {
            gunner++;
            amount.SetText(gunner + "/2");
            totalArmy++;
            armyText.SetText(totalArmy + "/" + maxArmy);
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
            amount.SetText(royal1 + "/2");
            totalRoyals--;
            royalsText.SetText(totalRoyals + "/" + maxRoyals);
            totalPoints -= royal1Points;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Royal2" && royal2 > 0)
        {
            royal2--;
            amount.SetText(royal2 + "/2");
            totalRoyals--;
            royalsText.SetText(totalRoyals + "/" + maxRoyals);
            totalPoints -= royal2Points;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Mate" && mate > 5)
        {
            mate--;
            amount.SetText(mate + "/10");
            totalPoints -= matePoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Quartermaster" && quartermaster > 0)
        {
            quartermaster--;
            amount.SetText(quartermaster + "/2");
            totalArmy--;
            armyText.SetText(totalArmy + "/" + maxArmy);
            totalPoints -= quartermasterPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Cannon" && cannon > 0)
        {
            cannon--;
            amount.SetText(cannon + "/2");
            totalArmy--;
            armyText.SetText(totalArmy + "/" + maxArmy);
            totalPoints -= cannonPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Bomber" && bomber > 0)
        {
            bomber--;
            amount.SetText(bomber + "/2");
            totalArmy--;
            armyText.SetText(totalArmy + "/" + maxArmy);
            totalPoints -= bomberPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Vanguard" && vanguard > 0)
        {
            vanguard--;
            amount.SetText(vanguard + "/2");
            totalArmy--;
            armyText.SetText(totalArmy + "/" + maxArmy);
            totalPoints -= vanguardPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Navigator" && navigator > 0)
        {
            navigator--;
            amount.SetText(navigator + "/2");
            totalArmy--;
            armyText.SetText(totalArmy + "/" + maxArmy);
            totalPoints -= navigatorPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
        if (name == "Select Gunner" && gunner > 0)
        {
            gunner--;
            amount.SetText(gunner + "/2");
            totalArmy--;
            armyText.SetText(totalArmy + "/" + maxArmy);
            totalPoints -= gunnerPoints;
            pointsText.SetText(totalPoints + "/" + maxPoints);
        }
    }

    public void confirmSelection()
    {
        if(navySelecting)
        {
            navyLoadout[0] = royal1;
            navyLoadout[1] = royal2;
            navyLoadout[2] = quartermaster;
            navyLoadout[3] = cannon;
            navyLoadout[4] = bomber;
            navyLoadout[5] = vanguard;
            navyLoadout[6] = navigator;
            navyLoadout[7] = gunner;
            navyLoadout[8] = mate;
        }
        else
        {
            pirateLoadout[0] = royal1;
            pirateLoadout[1] = royal2;
            pirateLoadout[2] = quartermaster;
            pirateLoadout[3] = cannon;
            pirateLoadout[4] = bomber;
            pirateLoadout[5] = vanguard;
            pirateLoadout[6] = navigator;
            pirateLoadout[7] = gunner;
            pirateLoadout[8] = mate;
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
            totalRoyals = 0;
            totalArmy = 0;

            navySelecting = !navySelecting;

            if (navySelecting)
            {
                pointsText = navyPoints;
                royalsText = navyRoyals;
                armyText = navyArmy;

                navyPlayer.SetText("Player 2: Select Crew");
            }
            else
            {
                pointsText = piratePoints;
                royalsText = pirateRoyals;
                armyText = pirateArmy;

                piratePlayer.SetText("Player 2: Select Crew");
            }
        }
        else
        {
            Debug.Log("Begin placement here");
        }
    }
}
