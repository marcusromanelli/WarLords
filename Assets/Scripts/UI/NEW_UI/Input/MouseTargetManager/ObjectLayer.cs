using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class ObjectLayer
{
    public GameObject @object;
    public Dictionary<MouseEventType, HandleMouseAction>  Events;
    [ReadOnly] public bool IsHovering;
    [ReadOnly] public bool IsClicking;
    [ReadOnly] public bool IsDragging;
    [ReadOnly] public Vector3 lastMousePosition;

    public ObjectLayer(GameObject gameObject)
    {
        @object = gameObject;
        Events = new Dictionary<MouseEventType, HandleMouseAction>();
        IsHovering = false;
    }
    public void SetMousePosition(Vector3 mousePosition)
    {
        //When arrives here, we already know that the mouse move the minimum amount, so we just do the necessary logic

        if (IsHovering && IsClicking)
        {
            if (!IsDragging)
            {
                IsDragging = true;
                TriggerEventCallback(MouseEventType.LeftMouseDragStart);
            }
        }
        else
        {
            if (IsDragging)
            {
                IsDragging = false;
                TriggerEventCallback(MouseEventType.LeftMouseDragEnd);
            }
        }

    }
    public void SetHovering(bool hovering)
    {
        if (IsHovering && !hovering)
        {
            TriggerEventCallback(MouseEventType.EndHover);
        }
        else if (!IsHovering && hovering)
        {
            TriggerEventCallback(MouseEventType.StartHover);
        }

        IsHovering = hovering;

        if (!IsHovering)
            IsClicking = false;
    }
    public void SetClick(bool clicking)
    {
        if (!IsHovering)
            return;

        if (IsClicking && !clicking)
        {
            TriggerEventCallback(MouseEventType.LeftMouseButtonUp);
        }
        else if (!IsClicking && clicking)
        {
            TriggerEventCallback(MouseEventType.LeftMouseButtonDown);
        }

        IsClicking = clicking;

        if(IsClicking)
            TriggerEventCallback(MouseEventType.LeftMouseButton);
    }
    public void Register(MouseEventType type, HandleMouseAction @event)
    {
        AddTriggerCallback(type, @event);
    }
    public void Unregister(MouseEventType type, HandleMouseAction @event)
    {
        RemoveTriggerCallback(type, @event);
    }
    void RemoveTriggerCallback(MouseEventType type, HandleMouseAction @event)
    {
        var callback = GetTriggerCallback(type);

        callback -= @event;

        Events[type] = callback;
    }
    void AddTriggerCallback(MouseEventType type, HandleMouseAction @event)
    {
        var callback = GetTriggerCallback(type);

        callback += @event;

        Events[type] = callback;
    }
    HandleMouseAction GetTriggerCallback(MouseEventType type)
    {
        if(!HasEvent(type))
            Events.Add(type, null);

        return Events[type];
    }
    void TriggerEventCallback(MouseEventType type)
    {
        if (!HasEvent(type))
            return;

        Events[type]?.Invoke(@object);
    }
    bool HasEvent(MouseEventType @event)
    {
        return Events.ContainsKey(@event);
    }
}