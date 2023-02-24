using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Newtonsoft.Json;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class ElementComparer<T> : IEqualityComparer<T> where T: struct
{
    public bool Equals(T x, T y)
    {
        return x.GetHashCode() == y.GetHashCode();
    }

    public int GetHashCode(T obj)
    {
        return obj.GetHashCode();
    }
}

[Serializable]
public class SerializableHashSet<T> where T : struct
{
	public HashSet<T> Elements
	{
		get
		{
			if (_elementsHash == null)
				UpdateHash();

			return _elementsHash;
		}
		private set
        {
			_elementsHash = value;
        }
	}
	public T[] ElementList
	{
		get
		{
			if (_elementsList == null)
				_elementsList = new T[0];

			return _elementsList;
		}
		private set
		{
			_elementsList = value;
		}
	}

	[SerializeField, JsonProperty, ReadOnly] private T[] _elementsList;
	private HashSet<T> _elementsHash;

	public T[] GetList()
	{
		return ElementList;
	}
	public bool TryGetValue(T value, out T actualValue)
	{
		return Elements.TryGetValue(value, out actualValue);
	}
	public bool Add(T value)
    {
		var added = Elements.Add(value);

		if (added)
			UpdateArray();

		return added;
    }
	public HashSet<T>.Enumerator GetEnumerator()
    {
		return Elements.GetEnumerator();
    }
	void UpdateHash()
	{
		if (_elementsHash != null)
			return;

		_elementsHash = new HashSet<T>(new ElementComparer<T>());

		foreach (var element in ElementList)
			_elementsHash.Add(element);
	}
	void UpdateArray()
	{
#if UNITY_EDITOR
		if (EditorApplication.isPlaying)
		{
			Debug.Log("Cannot store elements at run-time.");
			return;
		}

		ElementList = Elements.ToArray();
#else
		Debug.Log("Cannot store elements at run-time.");
#endif
	}
}