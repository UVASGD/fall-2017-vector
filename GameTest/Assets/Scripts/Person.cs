using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person {

    GameObject person;
    GameObject healthBar;
    GameObject body;
    string sortingLayerNum;

    string renderName;
    string type;
    float loc;
    int size;

    public Person(string b, string n, string t, float l, int s) {
        person = (GameObject)MonoBehaviour.Instantiate(Resources.Load(t), new Vector3(l, 0, 0), Quaternion.identity);
        renderName = b;
        person.name = n;
        type = t;
        loc = l;
        size = s;
        sortingLayerNum = size.ToString();

        setHealthBar();

        setBody();
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setBody() {
        body = (GameObject)MonoBehaviour.Instantiate(Resources.Load(renderName), person.transform);
        SpriteRenderer bodyRender = body.GetComponent<SpriteRenderer>();

        bodyRender.sortingLayerName = sortingLayerNum;
    }

    public void setHealthBar() {
        healthBar = (GameObject)MonoBehaviour.Instantiate(Resources.Load("RHealthBar"), person.transform);
        SpriteRenderer healthRender = healthBar.GetComponent<SpriteRenderer>();

        healthRender.sortingLayerName = sortingLayerNum;
    }
}
