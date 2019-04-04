using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Monster : MonoBehaviour
//{
//    int hpValue;

//    // Start is called before the first frame update
//    void Start()
//    {
//        hpValue = 100;
//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }

//    //monster attack
//    public void Attack()
//    {
//        Debug.Log("Monster attack! HAHAHAHAHA~~");
//    }

//    //monster move
//    public void Move()
//    {
//        Debug.Log("Monster move! HAHAHAHA~~");
//    }

//    //call this when monster's HP change ( HP increase / decrease)
//    public void TakeDMG(int dmg)
//    {
//        Debug.Log("Monster take " + dmg + " damage.");
//        hpValue -= dmg;
//        UpdateHPBar();
//    }

//    //update monster's HP bar
//    public void UpdateHPBar()
//    {
//        Debug.Log("Monster HP bar change! I have " + hpValue + " HP. HAHAHAHA~~");
//    }
//}



public class Monster : Creature
{
    public float NormalAttackDMG { get; set; }
    public float PowerAttackDMG { get; set; }
    public float NormalAttackRange { get; set; }
    public float PowerAttackRange { get; set; }

   
    public void NormalAttack()
    {
        Debug.Log("Monster normal attack, making damage: " + NormalAttackDMG);
        BoardController.board.player.TakeDMG(NormalAttackDMG);
    }

    public void PowerAttack()
    {
        Debug.Log("Monster power attack, making damage: " + PowerAttackDMG);
        BoardController.board.player.TakeDMG(PowerAttackDMG);
    }

    

    /// <summary>
    /// Set monster attack dmg and range
    /// </summary>
    /// <param name="NormalAttackRange"></param>
    /// <param name="NormalAttackDMG"></param>
    /// <param name="PowerAttackRange"></param>
    /// <param name="PowerAttackDMG"></param>
    public void InitializeAttackInfo(float NormalAttackRange, float NormalAttackDMG, float PowerAttackRange, float PowerAttackDMG)
    {
        this.NormalAttackRange = NormalAttackRange;
        this.NormalAttackDMG = NormalAttackDMG;
        this.PowerAttackRange = PowerAttackRange;
        this.PowerAttackDMG = PowerAttackDMG;
    }
}
