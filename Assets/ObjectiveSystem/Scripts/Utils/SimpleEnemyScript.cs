using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyScript : MonoBehaviour {


    public float health = 100f;
    public float damagePerHit = 50f;

    private string myTag;
    private bool isKillable = false;

    // Start is called before the first frame update
    void Start() {
        myTag = gameObject.tag;
    }

    // Update is called once per frame
    void Update() {
        if (isKillable) {
            if (Input.GetKeyDown(KeyCode.F)) {
                health -= 50f;
                if (health <= 0) {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            isKillable = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        isKillable = false;
    }
}
