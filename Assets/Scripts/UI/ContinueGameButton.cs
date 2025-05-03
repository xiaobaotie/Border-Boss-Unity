using UnityEngine;
using UnityEngine.UI; // 需要引入 UI 命名空间
using UnityEngine.SceneManagement; // 需要引入 SceneManagement 命名空间
using System.Collections.Generic; // 需要引入 List

// 确保该 GameObject 上有一个 Button 组件
[RequireComponent(typeof(Button))]
public class ContinueGameButton : MonoBehaviour
{
    [Header("继续游戏场景配置")]
    // 修改 Tooltip 解释
    [Tooltip("根据 GameManager.CurrentWeek % 4 的余数决定加载的场景。\n索引 0 -> 余数 0 (即第4, 8, ...周)\n索引 1 -> 余数 1 (即第1, 5, ...周)\n索引 2 -> 余数 2 (即第2, 6, ...周)\n索引 3 -> 余数 3 (即第3, 7, ...周)")]
    [SerializeField]
    private List<string> weekRemainderScenes = new List<string>(new string[4]); // 初始化包含4个空字符串的列表

    private Button continueButton;

    void Awake()
    {
        // 获取 Button 组件的引用
        continueButton = GetComponent<Button>();
        // 为按钮的点击事件添加监听器
        continueButton.onClick.AddListener(OnContinueClicked);

        // 确保列表正好有4个元素
        EnsureSceneListSize();
    }

    void OnDestroy()
    {
        // 在对象销毁时移除监听器
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(OnContinueClicked);
        }
    }

    /// <summary>
    /// 确保场景列表正好有4个元素
    /// </summary>
    private void EnsureSceneListSize()
    {
         // 修改目标大小为 4
         if (weekRemainderScenes == null)
        {
            weekRemainderScenes = new List<string>(new string[4]);
            Debug.LogWarning("[ContinueGameButton] weekRemainderScenes list was null, initialized to size 4.", this);
        }
        while (weekRemainderScenes.Count < 4)
        {
            weekRemainderScenes.Add(""); // 添加空字符串直到满4个
        }
        if (weekRemainderScenes.Count > 4)
        {
            weekRemainderScenes.RemoveRange(4, weekRemainderScenes.Count - 4); // 移除多余的
            Debug.LogWarning("[ContinueGameButton] weekRemainderScenes list size was > 4, truncated.", this);
        }
    }

    /// <summary>
    /// 当 "Continue Game" 按钮被点击时执行此方法
    /// </summary>
    public void OnContinueClicked()
    {
        // 检查 GameManager 实例是否存在
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance not found! Cannot continue.", this);
            return;
        }

        // 1. 从 GameManager 获取当前周数
        int currentWeek = GameManager.Instance.CurrentWeek;

        if (currentWeek <= 0)
        {
             Debug.LogWarning($"[ContinueGameButton] CurrentWeek ({currentWeek}) is not positive. Cannot determine continue scene based on remainder.", this);
             return;
        }

        // 2. 计算余数 (索引 0-3)
        // 修改：直接使用 currentWeek % 4
        // Week 1 -> Remainder 1 (Index 1)
        // Week 2 -> Remainder 2 (Index 2)
        // Week 3 -> Remainder 3 (Index 3)
        // Week 4 -> Remainder 0 (Index 0)
        int remainderIndex = currentWeek % 4;

        Debug.Log($"[ContinueGameButton] Clicked. CurrentWeek={currentWeek}, Remainder Index={remainderIndex}");

        // 3. 从本脚本的配置列表中获取目标场景名称
        string targetSceneName = null;
        if (remainderIndex >= 0 && remainderIndex < weekRemainderScenes.Count) // Count 现在应该是 4
        {
            targetSceneName = weekRemainderScenes[remainderIndex];
            if (string.IsNullOrEmpty(targetSceneName))
            {
                Debug.LogError($"[ContinueGameButton] Scene name for week remainder index {remainderIndex} is not configured in the Inspector on {gameObject.name}!", this);
                return; // 未配置场景名，停止执行
            }
        }
        else
        {
            // 理论上 EnsureSceneListSize 会防止这种情况，但作为安全检查
            Debug.LogError($"[ContinueGameButton] Calculated remainder index {remainderIndex} is out of bounds for configured scenes list!", this);
            return; // 索引越界，停止执行
        }

        // 4. (可选) 更新 GameManager 中的逻辑场景名称
        // 如果你仍然需要 GameManager 跟踪逻辑场景名，可以在这里更新
        // GameManager.Instance.UpdateLogicalSceneName(targetSceneName);

        // 5. 加载目标场景
        Debug.Log($"[ContinueGameButton] Loading scene: {targetSceneName}");
        SceneManager.LoadScene(targetSceneName);
    }
}