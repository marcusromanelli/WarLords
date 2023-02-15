using System;
using System.IO;
using UnityEngine;


[Serializable]
public abstract class LoadableObject<T> where T : new()
{
	public void Write()
	{
		Type t = typeof(T);

		string savePath = Path.Combine(Application.persistentDataPath, t.FullName);

		savePath += ".json";

		var file = JsonUtility.ToJson(this);

		File.WriteAllText(savePath, file);
	}

	public static T Load()
	{
		Type t = typeof(T);

		string savePath = Path.Combine(Application.persistentDataPath, t.FullName);

		savePath += ".json";

		string file = "";
		T obj;

		try
		{
			file = File.ReadAllText(savePath);
			obj = JsonUtility.FromJson<T>(file);
		}
        catch (FileNotFoundException)
		{
			obj = new T();
		}

		return obj;
	}
}
