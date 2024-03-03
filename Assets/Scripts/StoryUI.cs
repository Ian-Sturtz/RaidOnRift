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
                    pieceDesc.SetText("Admiral lore here.");
                    break;
                case "Tactician":
                    pieceName.SetText("THE TACTICIAN");
                    pieceDesc.SetText("In the distant future, the iron reach of the Imperial Navy spans far across the cosmos. Powered by the energy we harvest from Galactic Rifts, refined into pure crystals we call “the ore”, our weapons are charged and our cloning devices are fueled, letting us clone our ranks with ease. But great power attracts great threats. The fearsome and terrible Space Pirates now raid our refineries, seeking to steal our Ore for their own, putting a stop to our reign and letting their villany take hold in our ever peaceful empire.");
                    break;
                case "Navy Mate":
                    pieceName.SetText("THE CREWMATE");
                    pieceDesc.SetText("Navy Mate lore here.");
                    break;
                case "Captain":
                    pieceName.SetText("THE CAPTAIN");
                    pieceDesc.SetText("Captain lore here.");
                    break;
                case "Corsair":
                    pieceName.SetText("THE CORSAIR");
                    pieceDesc.SetText("TACTICIAN JUMSCARE (there is no corsiar bust in the github)");
                    break;
                case "Pirate Mate":
                    pieceName.SetText("THE CREWMATE");
                    pieceDesc.SetText("Pirate Mate lore here.");
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