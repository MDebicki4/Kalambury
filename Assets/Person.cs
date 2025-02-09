using UnityEngine;
using System.IO;

[System.Serializable]
public class PersonRoot
{
    public Person[] people;
}

[System.Serializable]
public class Person
{
    public string name;
    public bool active;
    public string key;
}

// public class PersonManager : MonoBehaviour
// {
//     public void Start()
//     {
//         string filePath = Path.Combine(Application.streamingAssetsPath, "persons.json");
//         string json = File.ReadAllText(filePath);
        
//         Person[] persons = JsonUtility.FromJson<Person[]>(json);

//         // Now, you have an array of 1,000 Person objects loaded from the JSON file
//     }
// }
