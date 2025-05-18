using UnityEngine;
using UnityEngine.UI;
using TMPro; // 如果你使用 TextMeshPro 显示文本，请保留这行；如果使用 Unity 标准 Text，可以注释掉或删除

public class ShopItemUI : MonoBehaviour
{
    [Header("UI References")]
    // 在 Inspector 中将对应的 UI 元素拖拽到这里
    public Image itemIcon;
    // public TextMeshProUGUI itemNameText; // <-- 如果你确定不要名称，可以保持注释或删除
    public TextMeshProUGUI itemPriceText; // 如果使用标准 Text，请改为: public Text itemPriceText;
    // public Button buyButton; // <-- 移除购买按钮的引用

    [Header("Data (Internal)")]
    private Item currentItem;
    private ShopManager shopManager; // 引用 ShopManager 以便调用购买方法

    /// <summary>
    /// 设置此商品项 UI 的显示内容和购买逻辑。
    /// 由 EggShopPanel 在实例化时调用。
    /// </summary>
    /// <param name="item">要显示的商品数据。</param>
    /// <param name="manager">当前商店的 ShopManager。</param>
    public void Setup(Item item, ShopManager manager)
    {
        currentItem = item;
        shopManager = manager; // 暂时保留 manager，因为 EggShopPanel 会传递它

        // --- 更新 UI 显示 ---
        if (itemIcon != null)
        {
            if (item != null && item.icon != null) // 假设 Item ScriptableObject 有一个 Sprite 类型的 icon 字段
            {
                itemIcon.sprite = item.icon;
                itemIcon.enabled = true; // 确保 Image 组件是启用的
            }
            else
            {
                itemIcon.enabled = false; // 如果没有图标，禁用 Image 组件
                Debug.LogWarning($"商品 '{item?.itemName ?? "N/A"}' 缺少图标。");
            }
        }

        /* // <-- 如果不要名称，保持注释或删除
        if (itemNameText != null)
        {
            itemNameText.text = item != null ? item.itemName : "无效物品";
        }
        */

        if (itemPriceText != null)
        {
            itemPriceText.text = item != null ? $"${item.basePrice}" : "$?"; // 假设 Item 有 basePrice 字段
        }

        // --- 移除购买按钮的设置 ---
        /* // <-- 移除或注释掉这部分代码
        if (buyButton != null)
        {
             buyButton.onClick.RemoveAllListeners();
             buyButton.onClick.AddListener(OnBuyButtonClicked);
             buyButton.interactable = true;
        }
        */ // <-- 移除或注释掉这部分代码结束
    }

    // --- 移除购买按钮相关的旧方法 ---
    /* // <-- 移除或注释掉这个方法
    private void OnBuyButtonClicked()
    {
        // 确保我们有关联的物品和商店管理器
        if (currentItem != null && shopManager != null)
        {
            Debug.Log($"[ShopItemUI] 玩家点击购买: {currentItem.itemName}");
            // 调用 ShopManager 的购买方法
            bool purchaseSuccess = shopManager.AttemptPurchase(currentItem, 1); // 默认购买数量为 1

            if (purchaseSuccess)
            {
                Debug.Log($"[ShopItemUI] 购买 '{currentItem.itemName}' 成功!");
                // (可选) 在这里可以添加购买成功的反馈，例如播放音效、更新UI等
            }
            else
            {
                 Debug.LogWarning($"[ShopItemUI] 购买 '{currentItem.itemName}' 失败 (可能是金钱不足或背包已满)。");
                 // (可选) 在这里可以添加购买失败的反馈
            }
        }
        else
        {
            Debug.LogError("[ShopItemUI] 购买按钮被点击，但 currentItem 或 shopManager 为 null!", this);
        }
    }
    */ // <-- 移除或注释掉这个方法结束
}