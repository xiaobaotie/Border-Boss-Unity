using UnityEngine;

// [System.Serializable] 属性让这个类的实例可以在 Inspector 中显示（例如在一个 List<InventorySlot> 中）
[System.Serializable]
public class InventorySlot
{
    // --- 变量 ---
    [SerializeField] // 即使是 private，也强制 Unity 序列化并在 Inspector 中显示（如果可能）
    private Item itemData; // 格子中存储的物品数据 (ScriptableObject 引用)
    [SerializeField]
    private int quantity; // 格子中物品的数量

    // --- 属性 (Properties) ---

    /// <summary>
    /// 获取格子中的物品数据 (只读)
    /// </summary>
    public Item ItemData => itemData;

    /// <summary>
    /// 获取格子中的物品数量 (只读)
    /// </summary>
    public int Quantity => quantity;

    // --- 构造函数 ---

    /// <summary>
    /// 创建一个空的格子
    /// </summary>
    public InventorySlot()
    {
        itemData = null;
        quantity = 0;
    }

    /// <summary>
    /// 创建一个包含指定物品和数量的格子
    /// </summary>
    /// <param name="item">物品数据</param>
    /// <param name="amount">物品数量</param>
    public InventorySlot(Item item, int amount)
    {
        itemData = item;
        quantity = amount;
        // 可以在这里加一些验证，例如 amount 不能超过 item.maxStackSize
        if (itemData != null && quantity > itemData.maxStackSize)
        {
            Debug.LogWarning($"Attempted to create slot with quantity ({quantity}) exceeding max stack size ({itemData.maxStackSize}) for item {itemData.itemName}. Clamping quantity.");
            quantity = itemData.maxStackSize;
        }
        if (quantity <= 0)
        {
            ClearSlot(); // 如果数量无效，则清空格子
        }
    }

    // --- 公共方法 ---

    /// <summary>
    /// 清空格子 (移除物品和数量)
    /// </summary>
    public void ClearSlot()
    {
        itemData = null;
        quantity = 0;
    }

    /// <summary>
    /// 向格子中添加指定数量的物品
    /// </summary>
    /// <param name="amount">要添加的数量</param>
    /// <returns>实际添加的数量 (可能小于 amount，如果格子满了)</returns>
    public int AddQuantity(int amount)
    {
        if (itemData == null)
        {
            Debug.LogWarning("Cannot add quantity to an empty slot.");
            return 0; // 不能向空格子添加数量
        }
        if (!itemData.isStackable && quantity >= 1)
        {
             Debug.LogWarning($"Item {itemData.itemName} is not stackable and the slot is already full.");
             return 0; // 不可堆叠且已有物品
        }

        int maxAdd = itemData.maxStackSize - quantity; // 最多还能添加多少
        int amountToAdd = Mathf.Min(amount, maxAdd); // 实际能添加的数量

        if (amountToAdd > 0)
        {
            quantity += amountToAdd;
        }
        else
        {
             Debug.LogWarning($"Slot for {itemData.itemName} is full. Cannot add more.");
        }

        return amountToAdd; // 返回实际添加了多少
    }

    /// <summary>
    /// 从格子中移除指定数量的物品
    /// </summary>
    /// <param name="amount">要移除的数量</param>
    public void RemoveQuantity(int amount)
    {
        if (itemData == null || quantity <= 0)
        {
            Debug.LogWarning("Cannot remove quantity from an empty or already zero quantity slot.");
            return;
        }

        quantity -= amount;

        if (quantity <= 0)
        {
            ClearSlot(); // 如果移除后数量小于等于0，则清空格子
        }
    }

    /// <summary>
    /// 直接设置格子的物品和数量 (用于加载数据或强制设置)
    /// </summary>
    /// <param name="item">新的物品数据</param>
    /// <param name="amount">新的数量</param>
    public void SetSlot(Item item, int amount)
    {
        if (item == null || amount <= 0)
        {
            ClearSlot();
            return;
        }
        itemData = item;
        quantity = Mathf.Clamp(amount, 1, item.maxStackSize); // 确保数量在有效范围内
    }
}