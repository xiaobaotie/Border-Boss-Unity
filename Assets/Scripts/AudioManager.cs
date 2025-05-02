using UnityEngine;
using UnityEngine.SceneManagement; // 如果需要根据场景切换音乐等

public class AudioManager : MonoBehaviour
{
    // --- 单例模式 ---
    public static AudioManager Instance { get; private set; }

    // --- 音频源引用 ---
    // 你可以根据需要添加更多 AudioSource，例如一个用于背景音乐，一个用于特效
    [Header("Audio Sources")]
    public AudioSource sfxSource; // 用于播放短音效 (Sound Effects)
    public AudioSource musicSource; // 用于播放背景音乐

    // --- 音效片段引用 (在 Inspector 中拖拽) ---
    [Header("Audio Clips")]
    public AudioClip buttonClickSound;
    public AudioClip otherSoundEffect; // 添加更多你需要的音效
    // public AudioClip backgroundMusicMainMenu;
    // public AudioClip backgroundMusicGame;

    void Awake()
    {
        // --- 实现单例模式 ---
        if (Instance == null)
        {
            Instance = this;
            // --- 让 AudioManager 在场景切换时不被销毁 ---
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            // 如果已存在实例，销毁这个重复的
            Destroy(gameObject);
            return; // 确保后续代码不执行
        }

        // --- 可以在这里添加场景加载事件监听，用于切换背景音乐等 ---
        // SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // --- 公开的播放方法 ---

    public void PlayButtonClickSound()
    {
        if (sfxSource != null && buttonClickSound != null)
        {
            // PlayOneShot 允许在同一个 AudioSource 上叠加播放多个短音效，不会打断之前的
            sfxSource.PlayOneShot(buttonClickSound);
            // 如果希望按钮音效有特定音量，可以这样：
            // sfxSource.PlayOneShot(buttonClickSound, 0.8f); // 0.8f 是音量比例
        }
        else
        {
            Debug.LogWarning("SFX Source or Button Click Sound not set in AudioManager!");
        }
    }

    public void PlaySoundEffect(AudioClip clip, float volume = 1.0f)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
        else
        {
             Debug.LogWarning("SFX Source or provided clip is null!");
        }
    }

    // --- 可以添加播放背景音乐的方法 ---
    /*
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        {
            if (musicSource.clip == clip && musicSource.isPlaying)
            {
                return; // 已经在播放同一首音乐
            }
            musicSource.clip = clip;
            musicSource.loop = true; // 背景音乐通常循环
            musicSource.Play();
        }
         else
        {
             Debug.LogWarning("Music Source or provided clip is null!");
        }
    }

    // --- 示例：根据场景切换音乐 ---
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            // PlayMusic(backgroundMusicMainMenu);
        }
        else if (scene.name == "USMarketScene") // 假设你的游戏场景叫这个
        {
            // PlayMusic(backgroundMusicGame);
        }
    }

    void OnDestroy()
    {
        // 移除事件监听，防止内存泄漏
        // SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    */
}