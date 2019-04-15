using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Creature
{
    public float NormalAttackRange { get; set; }
    public float NormalAttackDMG { get; set; }    
    public float PowerAttackRange { get; set; }
    public float PowerAttackMultiply { get; set; }

    private float distance;
    //monster have only one action per turn

    public void Action(float distance)
    {
        this.distance = distance;
        if (distance > Mathf.Max(PowerAttackRange, NormalAttackRange)) //if player out of attack range, monster move
        {
            Move(-3f);
            Debug.Log("Monster move 3.");
        }
        else if (distance <= PowerAttackRange) //player in power attack range and no dodge buff
        {
                Debug.Log("Monster power attack, making damage: " + NormalAttackDMG * PowerAttackMultiply);
                BattlefieldController.battlefield.player.TakeDMG(NormalAttackDMG * PowerAttackMultiply);
        }
        else if (distance > PowerAttackRange && distance <= NormalAttackRange)//player in normal attack range and no dodge buff
        {
                Debug.Log("Monster normal attack, making damage: " + NormalAttackDMG);
                BattlefieldController.battlefield.player.TakeDMG(NormalAttackDMG);
        }
    }
    //public void Action(float distance, bool isMiss)
    //{
    //    this.distance = distance;
    //    if (distance > Mathf.Max(PowerAttackRange, NormalAttackRange)) //if player out of attack range, monster move
    //    {
    //        Move(-5f);
    //    }
    //    else 
    //    {
    //        if (distance <= PowerAttackRange && !isMiss) //player in power attack range and no dodge buff
    //        {
    //            Debug.Log("Monster power attack, making damage: " + NormalAttackDMG * PowerAttackMultiply);
    //            BattlefieldController.battlefield.player.TakeDMG(NormalAttackDMG * PowerAttackMultiply);
    //        }
    //        else if (distance > PowerAttackRange && distance <= NormalAttackRange && !isMiss)//player in normal attack range and no dodge buff
    //        {
    //            Debug.Log("Monster normal attack, making damage: " + NormalAttackDMG);
    //            BattlefieldController.battlefield.player.TakeDMG(NormalAttackDMG);
    //        }
    //        else  //player has dodge buff, monster attack miss
    //        {
    //            Debug.Log("Monster attack miss!");
    //            BattlefieldController.battlefield.player.IsDodge = false;
    //        }                
    //    }       
    //}


    /// <summary>
    /// Use the first time generate a monster, set it's attack dmg and range
    /// </summary>
    /// <param name="NormalAttackRange"></param>
    /// <param name="NormalAttackDMG"></param>
    /// <param name="PowerAttackRange"></param>
    /// <param name="PowerAttackMultiply"> power attack dmg = normal attack dmg * power attack multiply</param>
    public void InitializeAttackInfo(float NormalAttackRange, float NormalAttackDMG, float PowerAttackRange, float PowerAttackMultiply)
    {
        this.NormalAttackRange = NormalAttackRange;
        this.NormalAttackDMG = NormalAttackDMG;
        this.PowerAttackRange = PowerAttackRange;
        this.PowerAttackMultiply = PowerAttackMultiply;
    }
}
