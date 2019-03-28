using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public GameObject player;

    //call this function when player attack
    /// <summary>
    /// Attack when green or/and red pieces matched and cleared
    /// </summary>
    /// <param name="green"></param>
    /// <param name="red"></param>
    public void Attack(int greenSum, int redSum)
    {
        Debug.Log(string.Format("Player attack. Green is {0}, red is {1}", greenSum, redSum));
    }

    //call this function when player move
    /// <summary>
    /// Move when blue or/and black pieces matched and cleared
    /// </summary>
    /// <param name="blueSum"></param>
    /// <param name="blackSum"></param>
    public void Move(int blueSum, int blackSum)
    {
        Debug.Log(string.Format("Player attack. Green is {0}, red is {1}", blueSum, blackSum));
    }
}
