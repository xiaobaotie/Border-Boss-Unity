using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class NewGameButton : MonoBehaviour
{
    [Header("游戏设置")]
    [SerializeField]
    [Tooltip("要实例化的 GameManager Prefab")]
    private GameObject gameManagerPrefab; // 在 Inspector 中拖入 GameManager Prefab

    [SerializeField]
    [Tooltip("开始新游戏时要加载的第一个场景名称")]
    private string firstGameSceneName = "Week1Scene"; // 设置你的第一个游戏场景名

    private Button newGameButton;

    void Awake()
    {
        newGameButton = GetComponent<Button>();
        newGameButton.onClick.AddListener(OnNewGameClicked);

        // 检查 Prefab 是否已设置
        if (gameManagerPrefab == null)
        {
            Debug.LogError("[NewGameButton] GameManager Prefab is not assigned in the Inspector!", this);
            newGameButton.interactable = false; // 禁用按钮，防止出错
        }
         // 检查场景名是否已设置
        if (string.IsNullOrEmpty(firstGameSceneName))
        {
             Debug.LogError("[NewGameButton] First Game Scene Name is not set in the Inspector!", this);
             newGameButton.interactable = false; // 禁用按钮
        }
    }

    void OnDestroy()
    {
        if (newGameButton != null)
        {
            newGameButton.onClick.RemoveListener(OnNewGameClicked);
        }
    }

    public void OnNewGameClicked()
    {
        // 检查 GameManager Prefab 是否已分配
        if (gameManagerPrefab == null)
        {
             Debug.LogError("[NewGameButton] Cannot start new game because GameManager Prefab is not assigned!", this);
             return;
        }
        // 检查场景名是否已分配
        if (string.IsNullOrEmpty(firstGameSceneName))
        {
             Debug.LogError("[NewGameButton] Cannot start new game because First Game Scene Name is not set!", this);
             return;
        }


        // 检查是否已经存在 GameManager 实例
        if (GameManager.Instance == null)
        {
            // 不存在，实例化 Prefab
            Debug.Log("[NewGameButton] GameManager instance not found. Instantiating GameManager Prefab.");
            Instantiate(gameManagerPrefab);
            // 注意：GameManager 的 Awake 方法应该包含 DontDestroyOnLoad(gameObject);
            // 并且 GameManager 应该能自己处理好单例模式 (Instance = this;)
        }
        else
        {
            // 已存在 (可能从游戏场景返回主菜单)，重置其状态
            Debug.Log("[NewGameButton] GameManager instance already exists. Resetting its state for a new game.");
            GameManager.Instance.ResetForNewGame(); // 假设 GameManager 有此方法重置数据
        }

        // 确保 GameManager 已经初始化完成（可能需要等待一帧，但通常 Awake 会立即执行）
        // 如果遇到问题，可以考虑用协程延迟加载场景

        // 加载第一个游戏场景
        Debug.Log($"[NewGameButton] Loading first game scene: {firstGameSceneName}");
        SceneManager.LoadScene(firstGameSceneName);

        // (可选) 更新 GameManager 的逻辑场景名
        // 如果 GameManager 初始化和场景加载之间有延迟，这可能需要在场景加载完成后做
        // 或者在 GameManager 的 ResetForNewGame 或 Awake 中设置初始逻辑场景
        // if (GameManager.Instance != null)
        // {
        //     GameManager.Instance.UpdateLogicalSceneName(firstGameSceneName);
        // }
    }
}