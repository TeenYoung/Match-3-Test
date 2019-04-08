using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float Damage { get; set; } // normal dagame
    public float Range { get; set; }
    public float PowerAttackMultiply { get; set; } //power attack dmg = Damage * PowerAttackMultiply
    public string Debuff { get; set; } //add debuff when normal attack make damage

    /* how skillful player can use this weapon,
    the real damage player can cause = damage * attack rate, 
    rate from 0 to 1:
    0 means never use this weapon, 1 means fully skilled*/
    private float normalAttackProficiency;
    private float powerAttackProficiency;

    /// <summary>
    /// initialize weapon proterities by input, use this function when first time generate a new weapon
    /// </summary>
    /// <param name="dmg"></param>
    /// <param name="range"></param>
    /// <param name="debuff">add debuff when normal attack make damage</param>
    /// <param name="powerAttackMultiply">power attack dmg = Damage * PowerAttackMultiply, default value = 1</param>
    /// <param name="normalAttackProficiency">how skill the player can use the weapon to make normal attack damage by percentage, default value = 1</param>
    /// <param name="powerAttackProficiency">how skill the player can use the weapon to make power attack damage by percentage, default value = 1</param>
    public void Initialize(float dmg, float range, string debuff, float powerAttackMultiply = 1, float normalAttackProficiency = 1, float powerAttackProficiency = 1)
    {
        this.Damage = dmg;
        this.Range = range;
        this.Debuff = debuff;
        this.PowerAttackMultiply = powerAttackMultiply;
        this.normalAttackProficiency = normalAttackProficiency;
        this.powerAttackProficiency = powerAttackProficiency;
    }

    //monster take damage when player normal attack, and add buff
    private void SingleNormalAttack()
    {
        BoardController.board.monster.TakeDMG(this.Damage * normalAttackProficiency);
        BoardController.board.monster.AddBuff(Debuff, 2);
    }

    ////monster take damage when player power attack, and multi damage
    private void SinglePowerAttack()
    {
        BoardController.board.monster.TakeDMG(this.Damage * PowerAttackMultiply * powerAttackRate);
    }

    //how many times normal attack will triggered
    public void NormalAttacks(int piece)
    {
        for(int i = 0; i < piece; i++)
        {
            SingleNormalAttack();
        }
    }

    //how many times power attack will triggered
    public void PowerAttacks(int piece)
    {
        for (int i = 0; i < piece; i++)
        {
            SinglePowerAttack();
        }
    }

}
