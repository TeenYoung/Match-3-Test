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
    private NextTurnAction nextTurnAction;   
    //monster have only one action per turn

    enum NextTurnAction
    {
        move,
        powerattack,
        normalattack,
    }

    public void Action(float distance)
    {
        this.distance = distance;

        switch (nextTurnAction)
        {
            case NextTurnAction.move:
                Move(-4f);
                Debug.Log("Monster move 4.");
                break;

            case NextTurnAction.powerattack:
                if (distance <= PowerAttackRange)
                {
                    Debug.Log("Monster power attack, making damage: " + NormalAttackDMG * PowerAttackMultiply);
                    BattlefieldController.battlefield.player.TakeDMG(NormalAttackDMG * PowerAttackMultiply);
                }
                else Debug.Log("Power attack out of range.");
                break;

            case NextTurnAction.normalattack:
                if (distance > PowerAttackRange && distance <= NormalAttackRange)
                {
                    Debug.Log("Monster normal attack, making damage: " + NormalAttackDMG);
                    BattlefieldController.battlefield.player.TakeDMG(NormalAttackDMG);
                }
                else Debug.Log("Normal attack out of range.");
                break;
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


    public void ActionNotice(float distance)
    {
        this.distance = distance;
        this.nextTurnAction = RollAction();
        switch (nextTurnAction)
        {
            case NextTurnAction.move:
                Debug.Log("Monster will move 3.");
                break;
            case NextTurnAction.powerattack:
                Debug.Log("Monster will make " + NormalAttackDMG * PowerAttackMultiply + " power attack.");
                break;
            case NextTurnAction.normalattack:
                Debug.Log("Monster will make " + NormalAttackDMG + " normal attack. "); 
                break;
        }       
    }

    private NextTurnAction RollAction()
    {
        List<NextTurnAction> actionAvailiable = new List<NextTurnAction>();
        if (this.distance != 1)
        {
            actionAvailiable.Add(NextTurnAction.move);
        }

        if (distance <= PowerAttackRange)
        {
            //actionAvailiable.Add(NextTurnAction.move);
            actionAvailiable.Add(NextTurnAction.powerattack);
            //actionAvailiable.Add(NextTurnAction.normalattack);
        }

        if (distance > PowerAttackRange && distance <= NormalAttackRange)
        {
            //actionAvailiable.Add(NextTurnAction.move);
            actionAvailiable.Add(NextTurnAction.normalattack);
        }
        // if distance < skill distance , add action in list

        int actionNum = Random.Range(0, actionAvailiable.Count);
        return actionAvailiable[actionNum];        
    }

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
