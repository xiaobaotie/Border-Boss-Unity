using UnityEngine;
using UnityEngine.SceneManagement; // 如果需要场景切换

public class BackButtonHandler : MonoBehaviour
{
    [Header("返回设置")]
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // 在 Inspector 中设置你的主菜单场景名称

    // 可选：如果你希望这个 GameObject 在场景切换时保留
    void Awake()
     {
         DontDestroyOnLoad(gameObject);
     }

    void Update()
    {
        // 检测 Escape 键 (对应 Android 返回键和编辑器 ESC)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
    }

    /// <summary>
    /// 处理返回按钮按下的逻辑
    /// </summary>
    private void HandleBackButton()
    {
        Debug.Log("Back button pressed!");

        // 获取当前活动场景的名称
        string currentSceneName = SceneManager.GetActiveScene().name;
        // 使用在 Inspector 中设置的 mainMenuSceneName
        // string mainMenuSceneName = "MainMenu"; // 移除这行

        // 检查当前场景是否不是主菜单场景
        if (currentSceneName != mainMenuSceneName)
        {
            // 检查 mainMenuSceneName 是否为空或无效 (可选但推荐)
            if (string.IsNullOrEmpty(mainMenuSceneName))
            {
                 Debug.LogError("MainMenuSceneName is not set in the Inspector!", this);
                 return;
            }

            Debug.Log($"Current scene is '{currentSceneName}', returning to '{mainMenuSceneName}'.");
            // 加载主菜单场景
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            // 如果当前已经是主菜单，可以选择退出应用或什么都不做
            Debug.Log("Already in Main Menu. Considering Application Quit.");
            // Application.Quit(); // 如果想在主菜单按返回键退出游戏，取消这行注释
            // #if UNITY_EDITOR
            // UnityEditor.EditorApplication.isPlaying = false; // 编辑器中停止播放
            // #endif
        }
    }
}