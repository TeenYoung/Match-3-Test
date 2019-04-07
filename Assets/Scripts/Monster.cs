using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Creature
{
    public float NormalAttackDMG { get; set; }
    public float PowerAttackDMG { get; set; }
    public float NormalAttackRange { get; set; }
    public float PowerAttackRange { get; set; }

   
    public void NormalAttack()
    {
        Debug.Log("Monster normal attack, making damage: " + NormalAttackDMG);
        BoardController.board.player.TakeDMG(NormalAttackDMG);
    }

    public void PowerAttack()
    {
        Debug.Log("Monster power attack, making damage: " + PowerAttackDMG);
        BoardController.board.player.TakeDMG(PowerAttackDMG);
    }

    

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
        this.PowerAttackDMG = PowerAttackDMG;
    }
}
