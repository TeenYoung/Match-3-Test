using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public GameObject itemPrefab;
    public RectTransform selectFrameRectTransform;

    private List<Item> items = new List<Item>();
    private Item selectedItem;

    // Start is called before the first frame update
    void Start()
    {
        int[] recoverHPs = { 30, 20, 60 };
        int[] recoverTurns = { 0, 3, 0 };
        string[] itemNames = { "Herb", "HealPotion", "QuickHealPotion" };

        for (int i = 0; i < 3; i++)
        {
            Item item = Instantiate(itemPrefab, transform).GetComponent<Item>();
            item.name = itemNames[i];
            item.SetItem(recoverHPs[i], recoverTurns[i]);
            items.Add(item);
        }

        selectedItem = items[1];//temp
    }

    public void Select(Item item)
    {
        selectedItem = item;

        // Keep y unchaned wile move select frame horizontally
        float y = selectFrameRectTransform.anchoredPosition.y;
        selectFrameRectTransform.anchoredPosition = new Vector2(selectedItem.GetComponent<RectTransform>().anchoredPosition.x, y);
    }

    public void UseSelectedItem()
    {
        if (selectedItem.RecoverTurns > 0)
        {
            BattlefieldController.battlefield.player.AddBuff(BuffType.healing, selectedItem.RecoverHP, selectedItem.RecoverTurns);
        }
        else if (selectedItem.RecoverHP > 0)
        {
            BattlefieldController.battlefield.player.Heal(selectedItem.RecoverHP);
        }
    }
}
