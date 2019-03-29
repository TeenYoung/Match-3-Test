using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public GameObject player;
    //public GameObject hpSlider;
    int hpValue;

    // Start is called before the first frame update
    void Start()
    {
        hpValue = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    //call this function when player attack
    /// <summary>
    /// Attack when green or/and red pieces matched and cleared
    /// </summary>
    /// <param name="green"></param>
    /// <param name="red"></param>
    public void Attack(int greenSum, int redSum)
    {
        Debug.Log(string.Format("Player attack. Green is {0}, red is {1}.", greenSum, redSum));
    }

    //call this function when player move
    /// <summary>
    /// Move when blue or/and black pieces matched and cleared
    /// </summary>
    /// <param name="blueSum"></param>
    /// <param name="blackSum"></param>
    public void Move(int blueSum, int blackSum)
    {
        Debug.Log(string.Format("Player move. Blue is {0}, black is {1}.", blueSum, blackSum));
    }

    //call this function when player use special
    /// <summary>
    /// Use special when purple pieces matched and cleared
    /// </summary>
    /// <param name="purpleSum"></param>
    public void Special(int purpleSum)
    {
        Debug.Log(string.Format("Player special. Purple is {0}.", purpleSum));
    }

    //cal this when player HP change ( HP increase / decrease)
    public void TakeDMG(int dmg)
    {
        Debug.Log("Player take " + dmg + " damage.");
        UpdateHPBar();
    }

    //update player's HP bar 
    public void UpdateHPBar()
    {
        Debug.Log("Player's HPbar changed. Now is " + hpValue);
    }
}
