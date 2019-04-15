using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float WeaponDamage { get; set; } // normal dagame
    public float WeaponRange { get; set; }
    public float PowerAttackMultiply { get; set; } //power attack dmg = Damage * PowerAttackMultiply
    public BuffType Debuff { get; set; } //add debuff when normal attack make damage

    /* 
     */
    float normalAttackRate;
    float powerAttackRate;
    float chargeRate;
    int greenActionNum, redActionNum;
    float powRate = 1.4f;//initialize this when initializ weapon =================================================================

    /// <summary>
    /// initialize weapon proterities by input, use this function when first time generate a new weapon
    /// </summary>
    /// <param name="dmg"></param>
    /// <param name="range"></param>
    /// <param name="debuff">add debuff when normal attack make damage</param>
    /// <param name="greenActionNum">0:normal attack;</param>
    /// <param name="redActionNum">0:charge to enhance normal attack</param>
    /// <param name="powerAttackMultiply">power attack dmg = Damage * PowerAttackMultiply, default value = 1</param>
    /// <param name="normalAttackRate"> default value = 1</param>
    /// <param name="powerAttackRate"> default value = 1</param>
    public void Initialize(float dmg, float range, BuffType debuff, int greenActionNum, int redActionNum, float chargeRate = 0, float powerAttackMultiply = 1, float normalAttackRate = 0, float powerAttackRate = 1)
    {
        this.WeaponDamage = dmg;
        this.WeaponRange = range;
        this.Debuff = debuff;
        this.greenActionNum = greenActionNum;
        this.redActionNum = redActionNum;
        this.chargeRate = chargeRate;
        this.PowerAttackMultiply = powerAttackMultiply;
        this.normalAttackRate = normalAttackRate;
        this.powerAttackRate = powerAttackRate;
    }

    //monster take damage when player normal attack, and add buff, 
    private void SingleNormalAttack(int num)
    {
        BattlefieldController.battlefield.monster.TakeDMG(this.WeaponDamage *(1 + this.normalAttackRate + this.chargeRate));
        if (this.chargeRate != 0)
        {
            this.chargeRate = 0;
            BattlefieldController.battlefield.player.ChargeReset();
            BattlefieldController.battlefield.ResetSingleScore("red");
        }
        BattlefieldController.battlefield.monster.AddBuff(Debuff, num);
    }

    ////monster take damage when player power attack, and multi damage
    private void SinglePowerAttack()
    {
        BattlefieldController.battlefield.monster.TakeDMG(this.WeaponDamage * PowerAttackMultiply * powerAttackRate);
    }

    //how many times normal attack will triggered *******************************edit retain piece info, it should = 0 if used, or retain piece
    public int GreenAction(int piece)
    {
        switch (this.greenActionNum)
        {
            case 0: //normal attack
                if (piece != 0)
                {
                    for (int i = 0; i < Mathf.Floor(piece / 3f); i++)
                    {
                        SingleNormalAttack(piece);
                    }
                }
                break;
        }
        
        return piece;
    }

    //how many times power attack will triggered
    //public int RedAction(int piece)
    //{
    //    if (piece != 0)
    //    {
    //        for (int i = 0; i < piece; i++)
    //        {
    //            SinglePowerAttack();
    //        }
    //    }
    //    return piece;
    //}
    /// <summary>
    /// 1:charge
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    public int RedAction(int piece)
    {
        //1:charge,  enhance normal attack, move and dodge will make charge rate reset
        switch (this.redActionNum)
        {
            case 0: //charge, enhance normal attack, it will reset when move or take dmg
                if (piece != 0)
                {                       
                    chargeRate = Mathf.Pow((1 + piece / 3), powRate) - 1;
                }
                BattlefieldController.battlefield.player.AddBuff(BuffType.charge, piece);                
                break;
        }
        return piece;
    }
}
