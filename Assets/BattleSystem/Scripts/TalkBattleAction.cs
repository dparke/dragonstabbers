using UnityEngine;
using System.Collections;
using System.Text;

public class TalkBattleAction : GenericBattleAction {

  public override void Action(Actor target1, Actor target2, ShakeManager shake = null)
  {
    //var talkValue = (int)Random.Range(target1.talkRange.x, target1.talkRange.y);
    var talkValue = Mathf.RoundToInt((
      Random.Range(target1.talkRange.x, target1.talkRange.y)
      + Random.Range(target1.talkRange.x, target1.talkRange.y)) / 2);

    target2.DecreaseGold(talkValue);

    var sb = new StringBuilder();
    sb.Append(target1.name);
    sb.Append(" haggles with ");
    sb.Append(target2.name);
    sb.Append(". ");

    if (talkValue != 0 && talkValue >= target1.talkRange.y - 1)
    {
      sb.Append("Compelling point! ");
      sb.Append(target2.name);
      sb.Append(" loses ");
      sb.Append(talkValue);
      sb.Append(" gold.");
      shake.DoPreparedShake(.5f, 3f);
    }
    else if (talkValue > 0)
    {
      sb.Append(target2.name);
      sb.Append(" loses ");
      sb.Append(talkValue);
      sb.Append(" gold.");
      shake.DoPreparedShake(.5f, 1f);
    }
    else
    {
      sb.Append(target1.name);
      sb.Append(" stammers!");
      shake.DoPreparedShake(.2f, .75f);
    }

    message = sb.ToString();
  }
}