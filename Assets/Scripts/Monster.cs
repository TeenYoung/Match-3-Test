using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
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

    //call this when monster's HP change ( HP increase / decrease)
    public void TakeDMG(int dmg)
    {
        Debug.Log("Monster take " + dmg + " damage.");
        UpdateHPBar();
    }

    //update monster's HP bar
    public void UpdateHPBar()
    {
        Debug.Log("Monster HP bar change! I have " + hpValue + " HP. HAHAHAHA~~");
    }
}
