using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class card : MonoBehaviour
{
    cardModel cardModel;

    public TMP_Text manaCostValueUI;
    bool focus;
    public void setCardModel(cardModel cardModel)
    {
        this.cardModel = cardModel;
        //GetComponentInChildren<TMP_Text>().text = cardModel.name;
        manaCostValueUI.text = cardModel.manaCost.ToString();
        if (cardModel.icon) GetComponentInChildren<SpriteRenderer>().sprite = cardModel.icon;
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(click);
    }

    #region customMethod
    void click()
    {

        bool haveTrigger = cardModel.effect();
        if (!haveTrigger) return;



        Debug.Log("click");
    }
    void hold()
    {
        Debug.Log("hold");

    }
    #endregion

    #region mono-method
    void Awake()
    {

    }
    void Start()
    {


    }


    #endregion


}
