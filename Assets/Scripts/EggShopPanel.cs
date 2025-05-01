using UnityEngine;
using UnityEngine.UI; // 引入 UI 命名空间

public class EggShopPanel : MonoBehaviour
{
    [Header("UI References")]
    // 在 Inspector 中将你的关闭按钮拖拽到这里
    public Button closeButton;

    void Start()
    {
        // 确保关闭按钮已设置并添加监听器
        if (closeButton != null)
        {
            // 当按钮被点击时，调用 ClosePanel 方法
            closeButton.onClick.AddListener(ClosePanel);
        }
        else
        {
            Debug.LogError("Close Button 没有在 EggShopPanel 的 Inspector 中设置!", this);
        }

        // 确保面板初始是隐藏的（虽然 ClickableObject 也设置了，双重保险）
        // 如果你的 ClickableObject 脚本已经负责在 Start 时隐藏它，这行可以注释掉
        // gameObject.SetActive(false);
    }

    // 关闭按钮调用的方法
    public void ClosePanel()
    {
        Debug.Log("关闭鸡蛋商店面板...");
        gameObject.SetActive(false); // 隐藏此面板 GameObject
    }

    // （可选）你可以在这里添加一个公共方法，供 ClickableObject 调用来打开面板
    // public void OpenPanel()
    // {
    //     Debug.Log("打开鸡蛋商店面板...");
    //     gameObject.SetActive(true);
    // }
}