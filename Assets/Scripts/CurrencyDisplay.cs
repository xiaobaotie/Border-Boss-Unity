using UnityEngine;
using TMPro; // 确保引入 TextMeshPro 命名空间

// 确保 GameObject 上有 TextMeshProUGUI 组件 (或者你可以手动添加)
// [RequireComponent(typeof(TextMeshProUGUI))] // 如果你确定这个脚本只挂在有 TextMeshProUGUI 的对象上
public class CurrencyDisplay : MonoBehaviour
{
    // --- 变量 ---
    [Header("UI Elements")]
    // 将场景中的 TextMeshProUGUI 组件拖拽到这里
    public TextMeshProUGUI balanceText;
    // (可选) 如果需要动态改变图标，可以在这里引用 Image 组件
    // public Image currencyIcon;

    // 不再需要本地的 currentBalance 变量
    // [Header("Balance Data")]
    // public int currentBalance = 1000; // 移除

    // --- Unity 事件函数 ---

    void Start()
    {
        // 检查 balanceText 是否已分配
        if (balanceText == null)
        {
            Debug.LogError("Balance Text is not assigned in the Inspector!", this);
            // 尝试自动获取组件，如果脚本挂在同一个 GameObject 上
            balanceText = GetComponent<TextMeshProUGUI>();
            if (balanceText == null)
            {
                this.enabled = false; // 如果找不到，禁用脚本
                return;
            }
        }

        // 检查 GameManager 实例是否存在
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance not found! CurrencyDisplay cannot function.", this);
            this.enabled = false;
            return;
        }

        // 订阅 GameManager 的余额变化事件
        GameManager.Instance.OnBalanceChanged += UpdateDisplay;

        // 初始化时更新一次显示
        UpdateDisplay();
    }

    void OnDestroy()
    {
        // 当此对象销毁时，取消订阅事件，防止内存泄漏
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBalanceChanged -= UpdateDisplay;
        }
    }

    // --- 公共方法 ---

    /// <summary>
    /// 更新显示的余额文本，从 GameManager 获取数据
    /// </summary>
    public void UpdateDisplay()
    {
        if (balanceText != null && GameManager.Instance != null)
        {
            // 从 GameManager 获取当前余额并更新文本
            balanceText.text = GameManager.Instance.CurrentBalance.ToString();
        }
        // else // Start 中已经处理了 null 的情况
        // {
        //     Debug.LogError("Balance Text or GameManager Instance is missing!");
        // }
    }

    // 移除 AddCurrency 和 SpendCurrency 方法，余额管理由 GameManager 负责
    /*
    /// <summary>
    /// 增加余额
    /// </summary>
    /// <param name="amount">要增加的数量</param>
    public void AddCurrency(int amount)
    {
        if (amount > 0)
        {
            currentBalance += amount;
            UpdateDisplay(); // 更新显示
            Debug.Log("Added $" + amount + ". New balance: $" + currentBalance);
        }
    }

    /// <summary>
    /// 消费余额
    /// </summary>
    /// <param name="amount">要消费的数量</param>
    /// <returns>如果消费成功返回 true，否则返回 false</returns>
    public bool SpendCurrency(int amount)
    {
        if (amount <= 0) return false; // 不能消费负数或零

        if (currentBalance >= amount)
        {
            currentBalance -= amount;
            UpdateDisplay(); // 更新显示
            Debug.Log("Spent $" + amount + ". New balance: $" + currentBalance);
            return true;
        }
        else
        {
            Debug.LogWarning("Not enough balance to spend $" + amount + ". Current balance: $" + currentBalance);
            return false; // 余额不足
        }
    }
    */
}
