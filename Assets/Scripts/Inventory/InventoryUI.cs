using UnityEngine;
using UnityEngine.UI;
using TMPro; // 确保引入 TextMeshPro 命名空间
using System.Collections.Generic;
using System; // 如果使用了 Action

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform inventoryPanel; // 拖拽 InventoryPanel 对象到这里
    [SerializeField] private GameObject slotPrefab;   // 拖拽 InventorySlot_Prefab 预制件到这里
    [SerializeField] private TextMeshProUGUI itemDescriptionText; // 新增：拖拽用于显示物品描述的 TextMeshProUGUI 组件到这里

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
        public Button slotButton; // Button 组件现在从 HighLightBorder 获取
        public GameObject highlightBorderGameObject; // 对 HighLightBorder 游戏对象的引用 (可选，如果需要直接控制它)
    }

    private CanvasGroup inventoryCanvasGroup;

    private int selectedSlotIndex = -1; // 新增：当前选中的格子索引，-1 表示未选中

    void Awake()
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
    void InitializeSlots()
    {
        if (inventoryManager == null || slotPrefab == null || inventoryPanel == null)
        {
            Debug.LogError("Cannot initialize slots: InventoryManager, SlotPrefab, or InventoryPanel is not set.");
            return;
        }

        // 清理旧的格子 (如果需要重新初始化)
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }
        uiSlots.Clear();

        for (int i = 0; i < inventoryManager.Capacity; i++)
        {
            GameObject slotInstance = Instantiate(slotPrefab, inventoryPanel);
            slotInstance.name = $"InventorySlot_{i}";

            SlotUIElements elements = new SlotUIElements();
            elements.itemIcon = slotInstance.transform.Find("ItemIcon")?.GetComponent<Image>();
            elements.quantityText = slotInstance.transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>();
            
            // 修改这里：从 HighLightBorder 子对象获取 Button 组件
            Transform highlightBorderTransform = slotInstance.transform.Find("HighLightBorder");
            if (highlightBorderTransform != null)
            {
                elements.slotButton = highlightBorderTransform.GetComponent<Button>();
                elements.highlightBorderGameObject = highlightBorderTransform.gameObject; // 可选：存储对 GameObject 的引用
            }


            if (elements.itemIcon == null || elements.quantityText == null || elements.slotButton == null)
            {
                Debug.LogError($"Prefab structure error in {slotInstance.name}. Could not find ItemIcon, QuantityText, or Button component on HighLightBorder.");
                Destroy(slotInstance); // 销毁有问题的实例
                continue;
            }

            // if (elements.highlightBorderGameObject != null) elements.highlightBorderGameObject.SetActive(false); // 初始隐藏高亮

            // 为按钮添加点击事件监听器
            int currentIndex = i; // 捕获当前循环的索引值，用于 lambda 表达式
            elements.slotButton.onClick.AddListener(() => SelectSlot(currentIndex));

            uiSlots.Add(elements);
        }
    }

    /// <summary>
    /// 更新所有 UI 格子的显示
    /// </summary>
    public void UpdateUI() // 改为 public 以便外部调用 (例如 InventoryManager)
    {
        if (inventoryManager == null || uiSlots.Count != inventoryManager.Capacity)
        {
            // 如果格子数量不匹配 (例如在 Start 之后动态改变了 Capacity 且未重新 InitializeSlots)
            // 或者 inventoryManager 为空，则可能需要重新初始化或记录错误
            if (inventoryManager != null && uiSlots.Count != inventoryManager.Capacity)
            {
                Debug.LogWarning("InventoryUI slot count mismatch with InventoryManager capacity. Re-initializing slots.");
                InitializeSlots(); // 尝试重新初始化
                if (uiSlots.Count != inventoryManager.Capacity) // 再次检查
                {
                     Debug.LogError("Failed to re-initialize slots to match capacity. UI might be inconsistent.");
                     return;
                }
            }
            else if (inventoryManager == null)
            {
                Debug.LogError("InventoryManager is null in UpdateUI.");
                return;
            }
        }

        for (int i = 0; i < inventoryManager.Capacity; i++)
        {
            InventorySlot dataSlot = inventoryManager.slots[i];
            SlotUIElements uiSlot = uiSlots[i];

            if (dataSlot.ItemData != null)
            {
                uiSlot.itemIcon.sprite = dataSlot.ItemData.icon;
                uiSlot.itemIcon.enabled = true;

                if (dataSlot.ItemData.isStackable && dataSlot.Quantity > 1)
                {
                    uiSlot.quantityText.text = dataSlot.Quantity.ToString();
                    uiSlot.quantityText.enabled = true;
                }
                else
                {
                    uiSlot.quantityText.text = "";
                    uiSlot.quantityText.enabled = false;
                }
            }
            else
            {
                uiSlot.itemIcon.sprite = null;
                uiSlot.itemIcon.enabled = false;
                uiSlot.quantityText.text = "";
                uiSlot.quantityText.enabled = false;
            }

            // 更新高亮状态 (可选)
            // if (uiSlot.highlightBorderGameObject != null) // 使用 highlightBorderGameObject
            // {
            //     uiSlot.highlightBorderGameObject.SetActive(i == selectedSlotIndex && dataSlot.ItemData != null);
            // }
        }
        UpdateSelectedItemDescription(); // 当UI更新时，也刷新描述文本
    }

    /// <summary>
    /// 当一个格子被点击时调用
    /// </summary>
    /// <param name="slotIndex">被点击的格子索引</param>
    public void SelectSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= uiSlots.Count)
        {
            Debug.LogWarning($"Invalid slot index: {slotIndex}");
            return;
        }

        // 即使点击的是空格子，也更新 selectedSlotIndex
        // 如果点击的是当前已选中的格子，则取消选中 (或者根据你的需求决定行为)
        // if (selectedSlotIndex == slotIndex)
        // {
        //     selectedSlotIndex = -1; // 取消选中
        // }
        // else
        // {
        //    selectedSlotIndex = slotIndex;
        // }
        // 当前逻辑：直接设置为点击的格子
        selectedSlotIndex = slotIndex;


        Debug.Log($"Slot {slotIndex} selected.");
        UpdateSelectedItemDescription();
        // UpdateUI(); // 调用 UpdateUI 来更新高亮等视觉效果 (如果实现了高亮)
    }

    /// <summary>
    /// 更新选中物品的描述文本
    /// </summary>
    private void UpdateSelectedItemDescription()
    {
        if (itemDescriptionText == null)
        {
            // Debug.LogWarning("ItemDescriptionText is not assigned in InventoryUI."); // 首次运行时可能触发，可注释掉
            return;
        }

        if (selectedSlotIndex >= 0 && selectedSlotIndex < inventoryManager.Capacity)
        {
            InventorySlot dataSlot = inventoryManager.slots[selectedSlotIndex];
            if (dataSlot.ItemData != null)
            {
                itemDescriptionText.text = dataSlot.ItemData.description;
            }
            else
            {
                itemDescriptionText.text = ""; // 修改：当选中的格子为空时，显示空字符串
            }
        }
        else
        {
            itemDescriptionText.text = ""; // 修改：当没有选中任何物品时，也显示空字符串
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
        UpdateUI(); // <--- 添加这一行
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
        UpdateUI(); // <--- 添加这一行
    }
}