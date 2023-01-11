using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Xml.Serialization;


[Serializable]
public abstract class ISaveable{
	public void Write<T>(bool saveOutsideBuild = true) where T: ISaveable{

		Debug.Log ("Coisas salvas: "+((saveOutsideBuild==true)?"FORA":"Ok"));
		Type t = typeof (T);
		var serializer = new XmlSerializer(t);
		string path=Path.Combine(Application.persistentDataPath, t.FullName);
		string path2=Path.Combine(Application.dataPath+"/resources/XML/", t.FullName);
		var appendMode = false;

		/*persistentDataPath para salvar saves
        /Application.dataPath para salvar no projeto*/
		//Debug.Log(t.FullName+" salvo");
		var encoding = Encoding.GetEncoding("UTF-8");	
		path+=".xml";
		path2+=".xml";
		if (saveOutsideBuild) {
			using (StreamWriter sw = new StreamWriter (path, appendMode, encoding)) {
				serializer.Serialize (sw, this);
				sw.Close ();
			}					
		}
		if(Application.isEditor){
			using(StreamWriter sw = new StreamWriter(path2, appendMode, encoding)){
				serializer.Serialize(sw, this);
				sw.Close();
			}
		}
	}

	public static T Load<T>(bool loadFromBuild = false) where T: ISaveable{
		Type t = typeof (T);
		var serializer = new XmlSerializer(t);
		T aux;
		string final="";




		//Debug.Log(t.FullName+" carregado");

		StringBuilder sb = new StringBuilder();
		string path = Path.Combine (Application.persistentDataPath, t.FullName+".xml");

		TextAsset tx = Resources.Load<TextAsset>("XML/"+t.FullName);
		if (loadFromBuild) {
			final = tx.text;
		}else{
			if (File.Exists (path)) {
				if (tx != null) {
					using (FileStream fs = File.Create (path)) {
						Byte[] info = new UTF8Encoding (true).GetBytes (tx.text);
						fs.Write (info, 0, info.Length);
					}
				} else {
					Debug.LogError ("Faltando arquivo!");	
				}
			}

			try {
				using (StreamReader sr = new StreamReader (path, Encoding.UTF8)) {
					String line;
					while ((line = sr.ReadLine ()) != null) {
						sb.AppendLine (line);
					}
				}
			} catch (IOException) {
				Debug.LogWarning ("Erro de leitura de " + t.FullName);
				return null;
			}
			final = sb.ToString ();
		}

		using (var stream = new System.IO.StringReader(final)) {
			aux = (T)serializer.Deserialize (stream);
		}


		return aux;		
	}
}
