using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageWindow : GenericWindow {

  public float closeDelay = 2f;

  private float delayOverride;
  private float delay;
  private bool closing;
  private Text textInstance;

  public string text
  {
    set { textInstance.text = value; }
  }

  protected override void Awake()
  {
    textInstance = GetComponentInChildren<Text>();

    base.Awake();
  }

  public override void Open()
  {
    base.Open();
    closing = true;
    delayOverride = closeDelay;
    delay = 0;
  }

  public void ExtendedDisplay(string msg, float delay)
  {
    text = msg;
    delayOverride = delay;
  }

  private void Update()
  {
    if (closing)
    {
      delay += Time.deltaTime;
      if (delay >= delayOverride)
      {
        Close();
        closing = false;
      }
    }
  }

}