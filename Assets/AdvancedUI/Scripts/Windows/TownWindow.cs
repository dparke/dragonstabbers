using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TownWindow : GenericWindow {

  public Image[] decorations;
  public GameObject actionsGroup;
  public Text townieLabel;
  public Image townieImage;
  public GenericBattleAction[] actions;
  public bool nextActionPlayer = true;
  [Range(0, .9f)]
  public float runOdds = .6f;
  public RectTransform windowRect;
  public RectTransform monsterRect;
  public delegate void TownOver(bool playerWin);
  public TownOver townOverCallback;

  private ShakeManager shakeManager;
  private Actor player;
  private Actor townie;

  private System.Random rand = new System.Random();

  protected override void Awake()
  {
    shakeManager = GetComponent<ShakeManager>();
    base.Awake();
  }

  public override void Open()
  {
    base.Open();

    MusicManager.Instance.ChangeMusic(Windows.TownWindow);

    foreach (var decoration in decorations)
    {
      decoration.enabled = rand.NextDouble() >= .5;
    }

    actionsGroup.SetActive(false);
  }

  public void StartTown(Actor target1, Actor target2)
  {
    player = target1;
    townie = target2;

    DisplayMessage(townie.name + " approaches!");
    townieImage.sprite = townie.avatar;
    
    if (PasswordManager.Instance.CheckPassword("sprint"))
    {
        nextActionPlayer = true;
    }
    else
    {
        nextActionPlayer = rand.NextDouble() >= .5;
    }
    
    StartCoroutine(NextAction());
    UpdateTownieLabel();
  }

  public void OnAction(GenericBattleAction action, Actor target1, Actor target2, ShakeManager preparedShake)
  {
    action.Action(target1, target2, preparedShake);

    DisplayMessage(action.ToString());
    actionsGroup.SetActive(false);

    UpdatePlayerStats();
    UpdateTownieLabel();

    StartCoroutine(NextAction());
  }

  public void OnPlayerAction(int id)
  {
    switch (id)
    {
      case 1:
        StartCoroutine(OnLeave());
        break;
      default:
        var action = actions[id];
        OnAction(action, player, townie, shakeManager.PrepareShake(monsterRect));
        break;
    }

    nextActionPlayer = false;
  }

  public void OnTownieAction()
  {
    var action = actions[0];
    OnAction(action, townie, player, shakeManager.PrepareShake(windowRect));
    nextActionPlayer = true;
  }

  void DisplayMessage(string text)
  {
    var messageWindow = manager.Open((int)Windows.MessageWindow - 1, false) as MessageWindow;
    messageWindow.text = text;
  }

  IEnumerator NextAction()
  {
    yield return new WaitForSeconds(2);

    if (player.broke || townie.broke)
    {
      StartCoroutine(OnBattleOver());
    }
    else
    {
      if (nextActionPlayer)
      {
        actionsGroup.SetActive(true);
        OnFocus();
      }
      else
      {
        OnTownieAction();
      }
    }
  }

  void UpdatePlayerStats()
  {
    ((StatsWindow)manager.GetWindow((int)Windows.StatsWindow - 1)).UpdateStats();
  }

  void UpdateTownieLabel()
  {
    townieLabel.text = townie.name + " - Fee " + townie.gold.ToString("D2");
  }

  IEnumerator OnLeave()
  {
    actionsGroup.SetActive(false);

    var chance = Random.Range(0, 1f);
    if (chance < runOdds || PasswordManager.Instance.CheckPassword("sprint"))
    {
      DisplayMessage("You leave the town.");
      yield return new WaitForSeconds(2);
      if (townOverCallback != null)
      {
        townOverCallback(false);
      }
    }
    else
    {
      DisplayMessage(townie.name + " isn't done talking to you.");
      StartCoroutine(NextAction());
    }
  }

  IEnumerator OnBattleOver()
  {
    var message = (player.broke ? townie.name : player.name) + " has won the trade";

    if (!player.broke)
    {
      var reward = Mathf.RoundToInt((Random.Range(townie.rewardRange.x, townie.rewardRange.y) + Random.Range(townie.rewardRange.x, townie.rewardRange.y)) / 2);

      if ( reward > 0 )
      {
        MusicManager.Instance.PlaySound(Sounds.GetItem);

        message += "! ";

        switch (townie.rewardType)
        {
          case BattleReward.Heal:
            message += player.name + " recovers " + reward + " Health.";
            player.IncreaseHealth(reward);
            break;
          case BattleReward.Health:
            message += player.name + " gains " + reward + " Max Health.";
            player.IncreaseMaxHealth(reward);
            break;
          case BattleReward.Attack:
            message += player.name + " gains " + reward + " ATK.";
            player.IncreaseAttack(reward);
            break;
          case BattleReward.Talk:
            message += player.name + " gains " + reward + " TLK.";
            player.IncreaseTalk(reward);
            break;
          case BattleReward.Gold:
          default:
            message += player.name + " receives " + reward + " Gold.";
            player.IncreaseGold(reward);
            break;
        }
      }
      else
      {
        message += ", but " + townie.name + " backs out of the deal!";
      }
    }

    DisplayMessage(message);
    UpdatePlayerStats();

    yield return new WaitForSeconds(2);

    if (townOverCallback != null)
    {
      townOverCallback(!player.broke);
    }
  }
}