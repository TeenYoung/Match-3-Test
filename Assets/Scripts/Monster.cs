using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Creature
{
    public float NormalAttackDMG { get; set; }
    public float PowerAttackMultiply { get; set; }
    public float NormalAttackRange { get; set; }
    public float PowerAttackRange { get; set; }

    private float distance;
    //monster have only one action per turn
    public void Action(float distance, bool isMiss)
    {
        this.distance = distance;
        if (distance > Mathf.Max(PowerAttackRange, NormalAttackRange)) //if player out of attack range, monster move
        {
            Move(-5f);
        }
        else 
        {
            if (distance <= PowerAttackRange && !isMiss) //player in power attack range and no dodge buff
            {
                Debug.Log("Monster power attack, making damage: " + NormalAttackDMG * PowerAttackMultiply);
                BoardController.board.player.TakeDMG(NormalAttackDMG * PowerAttackMultiply);
            }
            else if (distance > PowerAttackRange && distance <= NormalAttackRange && !isMiss)//player in normal attack range and no dodge buff
            {
                Debug.Log("Monster normal attack, making damage: " + NormalAttackDMG);
                BoardController.board.player.TakeDMG(NormalAttackDMG);
            }
            else  //player has dodge buff
            {
                Debug.Log("Monster attack miss!");
                BoardController.board.player.IsDodge = false;
            }
                
        }
       
    }


    //public void NormalAttack(bool miss)
    //{
    //    if (miss)
    //    {
    //        Debug.Log(" Normal attack miss! ");
    //    }
    //    else
    //    {
    //        Debug.Log("Monster normal attack, making damage: " + NormalAttackDMG);
    //        BoardController.board.player.TakeDMG(NormalAttackDMG);
    //    }
    //}

    //public void PowerAttack(bool miss)
    //{
    //    if (miss)
    //    {
    //        Debug.Log(" Power attack miss! ");
    //    }
    //    else
    //    {
    //        Debug.Log("Monster power attack, making damage: " + NormalAttackDMG * PowerAttackMultiply);
    //        BoardController.board.player.TakeDMG(NormalAttackDMG * PowerAttackMultiply);
    //    }        
    //}
    

    /// <summary>
    /// Set monster attack dmg and range
    /// </summary>
    /// <param name="NormalAttackRange"></param>
    /// <param name="NormalAttackDMG"></param>
    /// <param name="PowerAttackRange"></param>
    /// <param name="PowerAttackDMG"></param>
    public void InitializeAttackInfo(float NormalAttackRange, float NormalAttackDMG, float PowerAttackRange, float PowerAttackDMG)
    {
        this.NormalAttackRange = NormalAttackRange;
        this.NormalAttackDMG = NormalAttackDMG;
        this.PowerAttackRange = PowerAttackRange;
        this.PowerAttackMultiply = PowerAttackDMG;
    }
}
