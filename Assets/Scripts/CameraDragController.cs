using UnityEngine;
using UnityEngine.EventSystems; // 引入事件系统命名空间

public class CameraDragController : MonoBehaviour
{
    [Header("拖动设置")]
    [SerializeField] private float dragSpeed = 0.01f; // 拖动速度/灵敏度，根据需要调整
    [SerializeField] private float minX = -10f;      // 相机可移动的最小 X 坐标
    [SerializeField] private float maxX = 10f;       // 相机可移动的最大 X 坐标

    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 startMousePosition;
    private Vector3 startCameraPosition;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("CameraDragController requires a Camera component on the same GameObject.", this);
            this.enabled = false; // 禁用脚本
        }
    }

    void Update()
    {
        // --- 检测拖动开始 ---
        // Input.GetMouseButtonDown(0) 检测鼠标左键按下的那一帧
        if (Input.GetMouseButtonDown(0))
        {
            // 检查鼠标指针是否在 UI 元素上
            // 对于 PC: EventSystem.current.IsPointerOverGameObject()
            // 对于触摸屏: 需要检查每个触摸点 EventSystem.current.IsPointerOverGameObject(touch.fingerId)
            // 这里我们先用 PC 的方式
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                // Debug.Log("Clicked on UI, ignoring drag start.");
                return; // 如果在 UI 上，则不开始拖动
            }

            // 记录拖动起始信息
            isDragging = true;
            startMousePosition = Input.mousePosition;
            startCameraPosition = mainCamera.transform.position;
            // Debug.Log("Drag Started at: " + startMousePosition);
        }

        // --- 检测拖动过程 ---
        // Input.GetMouseButton(0) 检测鼠标左键是否一直按着
        if (isDragging && Input.GetMouseButton(0))
        {
            // 计算鼠标在屏幕空间中的位移
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 mouseDelta = currentMousePosition - startMousePosition;

            // 计算相机应该移动的距离 (只考虑 X 轴)
            // 注意：鼠标的屏幕坐标 Y 轴向上，X 轴向右。我们希望鼠标向右拖动时，相机也向右移动。
            // 因此，鼠标 X 轴的增量对应相机 X 轴的增量。
            // dragSpeed 用于调整灵敏度
            float deltaX = -mouseDelta.x * dragSpeed; // 乘以负号是因为通常拖动方向与相机移动方向相反

            // 计算目标相机位置
            Vector3 targetPosition = startCameraPosition + new Vector3(deltaX, 0, 0);

            // 限制相机在定义的边界内移动
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            // Y 和 Z 坐标保持不变
            targetPosition.y = startCameraPosition.y;
            targetPosition.z = startCameraPosition.z;

            // 更新相机位置
            mainCamera.transform.position = targetPosition;
            // Debug.Log("Dragging... Delta: " + mouseDelta + ", New Cam Pos X: " + targetPosition.x);
        }

        // --- 检测拖动结束 ---
        // Input.GetMouseButtonUp(0) 检测鼠标左键抬起的那一帧
        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                isDragging = false;
                // Debug.Log("Drag Ended.");
            }
        }
    }
}
