using System;
using UnityEngine;

[Serializable]
public struct CardPositionData
{
    public Vector3 Position;
    public Quaternion Rotation => Quaternion.Euler(EulerRotation);
    public Vector3 EulerRotation;

    public static CardPositionData Create(Vector3 position, Quaternion rotation)
    {
        CardPositionData cardObjectData = new CardPositionData();
        cardObjectData.Position = position;
        cardObjectData.EulerRotation = rotation.eulerAngles;

        return cardObjectData;
    }
    public static CardPositionData Create(Vector3 position, Vector3 rotation)
    {
        CardPositionData cardObjectData = new CardPositionData();
        cardObjectData.Position = position;
        cardObjectData.EulerRotation = rotation;

        return cardObjectData;
    }
}
