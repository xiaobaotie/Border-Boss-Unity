using System.Collections.Generic;
using UnityEngine;
using System; // 如果使用了 Action

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory Settings")]
    [SerializeField] private int capacity = 20; // 背包容量
    public int Capacity => capacity; // <-- 新增: 公共属性以访问 capacity

    public List<InventorySlot> slots = new List<InventorySlot>(); // 背包槽位列表 (这个应该已经是 public 的)

    public event Action OnInventoryChanged;

    private void Awake()
    {
        // --- 单例实现 ---
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("InventoryManager: 发现多于一个实例，销毁新的实例。");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // --- 跨场景保留 ---
        DontDestroyOnLoad(gameObject);
        Debug.Log("InventoryManager: 实例已设置并标记为 DontDestroyOnLoad。");

        // --- 核心初始化逻辑：优先保留 Inspector 数据 ---
        // 检查 Inspector 中是否配置了 slots，并且数量是否与 Capacity 匹配
        if (slots != null && slots.Count > 0 && slots.Count == Capacity) // 使用公共的 Capacity 属性
        {
            Debug.Log($"InventoryManager: 使用 Inspector 中配置的 {slots.Count} 个槽位。容量: {Capacity}。");
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] == null) // 如果列表中的某个元素是 null (例如，Inspector 中只是一个空条目)
                {
                    slots[i] = new InventorySlot(); // 将其初始化为空槽位
                }
                // 如果槽位有数量但没有物品数据，这是来自 Inspector 的无效状态，将其清除。
                else if (slots[i].ItemData == null && slots[i].Quantity > 0)
                {
                    Debug.LogWarning($"InventoryManager: 槽位 {i} 有数量 {slots[i].Quantity} 但没有 ItemData。正在清空槽位。");
                    slots[i].ClearSlot();
                }
            }
        }
        // 如果配置了 slots 但数量与 Capacity 不符，则进行调整
        else if (slots != null && slots.Count > 0 && slots.Count != Capacity) // 使用公共的 Capacity 属性
        {
            Debug.LogWarning($"InventoryManager: Inspector 中配置了 {slots.Count} 个槽位，但容量是 {Capacity}。正在调整列表大小。");
            while (slots.Count < Capacity) // 使用公共的 Capacity 属性
            {
                slots.Add(new InventorySlot());
            }
            if (slots.Count > Capacity) // 使用公共的 Capacity 属性
            {
                slots.RemoveRange(Capacity, slots.Count - Capacity); // 使用公共的 Capacity 属性
            }
            Debug.Log($"InventoryManager: 槽位已调整。最终数量: {slots.Count}");
        }
        else
        {
            // Inspector 中没有配置 slots (slots 为 null 或 Count 为 0)，进行程序化初始化
            Debug.Log("InventoryManager: Inspector 中没有配置槽位或列表为空。将根据容量进行程序化初始化。");
            InitializeInventory(); // 这会根据 capacity 创建全新的空列表
        }

        // 确保最终 slots 列表不为 null
        if (slots == null) {
            slots = new List<InventorySlot>(); // 预防性措施
            Debug.LogError("InventoryManager: 在所有初始化检查后，Slots 列表意外为 null！已创建一个新的空列表。");
        }

        Debug.Log("InventoryManager: Awake 完成。");
        // OnInventoryChanged?.Invoke(); // 可以在 Start 或完全初始化后调用
    }

    public void InitializeInventory()
    {
        slots = new List<InventorySlot>(Capacity); // 使用公共的 Capacity 属性
        for (int i = 0; i < Capacity; i++) // 使用公共的 Capacity 属性
        {
            slots.Add(new InventorySlot());
        }
        Debug.Log($"InventoryManager: 背包已通过程序以 {Capacity} 个空槽位初始化。"); // 使用公共的 Capacity 属性
        OnInventoryChanged?.Invoke();
    }

    public bool AddItem(Item itemToAdd, int quantity = 1)
    {
        if (itemToAdd == null || quantity <= 0)
        {
            Debug.LogWarning("尝试添加 null 物品或零/负数量。");
            return false;
        }

        bool itemAdded = false;

        // 1. 尝试堆叠到现有槽位 (仅限可堆叠物品)
        if (itemToAdd.isStackable)
        {
            for (int i = 0; i < slots.Count; i++) // 遍历现有槽位
            {
                if (slots[i].ItemData == itemToAdd) // 检查槽位是否包含相同物品
                {
                    int addedAmount = slots[i].AddQuantity(quantity); // 尝试添加到堆叠
                    if (addedAmount > 0)
                    {
                        quantity -= addedAmount; // 减少剩余数量
                        itemAdded = true;
                        Debug.Log($"已将 {addedAmount} 个 {itemToAdd.itemName} 添加到槽位 {i} 的现有堆叠中。剩余待添加: {quantity}");
                        if (quantity <= 0) break; // 所有物品已添加
                    }
                }
            }
        }

        // 2. 如果还有剩余数量 (或者物品不可堆叠且第一次添加)，尝试放入新的空槽位
        if (quantity > 0)
        {
            for (int i = 0; i < slots.Count; i++) // 遍历现有槽位
            {
                if (slots[i].ItemData == null) // 找到一个空格子
                {
                    int amountForThisSlot = Mathf.Min(quantity, itemToAdd.maxStackSize);
                    slots[i].SetSlot(itemToAdd, amountForThisSlot);
                    quantity -= amountForThisSlot; // 减少剩余数量
                    itemAdded = true;
                    Debug.Log($"已将 {amountForThisSlot} 个 {itemToAdd.itemName} 放入新槽位 {i}。剩余待添加: {quantity}");
                    if (quantity <= 0) break; // 所有物品已添加
                }
            }
        }

        if (itemAdded)
        {
            OnInventoryChanged?.Invoke();
            if (quantity > 0)
            {
                Debug.LogWarning($"背包已满或无法添加所有物品。{quantity} 个 {itemToAdd.itemName} 未能添加。");
                return false; // 部分添加成功
            }
            return true; // 完全添加成功
        }
        else
        {
            Debug.LogWarning($"背包已满或物品无法添加: {itemToAdd.itemName}");
            return false; // 未能添加任何物品
        }
    }

    public void RemoveItem(int slotIndex, int amountToRemove)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
        {
            Debug.LogError($"无效的槽位索引: {slotIndex}");
            return;
        }

        InventorySlot slot = slots[slotIndex];
        if (slot.ItemData == null || slot.Quantity <= 0)
        {
            Debug.LogWarning($"槽位 {slotIndex} 已经是空的。");
            return;
        }

        if (amountToRemove <= 0)
        {
             Debug.LogWarning("要移除的数量必须为正数。");
             return;
        }

        Debug.Log($"正在从槽位 {slotIndex} 移除 {amountToRemove} 个 {slot.ItemData.itemName}。");
        slot.RemoveQuantity(amountToRemove);
        OnInventoryChanged?.Invoke();
    }

    public void ClearSlot(int slotIndex)
    {
         if (slotIndex < 0 || slotIndex >= slots.Count)
        {
            Debug.LogError($"无效的槽位索引: {slotIndex}");
            return;
        }
        if (slots[slotIndex].ItemData != null)
        {
            Debug.Log($"正在清空槽位 {slotIndex}，其中包含 {slots[slotIndex].ItemData.itemName}。");
            slots[slotIndex].ClearSlot();
            OnInventoryChanged?.Invoke();
        }
    }
    // ... 其他 InventoryManager 的方法 ...
}