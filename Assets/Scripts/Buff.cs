using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum Type
{
    power,
    dodge,
    stunning,
    bleeding,
}

public class Buff : MonoBehaviour
{   
    public Text remainTurnsText;
    public Type type;
    public Creature creature;

    public int RemainTurn { get; set; }
    
    //bufftype: Bleeding, Stunning, 
    public void Initialize(string buffType, int remainTurn, Transform buffZone, Creature creature)
    {
        this.creature = creature;
        switch (buffType)
        {
            case "power":
                this.type = Type.power;
                this.gameObject.GetComponent<Image>().color = new Color(0.925f, 0.408f, 0.471f);
                //this.gameObject.GetComponent<Image>().sprite = Resources.Load( Image path);
                break;
            case "dodge":
                this.type = Type.dodge;
                this.gameObject.GetComponent<Image>().color = new Color(0.537f, 0.667f, 0.859f);                
                this.creature.IsDodge = true;
                //this.gameObject.GetComponent<Image>().sprite = Resources.Load( Image path);
                break;
            case "bleeding":
                this.type = Type.bleeding;
                this.gameObject.GetComponent<Image>().color = new Color(0.69f, 0.059f, 0.059f);
                //this.gameObject.GetComponent<Image>().sprite = Resources.Load( Image path);
                break;
        }
        
        this.RemainTurn = remainTurn;
        UpdateBuffTurnText();
        this.gameObject.transform.SetParent(buffZone);
        
    }

    public void UpdateBuffTurnText()
    {
        remainTurnsText.text = RemainTurn.ToString();
    }   
    
    public void BuffAffect()
    {
        switch (this.type)
        {
            case Type.bleeding:
                Debug.Log(creature.name + type + " dmg " + RemainTurn * 10f);
                creature.TakeDMG(RemainTurn*10f);
                break;
        }
    }
    

 }
