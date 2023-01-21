using UnityEngine;

public interface ICardPlaceable
{
    public void CheckMouseOver(bool requiresClick);
    public Vector3 GetTopPosition();
    public Quaternion GetTopRotation();
}
