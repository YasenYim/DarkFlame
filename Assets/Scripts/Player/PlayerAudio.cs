using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public void PlayerStepAudio()
    {
        AudioManager.Instance.AudioPlay("FootStep");
    }

    public void PlayerRollAudio()
    {
        AudioManager.Instance.AudioPlay("Roll");
    }
    public void PlayerGeiHit()
    {
        AudioManager.Instance.AudioPlay("PlayerGetHit");
    }

    public void PlayerPunch()
    {
        AudioManager.Instance.AudioPlay("Punch");
    }

    public void PlayerSwitchWeaponAudio()
    {
        AudioManager.Instance.AudioPlay("SwitchAudio");
    }

    public void PlayerSwordNormalAudio()
    {
        AudioManager.Instance.AudioPlay("Sword");
    }

    public void PlayerSwordSpecialAudio()
    {
        AudioManager.Instance.AudioPlay("SwordSpecial");
    }

    public void PlayerBigSwordAudio()
    {
        AudioManager.Instance.AudioPlay("BigSword");
    }

    public void PlayerSpecialAudio()
    {
        AudioManager.Instance.AudioPlay("Fire");
    }
}
