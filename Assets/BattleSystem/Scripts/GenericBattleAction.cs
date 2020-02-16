using UnityEngine;
using System.Collections;

public class GenericBattleAction : ScriptableObject {

  public new string name;

  protected string message = "Undefined Battle Action Message";

  public virtual void Action(Actor target1, Actor target2, ShakeManager shake = null)
  {
    // override with action logic
  }

  public override string ToString()
  {
    return message;
  }
}