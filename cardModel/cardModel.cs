using UnityEngine;
using UnityEngine.VFX;

public enum typeTarget
{
    seft,
    enemys,
    allys
}
public enum cardClass
{
    wizzard,
    swordman,
    archer
}
public enum cardType
{
    attack,
    trap,
    equip,
    spell
}

[CreateAssetMenu(menuName = "card")]
public class cardModel : ScriptableObject
{
    #region properties
    public string CardName;
    public cardType CardType;
    public cardClass cardClass = cardClass.wizzard;

    public string CardDescription
    {
        get
        {
            string details = this.CardName + "\n" + this.CardType + '\n' + this.cardClass;
            foreach (Effect eff in this.cardEffect)
            {
                details += eff.effect_detail + " ";
            }
            return details;
        }
    }
    public Sprite icon;
    public int Count = 1;
    public int maxCount = 3;
    public float cooldown = 3;
    public GameObject skillObj;
    public Effect[] cardEffect;
    public float processTime = 1;
    #endregion

    #region methods
    public virtual void effect(Transform tf, Transform targetTranform = null) // lam sao cho cac vi du: target = enemys, allys, seft 
    {
        Debug.Log("Trigger effect:" + this.name);

        //instance of skill object or summon something
        GameObject vfxObj = Instantiate(skillObj, tf.position, Quaternion.identity);
        skillObj skillObjScript = vfxObj.GetComponent<skillObj>();
        Destroy(vfxObj, processTime);
        if (targetTranform)
        {
            var vfx = vfxObj.GetComponent<VisualEffect>();
            skillObjScript.target = targetTranform;
        }
        skillObjScript.collisionEnter.AddListener(vfxResolve);


    }
    #region count
    public bool addCard(int num)
    {
        if (num + Count > maxCount)
        {
            Count = maxCount;
            return false;
        }
        Count += num;
        return true;
    }

    public bool removeCard(int num)
    {
        if (Count - num < 0)
        {
            Count = 0;
            return false;
        }
        Count -= num;
        return true;
    }
    #endregion

    public virtual void vfxResolve(GameObject obj,GameObject collision)
    {
        Debug.Log(obj);
        foreach (Effect effect in cardEffect)
        {
            effect.triggerEffect(obj.GetComponent<skillObj>().objInRange.ToArray());
        }
    }
    #endregion
}

public interface ISummon
{
    void summon(GameObject target);
}

public interface IEffect
{
    void effect(GameObject[] target);
}
