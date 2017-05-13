using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : Person {

    public TimeManager time;
    Personality mind;

    int clocks = 0;

	// Use this for initialization
	void Start () {
        setHealthBar();
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
