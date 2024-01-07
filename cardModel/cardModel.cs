using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "card")]
public class cardModel : ScriptableObject
{
    #region state
    [Header("------------------State---------------------------")]
    public string CardName;
    public cardTag[] cardTag;
    public cardType CardType = cardType.spell;
    public charJob charJob = charJob.wizzard;
    public Effect[] cardEffect;
    public byte manaCost = 0;
    public int timeStandby = 0;

    public byte cooldown = 1;
    public float duration = 1;
    public float speed = 1;
    public Sprite icon;
    public byte num;
    public byte maxCount = 3;
    #endregion
    #region  references
    [Header("------------------Ref---------------------------")]

    public conditionTrigger[] conditions;
    public skillMoveType skillMoveType = skillMoveType.notMove;
    public typeTarget targetMethod = typeTarget.quickSeft;
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

    public GameObject skillObj;
    #endregion

    #region methods
    public void effect(Transform tf, Vector3 targetPosition)
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
    public virtual bool effect()
    {
        //xet dieu kien kich hoat
        var info = PlayerController.Instance.playerInfo;
        if (info.mp >= manaCost)
        {
            info.lostmana(manaCost);
        }
        else
        {
            Debug.Log("khong du mana de kich hoat bai: " + CardName + "\n mana:" + info.mp);
            return false;
        }
        if (conditions.Length > 0)
        {
            foreach (var condition in conditions)
            {
                if (!condition.checkCondittion())
                {
                    return false;
                }
            }
        }
        Transform tf = PlayerController.Instance.player.transform;
        Vector3 position = tf.position;
        Quaternion rot;

        Vector3 targetPosition = Dic.singleton.targetMethod[targetMethod].target();

        Vector3 dir = (targetPosition - tf.position).normalized * (1 + skillObj.GetComponent<SphereCollider>().radius);
        Debug.Log("sphere collider radius:" + skillObj.GetComponent<SphereCollider>().radius);
        position += 0.1f * dir;
        Debug.Log("card ins dir:" + dir);
        rot = Quaternion.LookRotation(dir);


        //spawnrpc
        spawnPlayerSystem.Instance.spawnCardSkillObjectServerRpc(NetworkManager.Singleton.LocalClientId, skillObj.name, position, rot, duration, skillMoveType,
        detecttype, triggerType, targetPosition, speed, timeStandby, itemPooling.Instance.getIndex(cardEffect));
        return true;
    }
    //public static void set
    #region count
    public bool addCard(int num)
    {
        try
        {
            if (num + this.num > maxCount)
            {
                num = (byte)(maxCount - this.num);
            }
            if (num < 0)
            {
                num = 0;
            }
            Debug.Log(num);
            this.num += (byte)num;
            return true;
        }
        catch
        {
            return false;
        }

    }

    public bool removeCard(int num)
    {
        try
        {
            if (this.num - num < 0)
            {
                num = this.num;
            }
            if (num < 0)
            {
                num = 0;
            }
            Debug.Log(num);

            this.num -= (byte)num;
            return true;
        }
        catch
        {
            return false;
        }

    }
    #endregion


    #endregion


}
