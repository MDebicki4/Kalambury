using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timeout : MonoBehaviour
{
    [SerializeField]
    public float timeValue;
    public Text timeText;
    public GameObject skipButton;
    public GameObject trueButton;
    public GameObject personPanel;
    public GameObject nextTeam;
    public GameObject nextTimeButton;
    public GameObject Team1;
    public GameObject Team2;

    void Update()
    {
        if (timeValue > 0)
            timeValue -= Time.deltaTime;
        else
        {
            timeValue = 0;
            skipButton.SetActive(false);
            trueButton.SetActive(false);
            personPanel.SetActive(false);
            nextTeam.SetActive(true);
            nextTimeButton.SetActive(true);
        }

        DisplayTime(timeValue);
    }
    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
            timeToDisplay = 0;
        else if (timeToDisplay > 0)
            timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
