using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TagFrenzy;

public class PersonCreator { //This class exists just to spawn in a person. Not only will it need to be updated to reflect the progress made in relation to the 'thing'
    //structure, but it will also be entirely replaced by the xml parser

    GameObject personBodyObject;
    GameObject healthBar;
    GameObject bodyRenderObject;


    AI mind;
    string sortingLayerNum;

    string renderName;
    string objectName;
    string prefab;
    float loc;
    int size;

    AINum mindNum;

    public PersonCreator(string _renderName, string _objectName, string _prefab, float _loc, int _size, AINum _minNum) {
        renderName = _renderName; //Name of the sprite rendering prefab found in Resources
        objectName = _objectName; //Name of the actual person game object once in play
        prefab = _prefab; //Name of the prefab of the actual person object found in Resources
        loc = _loc; //Location of the object
        size = _size; //Size of the person. For now, please keep sizes from 0 through 6
        sortingLayerNum = size.ToString(); //Where the gameobject is placed in the sorting layer. This will only affect render order, so objects can still collide
        mindNum = _minNum;

        SetObject(); //Establishes the actual game object with size and location and collider

        //SetHealthBar(); //Sets the healthbar. Make sure this comes before setBody

        SetBody(); //Sets the body render
    }


    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {		
	}

    public void SetObject() {
        personBodyObject = Object.Instantiate(Resources.Load(prefab), new Vector3(loc, 0, 0), Quaternion.identity) as GameObject;
        personBodyObject.AddComponent(System.Type.GetType("Body"));

        Body personBody = personBodyObject.GetComponent<Body>();

        BoxCollider2D coll = personBodyObject.GetComponent<BoxCollider2D>();
        coll.isTrigger = true;
        coll.size = new Vector3(size*2, 1f, 0f);

        personBodyObject.name = objectName;

        if (mindNum == AINum.player) {
            personBody.BodyConstructor(size, Direction.Left, new List<string> { "Hostile" }, new PlayerAI(new Personality(), personBody));
            personBody.Mind.Start();
            personBodyObject.AddTag("Player");

            Sword sword = new Sword(personBody, 1);
            personBody.Weapon = sword;
        }
        else if (mindNum == AINum.dummy) {
            personBody.BodyConstructor(size, Direction.Left, new List<string> { "Hostile" }, new AI(new Personality(), personBody));
            personBodyObject.AddTag("Hostile");
        }
        else if (mindNum == AINum.turret) {
            personBody.BodyConstructor(size, Direction.Right, new List<string> { "Hostile", "Player" }, new DogAI(new Personality(), personBody));
            personBodyObject.AddTag("Hostile");
            Sword sword = new Sword(personBody, 1);
            personBody.Weapon = sword;
        }
    }

    public void SetBody() {
        bodyRenderObject = (GameObject)MonoBehaviour.Instantiate(Resources.Load(renderName), personBodyObject.transform);
        SpriteRenderer bodyRender = bodyRenderObject.GetComponent<SpriteRenderer>();

        bodyRender.sortingLayerName = sortingLayerNum;
    }

    public void SetHealthBar() {
        healthBar = (GameObject)MonoBehaviour.Instantiate(Resources.Load("RHealthBar"), personBodyObject.transform);
        SpriteRenderer healthRender = healthBar.GetComponent<SpriteRenderer>();

        healthRender.sortingLayerName = sortingLayerNum;
    }
}
