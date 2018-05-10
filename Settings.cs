using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour 
{
    #region Public variables
    public Sampler sampler { get; set; }
    #endregion

    #region Private variables
    private TMP_InputField Header;
    private Slider volumeSlider;
    private Slider pitchSlider;
    private Toggle loopToggle;
    private Button exitButton;
    private Button defaultButton;
    private const float DEFAULT_VALUE = 1f;
    private DJ dj;
    #endregion

    #region Unity methods
    void Awake()
    {
        dj = GameObject.Find("DJ").GetComponent<DJ>();
        dj.settingsOpen = true;
    }

    void Start ()
    {
        Header = gameObject.transform.Find("Header").GetComponent<TMP_InputField>();
        Header.text = sampler.Name;

        volumeSlider = gameObject.transform.Find("Slider_Volume").GetComponent<Slider>();
        volumeSlider.value = sampler.track.volume;

        pitchSlider = gameObject.transform.Find("Slider_Pitch").GetComponent<Slider>();
        pitchSlider.value = sampler.track.pitch;

        loopToggle = gameObject.transform.Find("Toggle_Loop").GetComponent<Toggle>();
        loopToggle.isOn = sampler.isLooping;

        exitButton = gameObject.transform.Find("Button_Exit").GetComponent<Button>();
        defaultButton = gameObject.transform.Find("Button_Default").GetComponent<Button>();


        Header.onSubmit.AddListener(delegate { RenameSampler(); });
        volumeSlider.onValueChanged.AddListener(delegate { ChangeVolume(volumeSlider.value); });
        pitchSlider.onValueChanged.AddListener(delegate { ChangePitch(pitchSlider.value); });
        loopToggle.onValueChanged.AddListener(delegate { Loop(loopToggle.isOn); });
        exitButton.onClick.AddListener(Exit);
        defaultButton.onClick.AddListener(Default);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
        }
    }
    #endregion

    #region Private methods
    private void RenameSampler()//pretty self-explanatory
    {
        sampler.Name = Header.text;
        sampler.UpdateName();
    }

    private void ChangeVolume(float value)//Moving one of the sliders will change the audiosource's pitch and volume
    {
        sampler.track.volume = value;
    }
    private void ChangePitch(float value)
    {
        sampler.track.pitch = value;
        if (!dj.Playing)
        {
            dj.player.Stop();
            dj.player.clip = sampler.track.clip;
            dj.player.pitch = value;
            dj.player.Play();
        }
    }

    public void Loop(bool isOn)
    {
        if (isOn) //Start looping and remove button graphics
        {
            sampler.isLooping = true;
            sampler.track.loop = true;
            foreach (Step step in sampler.Steps)
            {
                step.isOn = false;
                step.GetComponent<Image>().enabled = false;
            }
        }
        else//Stop looping and add button graphics
        {
            sampler.isLooping = false;
            sampler.track.loop = false;
            foreach (Step step in sampler.Steps)
            {
                step.GetComponent<Image>().enabled = true;
                step.GetComponent<Button>().interactable = false;
                step.GetComponent<Button>().interactable = true;
                step.UpdateColors();
            }
        }
    }

    private void Default()//Reverts the sliders&audiosource to default values(1f)
    {
        sampler.track.volume = DEFAULT_VALUE;
        volumeSlider.value = DEFAULT_VALUE;

        sampler.track.pitch = DEFAULT_VALUE;
        pitchSlider.value = DEFAULT_VALUE;

        loopToggle.isOn = false;
    }

    public void Exit()//Removes all listeners from all buttons&sliders and destroys the settings gameobject
    {
        dj.player.Stop();
        dj.player.loop = false;
        dj.player.pitch = 1;

        dj.settingsOpen = false;

        Header.onSubmit.RemoveAllListeners();
        volumeSlider.onValueChanged.RemoveAllListeners();
        pitchSlider.onValueChanged.RemoveAllListeners();
        loopToggle.onValueChanged.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        defaultButton.onClick.RemoveAllListeners();
        Destroy(gameObject);
    }
    #endregion
}
