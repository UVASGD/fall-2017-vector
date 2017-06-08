using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplyQuantScript : MonoBehaviour {

    public ToggleGroup moodToggles;
    public GameObject person;
    public Personality personality;
    public GameObject moodCanvas;
    public InputField qInput;
    public TimeManager time;

    float input;

    void Start() {
        moodToggles = (ToggleGroup)FindObjectOfType(typeof(ToggleGroup));
        moodCanvas = GameObject.FindGameObjectWithTag("CanvasTest");
        person = GameObject.FindGameObjectWithTag("Player");
        personality = person.GetComponent<Personality>();
        //This is so the time variable is actually referencing the time manager object
        time = (TimeManager)FindObjectOfType(typeof(TimeManager));
    }

    void Update() {
        if (time.clock) {
            MoodSpeak();
        }
    }

    public int getToggleNum() {
        for (int i = 0; i < moodToggles.transform.childCount; i++) {
            if (moodToggles.transform.GetChild(i).GetComponent<Toggle>().isOn) {
                Debug.Log(i);
                return i;
            }
        }
        return 0;
    }

    public void onApply() {
        /*
        moodToggles = (ToggleGroup)FindObjectOfType(typeof(ToggleGroup));
        moodCanvas = GameObject.FindGameObjectWithTag("CanvasTest");
        person = GameObject.FindGameObjectWithTag("Player");
        //personality = person.GetComponent<Personality>();
        */

        qInput = (moodCanvas.transform.Find("QuantInput").gameObject).GetComponent<InputField>();
        input = System.Convert.ToSingle(qInput.text);
        Personality.Mood mood = (Personality.Mood)getToggleNum();
        Debug.Log("Mood quant before application: " + personality.GetQuant(mood));
        personality.ChangeQuant(mood, input);
        Debug.Log("Mood quant after application: " + personality.GetQuant(mood));
    }

    string[] quotes = { "ooooh my", "lolol", "Hoho!", "yey",
                        "ew", "o boi i mad", "o no i scare", "buhuhu"};
    void MoodSpeak() {
        for (int i = 0; i < 8; i++) {
            if (personality.IsOverThreshold((Personality.Mood)i))
                Debug.Log(quotes[i]);
        }
    }
}
