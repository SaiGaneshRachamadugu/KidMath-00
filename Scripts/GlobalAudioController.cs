using UnityEngine;
using System.Collections.Generic;

public class GlobalAudioController : MonoBehaviour
{
    public static GlobalAudioController Instance;

    private List<AudioSource> allAudioSources = new List<AudioSource>();
    private bool isPaused = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Refresh list (optionally move this to a coroutine or scene load for performance)
        allAudioSources.Clear();
        allAudioSources.AddRange(FindObjectsOfType<AudioSource>());
    }

    public void PauseAllAudio()
    {
        foreach (var audio in allAudioSources)
        {
            if (audio.isPlaying)
                audio.Pause();
        }
        isPaused = true;
    }

    public void ResumeAllAudio()
    {
        foreach (var audio in allAudioSources)
        {
            if (!audio.isPlaying)
                audio.UnPause();
        }
        isPaused = false;
    }

    public void ToggleAudio()
    {
        if (isPaused)
            ResumeAllAudio();
        else
            PauseAllAudio();
        
    }
}
