using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

    public bool clock = false;  // In a frame that this bool is set, all listeners with timers will clock one cycle.

    public float clockTime = 1f;  // The 'clock' bool will be set for one frame every $clockTime seconds.
    private float sinceClock = 0f;  // This will count up by deltaTime every frame, and once it has exceeded or equaled 'clockTime', then 'clock' will be set.

    private bool pause = false; // if this is set, then 'sinceClock' will not count up, and the clock signal will not be sent

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!pause)
            sinceClock += Time.deltaTime*25;
        if (sinceClock >= clockTime) {
            clock = true;
            sinceClock = 0;
        }
        else if (clock == true)
            clock = false;
	}

    void PauseToggle() {
        pause = !pause;
    }

    void ChangeClockTime(float newClockTime) {
        clockTime = newClockTime;
    }

    void Clock() {
        clock = true;
    }
}
