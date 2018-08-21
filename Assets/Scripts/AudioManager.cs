using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager:MonoBehaviour{

    private static AudioManager _instance = new AudioManager();
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.Find("AudioManager").GetComponent<AudioManager>();
            return _instance;
        }
    }
    public AudioSource m_AudioMgr;
    public AudioSource m_BackGoundMusicMgr;
    private string clipPath;

    private void Start()
    {
        
    }

    public void PlayBGM()
    {
        AudioClip bgmClip = (AudioClip)Resources.Load("Audio/bgm_game");
        m_BackGoundMusicMgr.clip = bgmClip;
        m_BackGoundMusicMgr.Play();
    }

    internal void PlayAudio(string audioPath)
    {
        AudioClip playClip = (AudioClip)Resources.Load(audioPath);
        m_AudioMgr.clip = playClip;
        m_AudioMgr.Play();
    }
}
