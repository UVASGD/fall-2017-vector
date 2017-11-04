using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Know {
    public List<Identity> AllId = new List<Identity>();
    public List<Identity> MiddleburgId = new List<Identity>();

    public string AddId(Identity newId, List<Identity> idList, int num = 1, bool forceAdd = false) {
        int check = ContainsId(newId, idList);
        bool addBool = false;
        if (check == 0)
            addBool = true;
        else if (check == 1) {
            if (forceAdd) {
                newId.ChangeId(newId.Id + "-" + num);
                AddId(newId, idList, num++, true);
            }
            addBool = false;
        }
        else if (check == 2)
            return newId.Id;

        if (addBool) {
            idList.Add(newId);
            AddToAll(newId, num, forceAdd);
        }

        return newId.Id;
    }

    public string AddToAll(Identity newId, int num = 1, bool forceAdd = false) {
        int check = ContainsId(newId, AllId);
        if (check == 0)
            AllId.Add(newId);
        else if (check == 1) {
            if (forceAdd) {
                newId.ChangeId(newId.Id + "-" + num);
                AddId(newId, AllId, num++, true);
            }
            return newId.Id;
        }
        return newId.Id;
    }

    public short ContainsId(Identity newId, List<Identity> idList) {
        if (idList.Contains(newId))
            return 2;
        foreach (Identity id in idList)
            if (id.Id.Equals(newId.Id))
                return 1;
        return 0;
    }
}
