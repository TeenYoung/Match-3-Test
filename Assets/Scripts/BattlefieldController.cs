using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlefieldController : MonoBehaviour
{
    public static BattlefieldController battlefield = null;

    //UI
    public Text blackScoreText, blueScoreText, greenScoreText, purpleScoreText, redScoreText;
    public Text distanceText;

    //Creatures
    public Player player;
    public Monster monster;
    public GameObject playerPrefab, monsterPrefab;

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
                    blueScore++;
                    break;
                case "Color_Blue":
                    blueScore++;
                    break;
                case "Color_Green":
                    greenScore++;
                    break;
                case "Color_Purple": //add purpleSum when pieces less than purple max
                    if (purpleScore <= MaxPurpleScore) { purpleScore++; }
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
        player.Initialize(100f, "longbow");
    }

    public void Battle()
    {
        if (purpleScore == MaxPurpleScore)
        {
            print("Item used");
            purpleScore = 0;
            UpdateScores();
        }

        if (blueScore != 0)
        {
            player.Dodge(blueScore);
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
        redScore = player.weapon.RedAction(redScore);
        UpdateScores();

        greenScore = player.weapon.GreenAction(greenScore);
        UpdateScores();

        monster.Action(Distance, player.IsDodge);
        UpdateDistance();


        //monster attack if player in attack range

        //if ( distance < Mathf.Max(monster.PowerAttackRange,monster.NormalAttackRange))
        //{                    
        //    //monster attack miss if player has dodge buff
        //    //if(player.buffList.Exists(x => x.GetComponent<Buff>())
        //    //{
        //    //    Debug.Log("Monster attack miss.");
        //    //}
        //    if (distance > monster.PowerAttackRange)
        //    {
        //        monster.NormalAttack(player.IsDodge);
        //    }
        //    else monster.PowerAttack(player.IsDodge);
        //}
        //// monster move if player out of attack range
        //else
        //{
        //    Debug.Log("Monster move, player out of attack range.");
        //    monster.Move(-5f);
        //    UpdateDistanceBoard();
        //}  

        //monster turn ends   
        monster.BuffDecreaseOne();
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
