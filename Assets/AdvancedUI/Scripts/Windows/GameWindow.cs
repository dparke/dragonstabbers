using UnityEngine;
using System.Collections;

public class GameWindow : GenericWindow {

  private RandomMapTester tester;

  protected override void Awake()
  {
    tester = GetComponent<RandomMapTester>();
    base.Awake();
  }

  public override void Open()
  {
    base.Open();

    MusicManager.Instance.ChangeMusic(Windows.GameWindow);

    tester.Reset();
    Camera.main.GetComponent<MoveCamera>().enabled = true;
  }

  public override void Close()
  {
    base.Close();
    tester.Shutdown();
    Camera.main.GetComponent<MoveCamera>().enabled = false;
  }

}
