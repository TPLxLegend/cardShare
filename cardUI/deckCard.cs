
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
/// <summary>
/// quản lý cardModel và Gameobject UI của chúng như 1 pooling 
/// </summary>
public class deckCard : Singleton<deckCard>
{
    [Header("------------Ref------------")]
    [SerializeField] List<cardModel> cards;
    List<cardModel> cardInHand;
    [SerializeField] GameObject cardUI;
    [SerializeField] List<GameObject> cardUIs;
    [Header("------------Data------------")]
    public byte index = 0;
    public byte handLimit = 5;
    public int sizeDeck { get => cards.Count; }


    #region for user to Custom their desk
    public bool addCard(cardModel card, byte num)
    {
        if (num + card.num > card.maxCount)
        {
            num = (byte)(card.maxCount - card.num);

        }
        else if (num < 0)
        {
            num = 0;
        }
        for (int i = 0; i < num; i++)
        {
            card.addCard(1);
            cards.Add(card);
        }


        return true;
    }
    public bool removeCard(cardModel card, int num)
    {
        if (num > card.num) { num = card.num; }
        if (num < 0) { num = 0; }
        for (byte i = 0; i < num; i++)
        {
            card.removeCard(1);
            cards.Remove(card);
        }
        return true;
    }
    #endregion
    public cardModel getCard(int index)
    {
        return cards[index];
    }
    void loadCard()
    {
        Debug.Log("start of deck card");
        cardInHand = new List<cardModel>();
        shuffle();
        Debug.Log("-------after shuffer------------");
        cardInHand = drawCard(handLimit);
        Debug.Log(cardInHand.Count);
    }
    public void shuffle()
    {
        for (int i = index; i < cards.Count; i++)
        {
            int index = UnityEngine.Random.Range(0, cards.Count);
            cardModel temp = cards[i];
            cards[i] = cards[index];
            cards[index] = temp;
        }
    }

    public cardModel drawCard()
    {
        if (cards.Count == 0) return null;
        if (index > sizeDeck - 1)
        {
            //deck out 
            index = 0;
            shuffle();
        }
        cardModel card = cards[index];
        Debug.Log("draw a card:" + card.cardEffect);
        GameObject cardInsUI = Instantiate(cardUI, gameObject.transform);
        cardUIs.Add(cardInsUI);
        cardInsUI.GetComponent<card>().setCardModel(card);

        index++;
        return card;
    }
    public List<cardModel> drawCard(int num)
    {
        List<cardModel> res = new List<cardModel>();
        for (int i = 0; i < num; i++)
        {
            var card = drawCard();
            res.Add(card);
        }
        return res;
    }

    #region  mono
    void Start()
    {
        loadCard();
    }
    #endregion
}


[System.Serializable]
class deckData : IData
{
    public List<cardModel> cards;
    public void Save(string filepath)
    {
        saveload save = new saveload();
        save.save("deckData", this);
    }

    public deckData Load(string filepath)
    {
        saveload load = new saveload();
        return load.load<deckData>("deckData");
    }
}