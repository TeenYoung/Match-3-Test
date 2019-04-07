using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buff : MonoBehaviour
{
    //public Image buffImage;
    public Text remainTurnsText;
    //public List<Image> buffImageList;

    public int remainTurn { get; set; }
    
    //bufftype: Bleeding, Stunning, 
    public void InitializeBuff(string buffType, int remainTurn)
    {
        //this.buffImage = buffImageList[0];
        this.remainTurn = remainTurn;
        UpdateBuff();
    }

    public void UpdateBuff()
    {
        remainTurnsText.text = remainTurn.ToString();
    }    
        
 }
