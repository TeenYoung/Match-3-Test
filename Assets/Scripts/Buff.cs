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

    public int RemainTurn { get; set; }
    
    //bufftype: Bleeding, Stunning, 
    public void Initialize(string buffType, int remainTurn, Transform buffZone)
    {
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
                //this.gameObject.GetComponent<Image>().sprite = Resources.Load( Image path);
                break;
            case "bleeding":
                this.type = Type.bleeding;
                this.gameObject.GetComponent<Image>().color = new Color(0.69f, 0.059f, 0.059f);
                //this.gameObject.GetComponent<Image>().sprite = Resources.Load( Image path);
                break;
        }
        this.RemainTurn = remainTurn;
        UpdateBuff();
        this.gameObject.transform.SetParent(buffZone);
        
    }

    public void UpdateBuff()
    {
        remainTurnsText.text = RemainTurn.ToString();
    }       
    
    //public void Clear()
    //{
    //    switch (this.type)
    //    {
    //        case 0:

    //            break;
    //        case 1:

    //            break;
    //        case 2:

    //            break;
    //    }
    //    RemainTurn--;
    //    UpdateBuff();
    //}

 }
