public class UserManager : Singleton<UserManager>
{
    private User userData;

    public static void Initialize()
    {
        Instance.initialize();
    }
    void initialize()
    {
        userData = FileManager.Load<User>();
    }


    public static void UpdateData(User user)
    {
        Instance.updateData(user);
    }
    void updateData(User user)
    {
        userData = user;

        FileManager.Save(userData);
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
