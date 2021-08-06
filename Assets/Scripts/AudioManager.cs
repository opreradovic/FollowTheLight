using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    [SerializeField]
    private AudioMixer masterMixer = null;
    private Slider masterSlider = null;
    [SerializeField]
    private AudioMixerGroup musicMixer = null; 
    private Slider musicSlider = null;
    [SerializeField]
    private AudioMixerGroup sfxMixer = null; 
    private Slider sfxSlider = null;

    private bool isBgMusicPlaying = false;

    //This shopuld be in a game manager, but I dont want to make a game manager
    //to manage just one variable, so im going to put it in the audiomanager singleton!
    public int MothDeathCount;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
                return;
        }

        foreach(Sound s in sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
              
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.IsLooping;
            s.Source.outputAudioMixerGroup = s.MixerGroup;
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        instance.StopPlaying("BugLightBuzzing");

        //Gets the volume sliders and play appropriate music
        if(scene.name == "Menu")
        {
            masterSlider = GameObject.Find("MasterSlider").GetComponent<Slider>();
            musicSlider = GameObject.Find("MusicSlider").GetComponent<Slider>();
            sfxSlider = GameObject.Find("SfxSlider").GetComponent<Slider>();
            instance.Play("MenuMusic");
            instance.StopPlaying("BackgroundMusic");
            isBgMusicPlaying = false;
        }
        else if(scene.name != "Menu" && isBgMusicPlaying == false)
        {
            instance.StopPlaying("MenuMusic");
            instance.Play("BackgroundMusic");
            isBgMusicPlaying = true;
        }
        if (scene.name == "Ending1" || scene.name == "Ending2")
            MothDeathCount = 0;
    }
    private void Start()
    {
        SetSliderValue();
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.Name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.Source.Play();
    }
    public void StopPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.Name == name);
        if(s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.Source.Stop();
    }
    //Sets the volume, according to the slider values.
    public void MasterVolume(float sliderValue)
    {
        masterMixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
    }
    public void MusicVolume(float sliderValue)
    {
        musicMixer.audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }
    public void SfxVolume(float sliderValue)
    {
        sfxMixer.audioMixer.SetFloat("SfxVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SfxVolume", sliderValue);
    }
    public void SetSliderValue()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume", 1.0f);
    }
}
