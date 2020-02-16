using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Actor))]
public class ActorEditor : Editor {

  [MenuItem("Assets/Create/Actor")]
  public static void CreateActor()
  {
    AssetUtil.CreateScriptableObject<Actor>();
  }

}
