using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

public class StatsWindow : GenericWindow {

  public Actor target;
  public Text nameLabel;
  public Text valueLabel;
  public bool degradationVisible = false;

  public void UpdateStats()
  {
    if (target == null || valueLabel == null)
    {
      return;
    }

    var sb = new StringBuilder();
    sb.Append(target.health.ToString("D2"));
    sb.Append("/");
    sb.Append(target.maxHealth.ToString("D2"));
    sb.Append("\n");
    sb.Append(target.gold.ToString("D4"));
    sb.Append("\n");
    sb.Append(((int)target.attackRange.y).ToString("D2"));
    sb.Append("\n");
    sb.Append(((int)target.talkRange.y).ToString("D2"));

    if (target.degradation > 0)
    {
      if (!degradationVisible)
      {
        GetComponent<RectTransform>().sizeDelta = new Vector2(48, 74);
        nameLabel.text += "\nDGR";
        degradationVisible = true;
      }

      sb.Append("\n");
      sb.Append(((int)target.degradation).ToString("D2"));
    }
    valueLabel.text = sb.ToString();
  }

  public List<string> FinalStats()
  {
    List<string> stats = new List<string>();
    stats.Add("HP");
    stats.Add(string.Format("{0}/{1}", target.health.ToString("D2"), target.maxHealth.ToString("D2")));
    stats.Add("Gold");
    stats.Add(target.gold.ToString("D4"));
    stats.Add("ATK");
    stats.Add(((int)target.attackRange.y).ToString("D2"));
    stats.Add("TLK");
    stats.Add(((int)target.talkRange.y).ToString("D2"));

    if (degradationVisible)
    {
      stats.Add("DGR");
      stats.Add(((int)target.degradation).ToString("D2"));
    }

    return stats;
  }
}