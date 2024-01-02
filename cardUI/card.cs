using UnityEngine;
using UnityEngine.UI;

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
        if (cardModel.icon)
        {
            var iconObj = transform.GetChild(0).GetChild(0).gameObject;
            iconObj.GetComponent<Image>().sprite = cardModel.icon;
        }
        GetComponent<Button>().onClick.AddListener(click);
    }

    #region customMethod
    public void click()
    {
        bool haveTrigger = cardModel.effect();
        Debug.Log("trigger " + haveTrigger);
        if (!haveTrigger) return;

        StartCoroutine(waitForReDraw(cardModel.cooldown));
        Debug.Log("click");
    }
    System.Collections.IEnumerator waitForReDraw(float time)
    {
        Debug.Log("wait");
        couterClock.SetActive(true);
        var coutUI = couterClock.GetComponentInChildren<TMPro.TMP_Text>();
        Debug.Log("coutUI: " + coutUI);
        for (float i = time; i > 0; i -= Time.deltaTime)
        {
            coutUI.text = i.ToString("F2");
            yield return new WaitForSeconds(Time.deltaTime);
        }
        couterClock.SetActive(false);
        deckCard.Instance.returnCard(this);
    }
    #endregion

}
