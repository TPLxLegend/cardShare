using UnityEngine;

[CreateAssetMenu(menuName = "card")]
public class cardModel : ScriptableObject
{
    #region properties
    public string CardName;
    public cardTag[] cardTag;
    public cardType CardType = cardType.spell;
    public charJob charJob = charJob.wizzard;
    public skillMoveType skillMoveType;
    public targetFilterType detecttype = targetFilterType.allObj;
    public triggerType triggerType = triggerType.whenHitEnemy;

    public string CardDescription
    {
        get
        {
            string details = this.CardName + "\n" + this.CardType + '\n' + this.charJob + "\n" + this.BaseDescription + "\n";
            foreach (Effect eff in this.cardEffect)
            {
                details += eff.effect_detail + " ";
            }
            return details;
        }
    }
    [SerializeField] string BaseDescription;
    public Sprite icon;
    public int Count = 1;
    public int maxCount = 3;
    public float cooldown = 3;
    public GameObject skillObj;
    public Effect[] cardEffect;
    public int timeStandby = 0;
    public float duration = 1;
    public float speed = 1;

    #endregion

    #region methods
    public virtual void effect(Transform tf, Vector3 targetPosition) // lam sao cho cac vi du: target = enemys, allys, seft 
    {
        Vector3 position = tf.position;
        Quaternion rot = Quaternion.identity;
        Vector3 dir = (targetPosition - tf.position).normalized * skillObj.GetComponent<SphereCollider>().radius;
        position += 0.1f * dir;
        rot = Quaternion.LookRotation(dir);

        GameObject InsSkillObj = Instantiate(skillObj, position, rot);
        skillObj skillObjScript = InsSkillObj.GetComponent<skillObj>();

        skillObjScript.source = tf.gameObject;
        Destroy(InsSkillObj, duration);

        Dic.singleton.moveTypes[skillMoveType].addMoveAsync(InsSkillObj, targetPosition, speed, timeStandby);
        Dic.singleton.filter[detecttype].addFilterion(skillObjScript);
        Dic.singleton.trigger[triggerType].addTrigger(skillObjScript, cardEffect);
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
