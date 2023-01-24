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
    public void RegisterTargetCallback(MouseEventType type, GameObject gameObject, HandleMouseAction onHoverAction)
    {
        mouseTargetManager.RegisterCallback(type, gameObject, onHoverAction);
    }
    public void UnregisterTargetCallback(MouseEventType type, GameObject gameObject, HandleMouseAction onHoverAction)
    {
        mouseTargetManager.UnregisterCallback(type, gameObject, onHoverAction);
    }
}
