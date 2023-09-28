using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ElementalColorModify : MonoBehaviour
{
   public DmgType dmgType;
   
   void Start()
   {
      updateColor();
   }
   public void updateColor()
   {
      var colorMap=Dic.singleton.colorMap;
      GetComponent<MeshRenderer>().material.SetColor("_Color", colorMap[dmgType]);
      GetComponentInChildren<TrailRenderer>().material.SetColor("_Color", colorMap[dmgType]);
     // GetComponentInChildren<VisualEffect>(includeInactive:true).SetVector4("color",colorMap[dmgType]);
   }
}
