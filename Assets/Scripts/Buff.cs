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
}

public class Buff : MonoBehaviour
{   
    public Text remainTurnsText;
    public BuffType type;
    public Creature creature;

    public int RemainTurn { get; set; }
    float bleedingDMGRate;



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
    public void Initialize(BuffType type, int remainTurn, Transform buffZone, Creature creature, Color color, string tagName,float bleedingDMGRate, float dodgeRate)
    {
        this.type = type;
        this.RemainTurn = remainTurn;
        this.gameObject.transform.SetParent(buffZone);
        this.creature = creature;
        this.gameObject.GetComponent<Image>().color = color;
        this.gameObject.tag = tagName;
        //this.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(imgPath) as Sprite; //need to re-edit

        this.bleedingDMGRate = bleedingDMGRate;
        UpdateBuffTurnText();
        this.creature.DodgeRate = dodgeRate;
    }

    public void UpdateBuffTurnText()
    {
        remainTurnsText.text = this.RemainTurn.ToString();
    }

    public void Trigger()
    {
        if (bleedingDMGRate != 0)
        {            
            Debug.Log(creature.name + type + " dmg " + RemainTurn * bleedingDMGRate);
            creature.TakeDMG(RemainTurn * bleedingDMGRate);
            this.RemainTurn--;
            UpdateBuffTurnText();
            ////destroy the object if remainturn =0
            //if (RemainTurn == 0)
            //{
            //    Destroy(this.gameObject);
            //}
        }        
    }

}
