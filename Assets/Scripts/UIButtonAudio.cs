using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonAudio : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        // 不再调用 RemoveAllListeners()
        // button.onClick.RemoveAllListeners();

        // 直接将播放声音的方法添加到按钮现有的 OnClick 事件列表中
        button.onClick.AddListener(PlayClickSound);
    }

    // 这个方法只负责播放声音
    void PlayClickSound()
    {
        if (AudioManager.Instance != null)
        {
            // 调用 AudioManager 播放按钮点击音效
            AudioManager.Instance.PlayButtonClickSound();
        }
        else
        {
            Debug.LogError("AudioManager instance not found when trying to play button sound!");
        }
    }

    // 不再需要 additionalOnClick 和 HandleClick 方法
}