using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Creature
{
    public Weapon weapon;

    /// <summary>
    /// use this function when first time generat a player, and include a weapon
    /// </summary>
    /// <param name="playerMaxHP"></param>
    /// <param name="weaponType"></param>
    public void Initialize(float playerMaxHP, string weaponType)//float dmg, float ran, float attRate)
    {
        this.InitializeHPSlider(playerMaxHP);
        switch (weaponType)
        {
            case "longbow":
                weapon.Initialize(5,1000,BuffType.bleeding);
                break;
        } 
    }

    public void Attack(int greenSum, int redSum)
    {
        ////green attack
        if (greenSum != 0) {
            weapon.GreenAction(greenSum);
            //BoardController.board.monster.TakeDMG(greenSum );
            //Debug.Log("Player normal attack. Green is "+ greenSum);
        }

        //red attack: 
        if (redSum != 0) {
          //// power attack, add bleeding buff to monster
            weapon.RedAction(redSum);
            Debug.Log("Player power attack. Red is " + redSum);
        }        
    }

    public void Dodge(int blueSum)
    {
        AddBuff(BuffType.dodge, blueSum);
    }
    
    //player use items
    public void UseItem(int purpleSum)
    {
        Debug.Log(string.Format("Player special. Purple is {0}.", purpleSum));
    }   

}