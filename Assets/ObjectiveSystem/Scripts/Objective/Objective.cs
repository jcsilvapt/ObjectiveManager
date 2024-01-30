using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Objective : MonoBehaviour {

    [HideInInspector]
    [Header("Objective Description")]
    public int id;                          // Set the ID of the Quest

    [HideInInspector]
    public string internalTitle;            // Internal Quest Naming
    public string title;                    // External Quest Naming
    public string description;              // Description of the Quest
    public string descriptionInProgress;    // Description of the Quest in Progress
    public string descriptionOnEnd;         // Description of the Quest On Ending

    [Header("Objective")]
    public ObjectiveGoal objGoal;           // Set the Quest Goal
    public GameObject objective;            // Used when the objective is type: Reach
    public GameObject[] objectives;         // Used when the objective is type: Action
    public string gameobjectTag;            // Used when the objective is type: Kill
    public float timeToInteract = 0;        // Used when you have to delay the interaction

    // TODO: Set as private after testing
    public KeyCode key;
    public bool isActive = false;
    public bool isCompleted = false;
    private bool updateStatus = false;

    // Settings
    private Collider col;
    private ActionCheck objectiveTrigger;
    private List<ActionCheck> objectivesTrigger = new List<ActionCheck>();
    private float interactionHoldTimer = 0;
    private string lastEnemyTag = "";
    public bool setAutomaticColliders = false;

    private void Start() {
        // Objective Type: Reach
        if (objGoal.objType == ObjectiveType.Reach) {
            SetGameObject();
        }

        // Objective Type: Action
        if (objGoal.objType == ObjectiveType.Action) {
            SetGameObjects();
        }

        // Objective Type: Kill
        if (objGoal.objType == ObjectiveType.Kill) {
            GetAllEnemies(gameobjectTag);
        }
    }

    // Update
    private void Update() {
        if (isActive) {
            switch (objGoal.objType) {
                case ObjectiveType.Reach:
                    if (objectiveTrigger.IsTrigger())
                        UpdateStatus();
                    break;
                case ObjectiveType.Action:
                    objectivesTrigger.ForEach(delegate (ActionCheck b) {
                        if (b.IsTrigger()) {
                            if (InteractionHold(Input.GetKey(key))) {
                                UpdateStatus();
                                b.DisableTrigger();
                            }
                        }
                    });
                    break;
                case ObjectiveType.Kill:
                    /*if (!updateStatus) {
                        objectives = null;
                        GetAllEnemies(gameobjectTag);
                        updateStatus = !updateStatus;
                    }*/
                    break;
                default:
                    break;
            }
        }
    }

    // Public Calls

    public Objective EnableObjective() {
        if (!isCompleted && !isActive) {
            Debug.Log("Objective Started! (" + this.name + ")");
            isActive = true;
            return this;
        }
        return null;
    }

    public void UpdateStatus() {
        objGoal.currentAmount++;
        if (CheckOBjectiveStatus()) {
            Debug.Log("Objective Completed! (" + this.name + ")");
            isActive = false;
            isCompleted = true;
        }
    }

    public bool CheckOBjectiveStatus() {
        if (objGoal.IsCompleted()) {
            return true;
        } else {
            return false;
        }
    }

    public void EnemyKilled(string tag) {
        if (tag == gameobjectTag) {
            UpdateStatus();
        }
    }


    // Settings

    private void SetGameObject() {
        if (objective.GetComponent<Collider>() == null) {
            throw new System.Exception(ObjectiveManagerConstants.m_col + objective.name);
        } else {
            col = objective.GetComponent<Collider>();
            if (!col.isTrigger) {
                col.isTrigger = true;
            }
        }

        if (objective.GetComponent<ActionCheck>() == null) {
            objectiveTrigger = objective.AddComponent<ActionCheck>();
        } else {
            objectiveTrigger = objective.GetComponent<ActionCheck>();
        }
    }

    private void SetGameObjects() {
        foreach (GameObject b in objectives) {
            if (b.GetComponent<Collider>() == null) {
                throw new System.Exception(ObjectiveManagerConstants.m_col + b.name);
            }

            if (setAutomaticColliders) {
                if (b.GetComponents<Collider>().Length == 1) {
                    BoxCollider temp = b.AddComponent<BoxCollider>();
                    temp.size = new Vector3(2, 1, 2);
                    temp.isTrigger = true;
                }
            }
            if (b.GetComponent<ActionCheck>() == null) {
                objectivesTrigger.Add(b.AddComponent<ActionCheck>());
            } else {
                objectivesTrigger.Add(b.GetComponent<ActionCheck>());
            }
        }
        objGoal.requiredAmount = objectives.Length;
    }

    private bool InteractionHold(bool value) {
        if (value) {
            interactionHoldTimer += Time.deltaTime;
            if (interactionHoldTimer > timeToInteract) {
                interactionHoldTimer = 0.0f;
                return true;
            }
        } else {
            if (interactionHoldTimer > 0) {
                interactionHoldTimer -= Time.deltaTime;
            } else {
                interactionHoldTimer = 0.0f;
            }
        }
        Debug.Log("Hold Time: " + interactionHoldTimer);
        return false;
    }

    private void GetAllEnemies(string tag) {
        if (tag != null || tag != "") {
            objectives = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject item in objectives) {
                item.AddComponent<ActionCheck>();
            }
            objGoal.requiredAmount = objectives.Length;
        } else {
            throw new System.Exception(ObjectiveManagerConstants.m_tag + tag);
        }
    }
}
