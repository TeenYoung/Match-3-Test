using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    int hpValue;
    public int distanceToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        hpValue = 100;
        distanceToPlayer = 10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //monster attack
    public void Attack()
    {
        Debug.Log("Monster attack! HAHAHAHAHA~~");
    }

    //monster move
    public void Move()
    {
        Debug.Log("Monster move! HAHAHAHA~~");
    }

    //monster hp change
    public void HPChange()
    {
        Debug.Log("Monster HP change! I have " + hpValue + " HP. HAHAHAHA~~");
    }
}
