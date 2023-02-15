public class UserManager : Singleton<UserManager>
{
    private User userData;
    private bool initialized;

    public static void SetData(User user)
    {
        Instance.setData(user);
    }
    void setData(User user)
    {
        if (initialized)
            return;

        this.userData = user;
        initialized = true;
    }
    public static User GetData()
    {
        return Instance.getData();
    }
    User getData()
    {
        return userData;
    }
}
