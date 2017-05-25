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
    float input;

    void Start() {
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
        moodToggles = (ToggleGroup)FindObjectOfType(typeof(ToggleGroup));
        moodCanvas = GameObject.FindGameObjectWithTag("CanvasTest");
        person = GameObject.FindGameObjectWithTag("Player");
        personality = person.GetComponent<Personality>();

        qInput = (moodCanvas.transform.Find("QuantInput").gameObject).GetComponent<InputField>();
        input = System.Convert.ToSingle(qInput.text);
        Personality.Mood mood = (Personality.Mood)getToggleNum();
        Debug.Log("Mood quant before application: " + personality.GetQuant(mood));
        personality.ChangeQuant(mood, input);
        Debug.Log("Mood quant after application: " + personality.GetQuant(mood));
    }
}
