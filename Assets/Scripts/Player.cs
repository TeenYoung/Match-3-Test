using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Creature
{
    public Weapon weapon;
    //public Player(string playerName, float initialHP) : base(playerName, initialHP)
    //{
    //    this.CreatureName = playerName;
    //    this.currentHP = initialHP;
    //}

    //**************change input variable according to the way of attack***************
    public void Initialize(float playerMaxHP, string weaponType)//float dmg, float ran, float attRate)
    {
        this.InitializeHPSlider(playerMaxHP);
        switch (weaponType)
        {
            case "longbow":
                weapon.Initialize(5,1000, "bleeding");
                break;
        }        
        
    }

    public void Attack(int greenSum, int redSum)
    {
        ////green attack
        if (greenSum != 0) {
            weapon.NormalAttacks(greenSum);
            //BoardController.board.monster.TakeDMG(greenSum );
            //Debug.Log("Player normal attack. Green is "+ greenSum);
        }

        //red attack: 
        if (redSum != 0) {
            //// power attack, add bleeding to monster
            weapon.PowerAttacks(redSum);
            //this.AddBuff("power", redSum);
            //BoardController.board.monster.AddBuff ("bleeding",redSum);
            //BoardController.board.monster.TakeDMG(redSum );
            Debug.Log("Player power attack. Red is " + redSum);
        }        
    }

    //player use items
    public void UseItem(int purpleSum)
    {
        Debug.Log(string.Format("Player special. Purple is {0}.", purpleSum));
    }   

}