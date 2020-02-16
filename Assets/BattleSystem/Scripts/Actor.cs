using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum BattleReward { None, Heal, Health, Attack, Talk, Gold }

public class Actor : ScriptableObject {

  public new string name
  {
    get { return PasswordManager.Instance.CheckPassword("source") ? secretName : publicName; }
    set { value = publicName; }
  }

  [Header("Display")]
  public string publicName;
  public string secretName;
  public Sprite avatar;
  public Sounds attackSound;
  public Sounds hitSound;
  public Sounds deathSound;

  [Space]
  [Header("Stats")]
  public int health;
  public int maxHealth;
  public int gold;
  public int degradation;
  public Vector2 attackRange = Vector2.one;
  public float attackDelta = 1;
  public Vector2 talkRange = Vector2.one;
  public float talkDelta = 1;

  [Space]
  [Header("Discoveries")]
  public bool foundWell;

  [Space]
  [Header("Rewards")]
  public BattleReward rewardType;
  public Vector2 rewardRange = Vector2.one;
  public float rewardDelta = 1;

  public bool alive
  {
    get { return health > 0; }
  }

  public bool broke
  {
    get { return gold <= 0; }
  }

  public void DecreaseHealth(int value)
  {
    health = Mathf.Max(health - value, 0);
  }

  public void IncreaseHealth(int value)
  {
    health = Mathf.Min(health + value, maxHealth);
  }

  public void DecreaseMaxHealth(int value)
  {
    maxHealth = Mathf.Max(maxHealth - value, 1);
    health = Mathf.Min(health, maxHealth);
  }

  public void IncreaseMaxHealth(int value)
  {
    maxHealth += value;
    IncreaseHealth(value);
  }

  public void ResetHealth()
  {
    health = maxHealth;
  }

  public void DecreaseGold(int value)
  {
    gold = Mathf.Max(gold - value, 0);
  }

  public void IncreaseGold(int value)
  {
    gold += value;
  }

  public void IncreaseAttack(int value)
  {
    attackRange.y += value;
  }

  public void DecreaseAttack(int value)
  {
    attackRange.y = Mathf.Max(attackRange.y - value, attackRange.x);
  }

  public void IncreaseTalk(int value)
  {
    talkRange.y += value;
  }

  public void DecreaseTalk(int value)
  {
    talkRange.y = Mathf.Max(talkRange.y - value, talkRange.x);
  }

  public T Clone<T>(int difficulty = 0) where T : Actor
  {
    var clone = ScriptableObject.CreateInstance<T>();

    clone.name = name;
    clone.publicName = publicName;
    clone.secretName = secretName;
    clone.avatar = avatar;
    clone.attackSound = attackSound;
    clone.hitSound = hitSound;
    clone.deathSound = deathSound;
    clone.health = health + difficulty;
    clone.maxHealth = maxHealth + difficulty;
    clone.gold = gold + difficulty;
    clone.degradation = degradation;
    clone.foundWell = foundWell;
    clone.rewardType = rewardType;
    clone.rewardRange.x = rewardRange.x;
    clone.rewardRange.y = rewardRange.y + (difficulty * rewardDelta);
    clone.attackRange.x = attackRange.x;
    clone.attackRange.y = attackRange.y + (difficulty * attackDelta);
    clone.talkRange.x = talkRange.x;
    clone.talkRange.y = talkRange.y + (difficulty * talkDelta);

    return clone;
  }

  public bool Resleeve()
  {
    // If the player has not found the Well, they need to pay gold
    if (!foundWell)
    {
        DecreaseGold(5 + degradation);
    }
    
    // Unless player has unlocked LEGION and Found The Well, they lose stats on resleeve
    if (!(PasswordManager.Instance.CheckPassword("legion") && foundWell))
    {
        degradation += Mathf.RoundToInt((Random.Range(1, 3) + Random.Range(1, 3)) / 2);
        
        DecreaseMaxHealth(Random.Range(0, degradation));
        DecreaseAttack(Random.Range(0, degradation));
        DecreaseTalk(Random.Range(0, degradation));
    }
    
    ResetHealth();

    // If any critical stats has hit 0, the resleeve failed.
    if (maxHealth == 0 || attackRange.y == 0 || talkRange.y == 0)
    {
      return false;
    }

    return true;
  }
}