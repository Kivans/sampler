using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Put this on canvas, and it will infect all other gameobjects
public class DestroyPrompt : MonoBehaviour, IPointerClickHandler
{
    private DJ dj;

    void Start()
    {
        dj = GameObject.Find("DJ").GetComponent<DJ>();
        
        if (gameObject.name == "Canvas")
        {
            Object[] objects = GameObject.FindGameObjectsWithTag("Disable");
            foreach (GameObject item in objects)
            {
                if (item.GetComponent<Button>() == null && item.GetComponent<Slider>() == null)
                item.AddComponent<DestroyPrompt>();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            foreach (var item in dj.samplers)//Before instantiating prompt, destroy all other prompts
            {
                if (item.currentPrompt != null)
                    item.currentPrompt.GetComponent<Prompt>().Exit();
            }

            if (dj.inExplorer == true)
                dj.showExplorer(false);
        }
    }
}
