using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prompt : MonoBehaviour
{
    #region Public variables
    public Sampler sampler { get; set; }
    public GameObject settingsPrefab;
    #endregion

    #region Private variables
    private Button fill2;
    private Button fill4;
    private Button fill8;
    private Button clone;
    private Button settings;
    private List<Button> buttons;
    private DJ dj;
    #endregion

    #region Unity Methods
    void Awake()
    {
        dj = GameObject.Find("DJ").GetComponent<DJ>();

        fill2 = gameObject.transform.Find("Button_Fill2").GetComponent<Button>();
        fill4 = gameObject.transform.Find("Button_Fill4").GetComponent<Button>();
        fill8 = gameObject.transform.Find("Button_Fill8").GetComponent<Button>();
        clone = gameObject.transform.Find("Button_Clone").GetComponent<Button>();
        settings = gameObject.transform.Find("Button_Settings").GetComponent<Button>();

        fill2.onClick.AddListener(delegate { Fill(2); });
        fill4.onClick.AddListener(delegate { Fill(4); });
        fill8.onClick.AddListener(delegate { Fill(8); });
        clone.onClick.AddListener(Clone);
        settings.onClick.AddListener(Settings);

        buttons = new List<Button>() { fill2, fill4, fill8, settings, clone };
    }
    #endregion

    #region Public methods
    public void Fill(int index)
    {
        foreach (var item in sampler.Steps)
        {
            item.Clicked(false);
        }

        for (int i = 0; i < 15; i += index)
        {
            sampler.Steps[i].Clicked();
        }
        Unloop();
        Exit();
    }

    public void Exit()
    {
        foreach (var item in buttons)
        {
            item.onClick.RemoveAllListeners();
        }
        sampler.currentPrompt = null;
        Destroy(gameObject);
    }
    #endregion

    #region Private methods

    private void Clone()
    {
        dj.instantiateSampler(sampler.Name + " Clone", sampler.track, sampler.Steps, false);

        if (sampler.isLooping == true) //In case the cloned sample is a loop sample
        {
            dj.samplers[dj.samplers.Count - 1].isLooping = true;
            foreach (Step step in dj.samplers[dj.samplers.Count - 1].Steps)
            {
                step.isOn = false;
                step.GetComponent<Image>().enabled = false;
            }
        }

        Exit();
    }

    private void Settings()
    {
        if (!dj.settingsOpen)
        {
            //Close the prompt
            Exit();

            //Show the settings panel
            GameObject go = Instantiate(settingsPrefab, GameObject.Find("DJ").transform) as GameObject;
            go.GetComponent<Settings>().sampler = sampler;
        }
    }

    private void Unloop()
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
    #endregion
}
