using UnityEngine;
using System;

/// <summary>
/// 负责管理核心游戏状态数据（如金钱、周数、通缉等级），
/// 提供单例访问，并在场景切换时保持存在。
/// 数据更新逻辑由专门的管理器处理。
/// </summary>
public class GameManager : MonoBehaviour
{
    // --- 单例模式 ---
    public static GameManager Instance { get; private set; }

    // --- 核心游戏状态数据 ---
    [Header("Game State Data (Initial Values)")]
    // 这些值将在 Inspector 中设置，并作为 ResetForNewGame 的目标值
    [SerializeField] private int initialBalance = 1000;
    [SerializeField] private int initialWeek = 1;
    [SerializeField] private int initialWantedLevel = 0;

    // --- 当前游戏状态属性 ---
    // 这些属性反映游戏过程中的实时状态
    // 在这里添加 [field: SerializeField]
    [field: SerializeField] public int CurrentBalance { get; private set; }
    // CurrentWeek 已经添加了
    [field: SerializeField] public int CurrentWeek { get; private set; }
    // 在这里添加 [field: SerializeField]
    [field: SerializeField] public int CurrentWantedLevel { get; private set; }


    // --- 事件 ---
    public event Action OnBalanceChanged;
    public event Action OnWeekChanged;
    public event Action OnWantedLevelChanged;

    // --- Unity 生命周期方法 ---
    void Awake()
    {
        // 实现单例模式
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("场景中已存在 GameManager 实例，销毁当前重复的实例。");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // --- 重要：在第一次 Awake 时，将 Inspector 设置的初始值赋给当前状态 ---
        // 这样游戏开始时，以及每次 Reset 时，都会使用 Inspector 的值
        CurrentBalance = initialBalance; // 仍然使用 initialBalance 初始化
        CurrentWeek = initialWeek;       // 仍然使用 initialWeek 初始化
        CurrentWantedLevel = initialWantedLevel; // 仍然使用 initialWantedLevel 初始化

        Debug.Log($"[GameManager] Instance initialized. Initial values - Balance: {CurrentBalance}, Week: {CurrentWeek}, WantedLevel: {CurrentWantedLevel}");
    }

    // --- 内部数据更新方法 ---
    internal void UpdateBalance(int newBalance)
    {
        if (newBalance != CurrentBalance)
        {
            CurrentBalance = newBalance;
            Debug.Log($"[GameManager] Balance updated to: {CurrentBalance}");
            OnBalanceChanged?.Invoke();
        }
    }

    internal void UpdateWeek(int newWeek)
    {
        if (newWeek != CurrentWeek)
        {
            CurrentWeek = newWeek;
            Debug.Log($"[GameManager] Week updated to: {CurrentWeek}");
            OnWeekChanged?.Invoke();
        }
    }

    internal void UpdateWantedLevel(int newLevel)
    {
        // 可以在这里或调用方加入等级限制逻辑
        if (newLevel != CurrentWantedLevel)
        {
            CurrentWantedLevel = newLevel;
            Debug.Log($"[GameManager] Wanted Level updated to: {CurrentWantedLevel}");
            OnWantedLevelChanged?.Invoke();
        }
    }

    // --- 公共方法 ---
    /// <summary>
    /// 将游戏状态重置为 Inspector 中配置的初始值。
    /// </summary>
    public void ResetForNewGame()
    {
        Debug.Log($"[GameManager] Resetting game state to initial values - Balance: {initialBalance}, Week: {initialWeek}, WantedLevel: {initialWantedLevel}");
        // 使用 Awake 时捕获的 Inspector 初始值进行重置
        UpdateBalance(initialBalance);
        UpdateWeek(initialWeek);
        UpdateWantedLevel(initialWantedLevel);
    }
}