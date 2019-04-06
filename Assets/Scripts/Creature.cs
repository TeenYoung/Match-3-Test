using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creature : MonoBehaviour {

    public Slider hpSlider;
    public Image fillImage;
    public Color fullHpColor, zeroHpColor;

    public GameObject buffZone;

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

    public void AddBuff(GameObject buffObj)
    {
        buffObj.gameObject.transform.SetParent(buffZone.transform);
    }

}
