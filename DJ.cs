using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DJ : MonoBehaviour
{
    #region Public variables
    public int BPM;
    public int i { get; set; }
    public float delay { get; set; }
    public List<Sampler> samplers;
    public Slider bpmSlider;
    public Slider volumeSlider;
    public AudioSource player;
    public GameObject samplerPrefab;
    public GameObject promptPrefab;
    public GameObject explorer;
    public GameObject playButton;
    public Texture pauseImage;
    public Texture playImage;
    public bool inExplorer { get; set; }
    public bool settingsOpen;
    public bool Playing { get; set; }

    #endregion

    #region Private variables
    private GameObject explorerType;
    private GameObject explorerTracks;
    private GameObject explorerHeader;
    private List<Button> trackButtons;
    private List<AudioClip> kicks;
    private List<AudioClip> snares;
    private List<AudioClip> claps;
    private List<AudioClip> loops;
    private List<AudioClip> hihats;
    private List<AudioClip> vocals;
    private const int spaceBetweenSamplers = 72;
    private const int audioFiles = 9;    
    private float doubleClickDelay = 0.32f;
    private bool clicked;
    #endregion

    #region Unity methods
    private void Start()
    {
        i = 0;
        LoadTracks();
        initializeExplorer();
        samplers = new List<Sampler>();
        GameObject.Find("BPM").GetComponent<TMPro.TMP_InputField>().text = BPM.ToString();
        GameObject.Find("Slider_BPM").GetComponent<Slider>().value = BPM;
        delay = (Mathf.Pow((float)BPM / 60, -1))/4;
        instantiateSampler("WELCOME", player, new List<Step>(), true);
    }
    private void Update()
    {
        if (inExplorer && Input.GetKeyDown(KeyCode.Escape))
        {
            showExplorer(false);
        }
    }
    #endregion

    #region Private methods
    private IEnumerator Play()//The main method, go to every open sampler and check if a step is activated, if it is - play the clip
    //if (isPlaying) -> get into while loop, play if (step.isOn), wait (float)delay, back to the start of the while loop
    //After each delay, i++
    {
        while (Playing) // breaks the loop if the user clicks the play/stop button
        {
            foreach (var sample in samplers)
            {
                if (!sample.isLooping)
                {
                    sample.PlayClip(i);
                }
                else
                {
                    /*if (!sample.track.isPlaying)
                        sample.track.Play();*/
                    if (!sample.track.isPlaying)
                        sample.track.Play();
                }
            }

            yield return new WaitForSeconds(delay); // wait (delay) accoarding to BPM

            i++; 

            if (i == 16) //Loop
            {
                i = 0;
            }
        }
    }

    private void LoadTracks() //Loads the .wav files from the Resources folder
    {
        kicks = LoadTrack("Kicks");
        snares = LoadTrack("Snares");
        claps = LoadTrack("Claps");
        loops = LoadTrack("Loops");
        hihats = LoadTrack("Hi-Hats");
        vocals = LoadTrack("Vocals");
    }

    private List<AudioClip> LoadTrack(string listName)
    {
        var tempArray = Resources.LoadAll(listName);
        List<AudioClip> list = new List<AudioClip>();
        foreach (var item in tempArray)
        {
            list.Add(item as AudioClip);
        }
        return list;
    }
    #endregion

    #region Public methods

    public void instantiateSampler(string name, AudioSource track, List<Step> steps, bool example)//Used for cloning and the example track
    {
        if (samplers.Count < 16)
        {
            if (samplers.Count == 7)
            {
                GetComponent<ScrollRect>().enabled = true;
                transform.Find("Scrollbar").GetComponent<Scrollbar>().interactable = true;
            }

            float y; //Y-Axis position of the sampler's transform

            if (samplers.Count == 0)
                y = 565;
            else
                y = samplers[samplers.Count - 1].transform.position.y - spaceBetweenSamplers;

            Vector3 pos = new Vector3(160, y, 0);

            GameObject sampler = Instantiate(samplerPrefab, transform.Find("Samplers"), true) as GameObject;
            sampler.transform.SetAsLastSibling();
            sampler.transform.position = pos;

            samplers.Add(sampler.GetComponent<Sampler>());
            sampler.GetComponent<Sampler>().track.clip = track.clip;

            sampler.GetComponent<Sampler>().Name = name;
            sampler.GetComponent<Sampler>().UpdateName();

            if (!example)//Used for cloning
            {
                List<int> onSteps = new List<int>();
                foreach (var item in steps)
                {
                    if (item.isOn)
                    {
                        onSteps.Add(steps.IndexOf(item));
                    }
                }
                for (int i = 0; i < steps.Count; i++)
                {
                    if (onSteps.Contains(i))
                    {
                        sampler.GetComponent<Sampler>().Steps[i].isOn = true;
                    }
                }
            }

            else //Example sampler
            {
                GameObject prompt = Instantiate(promptPrefab, sampler.transform) as GameObject;
                prompt.GetComponent<Prompt>().sampler = sampler.GetComponent<Sampler>();
                sampler.transform.Find("Prompt(Clone)").GetComponent<Prompt>().Fill(4);
            }

            foreach (Step item in sampler.GetComponent<Sampler>().Steps)
            {
                item.UpdateColors();
            }
        }
    }

    public void Button_AddSampler()//Constructor
    {
        if (samplers.Count < 16)
        {
            if (samplers.Count == 7)
            {
                GetComponent<ScrollRect>().enabled = true;
                transform.Find("Scrollbar").GetComponent<Scrollbar>().interactable = true;
            }

            float y; //Y-Axis position of the sampler's transform

            if (samplers.Count == 0)
                y = 565;
            else
                y = samplers[samplers.Count - 1].transform.position.y - spaceBetweenSamplers;

            Vector3 pos = new Vector3(160, y, 0);

            GameObject sampler = Instantiate(samplerPrefab, transform.Find("Samplers"), true) as GameObject;
            sampler.transform.SetAsLastSibling();
            sampler.transform.position = pos;

            samplers.Add(sampler.GetComponent<Sampler>());

            
        }
    }

    public void Button_PlayPause()//The user has clicked the Play/Pause button
    {
        //Play
        if (!Playing)
        {
            playButton.GetComponent<RawImage>().texture = pauseImage;
            Playing = true;
            i = 0;
            StartCoroutine(Play());
        }

        //Stop
        else
        {
            playButton.GetComponent<RawImage>().texture = playImage;
            Playing = false;
            StopCoroutine(Play());
            foreach (Sampler sample in samplers)
            {
                sample.track.Stop();
            }
        }
    }

    public void Button_Reset()//Resets the scene
    {
        SceneManager.LoadScene(0);
    }

    public void Slider_MasterVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    public void changeBPM()//The player used the slider to change the BPM(tempo)
    {
        BPM = (int)bpmSlider.value;
        GameObject.Find("BPM").GetComponent<TMPro.TMP_InputField>().text = BPM.ToString();

        //The delay between each beat is Beats per minute(BPM), divided by number of seconds in a minute(60), take all that and ^-1
        //For example, BPM = 60, 60/60 = 1, 1^-1 = 1, play one beat every second, 120BPM play beat every 0.5 sec and so on.
        //Then divide it by 4 because it is a 4/4 track
        delay = (Mathf.Pow((float)BPM / 60, -1)) / 4;
    }
    #endregion

    #region Explorer

    private void initializeExplorer()
    {
        explorerType = explorer.transform.Find("ChooseType").gameObject;
        explorerTracks = explorer.transform.Find("ChooseTrack").gameObject;
        explorerHeader = explorerTracks.transform.Find("Header").Find("Text").gameObject;
        trackButtons = new List<Button>();
        player = GetComponent<AudioSource>();

        foreach (Transform item in explorerTracks.transform.Find("Buttons"))
        {
            trackButtons.Add(item.gameObject.GetComponent<Button>());
        }
        showExplorer(false);

    }

    public void openExplorer(Sampler sampler)//Opens the explorer window and lets you choose a type for the sampler, i.e kicks/claps/hihats/snares etc.
    {
        showExplorer(true);
        Button btn_Kick = explorerType.transform.Find("Button_Kick").GetComponent<Button>();
        Button btn_Snare = explorerType.transform.Find("Button_Snare").GetComponent<Button>();
        Button btn_Clap = explorerType.transform.Find("Button_Clap").GetComponent<Button>();
        Button btn_Loop = explorerType.transform.Find("Button_Loop").GetComponent<Button>();
        Button btn_Hihat = explorerType.transform.Find("Button_Hi-Hat").GetComponent<Button>();
        Button btn_Vocal = explorerType.transform.Find("Button_Vocal").GetComponent<Button>();

        List<Button> buttons = new List<Button>() { btn_Kick, btn_Snare, btn_Clap, btn_Loop, btn_Hihat, btn_Vocal };

        btn_Kick.onClick.AddListener(delegate { listOfTracks("Kicks", sampler, buttons); });
        btn_Snare.onClick.AddListener(delegate { listOfTracks("Snares", sampler, buttons); });
        btn_Clap.onClick.AddListener(delegate { listOfTracks("Claps", sampler, buttons); });
        btn_Loop.onClick.AddListener(delegate { listOfTracks("Loops", sampler, buttons); });
        btn_Hihat.onClick.AddListener(delegate { listOfTracks("Hi-Hats", sampler, buttons); });
        btn_Vocal.onClick.AddListener(delegate { listOfTracks("Vocals", sampler, buttons); });
    }

    private void listOfTracks(string type, Sampler sampler, List<Button> buttons) //Opens the 2nd explorer window, for example after picking kicks, will show all different kicks
    {
        List<AudioClip> clips = new List<AudioClip>();

        switch (type)
        {
            case "Kicks":
                clips = kicks;
                break;

            case "Snares":
                clips = snares;
                break;

            case "Claps":
                clips = claps;
                break;

            case "Loops":
                clips = loops;
                break;

            case "Hi-Hats":
                clips = hihats;
                break;

            case "Vocals":
                clips = vocals;
                break;
        }

        foreach (var button in buttons)//
        {
            button.onClick.RemoveAllListeners();
        }

        explorerTracks.SetActive(true);
        explorerType.SetActive(false);
        explorerHeader.GetComponent<TMPro.TextMeshProUGUI>().text = type;
        for (int i = 0; i < audioFiles; i++)
        {
            int temp = i;
            trackButtons[i].onClick.AddListener(delegate { StartCoroutine(ChooseAtrack(type, temp, sampler, clips)); });
        }
    }

    private IEnumerator ChooseAtrack(string type, int index, Sampler sampler, List<AudioClip> clips)//Click once - hear the sound, double click - set the sampler to the sound
    {
        if (!clicked) //Single click
        {
            player.clip = clips[index];
            player.Play();
            clicked = true;

            while (doubleClickDelay > 0) //While loop for 0.25s, let the user have time to click another time
            {
                doubleClickDelay -= Time.fixedDeltaTime;
                yield return null;
            }

            //back to default values
            clicked = false;
            doubleClickDelay = 0.25f;
        }

        else //Double click
        {
            clicked = false;
            doubleClickDelay = 0.25f;
            sampler.track.clip = clips[index];
            showExplorer(false);

            sampler.Name = type.Remove(type.Length - 1) + " " + (index + 1);
            sampler.UpdateName();

            player.Stop();
            player.clip = null;
        }
    }

    public void showExplorer(bool visible)
    {
        if (visible)
        {
            inExplorer = true;
            explorer.SetActive(true);
            explorerTracks.SetActive(false);
        }

        else
        {
            inExplorer = false;
            explorerType.SetActive(true);
            explorerTracks.SetActive(true);
            foreach (var item in trackButtons)
            {
                item.GetComponent<Button>().onClick.RemoveAllListeners();
            }
            explorer.SetActive(false);
        }
    }

    #endregion

}
