using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WantedLevelController : MonoBehaviour
{
    [Header("通缉等级设置")]
    [SerializeField] private int maxWantedLevel = 6; // 最大通缉等级 (UI 相关)
    [SerializeField] private List<Image> starImages; // 星星 Image 列表
    [SerializeField] private Sprite litStarSprite;    // 点亮状态 Sprite
    [SerializeField] private Sprite unlitStarSprite;  // 未点亮状态 Sprite

    // 移除本地的 currentWantedLevel 变量
    // private int currentWantedLevel = 0;

    // --- 公共属性，现在从 GameManager 获取 ---
    // 如果其他脚本需要获取当前等级，它们可以直接访问 GameManager.Instance.CurrentWantedLevel
    // public int CurrentWantedLevel => GameManager.Instance?.CurrentWantedLevel ?? 0; // 提供只读访问
    public int MaxWantedLevel => maxWantedLevel; // 最大等级仍然由这个脚本定义 (基于星星数量)

    void Start()
    {
        // 确保星星列表和 Sprite 都已设置
        if (starImages == null || starImages.Count == 0) // 允许数量不严格等于 maxWantedLevel，但至少要有一个
        {
            Debug.LogError("星星 Image 列表未设置或为空！", this);
            this.enabled = false;
            return;
        }
        // 调整最大等级以匹配实际星星数量，防止索引越界
        maxWantedLevel = starImages.Count;

        if (litStarSprite == null || unlitStarSprite == null)
        {
            Debug.LogError("星星的 Sprite 未设置！", this);
            this.enabled = false;
            return;
        }

        // 检查 GameManager 实例
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance not found! WantedLevelController cannot function.", this);
            this.enabled = false;
            return;
        }

        // 订阅 GameManager 的通缉等级变化事件
        GameManager.Instance.OnWantedLevelChanged += UpdateStarUI;

        // 初始化星星显示，使用 GameManager 中的当前值
        UpdateStarUI();
    }

    void OnDestroy()
    {
        // 取消订阅事件
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWantedLevelChanged -= UpdateStarUI;
        }
    }

    // --- 用于测试的方法，可以稍后删除或修改 ---
    void Update()
    {
        // 按 '+' 键增加通缉等级 (仅用于测试)
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus))
        {
            IncreaseWantedLevel();
        }
        // 按 '-' 键减少通缉等级 (仅用于测试)
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            DecreaseWantedLevel();
        }
    }
    // --- 测试方法结束 ---


    /// <summary>
    /// 请求增加通缉等级 (通过 GameManager)
    /// </summary>
    public void IncreaseWantedLevel()
    {
        if (GameManager.Instance != null)
        {
            // 计算新等级，并确保不超过最大值
            int newLevel = Mathf.Clamp(GameManager.Instance.CurrentWantedLevel + 1, 0, maxWantedLevel);
            // 调用 GameManager 更新等级
            GameManager.Instance.UpdateWantedLevel(newLevel);
        }
    }

    /// <summary>
    /// 请求减少通缉等级 (通过 GameManager)
    /// </summary>
    public void DecreaseWantedLevel()
    {
         if (GameManager.Instance != null)
        {
            // 计算新等级，并确保不低于 0
            int newLevel = Mathf.Clamp(GameManager.Instance.CurrentWantedLevel - 1, 0, maxWantedLevel);
             // 调用 GameManager 更新等级
            GameManager.Instance.UpdateWantedLevel(newLevel);
        }
    }

    /// <summary>
    /// 请求直接设置通缉等级 (通过 GameManager)
    /// </summary>
    /// <param name="level">目标等级</param>
    public void SetWantedLevel(int level)
    {
        if (GameManager.Instance != null)
        {
            // 限制等级在 0 到 maxWantedLevel 之间
            int newLevel = Mathf.Clamp(level, 0, maxWantedLevel);
            // 调用 GameManager 更新等级
            GameManager.Instance.UpdateWantedLevel(newLevel);
            // 注意：GameManager 的 UpdateWantedLevel 内部会检查等级是否真的改变
            // Debug.Log 和 UI 更新现在由 GameManager 的事件触发
        }
    }

    /// <summary>
    /// 根据 GameManager 中的当前通缉等级更新星星 UI 显示
    /// </summary>
    private void UpdateStarUI()
    {
        if (GameManager.Instance == null) return; // GameManager 不存在则不执行

        int currentLevel = GameManager.Instance.CurrentWantedLevel;
        // Debug.Log($"Updating Star UI for Wanted Level: {currentLevel}"); // 添加调试信息

        for (int i = 0; i < starImages.Count; i++)
        {
            if (i < currentLevel)
            {
                starImages[i].sprite = litStarSprite;
            }
            else
            {
                starImages[i].sprite = unlitStarSprite;
            }
            // 可以选择是否在 0 级时隐藏所有星星
            // starImages[i].enabled = currentLevel > 0;
        }
    }
}