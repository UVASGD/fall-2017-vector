using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public TimeManager time;
    Personality mind;

    int obsTimer = 4;
    int moveTimer = 2;
    int actTimer = 2;

    char moveQ = 'w';
    float dashTime = 0.5f;



    public PlayerScript() {
    }

    // Use this for initialization
    void Start () {
        //setHealthBar();
        //This is so the time variable is actually referencing the time manager object
        time = (TimeManager)FindObjectOfType(typeof(TimeManager));
        tag = "Player";
    }

	// Update is called once per frame
	void Update () {
        //CONTROLS HERE
        if (Input.anyKeyDown) {
            int moveKey = (int)Input.GetAxisRaw("Horizontal");
            if (moveQ == 'w') {
                dashTime = 0.5f;
                if (moveKey == 1) {
                    moveQ = 'l';
                    moveTimer = 2;
                }
                else if (moveKey == -1) {
                    moveQ = 'r';
                    moveTimer = 2;
                }
            }
            else if (dashTime > 0){
                if (moveQ == 'l' && moveKey == 1) {
                    moveQ = 'L';
                    moveTimer = 1;
                    dashTime = 0;
                }
                else if (moveQ == 'r' && moveKey == -1) {
                    moveQ = 'R';
                    moveTimer = 1;
                    dashTime = 0;
                }
            }
        }

        if (dashTime > 0)
            dashTime -= Time.deltaTime;

        if (time.clock) {
            Perform();
        }
	}

    void Perform() {
        if (moveTimer == 0) {
            moveTimer = Move();
            //mind.Cool();
        }

        moveTimer--;
    }

    int Observe() {
        return 4;
    }

    int Move() {
        switch (moveQ) {
            case 'l':
                gameObject.transform.Translate(1, 0, 0);
                moveQ = 'w';
                break;
            case 'L':
                gameObject.transform.Translate(1, 0, 0);
                moveQ = 'l';
                moveTimer = 1;
                break;
            case 'r':
                gameObject.transform.Translate(-1, 0, 0);
                moveQ = 'w';
                break;
            case 'R':
                gameObject.transform.Translate(-1, 0, 0);
                moveQ = 'r';
                moveTimer = 1;
                break;
            case 'w':
                moveTimer++;
                break;
        }

        return moveTimer;
    }

    int Act() {
        return 2;
    }
}
