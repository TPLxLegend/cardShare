using UnityEngine;


public class cardModel : ScriptableObject
{
    #region properties
    public string CardName;
    public string CardType;

    public string CardDescription;
    public Sprite icon;
    public int Count;

    #endregion

    #region methods
    public virtual void effect(GameObject target)
    {
        Debug.Log("Trigger effect:" + this.name);
    }
    #endregion
}

class equipCard : cardModel
{
    override public void effect(GameObject target)
    {
        base.effect(target);


    }
}

class attackCard : cardModel
{
    override public void effect(GameObject target)
    {

    }
}

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

class continuesSpellCard:spellCard{

}
class trapCard : cardModel
{
    override public void effect(GameObject target)
    {

    }
}


