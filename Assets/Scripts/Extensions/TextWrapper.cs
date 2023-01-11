using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh)), ExecuteInEditMode]
public class TextWrapper : MonoBehaviour
{
	TextMesh TheMesh;
	public string text;
	public float MaxWidth;
	public bool NeedsLayout = true;
	public bool ConvertNewLines = false;

	void Start()
	{
		TheMesh = GetComponent<TextMesh>();
		if (ConvertNewLines)
			text = text.Replace("\\n", System.Environment.NewLine);

		NeedsLayout = true;
	}

	string BreakPartIfNeeded(string part)
	{
		string saveText = TheMesh.text;
		TheMesh.text = part;

		if (TheMesh.GetComponent<Renderer>().bounds.extents.x > MaxWidth)
		{
			string remaining = part;
			part = "";
			while (true)
			{
				int len;
				for (len = 2; len <= remaining.Length; len++)
				{
					TheMesh.text = remaining.Substring(0, len);
					if (TheMesh.GetComponent<Renderer>().bounds.extents.x > MaxWidth)
					{
						len--;
						break;
					}
				}
				if (len >= remaining.Length)
				{
					part += remaining;
					break;
				}
				part += remaining.Substring(0, len) + System.Environment.NewLine;
				remaining = remaining.Substring(len);
			}

			part = part.TrimEnd();
		}

		TheMesh.text = saveText;

		return part;
	}

	void Update()
	{
		if (!NeedsLayout)
			return;
		else {
			//NeedsLayout = false;
			if (MaxWidth == 0) {
				TheMesh.text = text;
				return;
			}
			string builder = "";
			string newText = text;
			TheMesh.text = "";
			string[] parts = newText.Split (' ');
			for (int i = 0; i < parts.Length; i++) {
				string part = BreakPartIfNeeded (parts [i]);
				TheMesh.text += part + " ";
				if (TheMesh.GetComponent<Renderer> ().bounds.extents.x > MaxWidth) {
					TheMesh.text = builder.TrimEnd () + System.Environment.NewLine + part + " ";
				}
				builder = TheMesh.text;
			}
		}
	}
}