using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCheck : MonoBehaviour {

    public string customTag = "";
    public bool isTrigger;
    public bool forceDisable = false;

    private bool isDestroyed = false;

    private void OnTriggerEnter(Collider other) {
        if (!forceDisable) {
            if (customTag.Length == 0) {
                if (other.tag == "Player")
                    isTrigger = true;
            } else {
                if (other.tag == customTag)
                    isTrigger = true;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        isTrigger = false;
    }

    public bool IsTrigger() {
        return isTrigger;
    }

    public void DisableTrigger() {
        isTrigger = false;
        forceDisable = true;
    }

    public void SetCustomTag(string customTag) {
        this.customTag = customTag;
    }

    private void OnDestroy() {
        if (!isDestroyed) {
            ObjectiveManager om = FindObjectOfType<ObjectiveManager>();
            Debug.Log("Got Killed");
            om.ConfirmKill(this.transform.tag);
            isDestroyed = true;
        }
    }
}
