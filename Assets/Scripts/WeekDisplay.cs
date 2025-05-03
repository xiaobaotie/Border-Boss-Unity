using UnityEngine;
using TMPro; // 引入 TextMeshPro 命名空间

public class WeekDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI weekText; // 在 Inspector 中拖拽赋值

    void Start()
    {
        // 检查 weekText 是否已分配
        if (weekText == null)
        {
            Debug.LogError("Week Text is not assigned in the Inspector!", this);
            weekText = GetComponent<TextMeshProUGUI>(); // 尝试自动获取
            if (weekText == null)
            {
                this.enabled = false; // 找不到则禁用脚本
                return;
            }
        }

        // 检查 GameManager 实例
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance not found! WeekDisplay cannot function.", this);
            this.enabled = false;
            return;
        }

        // 订阅 GameManager 的周数变化事件
        GameManager.Instance.OnWeekChanged += UpdateDisplay;

        // 初始化时更新一次显示
        UpdateDisplay();
    }

    void OnDestroy()
    {
        // 取消订阅事件
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWeekChanged -= UpdateDisplay;
        }
    }

    /// <summary>
    /// 更新显示的周数文本，从 GameManager 获取数据
    /// </summary>
    private void UpdateDisplay()
    {
        if (weekText != null && GameManager.Instance != null)
        {
            // 从 GameManager 获取当前周数并更新文本
            // 将 "Week: " 修改为 "WEEK "
            weekText.text = "WEEK " + GameManager.Instance.CurrentWeek.ToString();
        }
    }
}