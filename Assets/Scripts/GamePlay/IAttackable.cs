using System;

public interface IAttackable
{
    public void TakeDamage(uint damage);
    public void Heal(uint health);
    public uint GetLife();
    public string GetName();
}