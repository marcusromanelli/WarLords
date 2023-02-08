using UnityEngine;

public delegate void HandleMouseAction(GameObject obj);

public class InputController : MonoBehaviour
{
    [SerializeField] MouseManager mouseTargetManager;
    public Vector3 MousePosition => mouseTargetManager.MousePosition;

    private void Awake()
    {
        mouseTargetManager = new MouseManager();
    }
    private void Update()
    {
        mouseTargetManager.CheckHoverage();
    }
    public void Lock()
    {
        ToggleLock(true);
    }
    public void Unlock()
    {
        ToggleLock(false);
    }
    void ToggleLock(bool value)
    {
        mouseTargetManager.ToggleLock(value);
    }
    public void RegisterTargetCallback(MouseEventType type, GameObject gameObject, HandleMouseAction onAction)
    {
        mouseTargetManager.RegisterCallback(type, gameObject, onAction);
    }
    public void UnregisterTargetCallback(MouseEventType type, GameObject gameObject, HandleMouseAction onAction)
    {
        mouseTargetManager.UnregisterCallback(type, gameObject, onAction);
    }
}
