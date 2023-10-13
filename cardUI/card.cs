using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class card : MonoBehaviour
{
    cardModel cardModel;
    public GameObject target;
    public GameObject player;
    bool focus;
    public void setCardModel(cardModel cardModel)
    {
        this.cardModel = cardModel;
        GetComponentInChildren<TMP_Text>().text = cardModel.name;
        GetComponentInChildren<SpriteRenderer>().sprite = cardModel.icon;
        GetComponent<Button>().clicked+= ()=>click();
    }

    #region customMethod
    void click()
    {
        focus=!focus;
        
        Debug.Log("click");
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