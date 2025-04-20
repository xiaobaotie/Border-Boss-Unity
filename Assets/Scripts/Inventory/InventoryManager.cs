using System.Collections.Generic;
using UnityEngine;
using System; // 需要引入 System 命名空间才能使用 Action

public class InventoryManager : MonoBehaviour
{
    // --- 单例模式 (可选, 方便全局访问) ---
    public static InventoryManager Instance { get; private set; }

    // --- 事件 ---
    // 当背包内容发生变化时触发 (UI 可以监听这个事件来更新显示)
    public event Action OnInventoryChanged;

    // --- 配置 ---
    [Header("Inventory Settings")]
    [SerializeField] private int capacity = 20; // 背包容量 (格子数量)

    // --- 数据 ---
    [Header("Inventory Data")]
    // 使用 [SerializeField] 让私有列表在 Inspector 中可见，但不允许外部直接修改列表本身
    [SerializeField] private List<InventorySlot> slots;

    // --- 属性 ---
    public int Capacity => capacity;
    // 提供一个只读的背包格子列表访问器
    public IReadOnlyList<InventorySlot> Slots => slots.AsReadOnly();


    // --- Unity 事件函数 ---

    private void Awake()
    {
        // --- 单例实现 ---
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Found more than one InventoryManager instance. Destroying the new one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // (可选) 如果希望背包管理器在切换场景时不被销毁
        DontDestroyOnLoad(gameObject); // <--- 取消这一行的注释

        // --- 初始化背包格子 ---
        InitializeInventory();
    }

    // --- 初始化 ---

    private void InitializeInventory()
    {
        slots = new List<InventorySlot>(capacity);
        for (int i = 0; i < capacity; i++)
        {
            slots.Add(new InventorySlot()); // 添加指定数量的空格子
        }
        Debug.Log($"Inventory initialized with {capacity} slots.");
    }

    // --- 核心功能 ---

    /// <summary>
    /// 尝试向背包添加物品
    /// </summary>
    /// <param name="itemToAdd">要添加的物品数据</param>
    /// <param name="amountToAdd">要添加的数量</param>
    /// <returns>返回 true 如果所有物品都成功添加, false 如果部分或全部未能添加 (背包满)</returns>
    public bool AddItem(Item itemToAdd, int amountToAdd)
    {
        if (itemToAdd == null || amountToAdd <= 0)
        {
            Debug.LogWarning("Attempted to add invalid item or zero/negative quantity.");
            return false;
        }

        Debug.Log($"Attempting to add {amountToAdd} of {itemToAdd.itemName}...");

        int amountRemaining = amountToAdd;

        // 1. 优先堆叠到现有格子
        if (itemToAdd.isStackable)
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.ItemData == itemToAdd && slot.Quantity < itemToAdd.maxStackSize)
                {
                    int amountAdded = slot.AddQuantity(amountRemaining);
                    amountRemaining -= amountAdded;
                    Debug.Log($"Added {amountAdded} to existing stack. Remaining: {amountRemaining}");
                    if (amountRemaining <= 0)
                    {
                        OnInventoryChanged?.Invoke(); // 触发事件
                        Debug.Log($"Successfully added all {amountToAdd} of {itemToAdd.itemName}.");
                        return true; // 全部添加完毕
                    }
                }
            }
        }

        // 2. 如果还有剩余，寻找空格子添加
        if (amountRemaining > 0)
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.ItemData == null) // 找到空格子
                {
                    int amountCanFit = Mathf.Min(amountRemaining, itemToAdd.maxStackSize);
                    slot.SetSlot(itemToAdd, amountCanFit); // 设置新物品和数量
                    amountRemaining -= amountCanFit;
                    Debug.Log($"Placed {amountCanFit} in a new slot. Remaining: {amountRemaining}");
                    if (amountRemaining <= 0)
                    {
                        OnInventoryChanged?.Invoke(); // 触发事件
                        Debug.Log($"Successfully added all {amountToAdd} of {itemToAdd.itemName}.");
                        return true; // 全部添加完毕
                    }
                    // 如果物品不可堆叠，添加一个就够了
                    if (!itemToAdd.isStackable)
                    {
                         if (amountRemaining > 0) {
                             // 对于不可堆叠物品，如果还有剩余，继续找下一个空格子
                             continue;
                         } else {
                             OnInventoryChanged?.Invoke();
                             Debug.Log($"Successfully added all {amountToAdd} of {itemToAdd.itemName} (non-stackable).");
                             return true;
                         }
                    }
                }
            }
        }

        // 3. 如果遍历完所有格子仍有剩余，说明背包满了
        if (amountRemaining > 0)
        {
            Debug.LogWarning($"Inventory is full. Could not add {amountRemaining} of {itemToAdd.itemName}.");
            OnInventoryChanged?.Invoke(); // 即使失败了，之前的添加操作也可能改变了背包，触发事件
            return false; // 未能全部添加
        }

        // 理论上不应该执行到这里，但为了编译器满意
        return true;
    }


    /// <summary>
    /// 从指定索引的格子移除指定数量的物品
    /// </summary>
    /// <param name="slotIndex">格子的索引</param>
    /// <param name="amountToRemove">要移除的数量</param>
    public void RemoveItem(int slotIndex, int amountToRemove)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return;
        }

        InventorySlot slot = slots[slotIndex];
        if (slot.ItemData == null || slot.Quantity <= 0)
        {
            Debug.LogWarning($"Slot {slotIndex} is already empty.");
            return;
        }

        if (amountToRemove <= 0)
        {
             Debug.LogWarning("Amount to remove must be positive.");
             return;
        }

        Debug.Log($"Removing {amountToRemove} of {slot.ItemData.itemName} from slot {slotIndex}.");
        slot.RemoveQuantity(amountToRemove);
        OnInventoryChanged?.Invoke(); // 触发事件
    }

    /// <summary>
    /// 完全清空指定索引的格子
    /// </summary>
    /// <param name="slotIndex">格子的索引</param>
    public void ClearSlot(int slotIndex)
    {
         if (slotIndex < 0 || slotIndex >= slots.Count)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return;
        }
        if (slots[slotIndex].ItemData != null)
        {
            Debug.Log($"Clearing slot {slotIndex} which contained {slots[slotIndex].ItemData.itemName}.");
            slots[slotIndex].ClearSlot();
            OnInventoryChanged?.Invoke(); // 触发事件
        }
    }

    // --- (未来可能添加的功能) ---
    // SwapItems(int indexA, int indexB)
    // FindItem(Item itemToFind) -> returns index or -1
    // GetTotalQuantity(Item itemToCount)
    // SortInventory()
    // ... 等等
}