using UnityEngine;
using UnityEngine.UI; // 引入 UI 命名空间
using System.Collections.Generic; // 引入列表命名空间

public class WantedLevelController : MonoBehaviour
{
    [Header("通缉等级设置")]
    [SerializeField] private int maxWantedLevel = 6; // 最大通缉等级
    [SerializeField] private List<Image> starImages; // 用于显示星星的 Image 组件列表 (在 Inspector 中拖拽赋值)
    [SerializeField] private Sprite litStarSprite;    // 点亮状态的星星 Sprite (在 Inspector 中拖拽赋值)
    [SerializeField] private Sprite unlitStarSprite;  // 未点亮状态的星星 Sprite (在 Inspector 中拖拽赋值)

    private int currentWantedLevel = 0; // 当前通缉等级

    // --- 公共属性，用于外部获取当前等级 ---
    public int CurrentWantedLevel => currentWantedLevel;
    public int MaxWantedLevel => maxWantedLevel;

    void Start()
    {
        // 确保星星列表和 Sprite 都已设置
        if (starImages == null || starImages.Count != maxWantedLevel)
        {
            Debug.LogError($"星星 Image 列表未正确设置或数量 ({starImages?.Count}) 与最大等级 ({maxWantedLevel}) 不匹配！", this);
            this.enabled = false;
            return;
        }
        if (litStarSprite == null || unlitStarSprite == null)
        {
            Debug.LogError("星星的 Sprite 未设置！", this);
            this.enabled = false;
            return;
        }

        // 初始化星星显示
        UpdateStarUI();
    }

    // --- 用于测试的方法，可以稍后删除或修改 ---
    void Update()
    {
        // 按 '+' 键增加通缉等级 (仅用于测试)
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus)) // 通常 '+' 需要 Shift，所以也检测 '='
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
    /// 增加通缉等级
    /// </summary>
    public void IncreaseWantedLevel()
    {
        SetWantedLevel(currentWantedLevel + 1);
    }

    /// <summary>
    /// 减少通缉等级
    /// </summary>
    public void DecreaseWantedLevel()
    {
        SetWantedLevel(currentWantedLevel - 1);
    }

    /// <summary>
    /// 直接设置通缉等级
    /// </summary>
    /// <param name="level">目标等级</param>
    public void SetWantedLevel(int level)
    {
        int newLevel = Mathf.Clamp(level, 0, maxWantedLevel); // 限制等级在 0 到 maxWantedLevel 之间

        if (newLevel != currentWantedLevel)
        {
            currentWantedLevel = newLevel;
            Debug.Log("通缉等级变为: " + currentWantedLevel);
            UpdateStarUI();
            // 在这里可以触发其他与通缉等级变化相关的逻辑，比如敌人生成、警察行为变化等
        }
    }

    /// <summary>
    /// 根据当前通缉等级更新星星 UI 显示
    /// </summary>
    private void UpdateStarUI()
    {
        for (int i = 0; i < starImages.Count; i++)
        {
            if (i < currentWantedLevel)
            {
                // 点亮这颗星
                starImages[i].sprite = litStarSprite;
            }
            else
            {
                // 熄灭这颗星
                starImages[i].sprite = unlitStarSprite;
            }
            // 可以选择是否在 0 级时隐藏所有星星，或者只是显示未点亮状态
            // starImages[i].enabled = currentWantedLevel > 0; // 如果想在0级时隐藏星星，取消这行注释
        }
    }
}