// Copyright © bl4ck & XDev, 2022-2024
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioClip[] music;
    public AudioSource source;
    public bool shuffle;
    public bool onStart;
    public bool loop;

    public int current;
    public bool started;

    public static Music instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        source.loop = loop;
        if (onStart && !started)
        {
            Play();
            started = true;
        }
    }
    void Shuffle()
    {
        
    }
    private void Update()
    {
        if (!source.isPlaying && started)
        {
            Play();
            if (!shuffle)
            {
                if (music.Length - 1 != current)
                    current++;
            }
            else
            {
                int r = Random.Range(0, music.Length);
                while (r == current) r = Random.Range(0, music.Length);
                current = r;
                source.clip = music[current];
                source.Play();
            }
        }
    }

    public void Play()
    {
        if (shuffle) source.clip = music[Random.Range(0, music.Length)];
        else source.clip = music[current];
        source.Play();
    }

    public void Pause()
    {
        started = false;
        source.Pause();
    }

    public void Resume()
    {
        started = true;
        source.UnPause();
    }
    public void Mute(bool i)
    {
        source.mute = i;
    }

}
