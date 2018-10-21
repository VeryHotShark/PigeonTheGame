using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundClip
{
    public string name;
    public AudioClip audioClip;

    [Space]
    [Range(0f, 1f)]
    public float volume;

    [Range(0f, 1f)]
    public float pitch;

    [Range(0f, 1f)]
    public float spatialBlend;

    [Space]
    public bool playOnAwake;
    public bool loop;

    [HideInInspector]
    public AudioSource audioSource;
}

public class AudioManager : MonoBehaviour
{

    public List<SoundClip> soundClips = new List<SoundClip>();

    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);


        foreach (SoundClip soundClip in soundClips)
        {
            soundClip.audioSource = gameObject.AddComponent<AudioSource>();
            soundClip.audioSource.clip = soundClip.audioClip;

            soundClip.audioSource.volume = soundClip.volume;
            soundClip.audioSource.pitch = soundClip.pitch;
            soundClip.audioSource.spatialBlend = soundClip.spatialBlend;

            soundClip.audioSource.playOnAwake = soundClip.playOnAwake;
            soundClip.audioSource.loop = soundClip.loop;
        }
    }

    // Update is called once per frame
    public void Play(string name)
    {
        SoundClip clipToPlay = soundClips.Find(sound => sound.name == name);

		if(clipToPlay == null)
			return;

        clipToPlay.audioSource.Play();
    }
}
