using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

    public GameObject tickMarker;
    public bool clock = false;  // In a frame that this bool is set, all listeners with timers will clock one cycle.

    float clockTime = 0.001f;  // The 'clock' bool will be set for one frame every $clockTime seconds.
    private float sinceClock = 0f;  // This will count up by deltaTime every frame, and once it has exceeded or equaled 'clockTime', then 'clock' will be set.

    private bool pause = false; // if this is set, then 'sinceClock' will not count up, and the clock signal will not be sent

	// Use this for initialization
	void Start () {
        tickMarker = GameObject.FindGameObjectWithTag("TimerMark");
	}
	
	// Update is called once per frame
	void Update () {
        if (!pause)
            sinceClock += Time.deltaTime;
        if (sinceClock >= clockTime) {
            clock = true;
            sinceClock = 0;
        }
        else if (clock == true)
            clock = false;
        tickMarker.transform.localScale = new Vector3(32 * (sinceClock / clockTime), 0);
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
