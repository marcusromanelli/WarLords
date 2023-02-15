using TMPro;
using UnityEngine;

public delegate void OnListElementClicked();
public class SimpleListObject : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    private OnListElementClicked onListElementClicked;

    public void Setup(string text, OnListElementClicked onListElementClicked)
    {
        nameText.text = text;

        this.onListElementClicked = onListElementClicked;
    }
    public void OnClick()
    {
        onListElementClicked?.Invoke();
    }
}
