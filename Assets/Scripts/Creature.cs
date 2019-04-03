using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creature : MonoBehaviour {

    public Slider hpSlider;
    public Image fillImage;
    public Color fullHpColor, zeroHpColor;

    public string CreatureName { get; set; }

    public float CurrentHP { get; set; }

    public float MaxHP { get; set; }

    public void Move(int feet)
    {
        BoardController.distance += feet;
        Debug.Log("Distance is " + BoardController.distance);
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
}


//public class Player : Creature {

//    public Player(string playerName, float initialHP) : base (playerName, initialHP)
//    {
//        this.creatureName = playerName;
//        this.currentHP = initialHP;
//    }

//    public void Attack(int greenSum, int redSum)
//    {
//        Debug.Log(string.Format("Player attack. Green is {0}, red is {1}.", greenSum, redSum));
//    }

//    public void UseItem(int purpleSum)
//    {
//        Debug.Log(string.Format("Player special. Purple is {0}.", purpleSum));
//    }
    
//}

//public class Monster : Creature {

//    public Monster(string monsterName, float initialHP) : base (monsterName, initialHP)
//    {
//        this.creatureName = monsterName;
//        this.currentHP = initialHP;
//    }

//    public void Attack()
//    {
//        Debug.Log("Monster attack! HAHAHAHAHA~~");
//    }

//}
