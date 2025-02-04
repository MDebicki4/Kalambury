using UnityEngine;
using UnityEngine.UI;

public class RadioButtonController : MonoBehaviour
{
    // Odwołanie do Toggle Group, którą przypisujesz w edytorze
    public ToggleGroup toggleGroup;

    // Funkcja wywoływana po zmianie wartości Toggle'a
    public void OnToggleChanged()
    {
        // Sprawdź, który Toggle jest zaznaczony
        foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            if (toggle.isOn)
            {
                Debug.Log(toggle.name + " is selected");
            }
        }
    }
}