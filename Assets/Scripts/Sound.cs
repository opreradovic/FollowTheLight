using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    [SerializeField]
    private string name = null;

    public string Name
    {
        get { return name; }
        set { Name = name; }
    }


    [SerializeField]
    private AudioClip clip = null;

    public AudioClip Clip
    {
        get { return clip; }
        set { Clip = clip; }
    }


    [SerializeField, Range(0.0f, 1.0f)]
    private float volume = 0;

    public float Volume
    {
        get { return volume; }
        set { Volume = volume; }
    }



    [SerializeField, Range(0.0f, 3.0f)]
    private float pitch = 0;

    public float Pitch
    {
        get { return pitch; }
        set { Pitch = pitch; ; }
    }


    private AudioSource source;
    public AudioSource Source
    {
        get { return source; }
        set { source = value; }
    }

    [SerializeField]
    private bool isLooping = false;

    public bool IsLooping
    {
        get { return isLooping; }
        set { IsLooping = isLooping; }
    }

    [SerializeField]
    private AudioMixerGroup mixerGroup = null;

    public AudioMixerGroup MixerGroup
    {
        get { return mixerGroup; }
        set {MixerGroup = mixerGroup; }
    }


}
