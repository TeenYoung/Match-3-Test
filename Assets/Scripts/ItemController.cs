using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public GameObject itemPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GameObject item1 = Instantiate(itemPrefab,transform);
        GameObject item2 = Instantiate(itemPrefab, transform);
        GameObject item3 = Instantiate(itemPrefab, transform);
    }
}
