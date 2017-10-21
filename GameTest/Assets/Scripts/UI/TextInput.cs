using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInput : MonoBehaviour {
    InputField input;
    InputField.SubmitEvent se;
    public Text output;
    Body bodyReference;

	void Start () {
        input = gameObject.GetComponent<InputField>();
        se = new InputField.SubmitEvent();
        se.AddListener(SubmitInput);
        input.onEndEdit = se;
        output = transform.parent.parent.GetChild(1).GetChild(0).GetComponent<Text>();
    }

    public void SetBodyRef(Body _bodyRef) {
        bodyReference = _bodyRef;
    }

    public void SubmitInput(string args) {
        if (bodyReference.DiaStage == DialogueStage.Nil) { }
        else if (bodyReference.DiaStage == DialogueStage.Greeting) {
            if (args.Equals("e")) {
                output.text = "";
                bodyReference.Enquire();
            }
        }
        else if (bodyReference.DiaStage == DialogueStage.Enquiring) {
            int eventPicker = 0;
            if (System.Int32.TryParse(args, out eventPicker)) {
                output.text = "";
                bodyReference.Discuss(eventPicker);
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
	
}
