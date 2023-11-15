using UnityEngine;

public class card : MonoBehaviour
{
    public cardModel cardModel;

    [SerializeField] GameObject couterClock;
    public TMPro.TMP_Text manaCostValueUI;
    public void initFromCardModel(cardModel cardModel)
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

        StartCoroutine(waitForReDraw(cardModel.cooldown));
        Debug.Log("click");
    }
    System.Collections.IEnumerator waitForReDraw(float time)
    {
        couterClock.SetActive(true);
        var coutUI = couterClock.GetComponentInChildren<TMPro.TMP_Text>();
        for (float i = time; i > 0; i -= Time.deltaTime)
        {
            coutUI.text = i.ToString("F2");

            yield return new WaitForSeconds(Time.deltaTime);
        }
        //coutUI.text = time.ToString();
        deckCard.Instance.returnCard(cardModel);
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
