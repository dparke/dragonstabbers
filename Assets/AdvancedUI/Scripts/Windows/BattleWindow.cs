using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BattleWindow : GenericWindow {

  public Image[] decorations;
  public GameObject actionsGroup;
  public Text monsterLabel;
  public Image monsterImage;
  public GenericBattleAction[] actions;
  public bool nextActionPlayer = true;
  [Range(0, .9f)]
  public float runOdds = .3f;
  public RectTransform windowRect;
  public RectTransform monsterRect;
  public delegate void BattleOver(bool playerWin, bool playerRan = false);
  public BattleOver battleOverCallback;

  private ShakeManager shakeManager;
  private Actor player;
  private Actor monster;

  private System.Random rand = new System.Random();

  protected override void Awake()
  {
    shakeManager = GetComponent<ShakeManager>();
    base.Awake();
  }

  public override void Open()
  {
    base.Open();

    MusicManager.Instance.ChangeMusic(Windows.BattleWindow);

    foreach (var decoration in decorations)
    {
      decoration.enabled = rand.NextDouble() >= .5;
    }

    actionsGroup.SetActive(false);
  }

  public void StartBattle(Actor target1, Actor target2)
  {
    player = target1;
    monster = target2;
    
    if (PasswordManager.Instance.CheckPassword("sprint"))
    {
        nextActionPlayer = true;
    } else
    {
        nextActionPlayer = rand.NextDouble() >= .5;
    }

    monsterImage.sprite = monster.avatar;

    if (nextActionPlayer)
    {
      DisplayMessage("A " + monster.name + " approaches!");
    } else
    {
      DisplayMessage(monster.name + " attacks first!");
    }

    StartCoroutine(NextAction());
    UpdateMonsterLabel();
  }

  public void OnAction(GenericBattleAction action, Actor target1, Actor target2, ShakeManager preparedShake)
  {
    action.Action(target1, target2, preparedShake);

    DisplayMessage(action.ToString());
    actionsGroup.SetActive(false);

    UpdatePlayerStats();
    UpdateMonsterLabel();

    StartCoroutine(NextAction());
  }

  public void OnPlayerAction(int id)
  {
    switch (id)
    {
      case 1:
        StartCoroutine(OnRun());
        break;
      default:
        var action = actions[id];
        OnAction(action, player, monster, shakeManager.PrepareShake(monsterRect));
        break;
    }

    nextActionPlayer = false;
  }

  public void OnMonsterAction()
  {
    var action = actions[0];
    OnAction(action, monster, player, shakeManager.PrepareShake(windowRect));
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

    if (!player.alive || !monster.alive)
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
        OnMonsterAction();
      }
    }
  }

  void UpdatePlayerStats()
  {
    ((StatsWindow)manager.GetWindow((int)Windows.StatsWindow - 1)).UpdateStats();
  }

  void UpdateMonsterLabel()
  {
    monsterLabel.text = monster.name + " HP " + monster.health.ToString("D2");
  }

  IEnumerator OnRun()
  {
    actionsGroup.SetActive(false);

    var chance = Random.Range(0, 1f);
    if(chance < runOdds || PasswordManager.Instance.CheckPassword("sprint"))
    {
      DisplayMessage("You were able to run away.");
      yield return new WaitForSeconds(2);
      if (battleOverCallback != null)
      {
        battleOverCallback(player.alive, true);
      }
    }
    else
    {
      DisplayMessage("You were not able to run away.");
      StartCoroutine(NextAction());
    }
  }

  IEnumerator OnBattleOver()
  {
    var message = (player.alive ? player.name : monster.name) + " has won the battle";

    var reward = Mathf.RoundToInt((Random.Range(monster.rewardRange.x, monster.rewardRange.y) + Random.Range(monster.rewardRange.x, monster.rewardRange.y)) / 2);

    if (player.alive)
    {
      MusicManager.Instance.PlaySound(monster.deathSound);

      if (reward > 0)
      {
        message += "! ";

        switch (monster.rewardType)
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
        message += ", but " + monster.name + " has nothing.";
      }
    }

    DisplayMessage(message);
    UpdatePlayerStats();

    yield return new WaitForSeconds(2);

    if(battleOverCallback != null)
    {
      battleOverCallback(player.alive);
    } 
  }
}