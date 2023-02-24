using UnityEngine;
using UnityEngine.UI;

public delegate void OnClickAddRemoveButton();
public class UICardViewer : MonoBehaviour
{
    [SerializeField] CardContent cardContent;
    [SerializeField] Button addButton;
    [SerializeField] Button removeButton;

    public void Show(Card card, OnClickAddRemoveButton onClickAddButton = null, OnClickAddRemoveButton onClickRemoveButton = null)
    {
        if(onClickAddButton != null)
        {
            addButton.onClick.RemoveAllListeners();
            addButton.onClick.AddListener(() => { onClickAddButton(); });
            addButton.gameObject.SetActive(onClickAddButton != null);
        }

        if(removeButton != null)
        {
            removeButton.onClick.RemoveAllListeners();
            removeButton.onClick.AddListener(() => { onClickRemoveButton(); });
            removeButton.gameObject.SetActive(onClickRemoveButton != null);
        }        


        var runtimeCardData = new RuntimeCardData(card);

        cardContent.Show();
        cardContent.SetData(runtimeCardData, null);
    }

    public void Hide()
    {
        cardContent.Hide();
        addButton.gameObject.SetActive(false);
        removeButton.gameObject.SetActive(false);
    }

}
