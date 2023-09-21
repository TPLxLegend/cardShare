using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Repo", menuName = "Card pack")]
public class cardPack : ScriptableObject{
    public List<cardModel> cards;

}
