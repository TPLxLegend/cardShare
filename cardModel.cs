using UnityEngine;


public class cardModel : ScriptableObject
{
    #region properties
    public string CardName;
    public string CardType;

    public string CardDescription;
    public Sprite icon;
    public int Count;
    public Effect[] cardEffect;
    #endregion

    #region methods
    public virtual void effect(GameObject target)
    {
        Debug.Log("Trigger effect:" + this.name);
        foreach (Effect effect in this.cardEffect){
            effect.trigger(target);
        }
    }
    #endregion
}
[CreateAssetMenu(menuName = "card/equip")]
class equipCard : cardModel
{
    override public void effect(GameObject target)
    {
        base.effect(target);


    }
}
[CreateAssetMenu(menuName = "card/attack")]
class attackCard : cardModel
{
    override public void effect(GameObject target)
    {

    }
}
[CreateAssetMenu(menuName = "card/spell")]
class spellCard : cardModel
{
    public int dmg;

    public int range;
    public int cooldown;
    public int mana;
    public int mana_cost;

   // public Effect Effect;

    override public void effect(GameObject target)
    {

    }
}
[CreateAssetMenu(menuName = "card/continueSpell")]
class continuesSpellCard:spellCard{

}
[CreateAssetMenu(menuName = "card/trap")]
class trapCard : cardModel
{
    override public void effect(GameObject target)
    {

    }
}


