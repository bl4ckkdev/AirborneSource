// Copyright © bl4ck & XDev, 2022-2024
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using System;

public class SettingsMenu : MonoBehaviour
{
	[Header("Components")]
	[Space]
	public AudioMixer MainMixer;
	public Slider MusicSlider;
	public Slider SfxSlider;
    public Toggle PostProcessToggle;
    public Toggle FullscreenToggle;
    public TMP_Dropdown ResolutionDropdown;
    public Toggle SpeedrunToggle;
    public Toggle vsyncToggle;
    public Toggle expl;
	public static Toggle explorationToggle;
	public LevelLoader levelLoader;
	[Header("Testing")]
	public AudioSource testaudio;

	private Resolution[] resolutions;

	const string resname = "OV";

	public PostProcessProfile pp;


	private void Awake()
	{
		ResolutionDropdown.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(index =>
		{
			PlayerPrefs.SetInt(resname, ResolutionDropdown.value);
			PlayerPrefs.Save();
		}));

		try
		{
            Discord_Controller dcontroller = GameObject.Find("DiscordController").GetComponent<Discord_Controller>();
            dcontroller.details = "In main menu";
            dcontroller.state = "";
        }
		catch
		{

		}

	}

	public static void SetExploration(bool exp)
	{
		int result;
		if (exp) result = 1;
		else result = 0;
		explorationToggle.isOn = exp;
		PlayerPrefs.SetInt("Exploration", result);
	}

	public void SetSpeedrun(bool speed)
	{
		PlayerPrefs.SetInt("Speedrun", speed ? 1 : 0);
	}

	//Resolution
	void Start()
    {
		explorationToggle = expl;
		if (!PlayerPrefs.HasKey("VSync")) {
			PlayerPrefs.SetInt("VSync", 1);
			vsyncToggle.isOn = true;
		}

		if (!PlayerPrefs.HasKey("Beat"))
		{
			PlayerPrefs.SetInt("Beat", 0);
		}
		if (!PlayerPrefs.HasKey("Speedrun") | PlayerPrefs.GetInt("Speedrun", 0) == 0)
		{
			PlayerPrefs.SetInt("Speedrun", 0);
			SpeedrunToggle.isOn = false;
		}
		else
		{
			SpeedrunToggle.isOn = true;
		}
		if (!PlayerPrefs.HasKey("Exploration"))
		{
			levelLoader.ExplorationMode(false);
		}
		else
		{
			if (PlayerPrefs.GetInt("Exploration") == 0)
				levelLoader.ExplorationMode(false);
			else levelLoader.ExplorationMode(true);
		}
		if (!PlayerPrefs.HasKey("SFXVolume"))
		{
			MainMixer.SetFloat("SoundEffects", -20);
			SfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", -20);
		} else
		{
			MainMixer.SetFloat("SoundEffects", PlayerPrefs.GetFloat("SFXVolume"));
			SfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", -20);
		}

		if (!PlayerPrefs.HasKey("MusicVolume"))
		{
			MainMixer.SetFloat("Music", -4);
			MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", -20);
		}
		else
		{
			MainMixer.SetFloat("Music", PlayerPrefs.GetFloat("MusicVolume"));
			MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", -20);
		}


		if (PlayerPrefs.GetInt("FullscreenToggle", 1) == 1)
		{
			Screen.fullScreen = enabled;
			FullscreenToggle.isOn = true;
		} else
			FullscreenToggle.isOn = false;

		if (PlayerPrefs.GetInt("PostProcessToggle", 1) == 1)
		{
            PostProcessToggle.isOn = true;
			pp.GetSetting<Bloom>().active = true;
            pp.GetSetting<AmbientOcclusion>().active = true;
            pp.GetSetting<Vignette>().active = true;
        } else
		{
            PostProcessToggle.isOn = false;
            pp.GetSetting<Bloom>().active = false;
            pp.GetSetting<AmbientOcclusion>().active = false;
            pp.GetSetting<Vignette>().active = false;
        }

        QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync");
		vsyncToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("VSync"));




        resolutions = Screen.resolutions;
		Array.Reverse(resolutions);
		ResolutionDropdown.ClearOptions();
		List<string> options = new List<string>();

		//For Loop
		int currentResolutionIndex = 0;
		for (int i = 0; i < resolutions.Length; i++)
		{
			//string option = resolutions[i].width + " x " + resolutions[i].height;
			string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "hz";
			options.Add(option);
			//Check Resolution
			if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
			{
				currentResolutionIndex = i;
			}
		}
        ResolutionDropdown.AddOptions(options);
		ResolutionDropdown.value = PlayerPrefs.GetInt(resname, currentResolutionIndex);
        
        ResolutionDropdown.RefreshShownValue();
        
    }

	public void SetResolution(int resolutionIndex)
	{
		//Add Resolution To Screen
		Resolution resolution = resolutions[resolutionIndex];
		
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);
	}

	//Post processing
	public void PostProcess(bool enabled)
	{


		if (!enabled)
		{ 
			PlayerPrefs.SetInt("PostProcessToggle", 0);
			pp.GetSetting<Bloom>().active = false;
			pp.GetSetting<AmbientOcclusion>().active = false;
			pp.GetSetting<Vignette>().active = false;
		} 
		else
		{ 
            pp.GetSetting<Bloom>().active = true;
            pp.GetSetting<AmbientOcclusion>().active = true;
            pp.GetSetting<Vignette>().active = true;
            PlayerPrefs.SetInt("PostProcessToggle", 1);
		}
	}

	//Fullscreen
	public void Fullscreen(bool enabled)
	{
		Screen.fullScreen = enabled;

		if (!enabled)
			PlayerPrefs.SetInt("FullscreenToggle", 0);
		else
		{
			enabled = true;
			PlayerPrefs.SetInt("FullscreenToggle", 1);
		}
	}

	//Volume
	public void SFXVolume(float volume)
	{
		PlayerPrefs.SetFloat("SFXVolume", volume);
		MainMixer.SetFloat("SoundEffects", PlayerPrefs.GetFloat("SFXVolume"));
	}
	public void MusicVolume(float volume)
	{
		PlayerPrefs.SetFloat("MusicVolume", volume);
		MainMixer.SetFloat("Music", PlayerPrefs.GetFloat("MusicVolume"));
	}

	public void SetVSync(bool value)
	{
		int boolValue = value ? 1 : 0;

        PlayerPrefs.SetInt("VSync", boolValue);
        QualitySettings.vSyncCount = boolValue;
    }

    //Reset settings
    public void ResetSettings()
	{
		PlayerPrefs.SetFloat("SFXVolume", -15);
		MainMixer.SetFloat("SoundEffects", PlayerPrefs.GetFloat("SFXVolume"));
		SfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");

		PlayerPrefs.SetFloat("MusicVolume", -15);
		MainMixer.SetFloat("Music", PlayerPrefs.GetFloat("MusicVolume"));
		MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");

		PlayerPrefs.SetInt("FullscreenToggle", 1);
		FullscreenToggle.isOn = true;
		Screen.fullScreen = true;

		PlayerPrefs.SetInt("PostProcessToggle", 1);
		PostProcessToggle.isOn = true;

		PlayerPrefs.SetInt("Speedrun", 0);
		SpeedrunToggle.isOn = false;

		PlayerPrefs.SetInt("VSync", 1);
		vsyncToggle.isOn = true;
	}
}
