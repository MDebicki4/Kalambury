using UnityEngine;

public class ExitAppButton : MonoBehaviour
{
    public void ExitApplication()
    {
        Debug.Log("Aplikacja zamykana...");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}