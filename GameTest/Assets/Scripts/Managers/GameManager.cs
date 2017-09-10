using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Left = -1, Right = 1 };
//public enum Reaction { Dampening, Oiling, Watering, Dirtying, Drying, Burning, Fueling, Hindering, Freeing, Harming, Healing, Crushing, Slashing, Piercing};

public enum AINum { player, dummy, turret};

public enum Mood {
    Charm, Amuse, Inspire, Happy,
    Disgust, Anger, Intimidate, Sad,
    None
};

public class GameManager : MonoBehaviour {
    /*enum Mood
{
Charm, Amuse, Inspire, Happy,
Disgust, Anger, Intimidate, Sad
};*/

    public GameObject timeManager;

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

