using System;

[Serializable]
public class UserDataLoader : ILoadableComponentBaseClass
{
    public override bool HasLoaded()
    {
        return true;
    }
    public override void Load()
    {
        UserManager.Initialize();
    }
}
