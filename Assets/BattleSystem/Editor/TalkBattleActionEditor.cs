using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TalkBattleAction))]
public class TalkBattleActionEditor : Editor {

  [MenuItem("Assets/Create/Talk Action")]
  public static void CreateAction()
  {
    AssetUtil.CreateScriptableObject<TalkBattleAction>();
  }
}
