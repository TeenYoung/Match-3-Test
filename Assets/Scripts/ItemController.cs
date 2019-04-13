using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public GameObject itemPrefab;
    public RectTransform selectFrameRectTransform;

    private Item selectItem;

    // Start is called before the first frame update
    void Start()
    {
        Item item1 = Instantiate(itemPrefab, transform).GetComponent<Item>();
        item1.name = "Herb";
        item1.SetItem(30,0);

        Item item2 = Instantiate(itemPrefab, transform).GetComponent<Item>();
        item2.name = "HealPotion";
        item2.SetItem(20, 3);

        Item item3 = Instantiate(itemPrefab, transform).GetComponent<Item>();
        item3.name = "QuickHealPotion";
        item3.SetItem(60, 0);
    }

    public void Select(Item item)
    {
        selectItem = item;

        // Keep y unchaned wile move select frame horizontally
        float y = selectFrameRectTransform.anchoredPosition.y;
        selectFrameRectTransform.anchoredPosition = new Vector2(selectItem.GetComponent<RectTransform>().anchoredPosition.x, y);
    }
}
