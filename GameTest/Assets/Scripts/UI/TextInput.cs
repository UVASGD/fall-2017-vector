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
    }

    public void SetBodyRef(Body _bodyRef) {
        bodyReference = _bodyRef;
    }

    public void SubmitInput(string args) {
        if (bodyReference.DiaStage == DialogueStage.Nil)
            return;
        else if (bodyReference.DiaStage == DialogueStage.Greeting) {
            if (args.Equals("e"))
                bodyReference.Enquire();
        }
        else if (bodyReference.DiaStage == DialogueStage.Enquiring) {
            int eventPicker = 0;
            if (System.Int32.TryParse(args, out eventPicker)) {
                bodyReference.Discuss(eventPicker);
            }
        }
        else if (bodyReference.DiaStage == DialogueStage.Discussing) {
            if (args.Equals("e")) {
                bodyReference.Talk();
            }
        }
    }
	
}
