using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject 用于定义商店的库存（出售的物品列表）。
/// </summary>
[CreateAssetMenu(fileName = "NewShopInventory", menuName = "Inventory/Shop Inventory", order = 1)]
public class ShopInventorySO : ScriptableObject
{
    [Header("商店出售的物品列表")]
    [Tooltip("将商店要出售的 Item ScriptableObject 拖拽到这里")]
    public List<Item> availableItems = new List<Item>();

    // 未来可以扩展，例如：
    // public string shopName = "默认商店";
    // public float globalPriceMultiplier = 1.0f; // 全局价格乘数
    // public List<ShopItemEntry> detailedItems; // 如果需要更复杂的设置（如独立库存、价格调整）
}

/*
// 如果需要更复杂的设置，可以取消注释并使用这个结构代替 List<Item>
[System.Serializable]
public class ShopItemEntry
{
    public Item item;
    public int quantity = -1; // -1 代表无限库存
    public float priceMultiplier = 1.0f; // 针对此物品的价格乘数
}
*/