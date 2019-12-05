using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class scoreKeeper : MonoBehaviour
{
    // private
    public const string Hostname = "https://school-puzzle-api.herokuapp.com/api/";

    static List<string> Scores; // list of names of all stops scored
    static Text ScoresText; // text for showing score
    static int MaxStops; // how many stops are available

    // Start is called before the first frame update
    void Start()
    {
        Scores = new List<string>(); // list that contains names of all scored stops
        ScoresText = this.gameObject.GetComponent<Text>();

        // get and set max score
        StartCoroutine(GetAndSetTotalStops());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool IsStepScored(string recognizedTargetName)
    {
        return Scores.Contains(recognizedTargetName);
    }


    public static void AddScore(string recognizedTargetName)
    {
        Scores.Add(recognizedTargetName);
        UpdateScoreText(Scores.Count);
    }

    public static void UpdateScoreText(int score)
    {
        ScoresText.text = score + " of " + MaxStops;
    }

    IEnumerator GetAndSetTotalStops()
    {
        UnityWebRequest uwr = UnityWebRequest.Get(Hostname+"stats");
        yield return uwr.SendWebRequest();

        if (!uwr.isNetworkError)
        {
            Debug.Log("<color=blue>"+ uwr.downloadHandler.text+"</color>");
            var stats = JsonUtility.FromJson<Stats>(uwr.downloadHandler.text);
            MaxStops = stats.TotalStops;
            UpdateScoreText(0);
        }
        else
        {
            Debug.LogError("Failed to fetch data");
        }
    }
}
