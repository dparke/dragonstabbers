using UnityEngine;
using System.Collections;
using System.Text;

public class AttackBattleAction : GenericBattleAction
{

  public override void Action(Actor target1, Actor target2, ShakeManager shake = null)
  {
    //var attackValue = (int)Random.Range(target1.attackRange.x, target1.attackRange.y);
    var attackValue = Mathf.RoundToInt((
      Random.Range(target1.attackRange.x, target1.attackRange.y) 
      + Random.Range(target1.attackRange.x, target1.attackRange.y)) / 2);

    target2.DecreaseHealth(attackValue);

    var sb = new StringBuilder();
    sb.Append(target1.name);
    sb.Append(" attacks ");
    sb.Append(target2.name);
    sb.Append(". ");

    MusicManager.Instance.PlaySound(target1.attackSound);

    if (attackValue != 0 && attackValue >= target1.attackRange.y - 1)
    {
      MusicManager.Instance.PlaySound(target2.hitSound);
      sb.Append("Critical hit! ");
      sb.Append(target2.name);
      sb.Append(" loses ");
      sb.Append(attackValue);
      sb.Append(" hp.");
      shake.DoPreparedShake(.5f, 3f);
    }
    else if (attackValue > 0)
    {
      MusicManager.Instance.PlaySound(target2.hitSound);
      sb.Append(target2.name);
      sb.Append(" loses ");
      sb.Append(attackValue);
      sb.Append(" hp.");
      shake.DoPreparedShake(.5f, 1f);
    }
    else
    {
      sb.Append(target1.name);
      sb.Append(" misses!");
      shake.DoPreparedShake(.2f, .75f);
    }
    message = sb.ToString();
  }
}