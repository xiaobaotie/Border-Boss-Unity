using UnityEngine;
using UnityEngine.UI; // 如果使用旧版 UI Text
using TMPro; // 如果使用 TextMeshPro

public class CurrencyDisplay : MonoBehaviour
{
    // --- 变量 ---
    [Header("UI Elements")]
    // 将场景中的 TextMeshProUGUI 组件拖拽到这里
    public TextMeshProUGUI balanceText;
    // (可选) 如果需要动态改变图标，可以在这里引用 Image 组件
    // public Image currencyIcon;

    [Header("Balance Data")]
    // 当前余额 (可以设置为 private，通过方法修改)
    public int currentBalance = 1000; // 示例初始值

    // --- Unity 事件函数 ---

    void Start()
    {
        // 初始化时更新一次显示
        UpdateDisplay();
    }

    // --- 公共方法 ---

    /// <summary>
    /// 更新显示的余额文本
    /// </summary>
    public void UpdateDisplay()
    {
        if (balanceText != null)
        {
            // 格式化文本，去掉 '$' 符号
            balanceText.text = currentBalance.ToString();
        }
        else
        {
            Debug.LogError("Balance Text is not assigned in the Inspector!");
        }
    }

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
    /// <returns>如果消费成功返回 true，否则返回 false (余额不足)</returns>
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

    // --- (可选) 示例：用于按钮测试 ---
    public void TestAddCurrency()
    {
        AddCurrency(100);
    }

    public void TestSpendCurrency()
    {
        SpendCurrency(50);
    }
}
