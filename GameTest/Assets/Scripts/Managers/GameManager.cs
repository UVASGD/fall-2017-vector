using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Left = -1, Right = 1 };
//public enum Reaction { Dampening, Oiling, Watering, Dirtying, Drying, Burning, Fueling, Hindering, Freeing, Harming, Healing, Crushing, Slashing, Piercing};

public enum AINum { player, dummy, turret};

public class GameManager : MonoBehaviour {
    /*enum Mood
{
Charm, Amuse, Inspire, Happy,
Disgust, Anger, Intimidate, Sad
};*/

    public GameObject timeManager;


    //Sensitize MoodAffecters
    public PTrait sensitizeChaLow = new Sensitize(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait sensitizeChaMed = new Sensitize(Personality.Mood.Sad, 1f, 1.5f);
    public PTrait sensitizeChaHig = new Sensitize(Personality.Mood.Sad, 1.5f, 1.5f);
    public PTrait desensitizeChaLow = new Sensitize(Personality.Mood.Sad, -.5f, 1.5f);
    public PTrait desensitizeChaMed = new Sensitize(Personality.Mood.Sad, -1f, 1.5f);
    public PTrait desensitizeChaHig = new Sensitize(Personality.Mood.Sad, -1.5f, 1.5f);

    public PTrait sensitizeAmuLow = new Sensitize(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait sensitizeAmuMed = new Sensitize(Personality.Mood.Sad, 1f, 1.5f);
    public PTrait sensitizeAmuHig = new Sensitize(Personality.Mood.Sad, 1.5f, 1.5f);
    public PTrait desensitizeAmuLow = new Sensitize(Personality.Mood.Sad, -.5f, 1.5f);
    public PTrait desensitizeAmuMed = new Sensitize(Personality.Mood.Sad, -1f, 1.5f);
    public PTrait desensitizeAmuHig = new Sensitize(Personality.Mood.Sad, -1.5f, 1.5f);

    public PTrait sensitizeInsLow = new Sensitize(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait sensitizeInsMed = new Sensitize(Personality.Mood.Sad, 1f, 1.5f);
    public PTrait sensitizeInsHig = new Sensitize(Personality.Mood.Sad, 1.5f, 1.5f);
    public PTrait desensitizeInsLow = new Sensitize(Personality.Mood.Sad, -.5f, 1.5f);
    public PTrait desensitizeInsMed = new Sensitize(Personality.Mood.Sad, -1f, 1.5f);
    public PTrait desensitizeInsHig = new Sensitize(Personality.Mood.Sad, -1.5f, 1.5f);

    public PTrait sensitizeHapLow = new Sensitize(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait sensitizeHapMed = new Sensitize(Personality.Mood.Sad, 1f, 1.5f);
    public PTrait sensitizeHapHig = new Sensitize(Personality.Mood.Sad, 1.5f, 1.5f);
    public PTrait desensitizeHapLow = new Sensitize(Personality.Mood.Sad, -.5f, 1.5f);
    public PTrait desensitizeHapMed = new Sensitize(Personality.Mood.Sad, -1f, 1.5f);
    public PTrait desensitizeHapHig = new Sensitize(Personality.Mood.Sad, -1.5f, 1.5f);

    public PTrait sensitizeDisLow = new Sensitize(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait sensitizeDisMed = new Sensitize(Personality.Mood.Sad, 1f, 1.5f);
    public PTrait sensitizeDisHig = new Sensitize(Personality.Mood.Sad, 1.5f, 1.5f);
    public PTrait desensitizeDisLow = new Sensitize(Personality.Mood.Sad, -.5f, 1.5f);
    public PTrait desensitizeDisMed = new Sensitize(Personality.Mood.Sad, -1f, 1.5f);
    public PTrait desensitizeDisHig = new Sensitize(Personality.Mood.Sad, -1.5f, 1.5f);

    public PTrait sensitizeAngLow = new Sensitize(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait sensitizeAngMed = new Sensitize(Personality.Mood.Sad, 1f, 1.5f);
    public PTrait sensitizeAngHig = new Sensitize(Personality.Mood.Sad, 1.5f, 1.5f);
    public PTrait desensitizeAngLow = new Sensitize(Personality.Mood.Sad, -.5f, 1.5f);
    public PTrait desensitizeAngMed = new Sensitize(Personality.Mood.Sad, -1f, 1.5f);
    public PTrait desensitizeAngHig = new Sensitize(Personality.Mood.Sad, -1.5f, 1.5f);

    public PTrait sensitizeIntLow = new Sensitize(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait sensitizeIntMed = new Sensitize(Personality.Mood.Sad, 1f, 1.5f);
    public PTrait sensitizeIntHig = new Sensitize(Personality.Mood.Sad, 1.5f, 1.5f);
    public PTrait desensitizeIntLow = new Sensitize(Personality.Mood.Sad, -.5f, 1.5f);
    public PTrait desensitizeIntMed = new Sensitize(Personality.Mood.Sad, -1f, 1.5f);
    public PTrait desensitizeIntHig = new Sensitize(Personality.Mood.Sad, -1.5f, 1.5f);

    public PTrait sensitizeSadLow = new Sensitize(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait sensitizeSadMed = new Sensitize(Personality.Mood.Sad, 1f, 1.5f);
    public PTrait sensitizeSadHig = new Sensitize(Personality.Mood.Sad, 1.5f, 1.5f);
    public PTrait desensitizeSadLow = new Sensitize(Personality.Mood.Sad, -.5f, 1.5f);
    public PTrait desensitizeSadMed = new Sensitize(Personality.Mood.Sad, -1f, 1.5f);
    public PTrait desensitizeSadHig = new Sensitize(Personality.Mood.Sad, -1.5f, 1.5f);

    //Redirect MoodAffecters
    public PTrait redirectChaLow = new Redirect(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait redirectChaHig = new Redirect(Personality.Mood.Sad, 1f, 1.5f);

    public PTrait redirectAmuLow = new Redirect(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait redirectAmuHig = new Redirect(Personality.Mood.Sad, 1f, 1.5f);

    public PTrait redirectInsLow = new Redirect(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait redirectInsHig = new Redirect(Personality.Mood.Sad, 1f, 1.5f);

    public PTrait redirectHapLow = new Redirect(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait redirectHapHig = new Redirect(Personality.Mood.Sad, 1f, 1.5f);

    public PTrait redirectDisLow = new Redirect(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait redirectDisHig = new Redirect(Personality.Mood.Sad, 1f, 1.5f);

    public PTrait redirectAngLow = new Redirect(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait redirectAngHig = new Redirect(Personality.Mood.Sad, 1f, 1.5f);

    public PTrait redirectIntLow = new Redirect(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait redirectIntHig = new Redirect(Personality.Mood.Sad, 1f, 1.5f);

    public PTrait redirectSadLow = new Redirect(Personality.Mood.Sad, .5f, 1.5f);
    public PTrait redirectSadHig = new Redirect(Personality.Mood.Sad, 1f, 1.5f);

    //MoodLink MoodAffecters

    public static GameManager instance = null;
	// Use this for initialization
	void Awake () {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }

        PersonCreator player = new PersonCreator("RPlayer", "Player", "Player", 0.5f, 1, AINum.player);
        Body playerBody = (Body)FindObjectOfType(typeof(Body));
        //playerBody.AddAffecter(new Fire(playerBody, 40f));
        //playerBody.AddAffecter(new Fire(playerBody, 20f));
        PersonCreator innkeeper = new PersonCreator("RInnkeeper", "Innkeeper", "Innkeeper", -6.5f, 1, AINum.turret);

        //List<Item> items = new List<Item> { };

        //GameObject packObj = Instantiate(Resources.Load("ItemPackage"), new Vector3(5.5f, 0f), Quaternion.identity) as GameObject;

        //ItemPackage pack = packObj.GetComponent<ItemPackage>();
        //pack.CreateItemPackage(items, "RChest", 5.5f, 2);

        //Sword sword2 = new Sword(innkeeperBody, 1);

        DontDestroyOnLoad(gameObject);
    }

	// Update is called once per frame
	void Update () {
		
	}
}

