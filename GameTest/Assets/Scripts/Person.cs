using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person {

    GameObject person;
    GameObject healthBar;
    GameObject body;

    Personality mind;

    string script;
    string sortingLayerNum;

    string renderName;
    string objectName;
    string type;
    float loc;
    int size;

    public Person(string b, string n, string t, float l, int s, string sc) {
        renderName = b; //Name of the sprite rendering prefab found in Resources
        objectName = n; //Name of the actual person game object once in play
        type = t; //Name of the prefab of the actual person object found in Resources
        loc = l; //Location of the object
        size = s; //Size of the person. For now, please keep sizes from 0 through 6
        sortingLayerNum = size.ToString(); //Where the gameobject is placed in the sorting layer. This will only affect render order, so objects can still collide
        script = sc; //The name of the script that should be attached to the object

        SetObject(); //Establishes the actual game object with size and location and collider

        SetHealthBar(); //Sets the healthbar. Make sure this comes before setBody

        SetBody(); //Sets the body render
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetObject() {
        person = (GameObject)MonoBehaviour.Instantiate(Resources.Load(type), new Vector3(loc, 0, 0), Quaternion.identity);
        person.AddComponent(System.Type.GetType("Personality"));
        BoxCollider2D coll = person.GetComponent<BoxCollider2D>();
        coll.isTrigger = true;
        coll.size = new Vector3(size*2, 1f, 0f);
        person.AddComponent(System.Type.GetType(script));
        person.name = objectName;
    }

    public void SetBody() {
        body = (GameObject)MonoBehaviour.Instantiate(Resources.Load(renderName), person.transform);
        SpriteRenderer bodyRender = body.GetComponent<SpriteRenderer>();

        bodyRender.sortingLayerName = sortingLayerNum;
    }

    public void SetHealthBar() {
        healthBar = (GameObject)MonoBehaviour.Instantiate(Resources.Load("RHealthBar"), person.transform);
        SpriteRenderer healthRender = healthBar.GetComponent<SpriteRenderer>();

        healthRender.sortingLayerName = sortingLayerNum;
    }
}
