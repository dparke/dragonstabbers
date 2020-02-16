using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomMapTester : MonoBehaviour
{

  [Header("Map Dimensions")]
  public int mapWidth = 21;
  public int mapHeight = 21;

  [Space]
  [Header("Vizualize Map")]
  public GameObject mapContainer;
  public GameObject tilePrefab;
  public Vector2 tileSize = new Vector2(16, 16);

  [Space]
  [Header("Map Sprites")]
  public Texture2D islandTexture;
  public Texture2D fowTexture;

  [Space]
  [Header("Player")]
  public GameObject playerPrefab;
  public GameObject player;
  public int distance = 3;
  [Range(0, .9f)]
  public float randomBattleOdds = .05f;

  [Space]
  [Header("Actor Templates")]
  public Actor playerTemplate;
  public Actor monsterTemplate;
  public List<Actor> landscapeSpawns;
  public float rareHillsOdds = .3f;
  public List<Actor> rareHillsSpawns;
  public float rareTreeOdds = .3f;
  public List<Actor> rareTreeSpawns;
  public List<Actor> dungeonSpawns;
  private List<Actor> dungeonSpawnsRemaining;
  public List<Actor> townieSpawns;
  private List<Actor> townieSpawnsRemaining;
  private bool isAscending;

  [Space]
  [Header("Decorate Map")]
  [Range(0, .9f)]
  public float erodePercent = .5f;
  public int erodeIterations = 2;
  [Range(0, .9f)]
  public float treePercent = .3f;
  [Range(0, .9f)]
  public float hillPercent = .2f;
  [Range(0, .9f)]
  public float mountainsPercent = .1f;
  [Range(0, .9f)]
  public float townPercent = .15f;
  [Range(0, .9f)]
  public float monsterPercent = .1f;
  [Range(0, .9f)]
  public float lakePercent = .05f;

  public Map map;

  private int tmpX;
  private int tmpY;
  private Sprite[] islandTileSprites;
  private Sprite[] fowTileSprites;
  private BattleWindow battleWindow;
  private TownWindow townWindow;
  private Actor playerActor;
  private StatsWindow statsWindow;
  private QuestWindow questWindow;

  public WindowManager windowManager
  {
    get { return GenericWindow.manager; }
  }

  public void Reset()
  {
    islandTileSprites = Resources.LoadAll<Sprite>(islandTexture.name);
    fowTileSprites = Resources.LoadAll<Sprite>(fowTexture.name);

    dungeonSpawnsRemaining = new List<Actor>(dungeonSpawns);

    for (int i = 0; i < dungeonSpawnsRemaining.Count; i++)
    {
      Actor temp = dungeonSpawnsRemaining[i];
      int randomIndex = Random.Range(i, dungeonSpawnsRemaining.Count);
      dungeonSpawnsRemaining[i] = dungeonSpawnsRemaining[randomIndex];
      dungeonSpawnsRemaining[randomIndex] = temp;
    }

    townieSpawnsRemaining = new List<Actor>(townieSpawns);
    isAscending = false;

    map = new Map();
    MakeMap();

    questWindow = windowManager.Open((int)Windows.QuestWindow - 1, false) as QuestWindow;
    questWindow.target = map;
    questWindow.InitQuest();

    StartCoroutine(AddPlayer());
  }

  public void Shutdown()
  {
    ClearMapContainer();
  }

  IEnumerator AddPlayer()
  {
    yield return new WaitForEndOfFrame();
    CreatePlayer();

  }

  public void MakeMap()
  {
    map.NewMap(mapWidth, mapHeight);
    map.CreateIsland(
      erodePercent,
      erodeIterations,
      treePercent,
      hillPercent,
      mountainsPercent,
      townPercent,
      monsterPercent,
      lakePercent,
      dungeonSpawns.Count
    );
    CreateGrid();
    CenterMap(map.castleTile.id);
  }

  void CreateGrid()
  {
    ClearMapContainer();

    var total = map.tiles.Length;
    var maxColumns = map.columns;
    var column = 0;
    var row = 0;

    for (var i = 0; i < total; i++)
    {

      column = i % maxColumns;

      var newX = column * tileSize.x;
      var newY = -row * tileSize.y;

      var go = Instantiate(tilePrefab);
      go.name = "Tile " + i;
      go.transform.SetParent(mapContainer.transform);
      go.transform.position = new Vector3(newX, newY, 0);

      DecorateTile(i);

      if (column == (maxColumns - 1))
      {
        row++;
      }
    }
  }

  private void DecorateTile(int tileID)
  {
    var tile = map.tiles[tileID];
    var spriteID = tile.autotileID;
    var go = mapContainer.transform.GetChild(tileID).gameObject;

    if (spriteID >= 0)
    {
      var sr = go.GetComponent<SpriteRenderer>();

      if (tile.visited)
      {
        sr.sprite = islandTileSprites[spriteID];
      }
      else
      {
        tile.CalculateFoWAutotileID();
        sr.sprite = fowTileSprites[Mathf.Min(tile.fowAutotileID, fowTileSprites.Length - 1)];
      }
    }
  }

  public void CreatePlayer()
  {
    player = Instantiate(playerPrefab);
    player.name = "Player";
    player.transform.SetParent(mapContainer.transform);

    var controller = player.GetComponent<MapMovementController>();
    controller.map = map;
    controller.tileSize = tileSize;
    controller.tileActionCallback += TileActionCallback;

    var moveScript = Camera.main.GetComponent<MoveCamera>();
    moveScript.target = player;

    controller.MoveTo(map.castleTile.id);
    playerActor = playerTemplate.Clone<Actor>();

    // Power-up Password Checks
    if (PasswordManager.Instance.CheckPassword("justin"))
    {
      playerActor.IncreaseMaxHealth(5);
      playerActor.IncreaseTalk(2);
    }
    if (PasswordManager.Instance.CheckPassword("bailey"))
    {
      playerActor.IncreaseMaxHealth(5);
      playerActor.IncreaseAttack(2);
    }
    if (PasswordManager.Instance.CheckPassword("payday"))
    {
      playerActor.IncreaseGold(10);
    }
    playerActor.ResetHealth();

    statsWindow = windowManager.Open((int)Windows.StatsWindow - 1, false) as StatsWindow;
    statsWindow.target = playerActor;
    statsWindow.UpdateStats();

    questWindow.player = playerActor;
    questWindow.UpdateQuest();
  }

  void TileActionCallback(int type)
  {
    var tileID = player.GetComponent<MapMovementController>().currentTile;
    if ((TileType)type == TileType.Hills)
    {
      // If you have VISION, Hills let you see further
      VisitTile(tileID, distance + (PasswordManager.Instance.CheckPassword("vision") ? 10 : 2));
    }
    else
    {
      VisitTile(tileID);
    }
    questWindow.UpdateQuest();

    switch ((TileType)type)
    {
      case TileType.Towns:
        if (PasswordManager.Instance.CheckPassword("berlin"))
        {
          if (playerActor.broke)
          {
            DisplayMessage("Without any gold, the town does not let you enter.");
          }
          else {
            DisplayMessage("You enter the town.");
            StartTown(map.tiles[tileID]);
          }
        }
        break;
      case TileType.Ruins:
        DisplayMessage("The town lies ruined and deserted.");
        break;
      case TileType.Castle:
        if (map.evoTile == null)
        {
          DisplayMessage("The castle is empty. There is no ruler.\nAscend the Tower and restore the Tree.", 4f);
        }
        else
        {
          DisplayMessage("Your quest is complete, yet the castle remains empty.\nThere is work left to do, but not here.\nTell the Well the tree blooms.", 6f);
          StartCoroutine(ExitGame());
        }
        break;
      case TileType.Monster:
        StartBattle();
        break;
      case TileType.Skull:
        DisplayMessage("The dungeon has collapsed, but something has escaped.");
        break;
      case TileType.Well:
        if (playerActor.foundWell)
        {
          if (PasswordManager.Instance.CheckPassword("legion"))
          {
              DisplayMessage("The Well hums with life and potential for change under its new custodian.");
          } else
          {
              DisplayMessage("The Well will keep you 'alive', after a fashion.");
          }
        } else {
          if (PasswordManager.Instance.CheckPassword("legion"))
          {
              DisplayMessage("A new bartender, familiar and yet strange, greets you and grants you access to The Well.");
          }
          else
          {
              DisplayMessage("InToGetOut greets you and nods, confirming your access to The Well.");
          }

          MusicManager.Instance.PlaySound(Sounds.Secret);
          playerActor.foundWell = true;
        }
        break;
      case TileType.Tower:
        if (!questWindow.IsTowerUnlocked())
        {
          DisplayMessage("The forbidding Tower does not grant you access.");
        }
        else
        {
          StartCoroutine(EnterTower());
        }
        break;
      default:
        var chance = Random.Range(0, 1f);
        if (chance < randomBattleOdds && map.evoTile == null)
        {
          StartBattle();
        }
        break;
    }
  }

  void DisplayMessage(string text, float closeDelay = 2f)
  {
    var messageWindow = windowManager.Open((int)Windows.MessageWindow - 1, false) as MessageWindow;
    messageWindow.ExtendedDisplay(text, closeDelay);
  }

  void ClearMapContainer()
  {
    var children = mapContainer.transform.GetComponentsInChildren<Transform>();
    for (var i = children.Length - 1; i > 0; i--)
    {
      Destroy(children[i].gameObject);
    }
  }

  void CenterMap(int index)
  {

    var camPos = Camera.main.transform.position;
    var width = map.columns;

    PosUtil.CalculatePos(index, width, out tmpX, out tmpY);

    camPos.x = tmpX * tileSize.x;
    camPos.y = -tmpY * tileSize.y;
    Camera.main.transform.position = camPos;
  }

  void VisitTile(int index, int visionRange = 0)
  {

    visionRange = visionRange > 0 ? visionRange : distance;

    int column, newX, newY, row = 0;

    PosUtil.CalculatePos(index, map.columns, out tmpX, out tmpY);

    var half = Mathf.FloorToInt(visionRange / 2f);
    tmpX -= half;
    tmpY -= half;

    var total = visionRange * visionRange;
    var maxColumns = visionRange - 1;

    for (int i = 0; i < total; i++)
    {

      column = i % visionRange;

      newX = column + tmpX;
      newY = row + tmpY;

      PosUtil.CalculateIndex(newX, newY, map.columns, out index);

      if (index > -1 && index < map.tiles.Length)
      {
        var tile = map.tiles[index];
        tile.visited = true;
        DecorateTile(index);

        foreach (var neighbor in tile.neighbors)
        {

          if (neighbor != null)
          {

            if (!neighbor.visited)
            {

              neighbor.CalculateFoWAutotileID();
              DecorateTile(neighbor.id);
            }

          }
        }
      }

      if (column == maxColumns)
      {
        row++;
      }
    }

  }

  public void StartBattle()
  {
    Actor monsterActor = null;
    Tile tile = map.tiles[player.GetComponent<MapMovementController>().currentTile];
    TileType tileID = (TileType)tile.autotileID;

    // Determine the enemy for the fight based on the tile triggering the battle
    switch (tileID)
    {
      case TileType.Monster:
        if (tile != null && tile.savedEncounter != null)
        {
          monsterActor = tile.savedEncounter;
        }
        else
        {
          int r = Random.Range(0, dungeonSpawnsRemaining.Count);
          monsterActor = dungeonSpawnsRemaining[r].Clone<Actor>(questWindow.difficulty);
          tile.savedEncounter = monsterActor;
          dungeonSpawnsRemaining.RemoveAt(r);
        }
        break;
      case TileType.Tower:
        monsterActor = townieSpawnsRemaining[0].Clone<Actor>(questWindow.difficulty);
        break;
      case TileType.Hills:
        var hillsChance = Random.Range(0, 1f);
        if (hillsChance < rareHillsOdds)
        {
          monsterActor = rareHillsSpawns[Random.Range(0, rareHillsSpawns.Count)].Clone<Actor>(questWindow.difficulty);
        }
        else
        {
          monsterActor = landscapeSpawns[Random.Range(0, landscapeSpawns.Count)].Clone<Actor>(questWindow.difficulty);
        }
        break;
      case TileType.Tree:
        var treeChance = Random.Range(0, 1f);
        if (treeChance < rareTreeOdds)
        {
          monsterActor = rareTreeSpawns[Random.Range(0, rareTreeSpawns.Count)].Clone<Actor>(questWindow.difficulty);
        }
        else
        {
          monsterActor = landscapeSpawns[Random.Range(0, landscapeSpawns.Count)].Clone<Actor>(questWindow.difficulty);
        }
        break;
      default:
        monsterActor = landscapeSpawns[Random.Range(0, landscapeSpawns.Count)].Clone<Actor>(questWindow.difficulty);
        break;
    }
    monsterActor.ResetHealth();

    battleWindow = windowManager.Open((int)Windows.BattleWindow - 1, false) as BattleWindow;
    battleWindow.battleOverCallback += BattleOver;
    battleWindow.StartBattle(playerActor, monsterActor);
    TogglePlayerMovement(false);
  }

  public void EndBattle()
  {
    if (!isAscending)
    {
      MusicManager.Instance.ChangeMusic(Windows.GameWindow);
      TogglePlayerMovement(true);
    }
    battleWindow.Close();
  }

  public void StartTown(Tile tile = null)
  {
    Actor townieActor;
    if (tile != null && tile.savedEncounter != null)
    {
      townieActor = tile.savedEncounter;
    }
    else
    {
      var r = Random.Range(0, townieSpawns.Count);
      townieActor = townieSpawns[r].Clone<Actor>(questWindow.difficulty);
      townieActor.ResetHealth();
      tile.savedEncounter = townieActor;
    }

    townWindow = windowManager.Open((int)Windows.TownWindow - 1, false) as TownWindow;
    townWindow.townOverCallback += TownOver;
    townWindow.StartTown(playerActor, townieActor);
    TogglePlayerMovement(false);
  }

  public void EndTown()
  {
    townWindow.Close();
    MusicManager.Instance.ChangeMusic(Windows.GameWindow);
    TogglePlayerMovement(true);
  }

  private void TogglePlayerMovement(bool value)
  {
    player.GetComponent<MapMovementController>().enabled = value;
    Camera.main.GetComponent<MoveCamera>().enabled = value;
  }

  private void BattleOver(bool playerWin, bool playerRan)
  {
    EndBattle();
    battleWindow.battleOverCallback -= BattleOver;

    if (!playerWin)
    {
      PlayerDied();
    }
    else if (!playerRan)
    {
      var tileID = player.GetComponent<MapMovementController>().currentTile;
      var tile = map.tiles[tileID];
      if (tile.autotileID == (int)TileType.Monster)
      {
        tile.autotileID = (int)TileType.Skull;
        DisplayMessage("You barely escape as the dungeon collapses behind you.", 3f);
        DecorateTile(tileID);
      }
      else if (tile.autotileID == (int)TileType.Tower)
      {
        if (townieSpawnsRemaining.Count > 1)
        {
          StartCoroutine(AscendTower());
        }
        else
        {
          tile.autotileID = (int)TileType.Evo;
          DecorateTile(tileID);
          DisplayMessage("The Tower is overthrown, return to the Castle.", 3f);
          MusicManager.Instance.ChangeMusic(Windows.StartWindow);
          TogglePlayerMovement(true);
        }
      }
    }

    questWindow.UpdateQuest();
  }

  private void PlayerDied()
  {
    if (PasswordManager.Instance.CheckPassword("sleeve"))
    {
      if (isAscending)
      {
        DisplayMessage("You have been slagged.\nThere is no return.", 3f);
        return;
      }

      // Eternal life costs money, honey. (Or, not, if you found The Well)
      if (playerActor.foundWell || playerActor.gold > 5 + playerActor.degradation)
      {
        player.GetComponent<MapMovementController>().MoveTo(map.castleTile.id);

        if (playerActor.Resleeve())
        {
          statsWindow.UpdateStats();
          DisplayMessage("You wake up groggy, but alive.");
          return;
        }
        else
        {
          statsWindow.UpdateStats();
          DisplayMessage("Sometimes science fails us.");
        }
      }
      else
      {
        DisplayMessage("Eternal life ain't cheap.\nIf you can't pay, you can't play.", 3f);
      }
    }

    // Resurrection is not enabled.
    Debug.Log("True Death: Destroying Player");
    Destroy(player);
    playerActor = null;
    StartCoroutine(ExitGame());
  }

  private void TownOver(bool playerWin)
  {
    EndTown();

    if (playerActor.broke)
    {
      DisplayMessage("Out of gold, you are forced to leave the town.", 3f);
    }
    else if (playerWin)
    {
      var tileID = player.GetComponent<MapMovementController>().currentTile;
      var tile = map.tiles[tileID];
      if (tile.autotileID == (int)TileType.Towns)
      {
        tile.autotileID = (int)TileType.Ruins;
        DecorateTile(tileID);
        questWindow.UpdateQuest();
      }
    }
  }

  public void EndRun()
  {
    StartCoroutine(ExitGame());
  }

  public IEnumerator ExitGame()
  {
    yield return new WaitForSeconds(4);
    DisplayMessage("The game is over.");
    yield return new WaitForSeconds(2);
    GameOverWindow gameOverWindow = windowManager.Open((int)Windows.GameOverWindow - 1) as GameOverWindow;
    gameOverWindow.CurrentRunStats(statsWindow.FinalStats());
    gameOverWindow.CurrentRunQuest(questWindow.FinalQuest());
    gameOverWindow.CurrentRunVictory(true);
  }

  public IEnumerator EnterTower()
  {
    TogglePlayerMovement(false);
    isAscending = true;
    MusicManager.Instance.PlaySound(Sounds.Secret);
    DisplayMessage("Having mastered both dungeon and town, you enter the Tower.", 3f);
    yield return new WaitForSeconds(3);
    TowerFloorApproaching(townieSpawnsRemaining[0].name, 3f);
    yield return new WaitForSeconds(3);
    StartBattle();
  }

  IEnumerator AscendTower()
  {
    float delay = 3f;
    TowerBossDefeated(townieSpawnsRemaining[0].name, delay);

    townieSpawnsRemaining.RemoveAt(0);
    yield return new WaitForSeconds(delay);

    TowerFloorApproaching(townieSpawnsRemaining[0].name, delay);

    yield return new WaitForSeconds(delay);
    StartBattle();
  }

  void TowerBossDefeated(string defeatedBoss, float delay)
  {
    // Show some flavor text specific to the boss just defeated...
    MusicManager.Instance.PlaySound(Sounds.Success);

    switch (defeatedBoss)
    {
      case "Doctor":
        DisplayMessage("The " + defeatedBoss + " expires, unable to treat their own wounds.", delay);
        break;
      case "Dr. V":
        DisplayMessage(defeatedBoss + " expires, unable to treat their own wounds.", delay);
        break;
      case "Mayor":
        DisplayMessage("The " + defeatedBoss + " couldn't persuade you to spare them.", delay);
        break;
      case "Cool Cat":
        DisplayMessage("The " + defeatedBoss + " couldn't persuade you to spare them.", delay);
        break;
      case "Inventor":
        DisplayMessage("The " + defeatedBoss + " dies without a brilliant idea to save themself.", delay);
        break;
      case "Augmentor X":
        DisplayMessage(defeatedBoss + " dies without a brilliant idea to save themself.", delay);
        break;
      case "Soldier":
        DisplayMessage("The " + defeatedBoss + " lived and died by the sword.", delay);
        break;
      case "Lt. Black":
        DisplayMessage(defeatedBoss + " lived and died by the vibroblade.", delay);
        break;
      case "Charlatan":
        DisplayMessage("For all of their lies and deceptions, the " + defeatedBoss + " can still be killed.", delay);
        break;
      case "CEO Sli_ck":
        DisplayMessage("For all of their lies and deceptions, " + defeatedBoss + " can still be killed.", delay);
        break;
      default:
        DisplayMessage("You defeat your foe and continue your ascent.", delay);
        break;
    }
  }

  void TowerFloorApproaching(string nextBoss, float delay)
  {
    // Show some flavor text specific to the boss you are about to fight
    switch (nextBoss)
    {
      case "Doctor":
        DisplayMessage("As you reach the next floor, you see " + nextBoss + " prepare to attack.", delay);
        break;
      case "Dr. V":
        DisplayMessage("As you reach the next floor, you see " + nextBoss + " prepare to attack.", delay);
        break;
      case "Mayor":
        DisplayMessage("As you reach the next floor, you see " + nextBoss + " prepare to attack.", delay);
        break;
      case "Cool Cat":
        DisplayMessage("As you reach the next floor, you see " + nextBoss + " prepare to attack.", delay);
        break;
      case "Inventor":
        DisplayMessage("As you reach the next floor, you see " + nextBoss + " prepare to attack.", delay);
        break;
      case "Augmentor X":
        DisplayMessage("As you reach the next floor, you see " + nextBoss + " prepare to attack.", delay);
        break;
      case "Soldier":
        DisplayMessage("As you reach the next floor, you see " + nextBoss + " prepare to attack.", delay);
        break;
      case "Lt. Black":
        DisplayMessage("As you reach the next floor, you see " + nextBoss + " prepare to attack.", delay);
        break;
      case "Charlatan":
        DisplayMessage("As you reach the next floor, you see " + nextBoss + " prepare to attack.", delay);
        break;
      case "CEO Sli_ck":
        DisplayMessage("As you reach the next floor, you see " + nextBoss + " prepare to attack.", delay);
        break;
      default:
        DisplayMessage("As you reach the next floor, another threat looms!", delay);
        break;
    }
  }
}