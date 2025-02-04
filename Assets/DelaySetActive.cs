using UnityEngine;
using System.Collections;

public class DelaySetActive : MonoBehaviour
{
    [SerializeField] private GameObject Aktywacja; // Field for the target object
    [SerializeField] private GameObject Dezaktywacja; // Field for the target object
    [SerializeField] private float delay = 1f; // Delay field, editable in Inspector

    public void ActivateTargetWithDelay()
    {
        StartCoroutine(SetActiveAfterDelay(Aktywacja, true, delay));
    }

    public void DeactivateTargetWithDelay()
    {
        StartCoroutine(SetActiveAfterDelay(Dezaktywacja, false, delay));
    }

    private IEnumerator SetActiveAfterDelay(GameObject target, bool state, float delay)
    {
        yield return new WaitForSeconds(delay);
        target.SetActive(state);
    }
}