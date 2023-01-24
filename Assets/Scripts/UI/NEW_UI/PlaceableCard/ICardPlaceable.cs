using UnityEngine;

public interface ICardPlaceable
{    
    public Vector3 GetTopCardPosition();
    public Quaternion GetRotationReference();
}