using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInput : MonoBehaviour {
    InputField input;
    InputField.SubmitEvent se;
    public Text output;
    Body bodyReference;
    Dictionary<string, string[]> eventOptions;

	void Start() {
        input = gameObject.GetComponent<InputField>();
        se = new InputField.SubmitEvent();
        se.AddListener(SubmitInput);
        input.onEndEdit = se;
        output = transform.parent.parent.GetChild(1).GetChild(0).GetComponent<Text>();
        eventOptions = new Dictionary<string, string[]>() { };
    }

    public void SetBodyRef(Body _bodyRef) {
        bodyReference = _bodyRef;
    }

    public void SubmitInput(string args) {
        if (bodyReference.DiaStage == DialogueStage.Nil) { }
        else if (bodyReference.DiaStage == DialogueStage.Greeting) {
            if (args.Equals("e")) {
                output.text = "";
                int i = 1;
                foreach (string[] option in bodyReference.Enquire()) {
                    eventOptions.Add(i.ToString(), option);
                    output.text += i + ": " + string.Join(" ", option);
                    i++;
                }
            }
            else if (args.Equals("r")) {
                output.text = "";
                int i = 1;
                foreach (string[] option in bodyReference.Reveal()) {
                    eventOptions.Add(i.ToString(), option);
                    output.text += i + ": " + string.Join(" ", option);
                    i++;
                }
            }
        }
        else if (bodyReference.DiaStage == DialogueStage.Enquiring) {
            int eventPicker = 0;
            if (System.Int32.TryParse(args, out eventPicker)) {
                output.text = "";
                bodyReference.Discuss(eventOptions[eventPicker.ToString()], true);
                eventOptions = new Dictionary<string, string[]>() { };
            }
        }
        else if (bodyReference.DiaStage == DialogueStage.Revealing) {
            int eventPicker = 0;
            if (System.Int32.TryParse(args, out eventPicker)) {
                output.text = "";
                bodyReference.Discuss(eventOptions[eventPicker.ToString()], false);
                eventOptions = new Dictionary<string, string[]>() { };
            }
        }
        else if (bodyReference.DiaStage == DialogueStage.Discussing) {
            if (args.Equals("e")) {
                output.text = "";
                bodyReference.Talk();
            }
        }
        input.text = "";
        input.ActivateInputField();
    }

    public void Deactivate() {
        input.DeactivateInputField();
        input.interactable = false;
    }

    public void SetInteractable() {
        input.interactable = true;
        input.ActivateInputField();
    }
	
}
