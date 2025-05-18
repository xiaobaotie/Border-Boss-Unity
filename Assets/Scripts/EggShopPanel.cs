using System.Collections.Generic; // 需要引入 Generic 命名空间
using UnityEngine;
using UnityEngine.UI; // 引入 UI 命名空间

public class EggShopPanel : MonoBehaviour
{
    [Header("UI References")]
    // 在 Inspector 中将你的关闭按钮拖拽到这里
    public Button closeButton;
    // 新增：商品项 UI 的容器 (需要你在 Inspector 中拖拽对应的 GameObject)
    [SerializeField] private Transform itemContainer;
    // 新增：单个商品项的 UI 预制件 (需要你在 Inspector 中拖拽对应的 Prefab)
    [SerializeField] private GameObject shopItemPrefab;

    [Header("Data")]
    // 新增：当前交互的商店逻辑
    private ShopManager currentShopManager;

    void Start()
    {
        // 确保关闭按钮已设置并添加监听器
        if (closeButton != null)
        {
            // 当按钮被点击时，调用 ClosePanel 方法
            closeButton.onClick.AddListener(ClosePanel);
        }
        else
        {
            Debug.LogError("Close Button 没有在 EggShopPanel 的 Inspector 中设置!", this);
        }

        // 确保必要的引用已设置
        if (itemContainer == null)
        {
             Debug.LogError("Item Container 没有在 EggShopPanel 的 Inspector 中设置!", this);
        }
        if (shopItemPrefab == null)
        {
             Debug.LogError("Shop Item Prefab 没有在 EggShopPanel 的 Inspector 中设置!", this);
        }


        // 确保面板初始是隐藏的
        gameObject.SetActive(false);
    }

    // 关闭按钮调用的方法
    public void ClosePanel()
    {
        Debug.Log($"关闭商店面板: {currentShopManager?.ShopName ?? "N/A"}");
        gameObject.SetActive(false); // 隐藏此面板 GameObject
        currentShopManager = null; // 清除当前商店引用
        ClearShopItems(); // 清空UI项
        // (可选) 可以在这里恢复游戏或执行其他关闭商店时的逻辑
    }

    // 修改：打开面板的方法，现在接收 ShopManager
    public void OpenPanel(ShopManager shopToOpen)
    {
        if (shopToOpen == null)
        {
            Debug.LogError("尝试打开商店但传入的 ShopManager 为 null!", this);
            return;
        }
        currentShopManager = shopToOpen;

        Debug.Log($"打开商店面板: {currentShopManager.ShopName}");

        // 填充商品列表
        PopulateShopItems();

        // 显示面板
        gameObject.SetActive(true);
        // (可选) 可以在这里暂停游戏或执行其他打开商店时的逻辑
    }

    // --- 新增私有方法 ---

    /// <summary>
    /// 根据当前 ShopManager 的商品列表填充 UI。
    /// </summary>
    private void PopulateShopItems()
    {
        // 先清空旧的商品项
        ClearShopItems();

        if (currentShopManager == null || itemContainer == null || shopItemPrefab == null)
        {
            Debug.LogError("无法填充商品项。请检查 ShopManager, ItemContainer, 或 ShopItemPrefab 的引用。", this);
            return;
        }

        List<Item> items = currentShopManager.GetAvailableItems();
        Debug.Log($"填充商店 UI，共 {items.Count} 个商品。");

        foreach (Item item in items)
        {
            // 实例化商品项 UI 预制件
            GameObject itemGO = Instantiate(shopItemPrefab, itemContainer);

            // --- 取消注释以下代码 ---
            // 获取预制件上的脚本 (假设有一个 ShopItemUI 脚本)
            ShopItemUI itemUI = itemGO.GetComponent<ShopItemUI>();
            if (itemUI != null)
            {
                // 设置商品信息并传入 ShopManager 引用
                // 注意：我们调用 Setup 时仍然传递 currentShopManager，因为 ShopItemUI.cs 当前版本还需要它
                itemUI.Setup(item, currentShopManager);
            }
            else
            {
                Debug.LogWarning($"ShopItemPrefab '{shopItemPrefab.name}' 上缺少 ShopItemUI 组件。", itemGO);
            }
            // --- 取消注释结束 ---
        }
    }

    /// <summary>
    /// 清空商品容器中的所有 UI 项。
    /// </summary>
    private void ClearShopItems()
    {
        if (itemContainer != null)
        {
            // 销毁所有子对象 (商品项)
            foreach (Transform child in itemContainer)
            {
                Destroy(child.gameObject);
            }
             Debug.Log("清空商店商品项 UI。");
        }
    }
}

// --- (需要额外创建的脚本，放在单独的文件 ShopItemUI.cs 中) ---
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro; // 如果使用 TextMeshPro
//
// public class ShopItemUI : MonoBehaviour
// {
//     public Image itemIcon;
//     public TextMeshProUGUI itemNameText; // 或者 public Text itemNameText;
//     public TextMeshProUGUI itemPriceText; // 或者 public Text itemPriceText;
//     public Button buyButton;
//
//     private Item currentItem;
//     private ShopManager shopManager;
//
//     public void Setup(Item item, ShopManager manager)
//     {
//         currentItem = item;
//         shopManager = manager;
//
//         // 更新 UI 显示
//         if (itemIcon != null && item.icon != null) itemIcon.sprite = item.icon; // 假设 Item 有 icon 字段
//         if (itemNameText != null) itemNameText.text = item.itemName;
//         if (itemPriceText != null) itemPriceText.text = $"${item.basePrice}"; // 假设 Item 有 basePrice
//
//         // 设置购买按钮点击事件
//         if (buyButton != null)
//         {
//              buyButton.onClick.RemoveAllListeners(); // 先移除旧监听器
//              buyButton.onClick.AddListener(OnBuyButtonClicked);
//         }
//     }
//
//     private void OnBuyButtonClicked()
//     {
//         if (currentItem != null && shopManager != null)
//         {
//             Debug.Log($"UI: 尝试购买 {currentItem.itemName}");
//             // 调用 ShopManager 的购买方法
//             bool success = shopManager.AttemptPurchase(currentItem, 1); // 默认买1个
//             if (success)
//             {
//                 Debug.Log($"UI: 购买 {currentItem.itemName} 成功");
//                 // (可选) 播放音效或显示成功提示
//             }
//             else
//             {
//                  Debug.Log($"UI: 购买 {currentItem.itemName} 失败 (可能是背包满了)");
//                 // (可选) 显示失败提示
//             }
//         }
//     }
// }