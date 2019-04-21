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

    public float DodgeRate { get; set; }
    public int ChargeLayer { get; set; }

    float maxHP;    

    public void Move(float feet)
    {
        BattlefieldController.battlefield.Distance += feet;
        if (BattlefieldController.battlefield.Distance < 1)
        {
            BattlefieldController.battlefield.Distance = 1;
        }
        Debug.Log(this.name + " move.");
    }    

    public void TakeDMG(float dmg)
    {
        float r = Random.Range(0, 100f); // use to calculate if dodge success
        if ( this.DodgeRate != 0 && r <= this.DodgeRate) 
        {
            Debug.Log("Attack miss! Random is " + r);           
        }
        else
        {
            if (this.DodgeRate != 0)
            {
                Debug.Log("Dodge failed. Random is " + r);
            }
            Debug.Log(this.name + "take dmg" + dmg);
            this.CurrentHP -= dmg;
            SetHPSlider();            
        }
        this.ChargeReset();

        //Debug.Log("Chargelayer is " + this.ChargeLayer);
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
            if (buffType == BuffType.charge) //charge buff add charge layer
            {
                this.ChargeLayer += System.Convert.ToInt32(num);
                buff.BuffNum = this.ChargeLayer;
                //Debug.Log("charge layer is" + ChargeLayer);
            }
            else
            {
                buff.BuffNum += buffTurn;
            }
            buff.UpdateBuffTurnText();
        }
        else
        {            
            GameObject buffObj = Instantiate(buffPrefab, this.transform.position, Quaternion.identity);
            buff = buffObj.GetComponent<Buff>();
            // search buff type in database and generat object then get Buff instance and initialize it ************************** edit here with database info and wrap spl commends 
            
            if(buffType == BuffType.dodge)//use piece to calculate dodge rate,
            {
                buff.Initialize(buffType, buffTurn, buffZone.transform, this, Color.blue, "TriggerAt_TakeDMG", 0, num, 0, 0);                
            }
            else if(buffType == BuffType.bleeding)
            {
                buff.Initialize(buffType, buffTurn, buffZone.transform, this, Color.red, "TriggerAt_TurnBegin", 2, 0, 0, 0);                
            }
            else if (buffType == BuffType.healing)
            {
                buff.Initialize(buffType, buffTurn, buffZone.transform, this, Color.green, "TriggerAt_TurnBegin", 0, 0, num, 0);
            }
            else if(buffType == BuffType.charge)
            {
                //this.ChargeLayer = System.Convert.ToInt32(num);
                buff.Initialize(buffType, buffTurn, buffZone.transform, this, Color.yellow, "TriggerAt_Attack", 0, 0, 0, System.Convert.ToInt32(num));                
            }
            BuffList.Add(buff);
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
                if (this.BuffList[i].BuffNum == 0)
                {
                    Destroy(this.BuffList[i].gameObject);
                    this.BuffList.RemoveAt(i);
                }
            }
        }
    }

    
    public void DodgeReset()
    {
        Buff buff = BuffList.Find(x => x.type == BuffType.dodge);
        BuffList.Remove(buff);
        this.DodgeRate = 0;
        if (buff)
        {
            Destroy(buff.gameObject);
        }
    }   

    public void ChargeReset()
    {
        Buff buff = BuffList.Find(x => x.type == BuffType.charge);
        BuffList.Remove(buff);
        this.ChargeLayer = 0;
        if (buff)
        {
            Destroy(buff.gameObject);
        }
        
    }
}
