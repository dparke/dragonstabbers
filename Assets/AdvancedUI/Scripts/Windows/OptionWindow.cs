using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class OptionWindow : GenericWindow {

  public ToggleGroup musicGroup;
  public ToggleGroup soundsGroup;

  public int music
  {
    get
    {
      var total = musicGroup.transform.childCount;
      for (var i = 0; i < total; i++)
      {
        var toggle = musicGroup.transform.GetChild(i).GetComponent<Toggle>();
        if (toggle.isOn)
        {
          return i;
        }
      }
      return 0;
    }
    set
    {
      value = (int)Mathf.Repeat(value, musicGroup.transform.childCount);
      var currentSelection = musicGroup.ActiveToggles().FirstOrDefault();
      if (currentSelection != null)
      {
        currentSelection.isOn = false;
      }

      currentSelection = musicGroup.gameObject.transform.GetChild(value).GetComponent<Toggle>();
      currentSelection.isOn = true;
    }
  }

  public int sounds
  {
    get
    {
      var total = soundsGroup.transform.childCount;
      for (var i = 0; i < total; i++)
      {
        var toggle = soundsGroup.transform.GetChild(i).GetComponent<Toggle>();
        if (toggle.isOn)
        {
          return i;
        }
      }
      return 0;
    }
    set
    {
      value = (int)Mathf.Repeat(value, soundsGroup.transform.childCount);
      var currentSelection = soundsGroup.ActiveToggles().FirstOrDefault();
      if (currentSelection != null)
      {
        currentSelection.isOn = false;
      }

      currentSelection = soundsGroup.gameObject.transform.GetChild(value).GetComponent<Toggle>();
      currentSelection.isOn = true;
    }
  }

  private string codeString = "uuddlrlrba";
  private string inputString;

  public override void Open()
  {
    inputString = "";
    music = PlayerPrefs.GetInt("Music", 1);
    sounds = PlayerPrefs.GetInt("Sounds", 1);

    base.Open();
  }

  public void OnSave()
  {
    MusicManager.Instance.MusicOption(music > 0 ? true : false);
    PlayerPrefs.SetInt("Music", music);
    MusicManager.Instance.SoundsOption(sounds > 0 ? true : false);
    PlayerPrefs.SetInt("Sounds", sounds);

    OnNextWindow();
  }

  public void OnKeyPress(string key)
  {
    if (inputString.Length < codeString.Length)
    {
      inputString += key;
    }

    if (inputString == codeString)
    {
      PasswordManager.Instance.TogglePassword("konami");
      inputString = "";
    }

    /*
    if (inputString == codeString)
    {
      if (PlayerPrefs.GetInt("KONAMI", 0) == 0)
      {
        MusicManager.Instance.PlaySound(Sounds.Secret);
        PlayerPrefs.SetInt("KONAMI", 1);
      }
      else
      {
        MusicManager.Instance.PlaySound(Sounds.Key);
        PlayerPrefs.SetInt("KONAMI", 0);
      }

      inputString = "";
    }
    */
  }
}