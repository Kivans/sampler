using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Sampler : MonoBehaviour, IPointerClickHandler
{
    #region Public variables
    public string Name { get; set; }
    public AudioSource track { get; set; }
    public List<Step> Steps { get; set; }
    public bool isLooping { get; set; }
    public GameObject currentPrompt { get; set; }
    #endregion

    #region Private variables
    private bool isOn;
    private DJ dj;
    #endregion

    #region Unity methods
    void Awake()
    {
        dj = GameObject.Find("DJ").GetComponent<DJ>();
        Name = "Sampler" + " " + dj.samplers.Count;
        Steps = new List<Step>();
        GetChildren();
        track = GetComponent<AudioSource>();
    }

    void Start ()
    {
        UpdateName();
    }
    #endregion

    #region Public methods

    public void OnPointerClick(PointerEventData eventData)
    {
        //Open Prompt(Right click)
        if (eventData.button == PointerEventData.InputButton.Right && !dj.settingsOpen)
        {
            GameObject prompt = Instantiate(dj.promptPrefab, transform) as GameObject;
            prompt.GetComponent<Prompt>().sampler = this;
            currentPrompt = prompt;

            foreach (var item in dj.samplers)//Before instantiating prompt, destroy all other prompts
            {
                if (item.currentPrompt != null && item != this)
                    item.currentPrompt.GetComponent<Prompt>().Exit();
            }

            FixPrompt(prompt);

            transform.SetAsLastSibling();
            prompt.transform.SetAsLastSibling();
        }

        //Open Explorer(Left click)
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            dj.openExplorer(this);
        }
    }

    public void GetChildren()
    {
        foreach (Transform item in transform.Find("Steps"))
        {
            Steps.Add(item.GetComponent<Step>());
        }
    }

    public void PlayClip(int i)
    {
        if (Steps[i].isOn)
        {
            track.Play();
            Steps[i].StartCoroutine(Steps[i].ChangeColor());
        }
    }

    public void UpdateName()
    {
        transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = Name;
    }
    #endregion

    #region Private methods
    private void FixPrompt(GameObject prompt)
    {
        int i = dj.samplers.Count - 1;
        if (i == 5 || i == 6 || i == 14 || i == 15 || i == 16)
        {
            if (this == dj.samplers[i] || this == dj.samplers[i - 1])
            {
                Vector3 pos = prompt.transform.position;
                pos.y += 155;
                prompt.transform.position = pos;
            }
        }

        //else if (i == 14)
    }
    #endregion
}
