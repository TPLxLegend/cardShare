
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class deckCard : MonoBehaviour
{
    [SerializeField] List<cardModel> cards;
    [SerializeField] GameObject cardUI;
    [SerializeField] List<GameObject> cardUIs;
    public int sizeDeck { get => cards.Count; }

    public GameObject playerObj;

    public bool addCard(cardModel card, int num)
    {
        for (int i = 0; i < num; i++)
        {
            if (!cards.Contains(card))
            {
                cards.Add(card);
            }
            else
            {
                int id = cards.IndexOf(card);
                cards[id].addCard(num - 1);
            }
        }
        return true;
    }
    public void removeCard(cardModel card, int num)
    {
        for (int i = 0; i < num; i++)
        {
            if (!cards.Contains(card))
            {
                return;
            }
            else
            {
                int id = cards.IndexOf(card);
                cards[id].removeCard(num - 1);
            }
        }
    }
    public cardModel getCard(int index)
    {
        return cards[index];
    }
    public void shuffle()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int index = UnityEngine.Random.Range(0, cards.Count);
            cardModel temp = cards[i];
            cards[i] = cards[index];
            cards[index] = temp;
        }
    }

    public void drawCard()
    {
        cardModel card = cards[0];
        if (!card)
        {
            //deck out 
            
            return;
        }
        removeCard(card, 1);
        GameObject cardInsUI = Instantiate(cardUI, gameObject.transform);
        cardUIs.Add(cardInsUI);
        cardInsUI.GetComponent<card>().setCardModel(card);
    }

    #region  mono
    void Start()
    {

    }
    #endregion
}
