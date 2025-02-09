using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.Networking;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject roundObject;
    public GameObject breakObject;
    public GameObject roundsObject;
    public GameObject resultsObject;
    public GameObject teamsObject;
    public GameObject mainMenuObject;
    public GameObject gameeObject;
    public GameObject breakTimeObject;
    public GameObject wrocButtonObject;
    public GameObject nextRoundButtonObject;
    [SerializeField]
    private Text timeBreakText;
    [SerializeField]
    private Text timeText;
    [SerializeField]
    private Text team1ScoreText;
    [SerializeField]
    private Text team2ScoreText;
    [SerializeField]
    private Text team1FinalScoreText;
    [SerializeField]
    private Text team2FinalScoreText;
    [SerializeField]
    private Text roundCounterText;
    [SerializeField]
    private TextMeshProUGUI nextTeamText;
    [SerializeField]
    private TextMeshProUGUI personText;
    [SerializeField]
    private TextMeshProUGUI resultsText;
    [SerializeField]
    public Text team1Text;
    [SerializeField]
    public Text team2Text;
    [SerializeField]
    public Text team1FinalText;
    [SerializeField]
    public Text team2FinalText;

    private Person[] people;
    public List<Person> activePeople;

    public InputField display1;
    public InputField display2;

    private static List<Person> unasweredPeople;
    private static List<Person> unasweredPeopleTemp;
    private Person currentPerson;

    [SerializeField]
    private static float timeValue = time;
    [SerializeField]
    private static float timeBreakValue = 3;
    private static int roundCounter = 10;
    private static int roundCounterLessQuestions;
    private static int hiddenCounter = 10;
    [SerializeField]
    private static int number = 0;
    [SerializeField]
    private static float timeBetweenQuestions = 0.5f;
    [SerializeField]
    private static int team1Score = 0;
    [SerializeField]
    private static int team2Score = 0;
    [SerializeField]
    private static int team1RoundScore = 0;
    [SerializeField]
    private static int team2RoundScore = 0;
    [SerializeField]
    private static int team1FinalScore;
    [SerializeField]
    private static int team2FinalScore;
    [SerializeField]
    private static int activeTeam = 2;
    [SerializeField]
    private static float time = 18;
    private static float timeAfterRestart;
    private static int rounds;
    private static int roundsAfterRestart;
    private static int roundsSlider;
    private static int roundsNumber = 0;
    private static bool doWeNeedAnotherList = true;
    private static bool firstRound = true;

    private string fileName = "persons.json";
    private string persistentFilePath;

    public Slider sliderRounds;

    void Start()
    {

        rounds = 1;
        roundsAfterRestart = 1;
        timeAfterRestart = 33;
        time = 33;
        roundCounter = 20;
        hiddenCounter = 20;

        roundsSlider = 1;
        PlayerPrefs.SetString("team1name", "Drużyna 1");
        PlayerPrefs.SetString("team2name", "Drużyna 2");
        PlayerPrefs.Save();

        persistentFilePath = Path.Combine(Application.persistentDataPath, fileName);
        StartCoroutine(CopyJsonToPersistentDataPath());

        roundObject.SetActive(false);
        breakObject.SetActive(true);
        UpdateAllRecordsToActive();
        LoadPeopleFromJSON();
    }
    private IEnumerator CopyJsonToPersistentDataPath()
    {
        if (!File.Exists(persistentFilePath))
        {
            string streamingFilePath = Path.Combine(Application.streamingAssetsPath, fileName);

#if UNITY_ANDROID
            UnityWebRequest request = UnityWebRequest.Get(streamingFilePath);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllText(persistentFilePath, request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Failed to load JSON from StreamingAssets: " + request.error);
            }
#else
            if (File.Exists(streamingFilePath))
            {
                File.Copy(streamingFilePath, persistentFilePath);
            }
            else
            {
                Debug.LogError("JSON file not found in StreamingAssets.");
            }
#endif
        }
    }
    void Update()
    {
        if (timeValue > 0)
            timeValue -= Time.deltaTime;
        else
        {
            timeValue = 0;
            roundObject.SetActive(false);
            breakObject.SetActive(true);
        }
        if (timeBreakValue > 0)
            timeBreakValue -= Time.deltaTime;
        else
        {
            timeBreakValue = 0;
            breakTimeObject.SetActive(false);
        }
        DisplayTime(timeBreakValue);
        DisplayTime(timeValue);

        sliderRounds.value = hiddenCounter - roundCounter;
        sliderRounds.maxValue = hiddenCounter;
    }
    public void Create()
    {
        string team1Name = string.IsNullOrEmpty(display1.text) ? "Drużyna 1" : display1.text;
        string team2Name = string.IsNullOrEmpty(display2.text) ? "Drużyna 2" : display2.text;
        team1Text.text = team1Name;
        team2Text.text = team2Name;
        PlayerPrefs.SetString("team1name", team1Name);
        PlayerPrefs.SetString("team2name", team2Name);
        nextTeamText.text = "Teraz będzie odpowiadać drużyna " + PlayerPrefs.GetString("team1name");
        PlayerPrefs.Save();
    }
    public void InitializeGame()
    {
        if ((unasweredPeople == null || unasweredPeople.Count == 0) && doWeNeedAnotherList == false)
        {
            unasweredPeople = new List<Person>(unasweredPeopleTemp);
            Shuffle(unasweredPeople);
        }
        if ((unasweredPeople == null || unasweredPeople.Count == 0) && doWeNeedAnotherList == true)
        {
            unasweredPeople = people.ToList<Person>();
            unasweredPeopleTemp = new List<Person>(unasweredPeople);
            if (unasweredPeopleTemp.Count < roundCounter)
            {
                roundCounter = unasweredPeopleTemp.Count;
                roundCounterLessQuestions = roundCounter;
                rounds = 1;
            }
            doWeNeedAnotherList = false;
        }

        if (roundCounter == 0)
        {

            rounds -= 1;
            //gameeObject.SetActive(false);
            StartCoroutine(SetActiveAfterDelay(resultsObject, true, 0.5f));
            StartCoroutine(SetActiveAfterDelay(gameeObject, false, 0.5f));
            //StartCoroutine(SetActiveAfterDelay(resultsObject, true, 1f));
            //resultsObject.SetActive(true);
            team1FinalScore = team1Score;
            team2FinalScore = team2Score;
            team1FinalText.text = team1Text.text;
            team2FinalText.text = team2Text.text;
            team1FinalScoreText.text = team1Score.ToString();
            team2FinalScoreText.text = team2Score.ToString();
            //ENDING CONDITIONS WITH 1 ROUND
            if ((team1Score > team2Score) && firstRound == true && rounds == 0)
            {
                resultsText.text = "Wygrywa drużyna \n" + PlayerPrefs.GetString("team1name") + " z " + team1RoundScore + " punktami!";
                wrocButtonObject.SetActive(true);
            }
            else if ((team1Score < team2Score) && firstRound == true && rounds == 0)
            {
                resultsText.text = "Wygrywa drużyna \n" + PlayerPrefs.GetString("team2name") + " z " + team2RoundScore + " punktami!";
                wrocButtonObject.SetActive(true);
            }
            else if ((team1Score == team2Score) && firstRound == true && rounds == 0)
            {
                resultsText.text = "Nastąpił remis!"; ;
                wrocButtonObject.SetActive(true);
            }
            //ENDING CONDITIONS WITH MORE THAN 1 ROUND
            else if ((team1Score > team2Score) && firstRound == false && rounds == 0)
            {
                resultsText.text = "Wyniki po ostatniej rundzie to:\n" + PlayerPrefs.GetString("team1name") + " - " + team1Score + "\n" + PlayerPrefs.GetString("team2name") + " - " + team2Score + "\nWygrywa drużyna " + PlayerPrefs.GetString("team1name") + " !";
                wrocButtonObject.SetActive(true);
                nextRoundButtonObject.SetActive(false);
            }
            else if ((team1Score < team2Score) && firstRound == false && rounds == 0)
            {
                resultsText.text = "Wyniki po ostatniej rundzie to:\n" + PlayerPrefs.GetString("team1name") + " - " + team1Score + "\n" + PlayerPrefs.GetString("team2name") + " - " + team2Score + "\nWygrywa drużyna " + PlayerPrefs.GetString("team2name") + " !";
                wrocButtonObject.SetActive(true);
                nextRoundButtonObject.SetActive(false);
            }
            else if ((team1Score == team2Score) && firstRound == false && rounds == 0)
            {
                resultsText.text = "Wyniki po ostatniej rundzie to:\n" + PlayerPrefs.GetString("team1name") + " - " + team1Score + "\n" + PlayerPrefs.GetString("team2name") + " - " + team2Score + "\nRemis!"; ;
                wrocButtonObject.SetActive(true);
                nextRoundButtonObject.SetActive(false);
            }
            //ROUND CONDITIONS FIRST ROUND
            else if ((team1Score == team2Score) && firstRound == true && rounds != 0)
            {
                firstRound = false;
                nextRoundButtonObject.SetActive(true);
                wrocButtonObject.SetActive(false);
                roundsNumber += 1;
                resultsText.text = "Po pierwszej rundzie drużyna " + PlayerPrefs.GetString("team1name") + " remisuje z drużyną " + PlayerPrefs.GetString("team2name");
            }
            else if ((team1Score < team2Score) && firstRound == true && rounds != 0)
            {
                firstRound = false;
                nextRoundButtonObject.SetActive(true);
                wrocButtonObject.SetActive(false);
                roundsNumber += 1;
                resultsText.text = "Po pierwszej rundzie drużyna " + PlayerPrefs.GetString("team2name") + " prowadzi z drużyna " + PlayerPrefs.GetString("team1name") + " o " + (team2Score - team1Score) + " punktów.";
            }
            else if ((team1Score > team2Score) && firstRound == true && rounds != 0)
            {
                firstRound = false;
                nextRoundButtonObject.SetActive(true);
                wrocButtonObject.SetActive(false);
                roundsNumber += 1;
                resultsText.text = "Po pierwszej rundzie drużyna " + PlayerPrefs.GetString("team1name") + " prowadzi z drużyną " + PlayerPrefs.GetString("team2name") + " o " + (team1Score - team2Score) + " punktów.";
            }
            //ROUND CONDITIONS ANTOTHER ROUND AT ALL WINS TEAM 1 
            else if ((team1Score > team2Score) && (team1RoundScore == team2RoundScore) && firstRound == false && rounds != 0)
            {
                nextRoundButtonObject.SetActive(true);
                wrocButtonObject.SetActive(false);
                roundsNumber += 1;
                resultsText.text = "W rundzie " + roundsNumber + " doszło do remisu, co sprawia, że drużyna" + PlayerPrefs.GetString("team1name") + " jest nadal na prowadzeniu!";
            }
            else if ((team1Score > team2Score) && (team1RoundScore > team2RoundScore) && firstRound == false && rounds != 0)
            {
                nextRoundButtonObject.SetActive(true);
                wrocButtonObject.SetActive(false);
                roundsNumber += 1;
                resultsText.text = "W rundzie " + roundsNumber + " drużyna " + PlayerPrefs.GetString("team1name") + " powiększyła przewagę do " + (team1Score - team2Score) + " punktów.";
            }
            else if ((team1Score > team2Score) && (team1RoundScore < team2RoundScore) && firstRound == false && rounds != 0)
            {
                nextRoundButtonObject.SetActive(true);
                wrocButtonObject.SetActive(false);
                roundsNumber += 1;
                resultsText.text = "W rundzie " + roundsNumber + " drużyna " + PlayerPrefs.GetString("team2name") + " odrobiła " + (team2RoundScore - team1RoundScore) + " punktów.";
            }
            //ROUND CONDITIONS ANTOTHER ROUND AT ALL WINS TEAM 2 
            else if ((team1Score < team2Score) && (team1RoundScore == team2RoundScore) && firstRound == false && rounds != 0)
            {
                nextRoundButtonObject.SetActive(true);
                wrocButtonObject.SetActive(false);
                roundsNumber += 1;
                resultsText.text = "W rundzie " + roundsNumber + " doszło do remisu, co sprawia, że drużyna" + PlayerPrefs.GetString("team2name") + " jest nadal na prowadzeniu!";
            }
            else if ((team1Score < team2Score) && (team1RoundScore < team2RoundScore) && firstRound == false && rounds != 0)
            {
                nextRoundButtonObject.SetActive(true);
                wrocButtonObject.SetActive(false);
                roundsNumber += 1;
                resultsText.text = "W rundzie " + roundsNumber + " drużyna " + PlayerPrefs.GetString("team2name") + " powiększyła przewagę do " + (team2Score - team1Score) + " punktów.";
            }
            else if ((team1Score < team2Score) && (team1RoundScore > team2RoundScore) && firstRound == false && rounds != 0)
            {
                nextRoundButtonObject.SetActive(true);
                wrocButtonObject.SetActive(false);
                roundsNumber += 1;
                resultsText.text = "W rundzie " + roundsNumber + " drużyna " + PlayerPrefs.GetString("team2name") + " odrobiła " + (team2RoundScore - team1RoundScore) + " punktów.";
            }
            //ROUND CONDITIONS ANTOTHER ROUND AT ALL DRAW BEFORE
            else if ((team1Score == team2Score) && (team1RoundScore == team2RoundScore) && firstRound == false && rounds != 0)
            {
                nextRoundButtonObject.SetActive(true);
                wrocButtonObject.SetActive(false);
                roundsNumber += 1;
                resultsText.text = "W rundzie " + roundsNumber + " doszło do remisu, co sprawia, że nadal jest remis, wow!";
            }
            else if ((team1Score == team2Score) && (team1RoundScore < team2RoundScore) && firstRound == false && rounds != 0)
            {
                nextRoundButtonObject.SetActive(true);
                wrocButtonObject.SetActive(false);
                roundsNumber += 1;
                resultsText.text = "W rundzie " + roundsNumber + " drużyna " + PlayerPrefs.GetString("team2name") + " doprowadza do remisu!";
            }
            else if ((team1Score == team2Score) && (team1RoundScore > team2RoundScore) && firstRound == false && rounds != 0)
            {
                nextRoundButtonObject.SetActive(true);
                wrocButtonObject.SetActive(false);
                roundsNumber += 1;
                resultsText.text = "W rundzie " + roundsNumber + " drużyna " + PlayerPrefs.GetString("team1name") + " doprowadza do remisu!";
            }

        }
        SetCurrentPerson();
        team1Text.text = PlayerPrefs.GetString("team1name");
        team2Text.text = PlayerPrefs.GetString("team2name");
        if (activeTeam == 1)
        {
            nextTeamText.text = "Teraz będzie odpowiadać drużyna " + PlayerPrefs.GetString("team2name");
        }
        else if (activeTeam == 2)
        {
            nextTeamText.text = "Teraz będzie odpowiadać drużyna " + PlayerPrefs.GetString("team1name");
        }

        unasweredPeople.RemoveRange(roundCounter, unasweredPeople.Count() - roundCounter);
    }
    public void NextRound()
    {
        team1RoundScore = 0;
        team2RoundScore = 0;
        roundCounter = hiddenCounter;
        timeValue = 0;
        resultsObject.SetActive(false);
        gameeObject.SetActive(true);
        InitializeGame();
    }
    public void RestartGame()
    {
        resultsObject.SetActive(false);
        mainMenuObject.SetActive(true);
        gameeObject.SetActive(false);
        firstRound = true;
        team1Score = 0;
        team2Score = 0;
        team1RoundScore = 0;
        team2RoundScore = 0;
        roundCounter = hiddenCounter;
        rounds = roundsAfterRestart;
        time = timeAfterRestart;
        timeValue = 0;
        roundsNumber = 0;
        doWeNeedAnotherList = true;
    }
    void Shuffle<T>(List<T> inputList)
    {
        for (int i = 0; i < inputList.Count - 1; i++)
        {
            T temp = inputList[i];
            int rand = UnityEngine.Random.Range(i, inputList.Count);
            inputList[i] = inputList[rand];
            inputList[rand] = temp;
        }
    }
    public void ShuffleArray<T>(T[] array)
    {
        System.Random rng = new System.Random();

        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = rng.Next(0, i + 1);
            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    void SetCurrentPerson()
    {
        currentPerson = unasweredPeople[number];
        roundCounterText.text = roundCounter.ToString();
        team1ScoreText.text = team1Score.ToString();
        team2ScoreText.text = team2Score.ToString();
        personText.text = currentPerson.name;
    }
    void TransitionToNextQuestion()
    {
        unasweredPeople.RemoveAt(number);
        StartCoroutine(TransitionToNextQuestionCoroutine());
    }
    IEnumerator TransitionToNextQuestionCoroutine()
    {
        yield return new WaitForSeconds(timeBetweenQuestions);
        InitializeGame();
    }
    IEnumerator TransitionToNextQuestionButThisIsSkipped()
    {
        unasweredPeople.Add(unasweredPeople[number]);
        unasweredPeople.RemoveAt(number);
        yield return new WaitForSeconds(timeBetweenQuestions);
        InitializeGame();
    }
    public void UserSelectTrue()
    {
        TransitionToNextQuestion();
        roundCounter--;
        if (activeTeam == 1)
        {
            team1Score++;
            team1RoundScore++;
        }
        else
        {
            team2Score++;
            team2RoundScore++;
        }
    }
    public void UserSelectFalse()
    {
    }
    public void UserSelectSkip()
    {
        StartCoroutine(TransitionToNextQuestionButThisIsSkipped());
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

        float breakTime = Mathf.Max(0, timeToDisplay - time + 3);
        timeBreakText.text = string.Format("{0}", Mathf.FloorToInt(breakTime));
    }
    public void Ready()
    {
        breakTimeObject.SetActive(true);
        timeBreakValue = 3;
        roundObject.SetActive(true);
        breakObject.SetActive(false);
        SwitchTeams();
        timeValue = time;
        InitializeGame();
    }
    public void SwitchTeams()
    {
        UserSelectSkip();
        if (activeTeam == 1)
        {
            activeTeam = 2;
        }
        else
        {
            activeTeam = 1;
        }
    }
    public void LoadPeopleFromJSON()
    {
        if (File.Exists(persistentFilePath))
        {
            string json = File.ReadAllText(persistentFilePath);
            PersonRoot personRoot = JsonUtility.FromJson<PersonRoot>(json);
            activePeople = new List<Person>(personRoot.people.Length);

            foreach (var person in personRoot.people)
            {
                if (person.active)
                {
                    activePeople.Add(person);
                }
            }
            people = activePeople.ToArray();
            ShuffleArray(people);
        }
        else
        {
            Debug.LogError("JSON file not found in persistentDataPath.");
        }
    }
    public void UpdateRecordActiveStatus(string key)
    {
        if (File.Exists(persistentFilePath))
        {
            try
            {
                // Read the JSON data from the file
                string jsonData = File.ReadAllText(persistentFilePath);

                // Log the content for debugging purposes
                Debug.Log("Original JSON Data: " + jsonData);

                // Deserialize JSON into PersonRoot
                PersonRoot rootObject = JsonConvert.DeserializeObject<PersonRoot>(jsonData);

                if (rootObject == null || rootObject.people == null)
                {
                    Debug.LogError("Failed to deserialize JSON or JSON structure is incorrect.");
                    return;
                }

                // Track if any person with the key was found
                bool personFound = false;

                // Update the active status of all persons with the given key
                foreach (var person in rootObject.people)
                {
                    if (person.key == key)
                    {
                        person.active = !person.active; // Toggle active status
                        personFound = true;
                    }
                }

                if (!personFound)
                {
                    Debug.LogWarning("No persons with key " + key + " found.");
                }

                // Serialize the updated object back to JSON
                string updatedJsonData = JsonConvert.SerializeObject(rootObject, Formatting.Indented);

                // Log the updated content for debugging purposes
                Debug.Log("Updated JSON Data: " + updatedJsonData);

                // Write the updated JSON data to the file
                File.WriteAllText(persistentFilePath, updatedJsonData);

                // Reload people from JSON to update in-memory data
                LoadPeopleFromJSON();
            }
            catch (Exception ex)
            {
                Debug.LogError("An error occurred while updating the record: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("JSON file not found in persistentDataPath: " + persistentFilePath);
        }
    }
    public void UpdateRoundsCount(int count)
    {
        rounds = count;
        roundsAfterRestart = count;
    }
    public void UpdateTimeRoundCount(int count)
    {
        time = count;
        timeAfterRestart = count;
    }
    public void UpdatePeopleCount(int count)
    {
        roundCounter = count;
        hiddenCounter = count;
    }
    public void UpdateRoundsSliderCount(int count)
    {
        roundsSlider = count;
    }
    private void UpdateAllRecordsToActive()
    {
        if (File.Exists(persistentFilePath))
        {
            string jsonData = File.ReadAllText(persistentFilePath);
            PersonRoot rootObject = JsonConvert.DeserializeObject<PersonRoot>(jsonData);

            foreach (var person in rootObject.people)
            {
                person.active = true;
            }

            string updatedJsonData = JsonConvert.SerializeObject(rootObject, Formatting.Indented);
            File.WriteAllText(persistentFilePath, updatedJsonData);
        }
        else
        {
            Debug.LogError("JSON file not found in persistentDataPath.");
        }
    }
    private IEnumerator SetActiveAfterDelay(GameObject target, bool state, float delay)
    {
        yield return new WaitForSeconds(delay);
        target.SetActive(state);
    }
}




