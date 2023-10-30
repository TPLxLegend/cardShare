using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Repo", menuName = "Card pack")]
public class cardPack : ScriptableObject
{
    public string namePack, notes, timeRemain;
    public List<cardModel> cards;

    /// <summary>
    /// rate is in % unit and it is integer
    /// </summary>
    public List<byte> rates;
    public cardModel outOfRateCard;
    public Sprite illustration;
    public (cardModel, byte) getCardWithRate(int id)
    {
        return (cards[id], rates[id]);
    }


    public void copyTo(gacchaSystem<cardModel> gaccha)
    {
        List<cardModel> res = new List<cardModel>();
        for (int i = 0; i < rates.Count; i++)
        {
            for (int j = 0; j < rates[i]; j++)
            {
                res.Add(cards[i]);
            }
        }
        gaccha.repo = res;
    }
}
