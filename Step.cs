using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Step : MonoBehaviour
{
    #region Public variables
    public bool isOn { get; set; }
    public static ColorBlock enabledColor;
    public static ColorBlock disabledColor;
    #endregion

    #region Private variables
    private Button step;
    private DJ dj;
    #endregion

    #region Unity methods
    void Awake()
    {
        InitiateColors();
        step = GetComponent<Button>();
        dj = GameObject.Find("DJ").GetComponent<DJ>();
    }
    #endregion

    #region Public methods
    public IEnumerator ChangeColor()
    {
        step.colors = disabledColor;
        yield return new WaitForSeconds(dj.delay); // wait the delay
        step.colors = enabledColor;
    }

    //Force update on ui buttons
    public void UpdateColors()
    {
        if (isOn)
        {
            gameObject.GetComponent<Button>().colors = enabledColor;
        }
        else
        {
            gameObject.GetComponent<Button>().colors = disabledColor;
        }
    }

    //Click a step
    public void Clicked()
    {
        //Turn step off
        if (isOn)
        {
            isOn = false;
            GetComponent<Button>().colors = disabledColor;
        }
        //Turn step on
        else
        {
            isOn = true;
            GetComponent<Button>().colors = enabledColor;
        }
    }

    //Click a step
    public void Clicked(bool state)
    {
        //Turn step off
        if (!state)
        {
            isOn = false;
            GetComponent<Button>().colors = disabledColor;
        }
    }

    //Aesthetics
    public void InitiateColors()
    {
        enabledColor = GetComponent<Button>().colors;
        disabledColor = GetComponent<Button>().colors;
        Color temp1 = GetComponent<Button>().colors.disabledColor;
        Color temp2 = GetComponent<Button>().colors.normalColor;
        enabledColor.normalColor = temp1;
        enabledColor.highlightedColor = temp1;
        disabledColor.normalColor = temp2;
        disabledColor.highlightedColor = temp2;
    }
    #endregion
}
