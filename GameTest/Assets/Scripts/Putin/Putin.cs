using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Putin : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKey || Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
            SceneManager.LoadScene("Menu");
        }
	}
}
