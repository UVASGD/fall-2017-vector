using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Left = -1, None = 0, Right = 1 };
//public enum Reaction { Dampening, Oiling, Watering, Dirtying, Drying, Burning, Fueling, Hindering, Freeing, Harming, Healing, Crushing, Slashing, Piercing};

public enum AINum { player, soldier, dummy, turret};

/*public enum Mood {
    Charm, Amuse, Inspire, Happy,
    Disgust, Anger, Intimidate, Sad,
    None
};*/

public enum CoreMood { CharmAxis, AmuseAxis, InspireAxis, HappyAxis, IntrigueAxis, PityAxis};

public class GameManager : MonoBehaviour {
    /*enum Mood
{
Charm, Amuse, Inspire, Happy,
Disgust, Anger, Intimidate, Sad
};*/

    public GameObject timeManager;
    public Know TheKnow = new Know();

    public static GameManager instance = null;
	// Use this for initialization
	void Awake () {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }

        PersonCreator player = new PersonCreator("MainCharacter", "Player", "Player", "player", 0.5f, 1, AINum.player, 
            new List<string> { "Stop talking to yourself." });
        Body playerBody = (Body)FindObjectOfType(typeof(Body));
        //playerBody.AddAffecter(new Fire(playerBody, 40f));
        //playerBody.AddAffecter(new Fire(playerBody, 20f));
        PersonCreator innkeeper = new PersonCreator("RSoldier", "Soldier", "Soldier", "soldier", -6.5f, 1, AINum.soldier, 
            new List<string> { "You want me to open the gate? No.",
                "This is Middleburg. Congrats on getting here.",
                "Why do your feet look like that?",
                "Know any jokes? No? Piss off then.",
                "Got any jokes? Oh right.No.",
                "Go away.",
                "Stop talking to me.",
                "Do the houses look like faces to you? I think I’ve been here too long.Go away.",
                "If you pee in that well, I’ll feed you to the bears.",
                "Why do you smell like a bear?",
                "Quit talking to me.",
                "Don’t press K again.",
                "Let me do my job. You wouldn’t know much about that, would you?",
                "Look at that thing over there. I wonder what it is.Too bad I can’t move.",
                "You fell from the sky? Well, in that case, I’m the authority here. Go away.",
                "Don’t bother me.",
                "Wonder what that thing over there is.Go touch it.",
                "Quit it.",
                "You’re bleeding? You should try not doing that.",
                "Attacked by a bear? This is a city. Don’t waste my time.",
                "Yeah sure. A bear. Go away.",
                "Stop.",
                "You got attacked by a bear? Have you tried not doing that? Then go away.",
                "Press escape.",
                "Don’t you have something to do? Things to pick up? Bears to get attacked by?",
                "What do you need--actually, scratch that. I don’t care.",
                "I wish you could do more in this game.",
                "AHHH, A BEAR!" });

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

