using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//public enum BuffType
//{
//    power,
//    dodge,
//    stunning,
//    bleeding,
//    healing,
//    charge,
//}

public class Buff : MonoBehaviour
{   
    public Text buffText;
    public string buffName;
    public Creature creature;

    public int BuffNum { get; set; }

    float bleedingDMGBase;
    float healHP;
       
    /// <summary>
    /// 
    /// </summary>
    /// <param name="buffName"></param>
    /// <param name="remainTurn"></param>
    /// <param name="buffZone"></param>
    /// <param name="creature"></param>
    /// <param name="color"></param>
    /// <param name="tagName">tagName: TriggerAt_TurnBegin, TriggerAt_TurnEnd, TriggerAt_Move, TriggerAt_Attack, TriggerAt_TakeDMG...</param>
    /// <param name="bleedingDMGBase"></param>
    /// <param name="dodgeRateBase"></param>
    ////public void Initialize(BuffType type, int remainTurn, Transform buffZone, Creature creature, Image image, string tagName,float bleedingDMGRate, float dodgeRate
    public void Initialize(string buffName, int remainTurn, Transform buffZone, Creature creature, Color color, string tagName,float bleedingDMGBase, float dodgeRateBase,float healHP, int chargeLayer)
    {
        this.buffName = buffName;

        //buff num depends on buff type
        if (this.buffName == "Dodge") //show dodge rate
        {
            this.BuffNum = System.Convert.ToInt32(dodgeRateBase);
            //Debug.Log("Add dodge, rate = " + BuffNum);
        }
        else if (this.buffName == "Charge")
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

        this.bleedingDMGBase = bleedingDMGBase;        
        this.creature.ChargeLayer = chargeLayer;
        this.creature.DodgeRate = dodgeRateBase;
        this.healHP = healHP;
        this.gameObject.transform.SetParent(buffZone);
        UpdateBuffTurnText();
    }

    public void UpdateBuffTurnText()
    {
        buffText.text = this.BuffNum.ToString();
    }

    public void Trigger()
    {
        if (bleedingDMGBase != 0)
        {            
            Debug.Log(creature.name + buffName + " dmg " + BuffNum * bleedingDMGBase);
            creature.TakeDMG(BuffNum * bleedingDMGBase);
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
