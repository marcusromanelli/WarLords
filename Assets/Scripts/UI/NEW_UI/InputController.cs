using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void HandleHoverObject(GameObject obj);

public class InputController : MonoBehaviour
{
    public enum HoverType
    {
        Start, End
    }
    [Serializable]
    class ObjectLayer
    {
        public GameObject @object;
        public HandleHoverObject OnHoverStart;
        public HandleHoverObject OnHoverEnd;
        [ReadOnly] public bool IsHovering;

        public ObjectLayer(GameObject gameObject)
        {
            @object = gameObject;
            OnHoverEnd = null;
            OnHoverStart = null;
            IsHovering = false;
        }
        public void SetHovering(bool value)
        {
            if (IsHovering && !value)
            {
                //Stopped
                OnHoverEnd?.Invoke(@object);
            }else if(!IsHovering && value)
            {
                //Started
                OnHoverStart?.Invoke(@object);
            }

            IsHovering = value;
        }
        public void Register(HoverType type, HandleHoverObject evnt)
        {
            switch (type)
            {
                case HoverType.Start:
                    OnHoverStart += evnt;
                    break;
                case HoverType.End:
                    OnHoverEnd += evnt;
                    break;
            }
        }
        public void Unregister(HoverType type, HandleHoverObject evnt)
        {
            switch (type)
            {
                case HoverType.Start:
                    OnHoverStart -= evnt;
                    break;
                case HoverType.End:
                    OnHoverEnd -= evnt;
                    break;
            }
        }
    }
    Dictionary<GameObject, ObjectLayer> objectsToWatch;


    [SerializeField] float minMouseMovement = 1f;
#if UNITY_EDITOR
    [BoxGroup("Debug"), SerializeField] bool enableDebugMode = false;
    [BoxGroup("Debug"), SerializeField] bool ignoreLayerMask = false;
    [BoxGroup("Debug"), SerializeField, ReadOnly] GameObject[] lastRecordedHitObjects;
#endif
    [BoxGroup("Debug")] public int registeredLayerMask;
    private Vector3 lastRecordedMousePosition;    
    private Ray lastRaycast;

    private void Awake()
    {
        objectsToWatch = new Dictionary<GameObject, ObjectLayer>();
    }
    private void Update()
    {
        CheckHoverage();
    }
    void CheckHoverage()
    {
        if (!MouseMoved())
            return;

        UpdateRaycastCollisions();
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

            if(!layers.ContainsKey(layerName))
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

        if(!ignoreLayerMask)
            results = Physics.RaycastAll(lastRaycast, 1000, registeredLayerMask);
        else
            results = Physics.RaycastAll(lastRaycast, 1000);


#if UNITY_EDITOR
        lastRecordedHitObjects = new GameObject[results.Length];
#endif
        /*
        foreach (var result in results)
        {
            var gameObject = result.collider.gameObject;
#if UNITY_EDITOR
            lastRecordedHitObjects[c++] = gameObject;
#endif
            if(objectsToWatch.ContainsKey(gameObject))
                objectsToWatch[gameObject].SetHovering(true);
        }*/
        int c = 0;

        foreach (var obj in objectsToWatch)
        {
            bool isHovering = false;

            foreach(var result in results)
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
    public void RegisterCallback(HoverType type, GameObject gameObject, HandleHoverObject onHoverAction)
    {
        if (!objectsToWatch.ContainsKey(gameObject))
        {
            objectsToWatch.Add(gameObject, new ObjectLayer(gameObject));
            UpdateLayers();
        }

        objectsToWatch[gameObject].Register(type, onHoverAction);
    }
    public void UnregisterCallback(HoverType type, GameObject gameObject, HandleHoverObject onHoverAction)
    {
        if (!objectsToWatch.ContainsKey(gameObject))
            return;

        objectsToWatch[gameObject].Unregister(type, onHoverAction);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawRay(lastRaycast);        
        Gizmos.DrawRay(Camera.main.transform.position, lastRaycast.direction * 1000);
    }
}
