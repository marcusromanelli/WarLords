using TMPro;
using UnityEngine;

public class CardNameObject : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    private MenuCardList mainMenu;
    private CivilizationData.CardNameAndBundle cardData;

    public void Setup(MenuCardList mainMenu, CivilizationData.CardNameAndBundle? cardData = null)
    {
        if(cardData == null)
        {
            nameText.text = "No cards found, or none loaded";
            return;
        }

        var card = cardData ?? default;

        nameText.text = card.Name;
        
        this.mainMenu = mainMenu;
        this.cardData = card;
    }
    public void OnClick()
    {
        mainMenu.OnClickCard(cardData);
    }
}
