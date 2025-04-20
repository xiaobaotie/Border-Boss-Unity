using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform inventoryPanel; // 拖拽 InventoryPanel 对象到这里
    [SerializeField] private GameObject slotPrefab;   // 拖拽 InventorySlot_Prefab 预制件到这里

    [Header("Dependencies")]
    [SerializeField] private InventoryManager inventoryManager; // 可以拖拽场景中的 InventoryManager 对象，或者在 Start 中查找

    // 存储对实例化的 UI 格子及其组件的引用
    private List<SlotUIElements> uiSlots = new List<SlotUIElements>();

    // 辅助类，用于存储每个 UI 格子的组件引用
    private class SlotUIElements
    {
        public Image slotBackground; // 格子背景 (可选)
        public Image itemIcon;       // 物品图标 Image 组件
        public TextMeshProUGUI quantityText; // 数量 TextMeshProUGUI 组件
    }

    // [Header("UI Control")] // 不再需要键盘控制相关的 Header
    // [SerializeField] private KeyCode toggleKey = KeyCode.I; // 移除键盘按键变量
    private CanvasGroup inventoryCanvasGroup; // 引用 InventoryPanel 上的 CanvasGroup

    void Start()
    {
        // 如果没有在 Inspector 中指定，尝试查找 InventoryManager
        if (inventoryManager == null)
        {
            inventoryManager = InventoryManager.Instance; // 使用单例获取
            if (inventoryManager == null)
            {
                Debug.LogError("InventoryManager instance not found!");
                this.enabled = false; // 禁用此脚本以防出错
                return;
            }
        }

        // 获取 InventoryPanel 上的 CanvasGroup 组件
        inventoryCanvasGroup = inventoryPanel.GetComponent<CanvasGroup>();
        if (inventoryCanvasGroup == null)
        {
            Debug.LogError("InventoryPanel is missing a CanvasGroup component! Please add one.", inventoryPanel.gameObject);
            // 可以选择禁用脚本或添加一个 CanvasGroup
            // inventoryCanvasGroup = inventoryPanel.gameObject.AddComponent<CanvasGroup>();
        }
        else
        {
             // 根据初始 Alpha 值设置初始状态 (可选，如果希望代码控制初始状态)
             // CloseInventoryPanel(); // 例如，默认关闭
        }


        InitializeSlots();

        // 订阅事件：当背包数据变化时，调用 UpdateUI 方法
        inventoryManager.OnInventoryChanged += UpdateUI;

        // 初始更新一次 UI
        UpdateUI();

        // 确保初始状态是关闭的 (如果 CanvasGroup 初始 alpha 不是 0)
        // CloseInventoryPanel(); // 可以取消注释这一行来强制初始关闭
    }

    // void Update() // 移除 Update 方法，不再检测键盘输入
    // {
    //     // 检测切换按键是否被按下
    //     // if (Input.GetKeyDown(toggleKey))
    //     // {
    //     //     ToggleInventoryPanel();
    //     // }
    // }

    void OnDestroy()
    {
        // 在对象销毁时取消订阅，防止内存泄漏
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryChanged -= UpdateUI;
        }
    }

    /// <summary>
    /// 根据 InventoryManager 的容量初始化 UI 格子
    /// </summary>
    private void InitializeSlots()
    {
        // 清理旧的 UI 格子（如果需要重新初始化）
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }
        uiSlots.Clear();

        // 根据背包容量创建 UI 格子
        for (int i = 0; i < inventoryManager.Capacity; i++)
        {
            GameObject slotInstance = Instantiate(slotPrefab, inventoryPanel);
            slotInstance.name = $"InventorySlot_{i}"; // 给实例化的格子一个有意义的名字

            SlotUIElements elements = new SlotUIElements();
            // 获取必要的组件引用 (根据你的预制件结构调整查找方式)
            elements.itemIcon = slotInstance.transform.Find("ItemIcon")?.GetComponent<Image>(); // 假设图标 Image 名为 ItemIcon
            elements.quantityText = slotInstance.transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>(); // 假设数量文本名为 QuantityText
            // elements.slotBackground = slotInstance.GetComponent<Image>(); // 如果需要引用背景

            if (elements.itemIcon == null || elements.quantityText == null)
            {
                Debug.LogError($"Prefab structure error in {slotInstance.name}. Could not find ItemIcon Image or QuantityText TMP.");
                continue; // 跳过这个损坏的格子
            }

            uiSlots.Add(elements);
        }
         Debug.Log($"Initialized {uiSlots.Count} UI slots.");
    }

    /// <summary>
    /// 更新所有 UI 格子的显示，以匹配 InventoryManager 中的数据
    /// </summary>
    private void UpdateUI()
    {
        if (inventoryManager == null || uiSlots.Count != inventoryManager.Capacity)
        {
             Debug.LogWarning("InventoryManager not ready or UI slots count mismatch. Skipping UI update.");
             // 可以考虑在这里重新初始化 InitializeSlots()
            return;
        }

        // Debug.Log("Updating Inventory UI...");

        for (int i = 0; i < inventoryManager.Capacity; i++)
        {
            InventorySlot dataSlot = inventoryManager.Slots[i]; // 获取对应的数据格子
            SlotUIElements uiSlot = uiSlots[i];                 // 获取对应的 UI 格子引用

            if (dataSlot.ItemData != null) // 如果数据格子里有物品
            {
                uiSlot.itemIcon.sprite = dataSlot.ItemData.icon; // 设置图标
                uiSlot.itemIcon.enabled = true;                  // 显示图标

                // 如果物品可堆叠且数量大于1，显示数量，否则隐藏数量文本
                if (dataSlot.ItemData.isStackable && dataSlot.Quantity > 1)
                {
                    uiSlot.quantityText.text = dataSlot.Quantity.ToString();
                    uiSlot.quantityText.enabled = true;
                }
                else
                {
                    uiSlot.quantityText.text = ""; // 或者设置为 "1" 如果你想显示不可堆叠物品的数量
                    uiSlot.quantityText.enabled = false; // 对于不可堆叠或数量为1的物品，通常隐藏数量
                }
            }
            else // 如果数据格子里是空的
            {
                uiSlot.itemIcon.sprite = null;    // 清除图标
                uiSlot.itemIcon.enabled = false;  // 隐藏图标
                uiSlot.quantityText.text = "";    // 清除文本
                uiSlot.quantityText.enabled = false; // 隐藏文本
            }
        }
    }


    /// <summary>
    /// 切换背包面板的显示/隐藏状态 (现在由按钮调用)
    /// </summary>
    public void ToggleInventoryPanel()
    {
        if (inventoryCanvasGroup == null) return; // 如果没有 CanvasGroup，则不执行任何操作

        // 切换状态
        bool isCurrentlyOpen = inventoryCanvasGroup.alpha > 0;
        if (isCurrentlyOpen)
        {
            CloseInventoryPanel();
        }
        else
        {
            OpenInventoryPanel();
        }
    }

    /// <summary>
    /// 打开背包面板
    /// </summary>
    public void OpenInventoryPanel()
    {
        if (inventoryCanvasGroup == null) return;
        inventoryCanvasGroup.alpha = 1f;
        inventoryCanvasGroup.interactable = true;
        inventoryCanvasGroup.blocksRaycasts = true;
        // (可选) 在打开背包时可能需要暂停游戏或显示鼠标光标
        // Time.timeScale = 0f;
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
        Debug.Log("Inventory Opened");
    }

    /// <summary>
    /// 关闭背包面板
    /// </summary>
    public void CloseInventoryPanel()
    {
        if (inventoryCanvasGroup == null) return;
        inventoryCanvasGroup.alpha = 0f;
        inventoryCanvasGroup.interactable = false;
        inventoryCanvasGroup.blocksRaycasts = false;
        // (可选) 在关闭背包时恢复游戏或隐藏鼠标光标
        // Time.timeScale = 1f;
        // Cursor.lockState = CursorLockMode.Locked; // 或者根据你的游戏设置
        // Cursor.visible = false;
         Debug.Log("Inventory Closed");
    }
}