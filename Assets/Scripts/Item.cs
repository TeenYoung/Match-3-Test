using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    private int recoverHP, recoverTurns;

    private ItemController items;

    private void Start()
    {
        items = transform.GetComponentInParent<ItemController>();
    }

    public void SetItem(int recoverHP, int recoverTurns)
    {
        this.recoverHP = recoverHP;
        this.recoverTurns = recoverTurns;
    }

    public void OnClick()
    {
        items.Select(this);
    }
}
