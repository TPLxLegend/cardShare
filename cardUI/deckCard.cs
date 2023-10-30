
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// quản lý cardModel thay cho card repository và Gameobject UI của chúng như 1 pooling 
/// </summary>
public class deckCard : Singleton<deckCard>
{
    [Header("------------Ref------------")]
    [SerializeField] List<cardModel> cards;
    public List<cardModel> cardInHand; //public for effect that need ref
    [SerializeField] GameObject cardUI;
    [SerializeField] GameObject cardBar;

    [SerializeField] cardModel brick;
    [SerializeField] List<GameObject> cardUIs;
    [Header("------------Data------------")]
    public byte index = 0;
    public byte handLimit = 5;
    [SerializeField] byte minDeckSize = 10;
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
        //truong hop loai bo khien cards xuong duoi min se tu dong bo sung brick card
        if (cards.Count < minDeckSize)
        {
            addCard(brick, (byte)(minDeckSize - cards.Count));
        }
        return true;
    }
    #endregion
    #region card operator
    public cardModel getCard(int index)
    {
        return cards[index];
    }

    public void shuffle()
    {
        for (int i = index; i < cards.Count; i++)
        {
            int index = Random.Range(i, cards.Count);
            (cards[index], cards[i], cardUIs[index], cardUIs[i]) = (cards[i], cards[index], cardUIs[i], cardUIs[index]);
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

        spawnCard(card);

        index++;
        return card;
    }

    public List<cardModel> drawCard(int num)
    {

        for (int i = 0; i < num; i++)
        {
            var card = drawCard();
            cardInHand.Add(card);
        }
        return cardInHand;
    }
    #endregion
    #region operator on UI 
    void loadCard()
    {
        Debug.Log("start of deck card");
        cardInHand = new List<cardModel>();
        shuffle();
        Debug.Log("-------after shuffer------------");
        drawCard(handLimit);
        Debug.Log(cardInHand.Count);
    }
    void spawnCard(cardModel card)
    {
        var res = cardUIs.FirstOrDefault(go =>
        {
            return (go.GetComponent<card>()?.cardModel == card) && (!go.activeSelf);
        });
        if (res != default)
        {
            GameObject cardInsUI = Instantiate(cardUI, cardBar.transform);
            cardUIs.Add(cardInsUI);
            cardInsUI.GetComponent<card>().initFromCardModel(card);
            return;
        }
        res.SetActive(true);
    }

    #endregion
    #region  mono
    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
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