using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum DialogueStage {Greeting, Enquiring, Revealing, Discussing, Nil}

public class Body : MonoBehaviour {

    protected int brawn = 5;
    public int Brawn { get { return brawn; } }

    protected int coordination = 5;
    public int Coordination { get { return coordination; } }

    public int Athletics { get { return (brawn + coordination) / 2; } }

    protected ImpedimentLevel impediment = ImpedimentLevel.unimpeded;
    public ImpedimentLevel Impediment { get { return impediment; } set { impediment = value; } }

    protected TimeManager time; //Reference to time manager
    protected GameManager gameManager;

    protected Transform bodyRenderTransform; //Reference to the body-renderer's transform

    protected Place currPlace; //What place gameobject you're currently colliding with
    public Place CurrPlace { get { return currPlace; } set { currPlace = value; } } //Yeah

    int bodyCollisions = 0;

    public float harmQuant; //Harm Variables
    protected float harmThreshold;

    protected float hinderQuant; //Hinder Variables
    protected float hinderThreshold;

    protected int size; //Self-Explanatory *MUST BE SET
    public int Size { get { return size; } }
    public Direction face; //Self-Explanatory *MUST BE SET

    protected Quest currQuest; //Current quest
    public Quest CurrQuest { get { return currQuest; } }
    protected Quest currSubQuest; //Current component quest of the main quest
    public Quest CurrSubQuest { get { return currSubQuest; } }
    protected int subQuestNum = 0;
    public int SubQuestNum { get { return subQuestNum; } set { subQuestNum = value; } }
    protected Action currMoveAct; //Current movement action
    public Action CurrMoveAct { get { return currMoveAct; } }
    protected Action currAct; //Current non-movement action
    public Action CurrAct { get { return currAct; } }

    protected List<string> targetTags = new List<string>(); //List of tag-filters that this NPC can strike*MUST BE SET
    public List<string> TargetTags { get { return targetTags;  } }

    protected AI mind; //The AI object that will generate actions *MUST BE SET
    public AI Mind { get { return mind; } }
    public bool Dashing { get { return mind.Dashing; } }

    protected List<Affecter> affecterList = new List<Affecter>(); //List of Affecters
    protected List<Affecter> traitList = new List<Affecter>(); //List of Traits 

    protected List<Affecter> spreadList = new List<Affecter>();
    protected List<Affecter> layerList = new List<Affecter>();

    protected List<Item> inventory = new List<Item>();
    public List<Item> Inventory { get { return inventory; } set { inventory = value; } }

    Item weapon;
    public Item Weapon { get { return weapon; } set { weapon = value; } }

    Armor armor;
    public Armor Armor { get { return armor; }
        set {
            armor.Deact();
            armor = value;
            armor.Enact();
        }
    }

    string id;
    public string Id { get { return id; }}

    Personality interactee;
    GameObject TalkBox;
    Text DialogueBox;
    TextInput TalkInput;
    bool talking = false;
    DialogueStage diaStage = DialogueStage.Nil;
    public DialogueStage DiaStage { get { return diaStage; } set { diaStage = value; } }

    public Vector3 Position { get { return transform.position; } }

    private InventoryInteraction inventoryUI;
    private NearbyInteraction nearbyUI;

    protected List<Direction> forbiddenDirects = new List<Direction>();
    protected bool CanMoveRight { get { return forbiddenDirects.Contains(Direction.Right); } }
    protected bool CanMoveLeft { get { return forbiddenDirects.Contains(Direction.Left); } }

    public void AddForbiddenDirect(Direction _direct) {
        forbiddenDirects.Add(_direct);
    }

    public void RemoveForbiddenDirect(Direction _direct) {
        forbiddenDirects.Remove(_direct);
    }

    void Start() {
        time = (TimeManager)FindObjectOfType(typeof(TimeManager)); //Set Time manager
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        foreach (Transform child in transform) if (child.CompareTag("Renderer")) { bodyRenderTransform = child; } 
        //Set bodyRender equal to the transform of the proper childObject

        harmQuant = 0f;
        harmThreshold = 1f;

        hinderQuant = 0f;
        hinderThreshold = 1f;

        currMoveAct = new HaltAction("Halt", 100, this);
        currAct = new Action("Open", 0, this);
        AddAffecter(new ResistanceAggregate(this, 0f));

        armor = new Armor(this, 1, "none");
    }

    void Update() {
        mind.Update();
        if (time.clock) {
            Tick();
        }
    }

    public void BodyConstructor(string _id, int _size, Direction _dir, List<string> _targetTags, AI _mind) {
        id = _id;
        size = _size;
        face = _dir;
        targetTags = _targetTags;
        mind = _mind;
        mind.Start();
        if (mind.GetType() == typeof(PlayerAI)) {
            TalkBox = GameObject.Find("Talk");
            TalkInput = GameObject.Find("OptText").GetComponent<TextInput>();
            TalkInput.SetBodyRef(this);
            inventoryUI = GameObject.Find("Inventory").transform.GetChild(1).GetComponent<InventoryInteraction>();
            nearbyUI = GameObject.Find("Inventory").transform.GetChild(3).GetComponent<NearbyInteraction>();
        }
    }

    //TICK FUNCTION
    protected void Tick() {
        for (int i = 0; i < affecterList.Count; i++) {
            Affecter affecter = affecterList.ElementAt(i);
            affecter.Tick();
        }
        mind.Tick();
        currMoveAct.Tick();
        currAct.Tick();
        if (harmQuant > harmThreshold) {
            DieIdiot();
        }
    }

    public void DieIdiot() {
        DumpItems();
        if (mind.GetType() == typeof(PlayerAI))
        {
            //transform.position = new Vector3(currPlace.Coordinate - currPlace.Size / 2, -3.5f, 0); //TODO FIX CurrPlace stuff
            transform.position = new Vector2(0.5f, -3.5f);
            Camera.main.transform.position = new Vector3(0.5f, 0,-10);
            //Camera.main.transform.position = new Vector3(currPlace.Coordinate - currPlace.Size / 2,0,-10);
            harmQuant = 0;
        }
        else
            Destroy(gameObject);
    }

    void DumpItems()
    {
        GameObject packageRes = Resources.Load<GameObject>("ItemPackage");
        GameObject package = Object.Instantiate(packageRes, new Vector3(transform.position.x, -2.75f, 0), Quaternion.identity) as GameObject;
        ItemPackage pack = package.transform.GetChild(0).GetComponent<ItemPackage>();
        pack.CreateItemPackage(new List<Item>(), name + @"'s Corpse", transform.position.x);
        package.name = name + @"'s Corpse";
        foreach(Item i in Inventory)
        {
            pack.AddItem(i);
            i.SwitchHolder(pack);
        }
        if(Weapon.GetType() != typeof(Fists))
        {
            pack.AddItem(Weapon);
            weapon = new Fists(this, 1);
        }
        if (Armor.Name != "none")
        {
            pack.AddItem(Armor);
            armor = new Armor(this, 1, "none");
        }

        inventory.Clear();
        if (mind.GetType() == typeof(PlayerAI))
        {
            GameObject weapon = GameObject.Find("WeaponEquip");
            GameObject armor = GameObject.Find("ArmorEquip");
            GameObject.Find("PlayerAll").GetComponent<InventoryInteraction>().UpdateUI();
            if(weapon.transform.childCount > 0)
                Destroy(weapon.transform.GetChild(0).gameObject);
            if (armor.transform.childCount > 0)
                Destroy(armor.transform.GetChild(0).gameObject);
        }
    }

    //ABILITY TO GET HURT AND TO BE TARGETED
    public void ChangeHarm(float delt) {
        harmQuant += delt;
    }

    public void Hinder(float delt) {
        hinderQuant += delt;
    }

    public void NPCTalk(Personality _interactee) {
        //When two NPCs talk, just have them share information and randomly feel emotions
    }

    public void BeginTalk(Personality _interactee) {
        if (mind.GetType() != typeof(PlayerAI))
            return;
        EntityInventory entInv = GameObject.Find("EntityInv").GetComponent<EntityInventory>();
        entInv.currentInventory = _interactee.GetBody;
        entInv.UpdateUI();
        TalkInput.SetInteractable();
        interactee = _interactee;
        Text Title = TalkBox.transform.GetChild(0).GetComponent<Text>();
        Title.text = interactee.GetBody.name;
        DialogueBox = TalkBox.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();
        DialogueBox.text = interactee.GetBody.name + " is feeling " + interactee.moodHandler.GetDominantMood() + ". \n\n";
        DialogueBox.text += interactee.OpeningText + "\n\n";
        DialogueBox.text += "[e]nquire, [r]eveal, [s]mall talk, [k] to leave ";
        interactee.ShuffleOpening();
        talking = true;
        diaStage = DialogueStage.Greeting;
    }

    public void Talk() {
        DialogueBox.text = interactee.GetBody.name + " is feeling " + interactee.moodHandler.GetDominantMood() + ". \n\n";
        DialogueBox.text += interactee.OpeningText + "\n\n";
        DialogueBox.text += "[e]nquire, [r]eveal, [s]mall talk, [k] to leave ";
        interactee.ShuffleOpening();
        diaStage = DialogueStage.Greeting;
    }

    public List<string[]> Enquire() {
        List<string[]> s = new List<string[]>() { };
        if (interactee.seenEvents.Count == 0) {
            DialogueBox.text = interactee.GetBody.name + " has nothing to discuss with you.\n\n";
            DialogueBox.text += "[e] to continue...";
            diaStage = DialogueStage.Discussing;
            return s;
        }
        diaStage = DialogueStage.Enquiring;
        foreach (string[] sentence in interactee.seenEvents.Keys) {
            if (interactee.seenEvents[sentence].Interest > 0.60f) {
                s.Add(sentence);
            }
        }
        return s;
    }

    public List<string[]> Reveal() {
        List<string[]> s = new List<string[]>() { };
        if (interactee.unseenEvents.Count == 0) {
            DialogueBox.text = "You have nothing interesting to share.\n\n";
            DialogueBox.text += "[e] to continue...";
            diaStage = DialogueStage.Discussing;
            return s;
        }
        diaStage = DialogueStage.Revealing;
        foreach (string[] sentence in interactee.unseenEvents.Keys) {
            //if (interactee.useenEvents[sentence].Strength > 60) {
            s.Add(sentence);
            //}
        }
        return s;
    }

    public void Discuss(string[] sentence, bool seen) {
        string response = interactee.DiscussPerceive(sentence, seen);
        if (response.Equals(""))
            DialogueBox.text = "There is nothing to say about this.";
        else
            DialogueBox.text = interactee.GetBody.name + " says this makes them feel " + response + ".";
        DialogueBox.text += "\n\n[e] to continue...";
        diaStage = DialogueStage.Discussing;
    }

    public void EndTalk() {
        interactee = null;
        EntityInventory entInv = GameObject.Find("EntityInv").GetComponent<EntityInventory>();
        entInv.currentInventory = null;
        entInv.UpdateUI();
        Text Title = TalkBox.transform.GetChild(0).GetComponent<Text>();
        Title.text = "MIDDLEBURG";
        DialogueBox.text = "You're in Middleburg.";
        TalkInput.Deactivate();
        talking = false;
        diaStage = DialogueStage.Nil;
    }

    //ABILITY TO MOVE
    public void Move(Direction _dir) {
        if (forbiddenDirects.Contains(_dir)) {
            if (Dashing) {
                Wound thump = new Wound(this, 0.01f);
                AddAffecter(thump);
            }
            return;
        }

        if (mind.GetType() == typeof(PlayerAI))
            Camera.main.transform.Translate((int)_dir, 0, 0);
        else transform.localScale = new Vector2((int)face, 1);
        
        gameObject.transform.Translate((int)_dir, 0, 0);
    }

    public void SetCurrMoveAct(Action _currMoveAct) {
        if (currMoveAct.Overridable)
            currMoveAct = _currMoveAct;
    }

    //GET/SET DIRECTION
    public void SetFace(Direction _dir) {
        face = _dir;
    }
    public Direction GetFace() {
        return face;
    }

    //COLLISION
    private void OnTriggerEnter2D(Collider2D other) {
        Body otherBody = other.gameObject.GetComponent<Body>();
        if (otherBody != null) {
            Spread(otherBody);
            bodyCollisions++;
            if(otherBody.GetType() == typeof(ItemPackage) && mind.GetType() == typeof(PlayerAI))
            {
                nearbyUI.currentInventory = otherBody as ItemPackage;
                nearbyUI.UpdateUI();
            }
        }
    }

    void Spread(Body otherBody) {
        foreach (Affecter otherAffecter in otherBody.GetSpreadList()) {
            bool skip = false;
            if (otherAffecter.IsLayered())
                if (otherBody.GetLayerVal(otherAffecter) > 2)
                    continue;
            foreach (Affecter ownAffecter in spreadList) {
                if (ownAffecter.IsLayered())
                    if (GetLayerVal(ownAffecter) > 2) {
                        skip = true;
                        break;
                    }
                if (ownAffecter.GetType() == otherAffecter.GetType()) {
                    skip = true;
                    break;
                }
            }
            if (skip)
                continue;
            AddAffecter(otherAffecter.GetAffecterClone(otherAffecter, true));
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other == null) return;
        Body otherBody = other.gameObject.GetComponent<Body>();
        if (otherBody != null) {
            bodyCollisions--;
            bodyRenderTransform.position = gameObject.transform.position;
            if (otherBody.GetType() == typeof(ItemPackage) && mind.GetType() == typeof(PlayerAI))
            {
                nearbyUI.currentInventory = null;
                nearbyUI.UpdateUI();
            }
        }
    }

    public void TakeItemPackage(ItemPackage package) {
        Debug.Log("Imma take dat");
        Debug.Log("Player inventory is: {");
        foreach (Item item in package.Inventory) {
            inventory.Add(item);
            Debug.Log(string.Format("{0},", item.Name));
        }
        while(package.inventory.Count > 0)
        {
            package.RemoveItem(package.inventory[0]);
        }
        Debug.Log("}");
        foreach (Item i in inventory)
        {
            Debug.Log("Item: " + i.Name);
        }
        if (Mind.GetType() == typeof(PlayerAI))
        {
            inventoryUI.UpdateUI();
        }
        
    }

    public void addItem(Item i)
    {
        inventory.Add(i);
        if(Mind.GetType() == typeof(PlayerAI))
        {
        //TODO: ???         
        }
    }

    //SET NEXT QUEST
    public void SetNextQuest(Quest nextQuest) {
        currQuest = nextQuest;
    }

    public bool SetNextSubQuest() {
        subQuestNum++;
        if (subQuestNum < currQuest.SubQuests.Count) {
            currSubQuest = currQuest.SubQuests[subQuestNum];
            return true;
        }
        subQuestNum = 0;
        return false;
    }


    //SET ACTION
    public void SetCurrAct(Action _currAct) {
        if (currAct.Overridable)
            currAct = _currAct;
    }

    //SET/GET MIND
    public void SetMind(AI _mind) {
        mind = _mind;
    }

    //GET PERSONALITY
    public Personality GetPersonality() {
        return mind.GetPersonality;
    }

    //ADD/REMOVE TO/FROM APPROPRIATE AFFECTER LIST
    public void AddAffecter(Affecter _affecter) {
        if (_affecter.Enact(this))
            affecterList.Add(_affecter);
    }

    public void RemoveFromAffecterList(Affecter _affecter) {
        affecterList.Remove(_affecter);
    }

    public void RemoveFromTraitList(Affecter _affecter) {
        traitList.Remove(_affecter);
    }

    //GET/SET EFFECTLIST AND TRAITLIST
    public List<Affecter> GetAffecterList() {
        return affecterList;
    }

    public void AddToAffecterList(Affecter _affecter) {
        affecterList.Add(_affecter);
    }

    public List<Affecter> GetTraitList() {
        return traitList;
    }

    public void AddToTraitList(Affecter _affecter) {
        traitList.Add(_affecter);
    }

    //GET/SET SPREADLIST
    public void AddToSpreadList(Affecter _affecter) {
        spreadList.Add(_affecter);
    }

    public List<Affecter> GetSpreadList() {
        return spreadList;
    }

    public void RemoveFromSpreadList(Affecter _affecter) {
        spreadList.Remove(_affecter);
    }

    //GET/SET LAYERLIST
    public void AddToLayerList(Affecter _affecter, int ind = 0) {
        layerList.Insert(ind, _affecter);
    }

    public void RemoveFromLayerList(Affecter _affecter) {
        layerList.Remove(_affecter);
    }

    public List<Affecter> GetLayerList() {
        return layerList;
    }

    public int GetLayerVal(Affecter _affecter) {
        return layerList.IndexOf(_affecter);
    }


    public int GetMoveSpeed() {
        return 15 - coordination;
    }

    public int GetDashSpeed() {
        //return 11 - coordination;
        return 2;
    }
}
