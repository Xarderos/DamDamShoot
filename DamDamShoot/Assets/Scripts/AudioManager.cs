using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Serializable]
    public class AudioClipInfo
    {
        public string clipName;
        public AudioClip clip;
    }

    public List<AudioClipInfo> audioClips;
    private Dictionary<string, AudioClip> audioDictionary;
    private AudioSource audioSource;

    private void Awake()
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

        audioDictionary = new Dictionary<string, AudioClip>();
        foreach (var audioClipInfo in audioClips)
        {
            audioDictionary[audioClipInfo.clipName] = audioClipInfo.clip;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayAudio(string clipName)
    {
        if (audioDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Audio clip '{clipName}' not found!");
        }
    }
}
