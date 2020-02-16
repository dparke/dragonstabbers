using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class KeyboardWindow : GenericWindow {

	public Text inputField;
	public int maxCharacters = 6;
  public Image screenBlocker;

	private float delay = 0;
	private float curserDelay = .5f;
	private bool blink;
	private string _text = "";

  private bool inputAllowed;

  public override void Open()
  {
    _text = "";
    
    base.Open();
  }

  void Update(){
		var text = _text;

		if (_text.Length < maxCharacters) {
			text += "_";

			if (blink) {
				text = text.Remove (text.Length - 1);
			}
		}

		inputField.text = text;

		delay += Time.deltaTime;
		if (delay > curserDelay) {
			delay = 0;
			blink = !blink;
		}
	}

	public void OnKeyPress(string key){
		if (_text.Length < maxCharacters) {
			_text += key;
		}
	}

	public void OnDelete(){
		if (_text.Length > 0) {
			_text = _text.Remove (_text.Length - 1);
		}
	}

	public void OnAccept(){
    if (PasswordManager.Instance.TogglePassword(_text))
    {
      StartCoroutine(PasswordReveal());
    }
    else
    {
      _text = "";
    }
  }

  public void OnCancel(){
      OnPreviousWindow ();
	}

  public IEnumerator PasswordReveal()
  {
    screenBlocker.enabled = true;
    yield return new WaitForSeconds(3);
    _text = "";
    screenBlocker.enabled = false;
  }

  void DisplayMessage(string text, float closeDelay = 2f)
  {
    var messageWindow = manager.Open((int)Windows.MessageWindow - 1, false) as MessageWindow;
    messageWindow.ExtendedDisplay(text, closeDelay);
  }
}
