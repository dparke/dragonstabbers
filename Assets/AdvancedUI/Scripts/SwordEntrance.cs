using UnityEngine;
using System.Collections;

public class SwordEntrance : MonoBehaviour {

  public void PlaySwordSwing()
  {
    MusicManager.Instance.PlaySound(Sounds.Sword);
  }

  public void PlaySwordHit()
  {
    MusicManager.Instance.PlaySound(Sounds.BossDie);
  }
}