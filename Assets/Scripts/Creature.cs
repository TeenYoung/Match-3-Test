using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creature : MonoBehaviour {

    public Slider hpSlider;
    public Image fillImage;
    public Color fullHpColor, zeroHpColor;
    public GameObject buffPrefab, buffZone;
    

    public List<GameObject> buffList = new List<GameObject>();

    public bool IsDodge { get; set; }

    public string CreatureName { get; set; }

    public float CurrentHP { get; set; }

    public float MaxHP { get; set; }

    public void Move(float feet)
    {
        BoardController.distance += feet;
        //Debug.Log("Distance is " + BoardController.distance);
    }

    public void TakeDMG(float dmg)
    {
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
    public void AddBuff(string buffType, int buffTurn = 1)
    {
        GameObject buffObj = buffList.Find(x=>x.GetComponent<Buff>().type.ToString()==buffType);        
        if (buffObj)
        {
            Buff buff = buffObj.GetComponent<Buff>();
            buff.RemainTurn += buffTurn;
            buff.UpdateBuff();
        }
        else
        {
            buffObj = Instantiate(buffPrefab, this.transform.position, Quaternion.identity);
            Buff buff = buffObj.GetComponent<Buff>();
            buff.Initialize(buffType, buffTurn, buffZone.transform);
            buffList.Add(buffObj);
        }
    }

    public void BuffDecreaseOne()
    {
        if (buffList != null)
        {
            for (int i = buffList.Count - 1; i >= 0; i--)
            {
                Buff buff = buffList[i].GetComponent<Buff>();
                buff.RemainTurn--;
                if (buff.RemainTurn < 0)
                {
                    Destroy(buffList[i]);
                    buffList.Remove(buffList[i]);
                }
                else if (buff.RemainTurn == 0) //keep the space of buff turn=0 , make it invisible
                {
                    buffList[i].GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    buff.remainTurnsText.text = "";
                }
                else
                {
                    buff.UpdateBuff();
                }
            }
        }
    }

}
