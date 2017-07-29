using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable {
    bool CheckInteract(Body player);
    void OnMouseOver();
    void OnMouseExit();
    void OnMouseDown();
}
