using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creature : MonoBehaviour {

    public Slider hpSlider;
    public Image fillImage;
    public Color fullHpColor, zeroHpColor;
    public GameObject buffPrefab, buffZone;

    /*
     buff結算三種類型：
        每輪開始:列表buffEffectInBeginning，按照buff生效順序結算，各自調用buff.effective()函數，調用完自動清算buff顯示
        效果觸發時:觸發時調用buff.effective()函數，調用完清算buff顯示
        每輪結束前：按照buff生效順序結算，各自調用buff.effective()函數，調用完自動清算buff顯示
    */
    public List<GameObject> decreaseByTurnBuffList = new List<GameObject>();

    public bool IsDodge { get; set; }

    public string CreatureName { get; set; }

    public float CurrentHP { get; set; }

    public float MaxHP { get; set; }

    public void Move(float feet)
    {
        BattlefieldController.battlefield.Distance += feet;
        //Debug.Log("Distance is " + BoardController.distance);
    }

    public void TakeDMG(float dmg)
    {
        Debug.Log(this.name + "take dmg" + dmg);
        this.CurrentHP -= dmg;
        SetHPSlider();
    }  
    
    public void InitializeHPSlider(float maxHP)
    {
        this.MaxHP = maxHP;
        hpSlider.maxValue = this.MaxHP;
        this.CurrentHP = this.MaxHP;
        hpSlider.value = this.CurrentHP;
        SetHPSlider();
    }

    void SetHPSlider()
    {
        hpSlider.value = this.CurrentHP;       
        fillImage.color = Color.Lerp(zeroHpColor, fullHpColor, CurrentHP / MaxHP);
    }

    //default buff work turn =1, if buff exist in list , add turns; if not , add new buff
    public void AddBuff(BuffType buffType, int buffTurn = 1)
    {
        GameObject buffObj = decreaseByTurnBuffList.Find(x=>x.GetComponent<Buff>().type == buffType);        
        if (buffObj)
        {
            Buff buff = buffObj.GetComponent<Buff>();
            buff.RemainTurn += buffTurn;
            buff.UpdateBuffTurnText();
        }
        else
        {
            
            buffObj = Instantiate(buffPrefab, this.transform.position, Quaternion.identity);
            Buff buff = buffObj.GetComponent<Buff>();
        // search buff type in database and generat object then get Buff instance and initialize it ************************** edit here with database info and wrap spl commends 
            string imgPath = "sprites/icons/BGM" ;

            buff.Initialize(buffType, buffTurn, buffZone.transform, this, imgPath, 1, true);
            decreaseByTurnBuffList.Add(buffObj);
        }
    }

    public void BuffDecreaseOne()
    {
        if (decreaseByTurnBuffList != null)
        {
            for (int i = decreaseByTurnBuffList.Count - 1; i >= 0; i--)
            {
                Buff buff = decreaseByTurnBuffList[i].GetComponent<Buff>();
                buff.Trigger();
                buff.RemainTurn--;
                if (buff.RemainTurn < 0)
                {
                    Destroy(decreaseByTurnBuffList[i]);
                    decreaseByTurnBuffList.Remove(decreaseByTurnBuffList[i]);
                }
                else if (buff.RemainTurn == 0) //keep the space of buff turn=0 , make it invisible
                {
                    decreaseByTurnBuffList[i].GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    buff.remainTurnsText.text = "";
                }
                else
                {
                    buff.UpdateBuffTurnText();
                }
            }
        }
    }

    public void BuffClear()
    {
        //buff calculate
        //buff turn decrease
        //buff update
    }

}
