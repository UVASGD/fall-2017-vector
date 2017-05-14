using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person {

    GameObject person;
    GameObject healthBar;
    GameObject body;
    string script;
    string sortingLayerNum;

    string renderName;
    string objectName;
    string type;
    float loc;
    int size;

    public Person(string b, string n, string t, float l, int s, string sc) {
        renderName = b;
        objectName = n;
        type = t;
        loc = l;
        size = s;
        sortingLayerNum = size.ToString();
        script = sc;

        setObject();

        setHealthBar();

        setBody();
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setObject() {
        person = (GameObject)MonoBehaviour.Instantiate(Resources.Load(type), new Vector3(loc, 0, 0), Quaternion.identity);
        BoxCollider2D coll = person.GetComponent<BoxCollider2D>();
        coll.size = new Vector3(size*2, 1f, 0f);
        person.AddComponent(System.Type.GetType(script));
        person.name = objectName;
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
