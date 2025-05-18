using UnityEngine;
using UnityEngine.UI; // 需要引入 UI 命名空间
using System.Collections; // 需要引入 Coroutine 相关的命名空间

// 建议将此脚本附加到包含按钮的父级 GameObject 上
// 并且该父级 GameObject 上有一个布局组件 (如 VerticalLayoutGroup)
[RequireComponent(typeof(VerticalLayoutGroup))] // 或者 HorizontalLayoutGroup, GridLayoutGroup
public class MainMenuButtonController : MonoBehaviour
{
    [Header("按钮引用")]
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject continueGameButton;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject quitGameButton;

    [Header("按钮显示控制")]
    [SerializeField] private bool showNewGame = true;
    [SerializeField] private bool showContinue = true; // Inspector 中的设置，仅用于编辑器预览或潜在的未来逻辑
    [SerializeField] private bool showSettings = true;
    [SerializeField] private bool showQuit = true;

    private LayoutGroup layoutGroup;

    void Awake()
    {
        layoutGroup = GetComponent<LayoutGroup>();
        if (layoutGroup == null)
        {
            Debug.LogError("[MainMenuButtonController] LayoutGroup component not found on this GameObject!", this);
        }
    }

    // 当 GameObject 变为激活状态时调用
    void OnEnable()
    {
        // 启动协程，延迟检查和应用布局
        StartCoroutine(CheckGameManagerAndApplyLayoutDelayed());
    }

    // 协程：等待一帧后执行检查和布局更新
    private IEnumerator CheckGameManagerAndApplyLayoutDelayed()
    {
        // 等待当前帧结束，确保所有 Awake 和 OnEnable 都已执行完毕
        yield return null; // 或者 yield return new WaitForEndOfFrame();

        // 在下一帧开始时执行实际的检查和布局更新
        ApplyButtonVisibilityAndLayout();
    }


    /// <summary>
    /// 根据设置更新按钮的可见性并强制刷新布局。
    /// Continue 按钮的可见性特殊处理，基于 GameManager.Instance。
    /// </summary>
    private void ApplyButtonVisibilityAndLayout()
    {
        bool changed = false; // 跟踪是否有按钮状态发生变化

        // --- 确定 Continue 按钮的运行时可见性 ---
        // 运行时严格根据 GameManager.Instance 是否存在来决定
        bool runtimeShowContinue = (GameManager.Instance != null);
        Debug.Log($"[MainMenuButtonController] (Delayed Check) Checking GameManager.Instance. Exists: {runtimeShowContinue}"); // 添加日志

        // --- 应用其他按钮的可见性设置 (来自 Inspector) ---
        if (newGameButton != null)
        {
            if (newGameButton.activeSelf != showNewGame) { newGameButton.SetActive(showNewGame); changed = true; }
        }

        // --- 应用 Continue 按钮的运行时可见性 ---
        if (continueGameButton != null)
        {
             // 使用运行时计算出的状态
            if (continueGameButton.activeSelf != runtimeShowContinue)
            {
                 continueGameButton.SetActive(runtimeShowContinue);
                 changed = true;
                 Debug.Log($"[MainMenuButtonController] (Delayed Check) Continue button active state set to: {runtimeShowContinue}"); // 添加日志
            }
        }

        // --- 应用其他按钮的可见性设置 (来自 Inspector) ---
        if (settingsButton != null)
        {
            if (settingsButton.activeSelf != showSettings) { settingsButton.SetActive(showSettings); changed = true; }
        }
        if (quitGameButton != null)
        {
            if (quitGameButton.activeSelf != showQuit) { quitGameButton.SetActive(showQuit); changed = true; }
        }

        // --- 强制刷新布局 ---
        if (changed)
        {
            Debug.Log("[MainMenuButtonController] (Delayed Check) Button visibility changed, forcing layout rebuild."); // 添加日志
            ForceLayoutRebuild();
        }
    }

    /// <summary>
    /// 强制布局组件立即重新计算布局
    /// </summary>
    private void ForceLayoutRebuild()
    {
        if (layoutGroup != null)
        {
            // 简单有效的方法是禁用再启用布局组件
            layoutGroup.enabled = false;
            layoutGroup.enabled = true;
        }
         else
        {
             Debug.LogWarning("[MainMenuButtonController] Cannot force layout rebuild because LayoutGroup is missing.", this);
        }
    }

    #if UNITY_EDITOR
    void OnValidate()
    {
        // 确保在编辑器模式下运行时也能获取组件引用
        if (layoutGroup == null) layoutGroup = GetComponent<LayoutGroup>();

        // 使用 UnityEditor.EditorApplication.delayCall 确保在 Inspector 更新后执行
        UnityEditor.EditorApplication.delayCall += () => {
            // 编辑器预览时，我们无法可靠地检查 GameManager.Instance
            // 因此，为了预览效果，我们暂时使用 Inspector 中的 showContinue 值
            // 避免在编辑器预览时启动协程
            if (this != null && gameObject != null && layoutGroup != null && !Application.isPlaying)
            {
                bool editorPreviewChanged = false;
                if (newGameButton != null && newGameButton.activeSelf != showNewGame) { newGameButton.SetActive(showNewGame); editorPreviewChanged = true; }
                // 在编辑器预览中使用 Inspector 的值
                if (continueGameButton != null && continueGameButton.activeSelf != showContinue) { continueGameButton.SetActive(showContinue); editorPreviewChanged = true; }
                if (settingsButton != null && settingsButton.activeSelf != showSettings) { settingsButton.SetActive(showSettings); editorPreviewChanged = true; }
                if (quitGameButton != null && quitGameButton.activeSelf != showQuit) { quitGameButton.SetActive(showQuit); editorPreviewChanged = true; }

                if (editorPreviewChanged)
                {
                    ForceLayoutRebuild();
                }
            }
        };
    }
    #endif
}