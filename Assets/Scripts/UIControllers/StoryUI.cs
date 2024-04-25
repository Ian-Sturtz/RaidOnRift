using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class StoryUI : MonoBehaviour
{
    private float topHeight = Screen.height;
    public float duration = 0.5f;
    public float transparency = 0.75f;
    public static int tutorialToLoad = 0;

    bool menuTop;
    Vector2 bottomPos;

    [SerializeField] private VideoClip navyRoyal1;
    [SerializeField] private VideoClip navyRoyal2;

    [SerializeField] private VideoClip pirateRoyal1;
    [SerializeField] private VideoClip pirateRoyal2;

    [SerializeField] public VideoClip quartermaster;
    [SerializeField] public VideoClip cannon;
    [SerializeField] public VideoClip engineer;
    [SerializeField] public VideoClip vanguard;
    [SerializeField] public VideoClip navigator;
    [SerializeField] public VideoClip gunner;
    [SerializeField] public VideoClip mate;

    [HideInInspector] public VideoClip royal1;
    [HideInInspector] public VideoClip royal2;

    public GameObject menu;
    public GameObject background;
    public GameObject Video;
    public TMP_Text pieceName;
    public TMP_Text pieceDesc;
    public Image character;

    public Sprite admiral;
    public Sprite tactician;
    public Sprite navyCrew;
    public Sprite navyGunner;
    public Sprite navyQuatermaster;
    public Sprite navyCannon;
    public Sprite navyEngineer;
    public Sprite navyNavigator;
    public Sprite navyVanguard;
    

    public Sprite pirateCrew;
    public Sprite pirateGunner;
    public Sprite Captain;
    public Sprite Corsair;
    public Sprite pirateCannon;
    public Sprite pirateEngineer;
    public Sprite pirateNavigator;
    public Sprite pirateQuatermaster;
    public Sprite pirateVanguard;

    public void Setup(bool isNavy)
    {
        if (isNavy)
        {
            royal1 = navyRoyal1;
            royal2 = navyRoyal2;
        }
        else
        {
            royal1 = pirateRoyal1;
            royal2 = pirateRoyal2;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        bottomPos = menu.transform.position;
        Scene currentScene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OperateInfo(string piece = "")
    {
        StopAllCoroutines();

        Scene currentScene = SceneManager.GetActiveScene();

        if (!menuTop)
        {
            switch(piece)
            {
                case "Admiral":
                    pieceName.SetText("THE ADMIRAL");
                    pieceDesc.SetText("They call me fierce. They call me cunning. Above all, they call me relentless.\r\n\nFollowing my monumental victory on the planet Gyzax, I was promoted to admiral and assigned lordship over the Ore refinery on the Rift. Now, as Lord of the Rift, I have supreme access to the entire facility, and can use the knowledge, agility, and skill in battle to maintain dominance over my domain.\r\n\nI am revered by my army but feared by my enemies. I promise defeat to all who challenge my rule.");
                    character.sprite = admiral;
                    tutorialToLoad = 8;                  
                    break;
                case "Tactician":
                    pieceName.SetText("THE TACTICIAN");
                    pieceDesc.SetText("My intellect is unrivaled, and my stratagems are unyielding.\r\n\nI have served the Imperial Navy for centuries now as chief military strategist. I have flourished under the Admiral since the war on planet Gyzax. I have witnessed a thousand enemies and assimilated tactics from each of them, fusing their styles into my own to form an unconquerable plan to defeat those who oppose me.\r\n\nI am the echo of countless talents, and the strength of many forged into one.");
                    character.sprite = tactician;
                    tutorialToLoad = 9;
                    break;
                case "Navy Mate":
                    pieceName.SetText("THE CREWMATE");
                    pieceDesc.SetText("It's not the rank that defines a sailor, but the strength of their resolve\n\nI've traveled the galaxys, as a proud crew mate in the Navy, embracing the stars and whispering winds. My journey began years ago, driven by a passion for adventure and a dedication to serve. Each day, amidst the vast space, I'm part of a family bound by courage and discipline. I navigate with unwavering resolve, ensuring safety and strength in every mission. My life is a testament to the power of teamwork and the endless pursuit of excellence.");
                    character.sprite = navyCrew;
                    tutorialToLoad = 1;
                    break;
                case "Navy Quartermaster":
                    pieceName.SetText("THE QUARTERMASTER");
                    pieceDesc.SetText("The crew is my duty, and I�m never one to shrink away from duty.\n\r\n Since emerging from the cloning device on the Rift, I have been in charge of keeping the ranks in line. I train with the clones, I eat with the clones, and I live with the clones. In the event that the refinery is ever put in harm�s way, it is my honor to lead the charge and advance the cause of the fight. My nimble agility and fierce willpower will inspire the troops to siege on.\n\r\nSquare up those shoulders, raise those heads, and prepare for our victory.");
                    character.sprite = navyQuatermaster;
                    tutorialToLoad = 2;
                    break;
                case "Navy Cannon":
                    pieceName.SetText("THE CANNONEER");
                    pieceDesc.SetText("In the heart of the galaxys tumult, my soul finds peace in the discipline of the gun, a cannoneer's vow to protect and serve.\n\nI stand as the Navy's cannoneer, a guardian of the ships mighty vessels. My journey is carved by the rhythm of firing cannons and the sound of the great laser cannon, a testament to my resolve and precision. Each day, I harness the power of these titans, protecting the crew with a steady hand and a keen eye. My life is a symphony of thunderous echoes and silent anticipation, where teamwork and bravery steer the course towards peace and security.");
                    character.sprite = navyCannon;
                    tutorialToLoad = 3;
                    break;
                case "Navy Engineer":
                    pieceName.SetText("THE ENGINEER");
                    pieceDesc.SetText("Amongst gears and steam, my spirit thrives, forging strength from the sea's challenges, an engineer's oath to endurance and innovation.\n\nIn the engine's roar and the vessels steady hum, I am the Navy's engineer, keeper of the vessel's heart. My hands, coated in grease and determination, bring life to the machinery that guides us through the rifts embrace. My journey is one of innovation and resilience, ensuring safety and efficiency in every wave we conquer, a silent guardian of the deep");
                    character.sprite = navyEngineer;
                    tutorialToLoad = 4;
                    break;
                case "Navy Vanguard":
                    pieceName.SetText("THE VANGUARD");
                    pieceDesc.SetText("At the edge of the galaxys end, where fear meets fate, I stand firm, a vanguard's resolve lighting the path to safe engagement\n\nAs the Navy's vanguard, I lead the charge into dangerous rift, a beacon of courage at the forefront of our fleet. My eyes, fixed on the horizon, guide us through the unknown, embodying the first line of defense and exploration. My life is a dance with the rifts might, a testament to valor and foresight, safeguarding peace across the galaxys vast expanses.");
                    character.sprite = navyVanguard;
                    tutorialToLoad = 5;
                    break;
                case "Navy Navigator":
                    pieceName.SetText("THE NAVIGATOR");
                    pieceDesc.SetText("Under the canopy of stars, I chart our course, a navigator's promise to turn the mysteries of the galaxy into pathways home.\n\nAs the Navy's navigator, my world unfolds in charts and stars, guiding our vessel through the vast tapestry of the rift. My hands chart the course of discovery and duty, blending ancient skills with modern precision. In the silent watch of the night, I steer our journey, a guardian of direction and destiny, weaving through whispers of the rift and the stars' secrets.");
                    character.sprite = navyNavigator;
                    tutorialToLoad = 6;
                    break;
                case "Navy Gunner":
                    pieceName.SetText("THE GUNNER");
                    pieceDesc.SetText("In the dance of photons and fate, my aim is true, a laser gunner's vow to safeguard the rift with light's silent strike.\n\nIn the shadow of tomorrow's warfare, I stand as the Navy's laser gunner, wielding light with lethal precision. My role marries technology and tenacity, mastering beams that cut through the silence of the skies. Each pulse is a testament to modern warfare's evolution, protecting the rift with an unseen force. My vigilance shapes the unseen battlefield, a silent guardian of the future's peace.");
                    character.sprite = navyGunner;
                    tutorialToLoad = 7;
                    break;

                case "Captain":
                    pieceName.SetText("THE CAPTAIN");
                    pieceDesc.SetText("In the vast canvas of the cosmos, I carve my destiny with bold strokes, a space pirate captain, unfettered by laws but bound by the stars.\n\n Amidst the endless expanse of stars, I navigate the cosmos as a space pirate captain, a master of the uncharted. With a crew as diverse as the planets we've seen, my ship cuts through the void, a beacon of freedom and fortune. In the silence of space, my command is law, chasing adventure and treasure beyond the grasp of galactic authorities, a rebel heart beating in the dark.");
                    character.sprite = Captain;
                    tutorialToLoad = 10;
                    break;
                case "Corsair":
                    pieceName.SetText("THE CORSAIR");
                    pieceDesc.SetText("Amongst the starlit void, my blade sings the anthem of the free, a Corsairs vow to carve a path of honor and rebellion.\n\n As a Corsair, I wield my blade with the precision of a comet slicing through the dark. My skill is honed in the vacuum of space, a dance of steel and stars against any who dare cross our path. Leading my crew with the edge of my sword, I seek fortune and glory beyond the frontiers, a master of combat in the endless night.");
                    character.sprite = Corsair;
                    tutorialToLoad = 11;
                    break;
                case "Pirate Mate":
                    pieceName.SetText("THE CREWMATE");
                    pieceDesc.SetText("In the vast embrace of the cosmos, I find my freedom, a crewmate charting the unknown with a heart untamed by gravity.\n\n Aboard the starship's deck, I stand as a loyal crewmate, navigating the celestial seas. My life is a tapestry of stars and adventures, bound by camaraderie and the thrill of the unknown. Each planet and asteroid belt brings new challenges and treasures, forging my path in the cosmos. In this boundless frontier, I am a wanderer, a seeker of fortune under the galaxy's watchful eyes.");
                    character.sprite = pirateCrew;
                    tutorialToLoad = 1;
                    break;
                case "Pirate Quartermaster":
                    pieceName.SetText("THE QUARTERMASTER");
                    pieceDesc.SetText("In the infinite chess game of the cosmos, I move with precision, a quartermaster guiding our crew through the stars with cunning and care.\n\n As the quartermaster of my crew, I am the backbone of our celestial odyssey, managing treasures and supplies with an iron hand. My expertise in logistics and strategy ensures our ship thrives in the unforgiving void of space. I stand at the intersection of survival and ambition, navigating the delicate balance between risk and reward. In the vast expanse, my wisdom steers us toward prosperity.");
                    character.sprite = pirateQuatermaster;
                    tutorialToLoad = 2;
                    break;
                case "Pirate Cannon":
                    pieceName.SetText("THE CANNONEER");
                    pieceDesc.SetText("Through the void, my cannons roar, a symphony of resistance and liberation, etching our mark among the stars.\n\n In the echoing silence of space, my role as a cannoneer speaks volumes. Sending volleys of defiance through the vacuum. Each shot is a declaration of our sovereignty, a blend of precision and power. Amidst asteroids and enemy fleets, I carve our path to freedom, a guardian of our crew's rebellious spirit.");
                    character.sprite = pirateCannon;
                    tutorialToLoad = 3;
                    break;
                case "Pirate Engineer":
                    pieceName.SetText("THE ENGINEER");
                    pieceDesc.SetText("In the engine's hum and the stars' glow, I find my purpose, engineering our path through the cosmos, where innovation meets adventure.\n\n Within the heart of our star-bound vessel, I serve as the engineer, a custodian of the technology that propels us through the cosmos. My hands, stained with oil and stardust, mend and master the machinery that defies gravity. In the vast silence, my work ensures our survival and success, turning the impossible into the backbone of our adventures among the stars.");
                    character.sprite = pirateEngineer;
                    tutorialToLoad = 4;
                    break;
                case "Pirate Vanguard":
                    pieceName.SetText("THE VANGUARD");
                    pieceDesc.SetText("Where the dark veil of space dares to suffocate hope, I shine as the vanguard, cutting through fear with the courage of a comet.\n\n As the vanguard of our crew, I lead our forays into the unknown, the first to face danger in the vacuum of space. My resolve is as unyielding as the metal of my armor, steering us through cosmic storms and enemy blockades. In the frontier of the galaxy, my bravery lights our way, a beacon of defiance and exploration, charting a course through the stars.");
                    character.sprite = pirateVanguard;
                    tutorialToLoad = 5;
                    break;
                case "Pirate Navigator":
                    pieceName.SetText("THE NAVIGATOR");
                    pieceDesc.SetText("Amongst the tapestry of the cosmos, I draw our path, a navigator's vow to sail the stars, where adventure and mystery intertwine.\n\n As the navigator of our crew, I chart our course through the star-studded infinity. My eyes decipher the cosmos, turning celestial bodies into waypoints on our treasure-laden journey. In this vast, silent ocean, my calculations fuel our adventures, threading through wormholes and asteroid fields. Guided by constellations, I steer our ship towards fortunes untold, a cosmic pathfinder in the endless night.");
                    character.sprite = pirateNavigator;
                    tutorialToLoad = 6;
                    break;
                case "Pirate Gunner":
                    pieceName.SetText("THE GUNNER");
                    pieceDesc.SetText("With every burst from my cannon, I write our legacy among the stars, a gunner's promise to defend our freedom in the cosmic sea.\n\n Amid the silence of space, I stand as the gunner, master of my personal arsenal. My aim is guided by the stars, firing with precision that rivals the pull of gravity. Each shot declares our freedom, echoing through the void. In the dance of combat and cosmos, my skills safeguard our quest for adventure and treasure, a relentless defender of our celestial domain.");
                    character.sprite = pirateGunner;
                    tutorialToLoad = 7;
                    break;
                case "Tutorial":
                    
                    switch (tutorialToLoad)
                    {                        
                        case 1:
                            pieceName.SetText("THE CREWMATE");
                            pieceDesc.SetText("Moves one square in any direction, but cannot move backwards unless the Mate has captured an enemy piece. Captures by replacement. Can perform Jailbreaks after reaching the opponent’s Royal Zone.\r\n\nUse the navy Mate to capture the enemy Mate up top first for it to gain the ability to move backwards, and then capture the other enemy beneath.\r\nTake into account that you will be taking turns for both the navy and the pirates, but the focus of this tutorial is the navy Mate.");
                            // character.sprite = navyCrew;
                            break;
                        case 2:
                            pieceName.SetText("THE QUARTERMASTER");
                            pieceDesc.SetText("Moves two spaces orthogonally and one space perpendicularly. Jumps over blockers and captures by replacement.\r\n\nUse the Quatermaster's movement to jump over the energy shield and capture the enemy Mate.\r\nTake into account that you will be taking turns for both the navy and the pirates, but the focus of this tutorial is the navy Quartermaster.");
                            // character.sprite = navyQuatermaster;
                            break;
                        case 3:
                            pieceName.SetText("THE CANNONEER");
                            pieceDesc.SetText("Can move one unblocked space in any direction, captures by jumping any unblocked distance orthogonally and must land on the opposite adjacent square to the captured piece. An Energy Shield can be jumped over in this way, but the Energy Shield won’t be captured.\r\n\nSince the cannoneer can't jump over energy shields, move to the side to be able to capture the unlbocked enemy Mate.\r\nTake into account that you will be taking turns for both the navy and the pirates, but the focus of this tutorial is the navy Cannoneer.");
                            // character.sprite = navyCannon;
                            break;
                        case 4:
                            pieceName.SetText("THE ENGINEER");
                            pieceDesc.SetText("Moves up to two unblocked spaces in any direction, can capture enemy Energy Shields, ore, and orebearers by replacement. Once an Engineer has captured an Energy Shield, it can capture any enemy piece for as long as that Land Mine remains captured. Can also place one enemy Land Mine from the Jail Zone back to the game board in any square orthogonally adjacent instead of moving or capturing in a turn.\r\n\nTest how the Engineer can capture enemy energy shields (you'll notice you can't do the same with friendly ones) and relocate the Energy Shields around you.\r\nTake into account that you will be taking turns for both the navy and the pirates, but the focus of this tutorial is the navy Engineer.");
                            // character.sprite = navyEngineer;
                            break;
                        case 5:
                            pieceName.SetText("THE VANGUARD");
                            pieceDesc.SetText("Moves one square forward or backward, both orthogonally and diagonally, but can move any unblocked distance sideways. Captures by replacement.\r\n\nTaking into account the Vanguard's movement, try figuring out which enemy Mate will take less moves to capture and then test if you were right.\r\nTake into account that you will be taking turns for both the navy and the pirates, but the focus of this tutorial is the navy Vanguard.");
                            // character.sprite = navyVanguard;
                            break;
                        case 6:
                            pieceName.SetText("THE NAVIGATOR");
                            pieceDesc.SetText("Moves one square sideways, both orthogonally and diagonally, but can move any unblocked distance forwards and backwards. Captures by replacement.\r\n\nTaking into account the Navigator's movement, try figuring out which enemy Mate will take less moves to capture and then test if you were right.\r\nTake into account that you will be taking turns for both the navy and the pirates, but the focus of this tutorial is the navy Navigator.");
                            // character.sprite = navyNavigator;
                            break;
                        case 7:
                            pieceName.SetText("THE GUNNER");
                            pieceDesc.SetText("Moves one unblocked space in any direction, captures by shooting a piece up to 4 unblocked spaces away in any direction. Cannot capture by replacement and must move after capturing a piece before capturing another piece.\r\n\nUse the Gunner's special capture to capture the enemy Mates, you'll notice that you cannot capture the mate to the right because it's too far away, neither the one above you because an Energy Shield blocks your way, start by capturing the one under and move to recharge your shot, then try to capture the rest of the mates in the least possible moves.\r\nTake into account that you will be taking turns for both the navy and the pirates, but the focus of this tutorial is the navy Gunner.");
                            // character.sprite = navyGunner;
                            break;
                        case 8:
                            pieceName.SetText("THE ADMIRAL");
                            pieceDesc.SetText("Moves any unblocked distance in any direction, captures by replacement.\r\n\nUse the Admiral's movement to capture all the enemy Mates with the least amount of moves possible.\r\nTake into account that you will be taking turns for both the navy and the pirates, but the focus of this tutorial is the Admiral.");
                            // character.sprite = admiral;             
                            break;
                        case 9:
                            pieceName.SetText("THE TACTICIAN");
                            pieceDesc.SetText("Moves up to two unblocked squares orthogonally, captures by replacement. Can also use the moveset of any enemy piece within whatever zone the Tactician is in (Allied Start Zone, Neutral Zone, or Enemy Start Zone).\r\n\nUse the Tacticians special ability to imitate the Gunner within the same zone, and then use the gunner's shot to capture the enemy Mate.\r\nTake into account that you will be taking turns for both the navy and the pirates, but the focus of this tutorial is the Tactician.");
                            // character.sprite = tactician;
                            break;
                        case 10:
                            pieceName.SetText("THE CAPTAIN");
                            pieceDesc.SetText("Moves exactly five squares in any orthogonal direction, and can change direction mid-move. Cannot visit the same square twice in a move. Jumps over blockers, and captures any enemy piece in the fifth square by replacement.\r\n\nUse the Captain's special movement to sort the Energy Shields around you and capture the enemy Mate behind the Shields.\r\nTake into account that you will be taking turns for both the navy and the pirates, but the focus of this tutorial is the Captain.");
                            // character.sprite = Captain;
                            break;
                        case 11:
                            pieceName.SetText("THE CORSAIR");
                            pieceDesc.SetText("Moves two spaces diagonally, captures by replacement, or can jump to any open square on the board. Cannot jump two turns in a row.\r\n\nUse the Corsair's jump to capture the enemy Mates on the opposite sides of the board, remember that the Corsair cannot use its jump two turns straight.\r\nTake into account that you will be taking turns for both the navy and the pirates, but the focus of this tutorial is the Corsair");
                            // character.sprite = Corsair;
                            break;
                    }
                    break;

            }
            Vector2 topPos = bottomPos + Vector2.up * topHeight;
            StartCoroutine(MoveInfo(topPos));
            if (currentScene.name != "TTBoard")
            {
                background.SetActive(true);
                StartCoroutine(SetTransparency(transparency, true));
            }
        }
        else
        {
            StartCoroutine(MoveInfo(bottomPos));
            if (currentScene.name != "TTBoard")
            {
            StartCoroutine(SetTransparency(0, false));
            }
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

    public void toTutorial()
    {
        SceneManager.LoadScene("TTBoard");
    }
}