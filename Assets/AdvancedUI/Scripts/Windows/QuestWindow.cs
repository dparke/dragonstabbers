using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

public class QuestWindow : GenericWindow {

  public Map target;
  public Actor player;
  public Text nameLabel;
  public Text valueLabel;
  public int difficulty
  {
    get { return target.skullTiles.Length; }
  }
  public bool wellVisible = false;

  private int totalTowns;
  private int totalMonsters;
  private int totalTowers;

  public void InitQuest()
  {
    totalTowns = target.townTiles.Length;
    totalMonsters = target.monsterTiles.Length;

    UpdateQuest();
  }

  public void UpdateQuest()
  {
    if (target == null || valueLabel == null)
    {
      return;
    }

    var sb = new StringBuilder();
    
    sb.Append(target.percentVisited.ToString());
    sb.Append("%");
    sb.Append("\n");

    if (totalTowns > 0)
    {
      sb.Append(target.ruinsTiles.Length.ToString("D2"));
      sb.Append("/");
      sb.Append(totalTowns.ToString("D2"));
      sb.Append("\n");
    }
    else
    {
      sb.Append("???\n");
    }

    sb.Append(target.skullTiles.Length.ToString("D2"));
    sb.Append("/");
    sb.Append(totalMonsters.ToString("D2"));
    sb.Append("\n");

    sb.Append(IsTowerUnlocked() ? " ! " : "???");

    if (player != null && PasswordManager.Instance.CheckPassword("baxter"))
    {
        if (!wellVisible)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(48, 74);
            nameLabel.text += "\nWEL";
            wellVisible = true;
        }

        sb.Append("\n");
        sb.Append(player.foundWell ? " ! " : "???");
    }

    valueLabel.text = sb.ToString();
  }

  public bool IsTowerUnlocked()
  {
    return (target.ruinsTiles.Length == totalTowns && target.skullTiles.Length == totalMonsters);
  }

  public bool IsWellUnlocked()
  {
    return PasswordManager.Instance.CheckPassword("baxter");
  }

  public List<string> FinalQuest()
  {
    List<string> quest = new List<string>();
    quest.Add("MAP");
    quest.Add(target.percentVisited.ToString() + "%");

    quest.Add("TWN");
    if (totalTowns > 0)
    {
      quest.Add(target.ruinsTiles.Length.ToString("D2") + "/" + totalTowns.ToString("D2"));
    }
    else
    {
      quest.Add("???\n");
    }

    quest.Add("DNG");
    quest.Add(target.skullTiles.Length.ToString("D2") + "/" + totalMonsters.ToString("D2"));

    quest.Add("TWR");
    if (IsTowerUnlocked())
    {
      if (target.evoTile != null)
      {   
        quest.Add("!");
      }
      else
      {
        quest.Add("WIN");
      }
    }
    else
    {
      quest.Add("???");
    }

    quest.Add("WEL");
    if (player.foundWell)
    {
      quest.Add("!");
    }
    else
    {
      quest.Add("???");
    }

    return quest;
  }
}