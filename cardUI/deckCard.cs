
using System.Collections.Generic;

using System.Linq;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public byte cardPointTranferWhenReachLimitNum = 100;
    public int cardPoint = 0;
    [SerializeField] byte minDeckSize = 10;
    public int sizeDeck { get => cards.Count; }

    #region for add or remove card to deck
    public bool addCard(cardModel card, byte num)
    {
        int cardPointNum = 0;
        if (num + card.num > card.maxCount)
        {
            num = (byte)(card.maxCount - card.num);
            cardPointNum = num + card.num - card.maxCount;
            //xu li UI 
            Debug.Log("card du se chuyen doi thanh diem x" + cardPointTranferWhenReachLimitNum);
            cardPoint = cardPointNum * cardPointTranferWhenReachLimitNum;
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
    public bool addCard(List<cardModel> listAdd)
    {
        for (int i = 0; i < listAdd.Count; i++)
        {
            cards.Add(listAdd[i]);
            Debug.Log("Card have been added to deck:" + listAdd[i].CardName);
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
        for (int i = this.index; i < cards.Count; i++)
        {
            int index = Random.Range(i, cards.Count);
            (cards[index], cards[i]) = (cards[i], cards[index]);
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
            if (card == null) continue;
            cardInHand.Add(card);
        }
        return cardInHand;
    }
    public void returnCard(int id)
    {
        var cardBeRemove = cardInHand[id];
        returnCard(cardBeRemove);
    }
    public void returnCard(cardModel md)
    {
        despawnCard(md);
        cardInHand.Remove(md);
        drawCard(1);
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
        if (res == default)
        {
            GameObject cardInsUI = Instantiate(cardUI, cardBar.transform);
            cardUIs.Add(cardInsUI);
            cardInsUI.GetComponent<card>().initFromCardModel(card);
            return;
        }
        res.SetActive(true);
    }
    void despawnCard(cardModel card)
    {
        var res = cardUIs.FirstOrDefault(go =>
                {
                    return (go.GetComponent<card>()?.cardModel == card) && go.activeSelf;
                });
        if (res == default)
        {
            Debug.Log("khong co bai nao de despawn");
            return;
        }
        res.SetActive(false);
    }

    #endregion
    public void handleInput(byte num)
    {
        Debug.Log(num);
        if (num > cardInHand.Count - 1) return;
        bool res = cardInHand[num].effect();
        if (!res) { Debug.Log("trigger fail"); }

    }
    #region  mono
    private void OnEnable()
    {
        var data = deckData.Load();
        if (data != null)
        {
            cards = data.cards;
        }
    }
    private void OnDisable()
    {
        PlayerController.Instance.input.card.card1.performed -= (ctx) => { handleInput(0); };
        PlayerController.Instance.input.card.card2.performed -= (ctx) => { handleInput(1); };
        PlayerController.Instance.input.card.card3.performed -= (ctx) => { handleInput(2); };
        PlayerController.Instance.input.card.card4.performed -= (ctx) => { handleInput(3); };
        PlayerController.Instance.input.card.card5.performed -= (ctx) => { handleInput(4); };
        PlayerController.Instance.input.card.card6.performed -= (ctx) => { handleInput(5); };
    }
    void Start()
    {
        loadCard();
        if (PlayerController.Instance == null) return;
        PlayerController.Instance.input.card.card1.performed += (ctx) => { handleInput(0); };
        PlayerController.Instance.input.card.card2.performed += (ctx) => { handleInput(1); };
        PlayerController.Instance.input.card.card3.performed += (ctx) => { handleInput(2); };
        PlayerController.Instance.input.card.card4.performed += (ctx) => { handleInput(3); };
        PlayerController.Instance.input.card.card5.performed += (ctx) => { handleInput(4); };
        PlayerController.Instance.input.card.card6.performed += (ctx) => { handleInput(5); };
    }
    #endregion
}


[System.Serializable]
class deckData : IData
{
    public List<cardModel> cards;
    public void Save()
    {
        saveload save = new saveload();
        save.save("deckData", this);
    }

    public static deckData Load()
    {
        saveload load = new saveload();
        return load.load<deckData>("deckData");
    }
}