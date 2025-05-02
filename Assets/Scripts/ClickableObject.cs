using UnityEngine;

// 确保 GameObject 上有 SpriteRenderer 组件
[RequireComponent(typeof(SpriteRenderer))]
public class ClickableObject : MonoBehaviour
{
    // 在 Inspector 中将你的“发光”状态 Sprite 拖拽到这里
    public Sprite glowSprite;
    // 在 Inspector 中将你的鸡蛋商店 UI Panel GameObject 拖拽到这里
    public GameObject eggShopPanel; // 引用商店面板

    private Sprite normalSprite; // 用于存储原始的 Sprite
    private SpriteRenderer spriteRenderer; // 缓存 SpriteRenderer 组件

    private bool isMouseDownOnObject = false; // 标记鼠标是否在此对象上按下

    void Start()
    {
        // 获取 SpriteRenderer 组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 存储初始的（普通状态）Sprite
        if (spriteRenderer != null)
        {
            normalSprite = spriteRenderer.sprite;
        }
        else
        {
            Debug.LogError("ClickableObject 需要一个 SpriteRenderer 组件!", this);
        }

        // 确保 glowSprite 已设置
        if (glowSprite == null)
        {
             Debug.LogWarning("Glow Sprite 没有在 Inspector 中设置!", this);
        }

        // 确保 eggShopPanel 已设置
        if (eggShopPanel == null)
        {
            Debug.LogError("Egg Shop Panel 没有在 Inspector 中设置!", this);
        }
        else
        {
            // 确保商店面板初始是隐藏的
            eggShopPanel.SetActive(false);
        }
    }

    // 鼠标左键在此对象上按下时调用
    void OnMouseDown()
    {
        isMouseDownOnObject = true;
        // 切换到发光 Sprite
        if (spriteRenderer != null && glowSprite != null)
        {
            spriteRenderer.sprite = glowSprite;
        }

        // --- 打开商店面板 ---
        OpenShopPanel();
    }

    void OpenShopPanel()
    {
        if (eggShopPanel != null)
        {
            Debug.Log("打开鸡蛋商店面板...");
            eggShopPanel.SetActive(true);
            // 你可能还需要在这里调用面板脚本的方法来初始化显示内容
            // EggShopPanel panelScript = eggShopPanel.GetComponent<EggShopPanel>();
            // if (panelScript != null)
            // {
            //     panelScript.InitializeShop(); // 假设有这样一个方法
            // }
        }
        else
        {
            Debug.LogError("无法打开商店：商店面板未设置！");
        }
    }

    // 鼠标左键在此对象上抬起时调用
    void OnMouseUp()
    {
        if (isMouseDownOnObject) // 只有在此对象上按下的才处理抬起
        {
             isMouseDownOnObject = false;
             // 切换回普通 Sprite
             if (spriteRenderer != null)
             {
                 spriteRenderer.sprite = normalSprite;
             }
        }
    }

    // 鼠标指针进入 Collider 范围时调用
    void OnMouseEnter()
    {
        // 可选：如果需要悬停效果，可以在这里处理
    }

    // 鼠标指针离开 Collider 范围时调用
    void OnMouseExit()
    {
        // 如果鼠标在按下的状态下离开，也恢复普通 Sprite
        if (isMouseDownOnObject)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = normalSprite;
            }
            // 注意：这里不重置 isMouseDownOnObject，因为鼠标仍然是按下的，
            // 只是不在这个对象上了。重置操作留给 OnMouseUp。
        }
        // 可选：恢复悬停效果
    }
}