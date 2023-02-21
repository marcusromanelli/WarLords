using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnSelectedDeck(bool localPlayer, UserDeck deckList);
public class SelectDeckObject : MonoBehaviour
{
    [SerializeField] CivilizationPanel civilizationsPanel;
    [SerializeField] DeckPanel decksPanel;

    private UserDeck selectedDeck;
    private CivilizationData selectedCivData;
    private DeckCollection deckCollection;
    private OnSelectedDeck onSelectedDeck;
    private bool isLocalPlayer;

    public void Setup(DeckCollection deckCollection, RawBundleData[] civilizations, bool isLocalPlayer, OnSelectedDeck onSelectedDeck)
    {
        this.deckCollection = deckCollection;
        this.onSelectedDeck = onSelectedDeck;
        this.isLocalPlayer = isLocalPlayer;

        civilizationsPanel.Setup(OnCivilizationClick);
        decksPanel.Setup(null, OnDeckClick);

        civilizationsPanel.Load(civilizations);
    }
    public void ReturnToCivilziationSelection()
    {
        civilizationsPanel.gameObject.SetActive(true);
        decksPanel.gameObject.SetActive(false);
    }


    void OnCivilizationClick(CivilizationData data)
    {
        selectedCivData = data;
        civilizationsPanel.gameObject.SetActive(false);
        decksPanel.gameObject.SetActive(true);

        decksPanel.Load(GetCurrentCivDeckList());
    }
    UserDeckList GetCurrentCivDeckList()
    {
        return deckCollection.GetCivilizationDeck(selectedCivData.GetId());
    }
    void OnDeckClick(UserDeck selectedDeck)
    {
        this.selectedDeck = selectedDeck;
        onSelectedDeck?.Invoke(isLocalPlayer, selectedDeck);
    }
}
