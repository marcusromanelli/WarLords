using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MouseManager
{
    Dictionary<GameObject, ObjectLayer> objectsToWatch;

    [SerializeField] float minMouseMovement = 1f;
#if UNITY_EDITOR
    [BoxGroup("Debug"), SerializeField] bool enableDebugMode = false;
    [BoxGroup("Debug"), SerializeField] bool ignoreLayerMask = false;
    [BoxGroup("Debug"), ReorderableList] GameObject[] lastRecordedHitObjects;
#endif
    [BoxGroup("Debug")] public int registeredLayerMask;
    private Vector3 lastRecordedMousePosition;
    private Ray lastRaycast;

    public MouseManager()
    {
        objectsToWatch = new Dictionary<GameObject, ObjectLayer>();
    }

    public void CheckHoverage()
    {
        CheckClick();

        if (!MouseMoved())
            return;

        UpdateRaycastCollisions();
    }
    void CheckClick()
    {
        foreach (var obj in objectsToWatch)
        {
            obj.Value.SetClick(Input.GetMouseButton(0));
        }
    }
    bool MouseMoved()
    {
        var currentPosition = Input.mousePosition;

        if (!enableDebugMode)
        {
            if (currentPosition == lastRecordedMousePosition)
                return false;


            var delta = Mathf.Abs((lastRecordedMousePosition - currentPosition).magnitude);

            if (delta < minMouseMovement)
                return false;
        }

        lastRecordedMousePosition = currentPosition;

        return true;
    }
    [Button("Force Update Layer")]
    void UpdateLayers()
    {
        registeredLayerMask = 0;

        LayerMask aux;
        Dictionary<string, int> layers = new Dictionary<string, int>();

        foreach (var obj in objectsToWatch)
        {
            string layerName = LayerMask.LayerToName(obj.Key.layer);

            if (!layers.ContainsKey(layerName))
                layers.Add(layerName, 0);

            layers[layerName]++;
        }

        string[] foundLayers = new string[layers.Count];
        int c = 0;
        foreach (var layer in layers)
        {
            foundLayers[c++] = layer.Key;
        }

        registeredLayerMask = LayerMask.GetMask(foundLayers);
    }
    void UpdateRaycastCollisions()
    {
        lastRaycast = Camera.main.ScreenPointToRay(lastRecordedMousePosition);

        RaycastHit[] results;

        if (!ignoreLayerMask)
            results = Physics.RaycastAll(lastRaycast, 1000, registeredLayerMask);
        else
            results = Physics.RaycastAll(lastRaycast, 1000);


#if UNITY_EDITOR
        lastRecordedHitObjects = new GameObject[results.Length];
#endif

        int c = 0;

        foreach (var obj in objectsToWatch)
        {
            bool isHovering = false;

            foreach (var result in results)
            {
                var gameObject = result.collider.gameObject;

                if (gameObject == obj.Key)
                {
                    isHovering = true;

#if UNITY_EDITOR
                    lastRecordedHitObjects[c++] = gameObject;
#endif
                    break;
                }
            }

            obj.Value.SetHovering(isHovering);
        }
    }
    public void RegisterCallback(MouseEventType type, GameObject gameObject, HandleMouseAction onHoverAction)
    {
        if (!objectsToWatch.ContainsKey(gameObject))
        {
            objectsToWatch.Add(gameObject, new ObjectLayer(gameObject));
            UpdateLayers();
        }

        objectsToWatch[gameObject].Register(type, onHoverAction);
    }
    public void UnregisterCallback(MouseEventType type, GameObject gameObject, HandleMouseAction onHoverAction)
    {
        if (!objectsToWatch.ContainsKey(gameObject))
            return;

        objectsToWatch[gameObject].Unregister(type, onHoverAction);
    }
}
