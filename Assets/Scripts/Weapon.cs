using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float Damage { get; set; } // normal dagame
    public float Range { get; set; }
    public float PowerAttackMultiply { get; set; } //power attack dmg = Damage * PowerAttackMultiply
    public BuffType Debuff { get; set; } //add debuff when normal attack make damage

    /* how skillful player can use this weapon,
    the real damage player can cause = damage * attack rate, 
    rate from 0 to 1:
    0 means never use this weapon, 1 means fully skilled*/
    private float normalAttackRate;
    private float powerAttackRate;

    /// <summary>
    /// initialize weapon proterities by input, use this function when first time generate a new weapon
    /// </summary>
    /// <param name="dmg"></param>
    /// <param name="range"></param>
    /// <param name="debuff">add debuff when normal attack make damage</param>
    /// <param name="powerAttackMultiply">power attack dmg = Damage * PowerAttackMultiply, default value = 1</param>
    /// <param name="normalAttackRate"> default value = 1</param>
    /// <param name="powerAttackRate"> default value = 1</param>
    public void Initialize(float dmg, float range, BuffType debuff, float powerAttackMultiply = 1, float normalAttackRate = 1, float powerAttackRate = 1)
    {
        this.Damage = dmg;
        this.Range = range;
        this.Debuff = debuff;
        this.PowerAttackMultiply = powerAttackMultiply;
        this.normalAttackRate = normalAttackRate;
        this.powerAttackRate = powerAttackRate;
    }

    //monster take damage when player normal attack, and add buff
    private void SingleNormalAttack()
    {
        BattlefieldController.battlefield.monster.TakeDMG(this.Damage * normalAttackRate);
        BattlefieldController.battlefield.monster.AddBuff(Debuff);
    }

    ////monster take damage when player power attack, and multi damage
    private void SinglePowerAttack()
    {
        BattlefieldController.battlefield.monster.TakeDMG(this.Damage * PowerAttackMultiply * powerAttackRate);
    }

    //how many times normal attack will triggered *******************************edit retain piece info, it should = 0 if used, or retain piece
    public int GreenAction(int piece)
    {
        if(piece != 0)
        {
            for (int i = 0; i < Mathf.Floor(piece / 3f); i++)
            {
                SingleNormalAttack();
            }
        }
        return piece;
    }

    //how many times power attack will triggered
    public int RedAction(int piece)
    {
        if (piece != 0)
        {
            for (int i = 0; i < piece; i++)
            {
                SinglePowerAttack();
            }
        }
        return piece;
    }

}
