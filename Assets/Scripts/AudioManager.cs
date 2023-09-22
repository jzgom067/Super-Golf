using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] Sound[] music;
    [SerializeField] Sound[] sounds;

    int currentMusic;
    int[] musicRandomQueue;
    [SerializeField] int randomMusicQueueSize;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        musicRandomQueue = new int[randomMusicQueueSize];

        foreach (Sound s in music)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;
        }
    }

    private void Start()
    {
        currentMusic = UnityEngine.Random.Range(1, music.Length);

        PlayMusic(currentMusic);

        musicRandomQueue[musicRandomQueue.Length - 1] = currentMusic;
    }

    private void Update()
    {
        string tempName = "Music " + currentMusic;

        Sound s = Array.Find(music, thing => thing.name == tempName);
        if (!s.source.isPlaying)
        {
            bool canProgress;
            do
            {
                canProgress = true;
                currentMusic = UnityEngine.Random.Range(1, music.Length);
                for (int i = 0; i < musicRandomQueue.Length; i++)
                {
                    if (currentMusic == musicRandomQueue[i])
                        canProgress = false;
                }
            } while (!canProgress);

            musicRandomQueue = PushBackArray(musicRandomQueue);
            musicRandomQueue[musicRandomQueue.Length - 1] = currentMusic;

            PlayMusic(currentMusic);
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        print(s.source.clip.ToString());
        s.source.Play();
    }

    int[] PushBackArray(int[] j)
    {
        for (int i = 0; i < j.Length - 1; i++)
        {
            j[i] = j[i + 1];
        }
        j[j.Length - 1] = 0;
        return j;
    }

    void PlayMusic(int i)
    {
        string tempName = "Music " + i;
        Sound d = Array.Find(music, thing => thing.name == tempName);
        d.source.Play();
    }
}
