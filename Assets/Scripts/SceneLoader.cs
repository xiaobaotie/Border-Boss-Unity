using UnityEngine;
using UnityEngine.SceneManagement; // 引入场景管理命名空间

public class SceneLoader : MonoBehaviour
{
    // 公共方法，用于根据场景名称加载场景
    // 可以被按钮的 OnClick 事件调用
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name cannot be empty!");
            return;
        }

        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
        // 注意：确保名为 sceneName 的场景已添加到 Build Settings 中
    }

    // (可选) 如果你想通过场景索引加载
    // public void LoadSceneByIndex(int sceneIndex)
    // {
    //     if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
    //     {
    //         Debug.LogError($"Invalid scene index: {sceneIndex}");
    //         return;
    //     }
    //     Debug.Log($"Loading scene with index: {sceneIndex}");
    //     SceneManager.LoadScene(sceneIndex);
    // }

    // (可选) 退出游戏的方法
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        #if UNITY_EDITOR
        // 在编辑器模式下停止播放
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}