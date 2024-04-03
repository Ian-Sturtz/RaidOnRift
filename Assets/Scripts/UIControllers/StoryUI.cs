using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryUI : MonoBehaviour
{
    private float topHeight = Screen.height;
    public float duration = 0.5f;
    public float transparency = 0.75f;

    bool menuTop;
    Vector2 bottomPos;

    public GameObject menu;
    public GameObject background;
    public TMP_Text pieceName;
    public TMP_Text pieceDesc;
    public Image character;

    public Sprite admiral;
    public Sprite tactician;
    public Sprite navyCrew;
    public Sprite navyGunner;

    public Sprite pirateCrew;
    public Sprite pirateGunner;

    // Start is called before the first frame update
    void Start()
    {
        bottomPos = menu.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OperateInfo(string piece = "")
    {
        StopAllCoroutines();

        if (!menuTop)
        {
            switch(piece)
            {
                case "Admiral":
                    pieceName.SetText("THE ADMIRAL");
                    pieceDesc.SetText("“They call me fierce. They call me cunning. Above all, they call me relentless.\r\n\nFollowing my monumental victory on the planet Gyzax, I was promoted to admiral and assigned lordship over the Ore refinery on the Rift. Now, as Lord of the Rift, I have supreme access to the entire facility, and can use the knowledge, agility, and skill in battle to maintain dominance over my domain.\r\n\nI am revered by my army but feared by my enemies. I promise defeat to all who challenge my rule.”");
                    character.sprite = admiral;
                    break;
                case "Tactician":
                    pieceName.SetText("THE TACTICIAN");
                    pieceDesc.SetText("“My intellect is unrivaled, and my stratagems are unyielding.\r\n\nI have served the Imperial Navy for centuries now as chief military strategist. I have flourished under the Admiral since the war on planet Gyzax. I have witnessed a thousand enemies and assimilated tactics from each of them, fusing their styles into my own to form an unconquerable plan to defeat those who oppose me.\r\n\nI am the echo of countless talents, and the strength of many forged into one.”");
                    character.sprite = tactician;
                    break;
                case "Navy Mate":
                    pieceName.SetText("THE CREWMATE");
                    pieceDesc.SetText("");
                    character.sprite = navyCrew;
                    break;
                case "Navy Quartermaster":
                    pieceName.SetText("THE QUARTERMASTER");
                    pieceDesc.SetText("“The crew is my duty, and I’m never one to shrink away from duty.\n\r\n Since emerging from the cloning device on the Rift, I have been in charge of keeping the ranks in line. I train with the clones, I eat with the clones, and I live with the clones. In the event that the refinery is ever put in harm’s way, it is my honor to lead the charge and advance the cause of the fight. My nimble agility and fierce willpower will inspire the troops to siege on.\n\r\nSquare up those shoulders, raise those heads, and prepare for our victory.”");
                    break;
                case "Navy Cannon":
                    pieceName.SetText("THE CANNONEER");
                    pieceDesc.SetText("Navy cannon lore here.");
                    break;
                case "Navy Bomber":
                    pieceName.SetText("THE ENGINEER");
                    pieceDesc.SetText("Navy engineer lore here.");
                    break;
                case "Navy Vanguard":
                    pieceName.SetText("THE VANGUARD");
                    pieceDesc.SetText("Navy vanguard lore here.");
                    break;
                case "Navy Navigator":
                    pieceName.SetText("THE NAVIGATOR");
                    pieceDesc.SetText("Navy navigator lore here.");
                    break;
                case "Navy Gunner":
                    pieceName.SetText("THE GUNNER");
                    pieceDesc.SetText("The Gunners have cyber-implanted cannons that they can use to keep targets at a distance and blast their way through enemy forces.");
                    character.sprite = navyGunner;
                    break;

                case "Captain":
                    pieceName.SetText("THE CAPTAIN");
                    pieceDesc.SetText("Captain lore here.");
                    break;
                case "Corsair":
                    pieceName.SetText("THE CORSAIR");
                    pieceDesc.SetText("The Corsair is a cunning and agile blademaster, using her wings to glide her around the map with ease, letting her get in close to use her blades like scalpels and surgically remove the enemy.");
                    break;
                case "Pirate Mate":
                    pieceName.SetText("THE CREWMATE");
                    pieceDesc.SetText("Pirate Mate lore here.");
                    character.sprite = pirateCrew;
                    break;
                case "Pirate Quartermaster":
                    pieceName.SetText("THE QUARTERMASTER");
                    pieceDesc.SetText("Pirate quartermaster lore here.");
                    break;
                case "Pirate Cannon":
                    pieceName.SetText("THE CANNONEER");
                    pieceDesc.SetText("Pirate cannon lore here.");
                    break;
                case "Pirate Bomber":
                    pieceName.SetText("THE ENGINEER");
                    pieceDesc.SetText("Pirate engineer lore here.");
                    break;
                case "Pirate Vanguard":
                    pieceName.SetText("THE VANGUARD");
                    pieceDesc.SetText("Pirate vanguard lore here.");
                    break;
                case "Pirate Navigator":
                    pieceName.SetText("THE NAVIGATOR");
                    pieceDesc.SetText("Pirate navigator lore here.");
                    break;
                case "Pirate Gunner":
                    pieceName.SetText("THE GUNNER");
                    pieceDesc.SetText("Pirate gunner lore here.");
                    character.sprite = pirateGunner;
                    break;
            }
            Vector2 topPos = bottomPos + Vector2.up * topHeight;
            StartCoroutine(MoveInfo(topPos));
            background.SetActive(true);
            StartCoroutine(SetTransparency(transparency, true));
        }
        else
        {
            StartCoroutine(MoveInfo(bottomPos));
            StartCoroutine(SetTransparency(0, false));
        }
        menuTop = !menuTop;
    }

    IEnumerator MoveInfo(Vector2 targetPos)
    {
        float timeElapsed = 0;
        Vector2 startPos = menu.transform.position;

        while (timeElapsed < duration)
        {
            float t = Mathf.SmoothStep(0, 1, timeElapsed / duration);
            menu.transform.position = Vector2.Lerp(startPos, targetPos, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        menu.transform.position = targetPos;
    }

    IEnumerator SetTransparency(float targetTrans, bool setAct)
    {
        float timeElapsed = 0;
        float startTrans = background.GetComponent<Image>().color.a;
        

        while(timeElapsed < duration)
        {   float val = Mathf.Lerp(startTrans, targetTrans, timeElapsed / duration);
            background.GetComponent<Image>().color = new Color(0, 0, 0, val);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        background.GetComponent<Image>().color = new Color(0, 0, 0, targetTrans);
        background.SetActive(setAct);
    }

    public void toMain()
    {
        SceneManager.LoadScene("Main Menu");
    }
}