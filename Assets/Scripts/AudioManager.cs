using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource musicSource;
    public AudioSource playerSoundSource;
    private Dictionary<string, AudioClip> audioDir = new Dictionary<string, AudioClip>();
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        musicSource.loop = true;
        playerSoundSource.loop = false;
    }

    public void PlayBGM(string bgm)
    {
        musicSource.clip = GetAudioClip(bgm);
        musicSource.Play();
    }

    public void StopBGM()
    {
        musicSource.Stop();
    }

    public void StopSound(string ClipName = "")
    {
        if (!string.IsNullOrEmpty(ClipName) && ClipName != playerSoundSource.clip.name)
        {
            return;
        }
        playerSoundSource.Stop();
    }
    public void AudioPlay(string name)
    {
        playerSoundSource.clip = GetAudioClip(name);
        playerSoundSource.Play();
    }
    
    public void AudioPlayOnce(string name, Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(GetAudioClip(name), pos);
    }

    AudioClip GetAudioClip(string name)
    {
        if (audioDir.ContainsKey(name))
        {
            return audioDir[name];
        }
        AudioClip clip = Resources.Load<AudioClip>("Audio/" + name);
        audioDir.Add(name, clip);
        return clip;
    }
}
