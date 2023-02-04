using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class ObjectInteraction
{
    public GameObject @object;
    public Dictionary<MouseEventType, HandleMouseAction>  Events;
    [ReadOnly] public bool IsHovering;
    [ReadOnly] public bool IsCurrentlyClicking;
    [ReadOnly] public bool IsDragging;

    public ObjectInteraction(GameObject gameObject)
    {
        @object = gameObject;
        Events = new Dictionary<MouseEventType, HandleMouseAction>();
        IsHovering = false;
    }
    public void SetMousePosition(Vector3 mousePosition)
    {
        if (!IsHovering || !IsCurrentlyClicking || IsDragging)
            return;

        IsDragging = true;
        TriggerEventCallback(MouseEventType.LeftMouseDragStart);
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

        if(IsHovering)
            TriggerEventCallback(MouseEventType.Hover);
    }
    public void SetClick(bool clickStatus)
    {
        var hasClicked = clickStatus == true;
        var hasReleased = clickStatus == false;

        if (IsHovering)
            if(!IsCurrentlyClicking && hasClicked)
            {
                TriggerEventCallback(MouseEventType.LeftMouseButtonDown);
            }else if(IsCurrentlyClicking && hasReleased)
            {
                TriggerEventCallback(MouseEventType.LeftMouseButtonUp);
            }


        if(IsCurrentlyClicking && hasReleased && IsDragging)
        {
            IsDragging = false;
            TriggerEventCallback(MouseEventType.LeftMouseDragEnd);
        }

        IsCurrentlyClicking = hasClicked;

        if(IsCurrentlyClicking)
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