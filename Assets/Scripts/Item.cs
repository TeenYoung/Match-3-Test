using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public int RecoverHP { get; set; }
    public int RecoverTurns { get; set; }


    private ItemController items;

    private void Start()
    {
        items = transform.GetComponentInParent<ItemController>();
    }

    public void SetItem(int recoverHP, int recoverTurns)
    {
        this.RecoverHP = recoverHP;
        this.RecoverTurns = recoverTurns;
    }

    public void OnClick()
    {
        items.Select(this);
    }
}
