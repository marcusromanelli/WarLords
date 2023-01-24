using System;
using UnityEngine;

[Serializable]
public struct CardObjectData
{
    public Vector3 Position;
    public Quaternion Rotation => Quaternion.Euler(EulerRotation);
    public Vector3 EulerRotation;

    public static CardObjectData Create(Vector3 position, Quaternion rotation)
    {
        CardObjectData cardObjectData = new CardObjectData();
        cardObjectData.Position = position;
        cardObjectData.EulerRotation = rotation.eulerAngles;

        return cardObjectData;
    }
    public static CardObjectData Create(Vector3 position, Vector3 rotation)
    {
        CardObjectData cardObjectData = new CardObjectData();
        cardObjectData.Position = position;
        cardObjectData.EulerRotation = rotation;

        return cardObjectData;
    }
}
