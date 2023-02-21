using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;


[Serializable]
public static class FileManager
{
	public static void Save<T>(T obj) where T: ILoadable
	{
		Type t = typeof(T);

		string savePath = Path.Combine(Application.persistentDataPath, t.FullName);

		savePath += ".json";

		var file = JsonConvert.SerializeObject(obj);

		Debug.Log("Saved file on " + savePath);

		File.WriteAllText(savePath, file);
	}

	public static T Load<T>() where T : ILoadable
	{
		Type t = typeof(T);

		string savePath = Path.Combine(Application.persistentDataPath, t.FullName);

		savePath += ".json";
		T obj = default;

		try
		{
			var file = File.ReadAllText(savePath);
			obj = JsonConvert.DeserializeObject<T>(file);
		}
        catch (FileNotFoundException)
		{
			obj.Initialize();
		}

		return obj;
	}
}
