using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;


[Serializable]
public abstract class LoadableObject<T> where T : new()
{
	public void Save()
	{
		Type t = typeof(T);

		string savePath = Path.Combine(Application.persistentDataPath, t.FullName);

		savePath += ".json";

		var file = JsonConvert.SerializeObject(this);

		Debug.Log("Saved file on " + savePath);

		File.WriteAllText(savePath, file);
	}

	public static T Load()
	{
		Type t = typeof(T);

		string savePath = Path.Combine(Application.persistentDataPath, t.FullName);

		savePath += ".json";
		T obj = new T();

		try
		{
			var file = File.ReadAllText(savePath);
			obj = JsonConvert.DeserializeAnonymousType(file, obj);
		}
        catch (FileNotFoundException)
		{
		}

		return obj;
	}
}
