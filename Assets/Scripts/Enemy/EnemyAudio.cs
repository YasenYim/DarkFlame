using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    public void EnemyDie()
    {
        AudioPlay("EnemyDie");
    }

    public void EnemySwordAudio()
    {
        AudioPlay("EnemySword");
    }

    public void EnemyBigWeaponAudio()
    {
        AudioPlay("BigSword");
    }

    public void BossStepAudio()
    {
        AudioPlay("BossStep");
    }

    public void BossAngerAudio()
    {
        AudioPlay("BossAnger");
    }

    public void BossSpecialAudio()
    {
        AudioPlay("Fire");
    }

    public void BossExplosionAudio()
    {
        AudioPlay("Explosion");
    }
    public void GetHitAudio()
    {
        AudioPlay("Hit");
    }

    void AudioPlay(string Name)
    {
        AudioManager.Instance.AudioPlayOnce(Name, transform.position);
    }
}
