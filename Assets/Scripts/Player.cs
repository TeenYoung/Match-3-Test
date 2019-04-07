using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Creature
{

    //public Player(string playerName, float initialHP) : base(playerName, initialHP)
    //{
    //    this.CreatureName = playerName;
    //    this.currentHP = initialHP;
    //}

    //**************change input variable according to the way of attack***************
    public void Attack(int greenSum, int redSum)
    {
        //green attack
        if (greenSum != 0) {
            BoardController.board.monster.TakeDMG(greenSum * 5);
            Debug.Log("Player attack. Green is "+ greenSum);
        }

        //red attack
        if (redSum != 0) {
            BoardController.board.monster.TakeDMG(redSum * 5);
            Debug.Log("Player attack. Red is " + redSum);
        }        
    }

    //player use items
    public void UseItem(int purpleSum)
    {
        Debug.Log(string.Format("Player special. Purple is {0}.", purpleSum));
    }

}