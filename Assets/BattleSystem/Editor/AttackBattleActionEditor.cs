using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AttackBattleAction))]
public class AttackBattleActionEditor : Editor {

  [MenuItem("Assets/Create/Attack Action")]
  public static void CreateAction()
  {
    AssetUtil.CreateScriptableObject<AttackBattleAction>();
  }
}
