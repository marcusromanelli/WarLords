using UnityEngine;

public interface ICardPlaceable
{    
    public UIDeckType GetDeckType();
    public Vector3 GetTopCardPosition();
    public Quaternion GetRotationReference();
}