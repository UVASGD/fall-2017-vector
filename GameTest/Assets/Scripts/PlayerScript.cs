using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public TimeManager time;
    Personality mind;

    int clocks = 0;


    public PlayerScript() {
    }

    // Use this for initialization
    void Start () {
        //setHealthBar();
        time = (TimeManager)FindObjectOfType(typeof(TimeManager)); //This is so the time variable is actually referencing the time manager object
        tag = "Player";
    }

	// Update is called once per frame
	void Update () {
        if (time.clock)
            Act();
	}

    void Act() {
        if (clocks % 2 == 0)
            Debug.Log("TICK");
        else
            Debug.Log("TOCK");
        clocks++;
        //mind.Cool();
    }
}
