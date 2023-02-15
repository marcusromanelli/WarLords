using System;

[Serializable]
public class UserDataLoader : ILoadableBaseClass
{
    public override bool HasLoaded()
    {
        return UserManager.GetData() != null;
    }
    public override void Load()
    {
        UserManager.SetData(User.Load());
    }
}
