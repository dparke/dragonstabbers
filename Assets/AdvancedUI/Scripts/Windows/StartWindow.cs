using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StartWindow : GenericWindow {

	public Button continueButton;
  public Text maker;
  public Text signs;
  public Animator dragonLeft;
  public Animator dragonRight;
  public Animator swordLeft;
  public Animator swordRight;

  private bool firstOpen = true;

	public override void Open()
	{
    CheckContinue();
    CheckUnlocks();

		base.Open ();

    if (firstOpen)
    {
      firstOpen = false;
    }
    else
    {
      dragonLeft.SetBool("SkipEnter", true);
      dragonRight.SetBool("SkipEnter", true);
      swordLeft.SetBool("SkipEnter", true);
      swordRight.SetBool("SkipEnter", true);
    }

    MusicManager.Instance.ChangeMusic(Windows.StartWindow);
  }

  public void CheckContinue()
  {
    var canContinue = PasswordManager.Instance.CheckPassword("konami");

    continueButton.gameObject.SetActive(canContinue);

    if (continueButton.gameObject.activeSelf)
    {
      firstSelected = continueButton.gameObject;
    }
  }

  public void CheckUnlocks()
  {
    maker.text = "Quest by Blue Guru";
    signs.text = "";

    if (PasswordManager.Instance.CheckPassword("konami"))
    {
      maker.text = "Brought to you by the letters...";

      foreach (KeyValuePair<string, Password> entry in PasswordManager.Instance.passwords)
      {
        signs.text += PasswordManager.Instance.CheckPassword(entry.Key) ? entry.Value.sign + " " : "";
      }
    }
  }

	public void NewGame(){
		OnNextWindow ();
	}

	public void Continue(){
    GenericWindow.manager.Open((int)Windows.KeyboardWindow - 1);
  }

	public void Options(){
    GenericWindow.manager.Open((int)Windows.OptionWindow - 1);
  }
}
