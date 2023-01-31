using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Linq;

[Serializable]
public class MacroCollection : ISaveable {

	/*public Macro[] Macros;

	public static MacroCollection Load(bool loadFromBuild = false){
		MacroCollection aux=MacroCollection.Load<MacroCollection>(loadFromBuild);

		return aux; 
	}
	public void Save(bool saveOutsideBuild = true){
		base.Write<MacroCollection>(saveOutsideBuild);
	}
	public Macro findMacro(MacroType type){
		return Macros.ToList ().Find (a => a.MacroType == type);
	}*/
}
