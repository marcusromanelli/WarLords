using TMPro;
using UnityEngine;

public class CivilizationNameObject : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    private CardVisualizerScreen mainMenu;
    private string civilizationName;

    public void Setup(CardVisualizerScreen mainMenu, string civilizationName)
    {
        nameText.text = civilizationName;
        
        this.mainMenu = mainMenu;
        this.civilizationName = civilizationName;
    }
    public void OnClick()
    {
        mainMenu.OnClickCivilization(civilizationName);
    }
}
