using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watchzone : MonoBehaviour {

    List<Body> others;

    private void OnTriggerEnter2D(Collider2D other) {
        others.Add(GetColliderBody(other));
        ShowContent();
    }

    private void OnTriggerExit2D(Collider2D other) {
        others.Remove(GetColliderBody(other));
        ShowContent();
    }

    private Body GetColliderBody(Collider2D other) {
        return other.gameObject.GetComponent<Body>();
    }

    private void ShowContent() {
        Debug.Log(others);
    }
}
