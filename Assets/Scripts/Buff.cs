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

    //bufftype: Bleeding, Stunning, 
    //public void Initialize(string buffType, int remainTurn, Transform buffZone, Creature creature)
    //{
    //    this.creature = creature;
    //    switch (buffType)
    //    {
    //        case "power":
    //            this.type = Type.power;
    //            this.gameObject.GetComponent<Image>().color = new Color(0.925f, 0.408f, 0.471f);
    //            //this.gameObject.GetComponent<Image>().sprite = Resources.Load( Image path);
    //            break;
    //        case "dodge":
    //            this.type = Type.dodge;
    //            this.gameObject.GetComponent<Image>().color = new Color(0.537f, 0.667f, 0.859f);                
    //            this.creature.IsDodge = true;
    //            //this.gameObject.GetComponent<Image>().sprite = Resources.Load( Image path);
    //            break;
    //        case "bleeding":
    //            this.type = Type.bleeding;
    //            this.gameObject.GetComponent<Image>().color = new Color(0.69f, 0.059f, 0.059f);
    //            //this.gameObject.GetComponent<Image>().sprite = Resources.Load( Image path);
    //            break;
    //    }

    //    this.RemainTurn = remainTurn;
    //    UpdateBuffTurnText();
    //    this.gameObject.transform.SetParent(buffZone);

    //}

    //public void Initialize(BuffType type, int remainTurn, Transform buffZone, Creature creature, string imgPath,float bleedingDMGRate, bool isDodge)
    public void Initialize(BuffType type, int remainTurn, Transform buffZone, Creature creature, Color color, float bleedingDMGRate, float dodgeRate)
    {
        this.type = type;
        this.RemainTurn = remainTurn;
        this.gameObject.transform.SetParent(buffZone);
        this.creature = creature;
        this.gameObject.GetComponent<Image>().color = color;
        //this.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(imgPath) as Sprite; //need to re-edit

        this.bleedingDMGRate = bleedingDMGRate;
        UpdateBuffTurnText();
        this.creature.DodgeRate = dodgeRate;
    }

    public void UpdateBuffTurnText()
    {
        remainTurnsText.text = RemainTurn.ToString();
    }

    public void Trigger()
    {
        if (bleedingDMGRate != 0)
        {
            Debug.Log(creature.name + type + " dmg " + RemainTurn * bleedingDMGRate);
            creature.TakeDMG(RemainTurn * bleedingDMGRate);
            RemainTurn--;
            UpdateBuffTurnText();
        }        
    }

}
