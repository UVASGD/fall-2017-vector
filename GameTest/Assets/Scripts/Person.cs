using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {

    GameObject healthBar;
    string sortingLayerName = "Size1";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setHealthBar() {
        healthBar = (GameObject)Instantiate(Resources.Load("HealthBar"), transform);
        SpriteRenderer healthRender = healthBar.GetComponent<SpriteRenderer>();

        healthRender.sortingLayerName = sortingLayerName;

        transform.Translate(Vector3.left);
    }
}
