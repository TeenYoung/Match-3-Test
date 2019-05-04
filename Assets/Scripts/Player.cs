using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Creature
{
    public Weapon weapon;
    private float dodgeRatePerPiece;

    /// <summary>
    /// use this function when first time generat a player, and include a weapon
    /// </summary>
    /// <param name="playerMaxHP"></param>
    /// <param name="weaponType"></param>
    public void Initialize(float playerMaxHP, string weaponType, float dodgeRatePerPiece)//float dmg, float ran, float attRate)
    {
        this.InitializeHPSlider(playerMaxHP);
        switch (weaponType)
        {
            case "longbow":
                weapon.Initialize(6, 1000, "Bleeding", 0, 0);
                break;
        }
        this.dodgeRatePerPiece = dodgeRatePerPiece;
        this.ChargeLayer = 0;
        //this.DodgeRate = 0;
    }

    //public void Attack(int greenScore, int redScore)
    //{
    //    ////green attack
    //    if (greenScore != 0) {
    //        weapon.GreenAction(greenScore);
    //        //BoardController.board.monster.TakeDMG(greenSum );
    //        //Debug.Log("Player normal attack. Green is "+ greenSum);
    //    }

    //    //red attack: 
    //    if (redScore != 0) {
    //      //// power attack, add bleeding buff to monster
    //        weapon.RedAction(redScore);
    //        //Debug.Log("Player power attack. Red is " + redScore);
    //    }        
    //}

    public void Dodge(int blueSum)
    {
        AddBuff("Dodge", blueSum * dodgeRatePerPiece);
    }
    
    //player use items
    public void UseItem(int purpleSum)
    {
        Debug.Log(string.Format("Player special. Purple is {0}.", purpleSum));
    }   

}