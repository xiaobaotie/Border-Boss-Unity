using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理单个商店的核心逻辑，处理商品获取和购买尝试。
/// </summary>
public class ShopManager : MonoBehaviour
{
    [Header("商店配置")]
    [SerializeField] private ShopInventorySO shopInventory; // 引用商店库存配置

    // --- 属性 ---
    public string ShopName => shopInventory != null ? shopInventory.name : "未命名商店"; // (可选) 获取商店名称

    // --- 核心方法 ---

    /// <summary>
    /// 获取此商店当前可供购买的物品列表。
    /// </summary>
    /// <returns>商店出售的物品列表。</returns>
    public List<Item> GetAvailableItems()
    {
        if (shopInventory == null)
        {
            Debug.LogError($"商店 {gameObject.name} 没有配置 ShopInventorySO!");
            return new List<Item>(); // 返回空列表避免错误
        }
        // 直接返回配置中定义的物品列表
        return shopInventory.availableItems;
    }

    /// <summary>
    /// 尝试购买指定物品。
    /// </summary>
    /// <param name="itemToBuy">要购买的物品。</param>
    /// <param name="quantity">要购买的数量 (默认为1)。</param>
    /// <returns>如果购买成功返回 true，否则返回 false。</returns>
    public bool AttemptPurchase(Item itemToBuy, int quantity = 1)
    {
        if (itemToBuy == null || quantity <= 0)
        {
            Debug.LogWarning("尝试购买无效物品或数量。");
            return false;
        }

        if (shopInventory == null || !shopInventory.availableItems.Contains(itemToBuy))
        {
            Debug.LogWarning($"商店 {ShopName} 不出售物品 {itemToBuy.itemName}。");
            return false;
        }

        // --- 检查玩家金钱 ---
        // TODO: 接入 GameManager 获取玩家金钱并检查
        // 假设 GameManager 有一个单例 Instance 和一个 CurrentBalance 属性
        // 以及一个 UpdateBalance(int amount) 方法
        int totalCost = itemToBuy.basePrice * quantity; // 假设 Item 有 basePrice 字段
        // if (GameManager.Instance == null || GameManager.Instance.CurrentBalance < totalCost)
        // {
        //     Debug.Log($"玩家金钱不足，需要 {totalCost}，拥有 {GameManager.Instance?.CurrentBalance ?? 0}。");
        //     // (可选) 可以在这里触发一个事件或UI提示
        //     return false;
        // }

        // --- 检查玩家背包空间 ---
        // 注意：InventoryManager.AddItem 内部会处理堆叠和空间检查，
        // 我们只需要调用它并检查返回值即可。
        bool addedSuccessfully = InventoryManager.Instance.AddItem(itemToBuy, quantity);

        if (addedSuccessfully)
        {
            // --- 扣除金钱 ---
            // TODO: 调用 GameManager 扣除金钱
            // GameManager.Instance.UpdateBalance(-totalCost);
            Debug.Log($"成功购买 {quantity} 个 {itemToBuy.itemName}，花费 {totalCost}。");

            // (可选) 如果商店物品有数量限制，可以在这里更新 shopInventory 的数据
            // (需要将 ShopInventorySO 改为使用 ShopItemEntry 结构)

            return true;
        }
        else
        {
            // AddItem 返回 false 意味着背包空间不足
            Debug.LogWarning($"购买失败：玩家背包空间不足，无法添加 {quantity} 个 {itemToBuy.itemName}。");
            // 不需要回滚金钱，因为是在添加物品成功后才扣钱
            return false;
        }
    }

    // --- (未来可能添加的功能) ---
    // public bool AttemptSell(InventorySlot playerSlot, int quantity = 1)
    // {
    //     // 实现出售逻辑...
    //     // 1. 获取物品信息 playerSlot.ItemData
    //     // 2. 计算出售价格 (e.g., item.basePrice * sellMultiplier)
    //     // 3. 调用 InventoryManager.Instance.RemoveItem(slotIndex, quantity)
    //     // 4. 调用 GameManager.Instance.UpdateBalance(sellPrice)
    //     // 5. 返回 true/false
    // }
}