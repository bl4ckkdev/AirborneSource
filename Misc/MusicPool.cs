// Copyright © bl4ck & XDev, 2022-2024
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPool : MonoBehaviour
{
    public AudioClip[] music;
    public bool shuffle;
    public bool onStart;
    public bool loop;
    public bool override_;

    private protected Music ms;

    private bool br;
    
    public void Start()
    {
        ms = GameObject.FindGameObjectWithTag("Music").GetComponent<Music>();

        if (MainEditorComponent.isOfficial && !br && SceneManager.GetActiveScene().name == "ActualEditor")
        {
            br = true;
            
            override_ = true;
            return;
        }
        if (!override_)
        {
            ms.source.Stop();
            ms.started = false;
            ms.music = music;
            ms.shuffle = shuffle;
            ms.onStart = onStart;
            ms.loop = loop;
            ms.Start();
        }
        if (override_ && !Enumerable.SequenceEqual(music, ms.music))
        {
            ms.source.Stop();
            ms.started = false;
            ms.music = music;
            ms.shuffle = shuffle;
            ms.onStart = onStart;
            ms.loop = loop;
            ms.Start();
        }


    }
}
