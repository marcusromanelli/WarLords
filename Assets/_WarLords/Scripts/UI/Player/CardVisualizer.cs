using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardVisualizer : MonoBehaviour
{
    [SerializeField] CardContent cardContent;

    private RuntimeCardData currentData;

    public void Show(RuntimeCardData card)
    {
        currentData = card;

        cardContent.SetData(card);
        cardContent.Show();
    }
    public void Hide(RuntimeCardData card)
    {
        if (currentData != card)
            return;

        cardContent.Hide();
    }
}