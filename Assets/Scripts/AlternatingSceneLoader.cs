using UnityEngine;
using UnityEngine.SceneManagement;

public class AlternatingSceneLoader : MonoBehaviour
{
    // 用于记录上一次加载的场景名 (使用静态变量以跨场景实例保持状态)
    private static string lastSceneLoaded = null;

    // --- 恢复这两个公共字段 ---
    [Tooltip("场景 A 的名称")]
    public string sceneA_Name;
    [Tooltip("场景 B 的名称")]
    public string sceneB_Name;
    // -------------------------

    // 公共方法，用于在 Scene A 和 Scene B 之间交替加载
    // 改回没有参数的方法
    public void LoadNextSceneAlternating() // <--- 移除参数
    {
        string targetScene;

        // 检查输入是否有效
        if (string.IsNullOrEmpty(sceneA_Name) || string.IsNullOrEmpty(sceneB_Name)) // <--- 使用字段检查
        {
            Debug.LogError("Scene A or Scene B name is not set in AlternatingSceneLoader!", this);
            return;
        }

        // 决定目标场景
        if (string.IsNullOrEmpty(lastSceneLoaded) || lastSceneLoaded == sceneB_Name) // <--- 使用字段比较
        {
            // 如果上次加载的是 B (或者是第一次加载)，则这次加载 A
            targetScene = sceneA_Name; // <--- 使用字段赋值
        }
        else // lastSceneLoaded == sceneA_Name
        {
            // 如果上次加载的是 A，则这次加载 B
            targetScene = sceneB_Name; // <--- 使用字段赋值
        }

        // 记录本次加载的场景
        lastSceneLoaded = targetScene;

        // 执行加载
        Debug.Log($"Alternating load: Loading scene: {targetScene}");
        SceneManager.LoadScene(targetScene);
        // 确保 targetScene 已添加到 Build Settings
    }
}