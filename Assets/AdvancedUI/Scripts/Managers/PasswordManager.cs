using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Password
{
  public string tip;
  public string reveal;
  public string hide;
  public string sign;
}

public class PasswordManager : MonoBehaviour {

  private static PasswordManager instance;
  public static PasswordManager Instance { get { return instance; } }

  public WindowManager windowManager
  {
    get { return GenericWindow.manager; }
  }

  public Dictionary<string, Password> passwords = new Dictionary<string, Password>()
  {
    {"konami", new Password { tip = "Remember, skiddos, the only real option is konami.",
                              reveal = "Always remember the classics.",
                              hide = "It's a secret to everyone!",
                              sign = "k" } },
    {"justin", new Password { tip = "Don't you know the Mother Brain of all passwords?",
                              reveal = "That's the first name...",
                              hide = "I heard it was all a checksum fluke.",
                              sign = "t" } },
    {"bailey", new Password { tip = "Don't you know the Mother Brain of all passwords?",
                              reveal = "That's the last name...",
                              hide = "Really? Hard mode?",
                              sign = "a" } },
    {"payday", new Password { tip = "The time when the job pays out and the checks clear.",
                              reveal = "Ka-ching!",
                              hide = "Easy come, easy go...",
                              sign = "$" } },
    {"source", new Password { tip = "This is not the end.\nIt's kind of like a code for the beginning.",
                              reveal = "The truth is in code.",
                              hide = "You can choose not to see.",
                              sign = "s" } },
    {"berlin", new Password { tip = "All cities are the same.\nTo enter, you must know its name.",
                              reveal = "Never forget the fallen.",
                              hide = "Buried once again.",
                              sign = "@" } },
    {"sleeve", new Password { tip = "What? You looking for a second chance that's 'ready to wear'?",
                              reveal = "The real cost: priceless.",
                              hide = "Sure, let's pretend we can walk that back.",
                              sign = "_" } },
    {"betray", new Password { tip = "When you trust a dragon, this is what it will do: Stab you in the back.",
                              reveal = "Sold out for money and influence.",
                              hide = "Pull out the knife, it only bleeds worse.",
                              sign = "M" } },
    {"sprint", new Password { tip = "Dwarves are naturals.",
                              reveal = "RUN BOY RUN",
                              hide = "They'll hobble you to keep you under their thumb.",
                              sign = "R" } },
    {"baxter", new Password { tip = "Visionary, hacker, friend.",
                              reveal = "Toss a coin to your hacker.",
                              hide = "Any well can run dry, I suppose...",
                              sign = "W" } },
    {"legion", new Password { tip = "Many & one, master creator, voracious consumer... redeemed?",
                              reveal = "Many hands make light work.",
                              hide = "Watch your back, if you can.",
                              sign = "L" } },
    {"vision", new Password { tip = "If you want to SEE the future, you need to have...",
                              reveal = "I can see clearly now...",
                              hide = "The clouds are rolling in...",
                              sign = "V" } },
    {"xyzzy", new Password { tip = "FOOL.",
                              reveal = "Ultra Cheat Mode: Activated",
                              hide = "Ultra Cheat Mode: Deactivated",
                              sign = "U" } },
  };

  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
  }

  public bool TogglePassword(string password)
  {
    if (PasswordManager.Instance.passwords.ContainsKey(password))
    {
      if (PlayerPrefs.GetInt(password, 0) == 0)
      {
        MusicManager.Instance.PlaySound(Sounds.Secret);
        PlayerPrefs.SetInt(password, 1);
        DisplayMessage(passwords[password].reveal, 3);
      }
      else
      {
        MusicManager.Instance.PlaySound(Sounds.Key);
        PlayerPrefs.SetInt(password, 0);
        DisplayMessage(passwords[password].hide, 3);
      }
      return true;
    }
    else
    {
      DisplayMessage("Password not recognized.");
      return false;
    }
  }

  public bool CheckPassword(string password)
  {
    if (PasswordManager.Instance.passwords.ContainsKey(password) && PlayerPrefs.GetInt("xyzzy", 0) > 0)
    {
      return true;
    }

    if (PasswordManager.Instance.passwords.ContainsKey(password) && PlayerPrefs.GetInt("konami", 0) > 0)
    {
      if (PlayerPrefs.GetInt(password, 0) > 0)
      {
        return true;
      }
    }

    return false;
  }

  public string GetTip()
  {
    if (!CheckPassword("konami"))
    {
      return passwords["konami"].tip;
    }
    else
    {
      Dictionary<string, Password> availableTips = new Dictionary<string, Password>();
      availableTips = passwords
                      .Where(x => PlayerPrefs.GetInt(x.Key, 0) == 0)
                      .ToDictionary(p => p.Key, p => p.Value);

      /*
      foreach (Password password in availableTips.Values)
      {
        Debug.Log("GetTip includes: " + password.tip);
      }
      */

      return availableTips.ElementAt(Random.Range(0, availableTips.Count)).Value.tip;
    }
  }

  void DisplayMessage(string text, float closeDelay = 2f)
  {
    var messageWindow = windowManager.Open((int)Windows.MessageWindow - 1, false) as MessageWindow;
    messageWindow.ExtendedDisplay(text, closeDelay);
  }
}