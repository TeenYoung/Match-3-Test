using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattlefieldState
{
    initializing,
    waiting,
    battle
}

public class BattlefieldController : MonoBehaviour
{
    public static BattlefieldController battlefield = null;
    public BattlefieldState battlefieldState = BattlefieldState.initializing;

    //UI
    public Text blackScoreText, blueScoreText, greenScoreText, purpleScoreText, redScoreText;
    public Text distanceText;

    //Creatures
    public Player player;
    public Monster monster;
    public GameObject playerPrefab, monsterPrefab;

    //
    public ItemController items;

    //Properties
    public int MaxPurpleScore { get; set; }
    public float InitialDistance { get; set; }
    public float Distance { get; set; }

    private int blackScore, blueScore, greenScore, redScore, purpleScore;

    private void Awake()
    {
        if (battlefield == null)
        {
            battlefield = this;
        }
        else if (battlefield != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        MaxPurpleScore = 6;//Temp
        InitialDistance = 10;//Temp
        SetupBattlefield();
    }

    public void AddScore(GameObject piece)
    {
        if (piece != null)
        {
            //count sum of matched pieces in colors
            switch (piece.tag)
            {
                case "Color_Black":
                    blackScore++;
                    break;
                case "Color_Blue":
                    blueScore++;
                    break;
                case "Color_Green":
                    greenScore++;
                    break;
                case "Color_Purple": //add purpleSum when pieces less than purple max
                    if (purpleScore < MaxPurpleScore) { purpleScore++; }
                    else { purpleScore = MaxPurpleScore; }
                    break;
                case "Color_Red":
                    redScore++;
                    break;
            }
        }
    }

    public void SetupBattlefield()
    {
        InstantiateMonster();
        InstantiatePlayer();
        ClearScores();
        ResetDistance();
    }

    void InstantiateMonster()
    {
        monster = Instantiate(monsterPrefab, new Vector3(4.5f, 8.8f, 90f), Quaternion.identity).GetComponent<Monster>();
        monster.InitializeHPSlider(200f);
        monster.InitializeAttackInfo(10f, 5f, 5f, 1.8f);
    }

    void InstantiatePlayer()
    {
        player = Instantiate(playerPrefab, new Vector3(0.5f, 8.8f, 90f), Quaternion.identity).GetComponent<Player>();
        player.Initialize(100f, "longbow", 20);
    }

    public void Battle()
    {
        //monster buff trigger in turn start
        player.BuffListTrigger("TriggerAt_TurnBegin");
        monster.BuffListTrigger("TriggerAt_TurnBegin");

        if (purpleScore == MaxPurpleScore)
        {
            items.UseSelectedItem();
            purpleScore = 0;
            UpdateScores();
        }

        if (blueScore != 0)
        {
            player.Dodge(blueScore);
            if (player.ChargeLayer != 0) // if there is charge buff, dodge will reset it 
            {
                player.ChargeReset();
            }
            player.Move(blueScore / 3);
            blueScore = 0;
            UpdateScores();
        }

        if (blackScore != 0)
        {
            player.Move(blackScore);
            UpdateDistance();
            blackScore = 0;
            UpdateScores();
        }

        //Execute weapon actions and return remaining scores
        if (redScore != 0)
        {
            redScore = player.weapon.RedAction(redScore);
            redScore = 0;
            UpdateScores();
        }

        if (greenScore != 0)
        {
            greenScore = player.weapon.GreenAction(greenScore);
            greenScore = 0;
            UpdateScores();
        }
        

     //monster action        

        monster.Action(Distance);
        UpdateDistance();

        battlefieldState = BattlefieldState.waiting;
        print("Battle End");

        player.DodgeReset(); //reset dodge if it haven't triggered, dodge only last one turn
    }

    public void ResetSingleScore(string color)
    {
        switch (color)
        {
            case "black":
                blackScore = 0;
                blackScoreText.text = blackScore.ToString();
                break;
            case "blue":
                blueScore = 0;
                blueScoreText.text = blueScore.ToString();
                break;
            case "green":
                greenScore = 0;
                greenScoreText.text = greenScore.ToString();
                break;
            case "red":
                redScore = 0;
                redScoreText.text = redScore.ToString();
                break;
            case "purple":
                purpleScore = 0;
                purpleScoreText.text = purpleScore.ToString();
                break;
        }
    }

    public void ClearScores()
    {
        blackScore = 0;
        blueScore = 0;
        greenScore = 0;
        redScore = 0;
        purpleScore = 0;

        UpdateScores();
    }

    public void UpdateScores()
    {
        blackScoreText.text = blackScore.ToString();
        blueScoreText.text = blueScore.ToString();
        greenScoreText.text = greenScore.ToString();
        redScoreText.text = redScore.ToString();
        purpleScoreText.text = purpleScore.ToString();
    }

    public void ResetDistance()
    {
        Distance = InitialDistance;

        UpdateDistance();
    }

    void UpdateDistance()
    {
        distanceText.text = Distance.ToString();
    }

    public void DestoryCreatures()
    {
        Destroy(player.gameObject);
        Destroy(monster.gameObject);
    }
}
