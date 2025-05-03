using UnityEngine;
using UnityEngine.UI; // 需要引入 UI 命名空间

// 确保该 GameObject 上有一个 Button 组件
[RequireComponent(typeof(Button))]
public class NextWeekButton : MonoBehaviour
{
    private Button nextWeekButton;

    void Awake()
    {
        // 获取 Button 组件的引用
        nextWeekButton = GetComponent<Button>();
        // 为按钮的点击事件添加监听器，指定调用 OnNextWeekClicked 方法
        nextWeekButton.onClick.AddListener(OnNextWeekClicked);
    }

    void OnDestroy()
    {
        // 在对象销毁时移除监听器，防止内存泄漏
        if (nextWeekButton != null)
        {
            nextWeekButton.onClick.RemoveListener(OnNextWeekClicked);
        }
    }

    /// <summary>
    /// 当 "Next Week" 按钮被点击时执行此方法
    /// </summary>
    public void OnNextWeekClicked()
    {
        // 检查 GameManager 实例是否存在
        if (GameManager.Instance == null)
        {
            Debug.LogError("[NextWeekButton] GameManager instance not found! Cannot proceed to next week.", this);
            return; // 如果找不到 GameManager，则不执行后续操作
        }

        // 1. 从 GameManager 获取当前周数
        int currentWeek = GameManager.Instance.CurrentWeek;

        // 2. 计算下一周的周数
        int nextWeek = currentWeek + 1;

        // 3. 更新 GameManager 中的周数
        //    假设 GameManager 有一个名为 UpdateWeek 的方法来处理周数更新和相关事件触发
        GameManager.Instance.UpdateWeek(nextWeek);

        Debug.Log($"[NextWeekButton] Proceeding to next week. Current week is now: {nextWeek}");

        // 你可以在这里添加其他需要在进入下一周时执行的逻辑，
        // 例如保存游戏状态、触发特定事件等。
    }
}