using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonDelay : MonoBehaviour
{
    public Button myButton;        // Reference to the button
    public float delayTime = 2f;   // Delay time in seconds

    void Start()
    {
        // Ensure the button is properly referenced in the inspector
        if (myButton != null)
        {
            myButton.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        // Start the coroutine to disable the button temporarily
        StartCoroutine(DisableButtonForSeconds());
    }

    IEnumerator DisableButtonForSeconds()
    {
        // Disable the button so it can't be clicked again
        myButton.interactable = false;

        // Wait for the specified delay time
        yield return new WaitForSeconds(delayTime);

        // Re-enable the button after the delay
        myButton.interactable = true;
    }
}