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

    public void AddBuff(string buffTpye, int buffTurn)
    {
        GameObject buffObj = Instantiate(buffPrefab, this.transform.position, Quaternion.identity);
        Buff buff = buffObj.GetComponent<Buff>();
        if (buffTpye == "green") buffObj.GetComponent<Image>().color = new Color(0.458f,0.737f,0.439f);
        if (buffTpye == "red") buffObj.GetComponent<Image>().color = new Color(0.925f,0.408f,0.471f);
        buffObj.transform.SetParent(buffZone.transform);
        buffList.Add(buffObj);
        buff.InitializeBuff(buffTpye, buffTurn);
    }


    public void BuffDecreaseOne()
    {
        if (buffList != null)
        {
            for(int i = buffList.Count-1; i >=0; i--)
            {
                Buff buff = buffList[i].GetComponent<Buff>();
                buff.remainTurn--;
                if (buff.remainTurn < 0)
                {
                    Destroy(buffList[i]);
                    buffList.Remove(buffList[i]);
                }
                else if(buff.remainTurn == 0)
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
