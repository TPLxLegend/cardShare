using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(menuName = "card")]
public class cardModel : ScriptableObject
{
    #region properties
    public string CardName;
    public cardTag[] cardTag;
    public cardType CardType;
    public cardClass cardClass = cardClass.wizzard;
    public targetFilterType detecttype;
    public triggerType triggerType;

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
        //instance of skill object or summon something
        Vector3 position = tf.position;
        if (targetTranform)
        {
            Vector3 dir = (targetTranform.position - tf.position).normalized * skillObj.GetComponent<SphereCollider>().radius;
            position += dir + new Vector3(0.1f, 0.1f, 0.1f);
        }

        GameObject vfxObj = Instantiate(skillObj, position, Quaternion.identity);
        skillObj skillObjScript = vfxObj.GetComponent<skillObj>();
        skillObjScript.source = tf.gameObject;
        Destroy(vfxObj, processTime);

        skillObjScript.target = targetTranform;
        Dic.singleton.filter[detecttype].addFilterion(skillObjScript);
        Dic.singleton.trigger[triggerType].addTrigger(skillObjScript,cardEffect);
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

    
    #endregion


}
