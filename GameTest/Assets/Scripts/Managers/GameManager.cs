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
    public GameObject thePlayer;
    public GameObject timeManager;
    public Know TheKnow = new Know();

    public static GameManager instance = null;
    // Use this for initialization
    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }

        PersonCreator player = new PersonCreator("Player", "player", 0.5f, 1, AINum.player,
            new List<string> { "Stop talking to yourself." });
        Body playerBody = (Body)FindObjectOfType(typeof(Body));
        //playerBody.AddAffecter(new Fire(playerBody, 40f));
        //playerBody.AddAffecter(new Fire(playerBody, 20f));
        PersonCreator oldMan = new PersonCreator("Old_Man", "old man", 85.5f, 1, AINum.soldier,
          new List<string> { "Why are you here ?",
        "Can I help you ?",
        "Gawd, why a forest ? This place constantly makes weird noises, and then there’s the oinkers…",
        "Look pal, can you stop sneaking up on me like that ? You realize this thing’s loaded, right ?",
        "No sudden movements please",
        "You’re… a little persistent aren’t you",
        "Bud, you’re creeping me out here",
        "Please stay away",
        "You know, I feel we could really look out for each other here",
        "H..Hello ?" }
            ) ;

            PersonCreator mapMaker = new PersonCreator("Mapmaker", "map maker", 14.5f, 1, AINum.soldier,
            new List<string> { "...Oh! Hello! I didn’t notice you there! Would you like to buy one of my maps?",
    "What do you mean ‘map of where’? A map of here and everywhere!Middleberg’s in the center, naturally… and I’m in the center of that!" ,
    "What do you mean I’m not in the middle? The map says so.",
    "Where are you? You’re in Middleberg, halfway between Easton and Weston…  " +
    "This quiet little town is pretty nice...but it doesn’t compare to the splendor of Easton!" +
    "I would visit it if you can.Though I doubt the soldier will let you without a weapon. " +
    "It’s a dangerous time, nowadays!Though it’s always been dangerous… so I guess it’s just a normal time, nowadays." ,
    "What to do now? Well--whatever you want. But not really.We do have laws. " +
    "Personally I was going to go check out if that old man forgot his ring was still in the well, " +
    "but I still have maps to sell. No one is buying them for some reason…",
    "Welcome to Middleberg!Located halfway between Weston and Easton.Buy a map."
            });
        PersonCreator soldier = new PersonCreator("Soldier", "soldier", -6.5f, 1, AINum.soldier, 
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
        GameObject soldierObject = soldier.CreatedPerson;
        //Sword newSword = new Sword(soldierObject.GetComponent<Body>(), 1);
        //soldierObject.GetComponent<Body>().Inventory.Add(newSword);

        List<Item> items = new List<Item> { };

        GameObject packObj = Instantiate(Resources.Load("ItemPackage"), new Vector3(5.5f, -2.75f), Quaternion.identity) as GameObject;

        ItemPackage pack = packObj.transform.GetChild(0).GetComponent<ItemPackage>();
        pack.CreateItemPackage(items, "Chest", -2.75f, 5);
        for(int i = 0; i < 5; i++) {
            pack.AddItem(new Sword(pack, 1));
        }
        Armor mail = new Armor(pack, 1, "mail", 10f);
        pack.AddItem(mail);

        GameObject ForestpackObj = Instantiate(Resources.Load("ItemPackage"), new Vector3(70.5f, -2.75f), Quaternion.identity) as GameObject;
        ItemPackage Forestpack = ForestpackObj.transform.GetChild(0).GetComponent<ItemPackage>();
        pack.CreateItemPackage(items, "Chest", -2.75f, 5);
        Forestpack.AddItem(new Sword(Forestpack, 1));
        Armor mail2 = new Armor(Forestpack, 1, "mail", 10f);
        Forestpack.AddItem(mail2);

        //Sword sword2 = new Sword(innkeeperBody, 1);

        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        thePlayer = GameObject.Find("Player");
    }

	// Update is called once per frame
	void Update () {
		
	}

    public bool ShouldSleep(Body bod) {
        float playerpos = thePlayer.transform.position.x;
        float bodpos = bod.transform.position.x;
        return Mathf.Abs(bodpos - playerpos) > 100f;
    }
}
