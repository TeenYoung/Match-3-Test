using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creature : MonoBehaviour {

    public Slider hpSlider;
    public Image fillImage;
    public Color fullHpColor, zeroHpColor;
    public GameObject buffPrefab, buffZone;
    
    public float CurrentHP { get; set; }

    /*
     buff結算三種類型：
        每輪開始:列表buffEffectInBeginning，按照buff生效順序結算，各自調用buff.effective()函數，調用完自動清算buff顯示
        效果觸發時:觸發時調用buff.effective()函數，調用完清算buff顯示
        每輪結束前：按照buff生效順序結算，各自調用buff.effective()函數，調用完自動清算buff顯示
    */

    public List<Buff> BuffList = new List<Buff>(); 

    public float dodgeRate;
    public bool isCharging; 

    float maxHP;    

    public void Move(float feet)
    {
        BattlefieldController.battlefield.Distance += feet;
        //Debug.Log("Distance is " + BoardController.distance);
    }    

    public void TakeDMG(float dmg)
    {
        float r = Random.Range(0, 100f); // use to calculate if dodge success
        if ( dodgeRate != 0 && r <= dodgeRate) 
        {
            Debug.Log("Attack miss! Random is " + r);           
        }
        else
        {
            if (dodgeRate != 0)
            {
                Debug.Log("Dodge failed. Random is " + r);
            }
            Debug.Log(this.name + "take dmg" + dmg);
            this.CurrentHP -= dmg;
            SetHPSlider();            
        }
        DodgeReset();//whether dodge success or not, dodge buff reset
    }  
    
    public void Heal(float heal)
    {
        Debug.Log("Recover HP: " + heal);
        this.CurrentHP += heal;
        if (this.CurrentHP >= this.maxHP)
        {
            this.CurrentHP = this.maxHP;
        }
        SetHPSlider();
    }

    public void InitializeHPSlider(float maxHP)
    {
        this.maxHP = maxHP;
        hpSlider.maxValue = this.maxHP;
        this.CurrentHP = this.maxHP;
        hpSlider.value = this.CurrentHP;
        SetHPSlider();
    }

    void SetHPSlider()
    {
        hpSlider.value = this.CurrentHP;       
        fillImage.color = Color.Lerp(zeroHpColor, fullHpColor, CurrentHP / maxHP);
    }

    //default buff work turn =1, if buff exist in list , add turns; if not , add new buff
    public void AddBuff(BuffType buffType,float num, int buffTurn = 1)
    {
        Buff buff = BuffList.Find(x => x.type == buffType);
        //GameObject buffObj = buff.gameObject;        
        if (buff)
        {
            //Buff buff = buffObj.GetComponent<Buff>();
            buff.RemainTurn += buffTurn;
            buff.UpdateBuffTurnText();
        }
        else
        {            
            GameObject buffObj = Instantiate(buffPrefab, this.transform.position, Quaternion.identity);
            buff = buffObj.GetComponent<Buff>();
            //Buff buff = buffObj.GetComponent<Buff>();
            // search buff type in database and generat object then get Buff instance and initialize it ************************** edit here with database info and wrap spl commends 
            
            if(buffType == BuffType.dodge)//use piece to calculate dodge rate,
            {
                buff.Initialize(buffType, buffTurn, buffZone.transform, this, Color.blue, "TriggerAt_TakeDMG", 0, num, 0);                
            }
            if(buffType == BuffType.bleeding)
            {
                buff.Initialize(buffType, buffTurn, buffZone.transform, this, Color.red, "TriggerAt_TurnBegin", 5, 0, 0);                
            }
            if (buffType == BuffType.healing)
            {
                buff.Initialize(buffType, buffTurn, buffZone.transform, this, Color.green, "TriggerAt_TurnBegin", 0, 0, num);
            }
            if(buffType == BuffType.charge)
            {
                buff.Initialize(buffType, System.Convert.ToInt32(num), buffZone.transform, this, Color.yellow, "TriggerAt_Attack", 0, 0, 0);
                this.isCharging = true;
            }
            BuffList.Add(buff);
            //decreaseByTurnBuffList.Add(buffObj);
        }
    }

    
    public void BuffListTrigger(string tagName)
    {
        for (int i = this.BuffList.Count - 1; i >= 0; i--)
        {
            if (this.BuffList[i].tag == tagName)
            {
                this.BuffList[i].Trigger();
                ////remove buff from list if the object = null;
                //if (!monster.BuffList[i])
                //{
                //    monster.BuffList.RemoveAt(i);
                //}
                if (this.BuffList[i].RemainTurn == 0)
                {
                    Destroy(this.BuffList[i].gameObject);
                    this.BuffList.RemoveAt(i);
                }
            }
        }
    }

    //public void BuffDecreaseOne()
    //{
    //    if (BuffList != null)
    //    {
    //        for (int i = BuffList.Count - 1; i >= 0; i--)
    //        {
    //            Buff buff = BuffList[i].GetComponent<Buff>();
    //            buff.Trigger();
    //            buff.RemainTurn--;
    //            if (buff.RemainTurn < 0)
    //            {
    //                Destroy(BuffList[i]);
    //                BuffList.Remove(BuffList[i]);
    //            }
    //            else if (buff.RemainTurn == 0) //keep the space of buff turn=0 , make it invisible
    //            {
    //                BuffList[i].GetComponent<Image>().color = new Color(0, 0, 0, 0);
    //                buff.remainTurnsText.text = "";
    //            }
    //            else
    //            {
    //                buff.UpdateBuffTurnText();
    //            }
    //        }
    //    }
    //}

    //use after dodge happen, reset dodgerate to 0 ,and clear buff object
    public void DodgeReset()
    {
        Buff buff = BuffList.Find(x => x.type == BuffType.dodge);
        BuffList.Remove(buff);
        dodgeRate = 0;
        if (buff)
        {
            Destroy(buff.gameObject);
        }
    }   

    public void ChargeReset()
    {
        Buff buff = BuffList.Find(x => x.type == BuffType.charge);
        BuffList.Remove(buff);
        dodgeRate = 0;
        if (buff)
        {
            Destroy(buff.gameObject);
        }
        this.isCharging = false;
    }
}
