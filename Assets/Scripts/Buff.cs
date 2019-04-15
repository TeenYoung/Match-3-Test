using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum BuffType
{
    power,
    dodge,
    stunning,
    bleeding,
    healing,
    charge,
}

public class Buff : MonoBehaviour
{   
    public Text BuffText;
    public BuffType type;
    public Creature creature;

    public int BuffNum { get; set; }

    float bleedingDMGRate;
    float healHP;



    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="remainTurn"></param>
    /// <param name="buffZone"></param>
    /// <param name="creature"></param>
    /// <param name="color"></param>
    /// <param name="tagName">tagName: TriggerAt_TurnBegin, TriggerAt_TurnEnd, TriggerAt_Move, TriggerAt_Attack, TriggerAt_TakeDMG...</param>
    /// <param name="bleedingDMGRate"></param>
    /// <param name="dodgeRate"></param>
    ////public void Initialize(BuffType type, int remainTurn, Transform buffZone, Creature creature, Image image, string tagName,float bleedingDMGRate, float dodgeRate
    public void Initialize(BuffType type, int remainTurn, Transform buffZone, Creature creature, Color color, string tagName,float bleedingDMGRate, float dodgeRate,float healHP, int chargeLayer)
    {
        this.type = type;

        //buff num depends on buff type
        if (type == BuffType.dodge) //show dodge rate
        {
            this.BuffNum = System.Convert.ToInt32(dodgeRate);
        }
        else if (type == BuffType.charge)
        {
            this.BuffNum = chargeLayer;
            //Debug.Log("Charge buff added.");
        }
        else this.BuffNum = remainTurn;
        //this.BuffNum = remainTurn;

        this.creature = creature;
        this.gameObject.GetComponent<Image>().color = color;
        this.gameObject.tag = tagName;
        //this.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(imgPath) as Sprite; //need to re-edit

        this.bleedingDMGRate = bleedingDMGRate;
        
        this.creature.ChargeLayer = chargeLayer;
        this.creature.DodgeRate = dodgeRate;
        this.healHP = healHP;
        this.gameObject.transform.SetParent(buffZone);
        UpdateBuffTurnText();

    }

    public void UpdateBuffTurnText()
    {
        BuffText.text = this.BuffNum.ToString();
    }

    public void Trigger()
    {
        if (bleedingDMGRate != 0)
        {            
            Debug.Log(creature.name + type + " dmg " + BuffNum * bleedingDMGRate);
            creature.TakeDMG(BuffNum * bleedingDMGRate);
            this.BuffNum--;
            UpdateBuffTurnText();
            ////destroy the object if remainturn =0
            //if (RemainTurn == 0)
            //{
            //    Destroy(this.gameObject);
            //}
        }
        if (healHP != 0)
        {
            Debug.Log(creature.name + " healing buff triggered.");
            creature.Heal(healHP);
            this.BuffNum--;
            UpdateBuffTurnText();
        }
        
    }

}
