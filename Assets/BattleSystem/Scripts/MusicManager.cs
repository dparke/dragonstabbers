using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public enum Sounds { Secret, Key, Success, GetItem, Sword, EnemyHit, EnemyDie, BossHit, BossDie }

public class MusicManager : MonoBehaviour
{
  public AudioClip titleMusic;
  public AudioClip mapMusic;
  public AudioClip townMusic;
  public AudioClip battleMusic;
  public AudioSource musicSource;
  private Windows curWindow;
  private bool musicOn;

  public AudioClip secretSound;
  public AudioClip keySound;
  public AudioClip successSound;
  public AudioClip getItemSound;
  public AudioClip[] swordSounds;
  public AudioClip enemyHitSound;
  public AudioClip enemyDieSound;
  public AudioClip[] bossHitsSounds;
  public AudioClip bossDieSounds;
  public AudioSource soundsSource;
  private bool soundsOn;

  private static MusicManager instance;
  public static MusicManager Instance { get { return instance; } }

  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
  }

  private void Start()
  {
    MusicOption(PlayerPrefs.GetInt("Music", 1) > 0 ? true : false);
    SoundsOption(PlayerPrefs.GetInt("Sounds", 1) > 0 ? true : false);
  }

  public void ChangeMusic(Windows window)
  {
    if (curWindow != window)
    {
      switch (window)
      {
        case Windows.StartWindow:
          musicSource.clip = titleMusic;
          break;
        case Windows.GameWindow:
          musicSource.clip = mapMusic;
          break;
        case Windows.TownWindow:
          musicSource.clip = townMusic;
          break;
        case Windows.BattleWindow:
          musicSource.clip = battleMusic;
          break;
      }

      curWindow = window;

      if (musicOn)
      {
        musicSource.Play();
      }
    }
  }

  public void MusicOption(bool isEnabled)
  {
    musicOn = isEnabled;

    if (isEnabled && !musicSource.isPlaying)
    {
      musicSource.Play();
    }
    else if (!isEnabled)
    {
      musicSource.Pause();
    }
  }

  public void PlaySound(Sounds sound)
  {
    if (soundsOn)
    {
      switch (sound)
      {
        case Sounds.Secret:
          soundsSource.PlayOneShot(secretSound);
          break;
        case Sounds.Key:
          soundsSource.PlayOneShot(keySound);
          break;
        case Sounds.Success:
          soundsSource.PlayOneShot(successSound);
          break;
        case Sounds.GetItem:
          soundsSource.PlayOneShot(getItemSound);
          break;
        case Sounds.Sword:
          soundsSource.PlayOneShot(swordSounds[Random.Range(0, swordSounds.Length - 1)]);
          break;
        case Sounds.EnemyHit:
          soundsSource.PlayOneShot(enemyHitSound);
          break;
        case Sounds.EnemyDie:
          soundsSource.PlayOneShot(enemyDieSound);
          break;
        case Sounds.BossHit:
          soundsSource.PlayOneShot(bossHitsSounds[Random.Range(0, bossHitsSounds.Length - 1)]);
          break;
        case Sounds.BossDie:
          soundsSource.PlayOneShot(bossDieSounds);
          break;
      }
    }
  }

  public void SoundsOption(bool isEnabled)
  {
    soundsOn = isEnabled;

    if (isEnabled && !soundsSource.isPlaying)
    {
      soundsSource.Play();
    }
    else if (!isEnabled)
    {
      soundsSource.Stop();
    }
  }
}