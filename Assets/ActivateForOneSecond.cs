using UnityEngine;
using System.Collections;

public class ActivateForOneSecond : MonoBehaviour
{
    public GameObject targetObject; // Obiekt, który chcesz aktywować na sekundę

    public void ActivateForOneSecondMethod()
    {
        StartCoroutine(ActivateTemporarily());
    }

    private IEnumerator ActivateTemporarily()
    {
        // Aktywuj obiekt
        targetObject.SetActive(true);

        // Poczekaj 1 sekundę
        yield return new WaitForSeconds(1f);

        // Dezaktywuj obiekt
        targetObject.SetActive(false);
    }
}