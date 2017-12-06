using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TagFrenzy;

public class PersonCreator { //This class exists just to spawn in a person. Not only will it need to be updated to reflect the progress made in relation to the 'thing'
    //structure, but it will also be entirely replaced by the xml parser

    GameObject personBodyObject;
    GameObject healthBar;
    GameObject bodyRenderObject;

    public GameObject CreatedPerson { get { return personBodyObject; } }

    AI mind;
    string sortingLayerNum;

    string renderName;
    string objectName;
    string id;
    string prefab;
    float loc;
    int size;

    AINum mindNum;

    List<string> smallTalk;

    public PersonCreator(string _objectName, string _id, float _loc, int _size, AINum _minNum, List<string> _smallTalk) {
        objectName = _objectName; //Name of the actual person game object once in play
        renderName = _objectName; //Name of the sprite rendering prefab found in Resources
        prefab = "Body"; //Name of the prefab of the actual person object found in Resources
        id = _id; //The name other NPCs will use to identify your character in the associator
        loc = _loc; //Location of the object
        size = _size; //Size of the person. For now, please keep sizes from 0 through 6
        sortingLayerNum = size.ToString(); //Where the gameobject is placed in the sorting layer. This will only affect render order, so objects can still collide
        mindNum = _minNum;
        smallTalk = _smallTalk;

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
        personBodyObject = Object.Instantiate(Resources.Load(prefab), new Vector3(loc, -3.5f, 0), Quaternion.identity) as GameObject;
        personBodyObject.AddComponent(System.Type.GetType("Body"));

        Body personBody = personBodyObject.GetComponent<Body>();

        BoxCollider2D coll = personBodyObject.GetComponent<BoxCollider2D>();
        coll.isTrigger = true;
        coll.size = new Vector3(size-0.1f, 2f, 0f);

        personBodyObject.name = objectName;

        List<Association> dummyAssociator = new List<Association>();
        MoodHandler dummyMoodHandler = new MoodHandler(new List<Mood>() { });

        if (mindNum == AINum.player) {
            personBody.BodyConstructor(id, size, Direction.Left, new List<string> { "Hostile" }, new PlayerAI(new Personality(personBody, dummyAssociator, 
                new Identity(id, false, "", "", personBodyObject), dummyMoodHandler, "...", smallTalk), personBody));
            personBodyObject.AddTag("Player");

            // Sword sword = new Sword(personBody, 1);
            // personBody.Weapon = sword;
            Fists fists = new Fists(personBody, 1);
            personBody.Weapon = fists;
        }
        else if (mindNum == AINum.soldier) {

            //ASSOCIATOR DEFINITION HERE
            List<Association> associator = new List<Association>() {
                new MoodAssoc("Charm", "charm", CoreMood.CharmAxis, 0.10f, 0.10f, 1, new Dictionary<string, Interaction>() { }),
                new MoodAssoc("Disgust", "disgust", CoreMood.CharmAxis, -0.45f, 0.20f, -1, new Dictionary<string, Interaction>() { }),
                new MoodAssoc("Amusement", "amusement", CoreMood.AmuseAxis, 0.05f, 0.05f, 1, new Dictionary<string, Interaction>() { }),
                new MoodAssoc("Anger", "anger", CoreMood.AmuseAxis, -0.60f, 0.50f, -1, new Dictionary<string, Interaction>() { }),
                new MoodAssoc("Happiness", "happiness", CoreMood.HappyAxis, 0.10f, 0.05f, 1, new Dictionary<string, Interaction>() { }),
                new MoodAssoc("Sadness", "sadness", CoreMood.HappyAxis, -0.10f, 0.10f, -1, new Dictionary<string, Interaction>() { }),
                new MoodAssoc("Inspiration", "inspiration", CoreMood.InspireAxis, 0.10f, 0.10f, 1, new Dictionary<string, Interaction>() { }),
                new MoodAssoc("Intimidation", "intimidation", CoreMood.InspireAxis, -0.70f, 0.70f, -1, new Dictionary<string, Interaction>() { }),

                new VerbAssoc("foils", "foils", "foiling", -0.70f, 0.50f, new Dictionary<string, Interaction>() {
                    {"anger", new Interaction(1, .65f)}}),
                new VerbAssoc("helps", "helps", "helping", 0.15f, 0.15f, new Dictionary<string, Interaction>() {
                    {"charm", new Interaction(1, 0.25f)},
                    {"happiness", new Interaction(1, 0.20f)},
                    {"inspiration", new Interaction(1, 0.10f)}}),
                new VerbAssoc("swings their weapon", "swings weapon", "swinging a weapon", -0.25f, 0.25f, new Dictionary<string, Interaction>() {
                 {"intimidation", new Interaction(1, 0.60f)}}),
                new VerbAssoc("kills", "kills", "killing", -0.95f, 1, new Dictionary<string, Interaction>() {
                 {"inspiration", new Interaction(1, 0.90f)},
                 {"happiness", new Interaction(1, 0.80f)}}),
                new VerbAssoc("brawls", "brawls", "starting a fight", -0.70f, 0.90f, new Dictionary<string, Interaction>() {
                {"charm", new Interaction(1, 0.25f)},
                {"amusement", new Interaction(1, 0.10f)},
                {"anger", new Interaction(1, 0.80f)}}),
                new VerbAssoc("gives", "gives", "giving", 0.35f, 0.75f, new Dictionary<string, Interaction>() {
                 {"charm", new Interaction(1, 0.25f)},
                 {"amusement", new Interaction(1, 0.50f)}}),

                 new PersonAssoc("Player", "player", 0, 0.50f, new Dictionary<string, Interaction>() { }),
                 new PersonAssoc("Soldier", "soldier", 0.80f, 0.70f, new Dictionary<string, Interaction>() { }),
                 new PersonAssoc("Bear", "bear", -0.80f, 0.60f, new Dictionary<string, Interaction>() {
                     { "sadness", new Interaction(1, 0.70f)},
                     { "anger", new Interaction(1, 0.50f)}
                 })};

            MoodHandler moodHandler = new MoodHandler(new List<Mood>() {
                new Mood("charmed", "charm", "disgusted", "disgust", 0, CoreMood.CharmAxis),
                new Mood("amused", "amusement", "angered", "anger", -.40f, CoreMood.AmuseAxis),
                new Mood("happy", "happiness", "sad", "sadness", 0, CoreMood.HappyAxis),
                new Mood("inspired", "inspiration", "frightened", "intimidation", 0, CoreMood.InspireAxis)});

            personBody.BodyConstructor(id, size, Direction.Left, new List<string> { "Hostile" }, new PersonAI(new Personality(personBody, associator, 
                new Identity(id, false, "", "", personBodyObject), moodHandler, "Welcome to Middleburg. Don't break anything.", smallTalk), 
                new QuestPicker(personBody),personBody));
            personBodyObject.AddTag("Hostile");
        }
        else if (mindNum == AINum.dummy) {
            personBody.BodyConstructor(id, size, Direction.Left, new List<string> { "Hostile" }, new AI(new Personality(), personBody));
            personBodyObject.AddTag("Hostile");
        }
        else if (mindNum == AINum.turret) {
            personBody.BodyConstructor(id, size, Direction.Right, new List<string> { "Hostile", "Player" }, new DogAI(new Personality(personBody, dummyAssociator,
                new Identity(id, false, "", "", personBodyObject), dummyMoodHandler, "...", smallTalk), personBody));
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
