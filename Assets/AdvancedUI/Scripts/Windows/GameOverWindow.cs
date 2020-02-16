using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameOverWindow : GenericWindow {

	public Text leftStatsLabel;
	public Text leftStatsValues;
	public Text rightStatsLabel;
	public Text rightStatsValues;
  public Text guruHeading;
  public Text guruTip;
  public Button nextButton;
	public float statsDelay = 1f;

  private List<string> runResults;
  private bool runVictory;
	private int currentStat = 0;
	private float delay = 0;

	private void UpdateStatText(Text label, Text value) {
		label.text += runResults[currentStat] + "\n";
		value.text += runResults[currentStat + 1] + "\n"; ;
	}

	private void ShowNextStat(){

		if (currentStat > runResults.Count - 1) {
      guruHeading.gameObject.SetActive(true);
      nextButton.enabled = true;
      if (runVictory)
      {
        guruTip.alignment = TextAnchor.MiddleCenter;
        guruTip.text = "You did the thing, skiddo!\nDrop by Daedalus and drinks are on me.";
      } else
      {
        guruTip.text = PasswordManager.Instance.GetTip();
      }
			currentStat = -1;
			return;
		}

		if (currentStat < runResults.Count / 2) {
			UpdateStatText (leftStatsLabel, leftStatsValues);
		} else {
			UpdateStatText (rightStatsLabel, rightStatsValues);
		}
    
		currentStat += 2;
	}

	void Update(){
		delay += Time.deltaTime;

		if (delay > statsDelay && currentStat != -1) {

			ShowNextStat ();
			delay = 0;
		}
	}

	public void ClearText(){
		leftStatsLabel.text = "";
		leftStatsValues.text = "";
		rightStatsLabel.text = "";
		rightStatsValues.text = "";
		guruTip.text = "";
	}

	public override void Open ()
	{
		ClearText ();
    guruHeading.gameObject.SetActive(false);
    if (PasswordManager.Instance.CheckPassword("konami"))
    {
      guruHeading.text += "  ";
      foreach (KeyValuePair<string, Password> entry in PasswordManager.Instance.passwords)
      {
        guruHeading.text += PasswordManager.Instance.CheckPassword(entry.Key) ? entry.Value.sign + "" : "";
      }
    }
    nextButton.enabled = false;
    runResults = new List<string>();

		base.Open ();
	}

  public void CurrentRunStats(List<string> runStats)
  {
    foreach (string stat in runStats)
    {
      runResults.Add(stat);
    }
  }

  public void CurrentRunQuest(List<string> runQuest)
  {
    foreach (string stat in runQuest)
    {
      runResults.Add(stat);
    }
  }

  public void CurrentRunVictory(bool isVictory)
  {
    runVictory = isVictory;
  }

  public override void Close ()
	{
		base.Close ();
		currentStat = 0;
	}

	public void OnNext(){
		OnNextWindow ();
	}

}
