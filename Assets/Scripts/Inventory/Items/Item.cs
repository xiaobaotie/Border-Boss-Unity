using UnityEngine;

// 定义物品类型的枚举 (简化版)
public enum ItemType
{
    Commodity,  // 商品 (用于交易等)
    Consumable, // 消耗品
    // Equipment,  // 装备 (暂时移除)
    // Material,   // 材料 (暂时移除)
    // QuestItem,  // 任务物品 (暂时移除)
    // Miscellaneous // 杂项 (暂时移除)
}

// [CreateAssetMenu] 属性允许我们在 Assets/Create 菜单中创建此类型的资源
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class Item : ScriptableObject
{
    [Header("基本信息")]
    public string itemName = "New Item"; // 物品名称
    [TextArea(3, 5)] // 让描述字段在 Inspector 中可以多行输入
    public string description = "Item Description"; // 物品描述
    public Sprite icon = null; // 物品图标

    [Header("物品属性")]
    public ItemType itemType = ItemType.Commodity; // 物品类型 (将默认值改为 Commodity)
    public int maxStackSize = 1; // 最大堆叠数量 (默认为1, 不可堆叠)
    public bool isStackable { get { return maxStackSize > 1; } } // 辅助属性，判断是否可堆叠
    public bool isUsable = false; // 新增：标记物品是否可被“使用” (默认为 false)

    // --- 在这里添加基础价格字段 ---
    [Tooltip("物品的基础价值")] // 可选的提示信息
    public int basePrice;
    // -----------------------------

    // --- (可选) 特定类型属性 ---
    // 例如，如果是装备，可以添加:
    // public int attackBonus;
    // public int defenseBonus;

    // 例如，如果是消耗品，可以添加一个效果方法或引用效果脚本
    // public virtual void Use()
    // {
    //     Debug.Log("Using " + itemName);
    // }

    // --- (未来可能需要) 唯一ID ---
    // 可以考虑添加一个只读的唯一ID，例如使用 GUID
    // public string itemID;
    // private void OnValidate() {
    //     if (string.IsNullOrEmpty(itemID)) {
    //         itemID = System.Guid.NewGuid().ToString();
    //     }
    // }
}