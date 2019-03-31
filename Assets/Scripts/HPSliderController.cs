using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPSliderController : MonoBehaviour
{
    public Slider hpSlider;
    public Image fillImage;
    public Color fullHpColor, zeroHpColor;

    public float MaxHp { get; set; }
    public float CurrentHp { get; set; }

    //// Tester
    //private void Start()
    //{
    //    MaxHp = 100f;
    //    CurrentHp = 30f;

    //    hpSlider.maxValue = MaxHp;
    //    SetHpSlider();
    //}

    // Update the value and color of the Hp Slider
    private void SetHpSlider()
    {
        hpSlider.value = CurrentHp;

        fillImage.color = Color.Lerp(zeroHpColor, fullHpColor, CurrentHp / MaxHp);
    }
}
