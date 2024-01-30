using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ObjectiveManager : MonoBehaviour {

    public static ObjectiveManager objManager;

    [Header("Current Objective List")]
    public List<Objective> objList = new List<Objective>();
    [Header("Active Objective")]
    public Objective activeObjective;

    private bool hasObjectives = false;
    private int currentOjectiveID = 0;

    private bool controlInformation = false;

    /// <summary>
    /// Runs before the start to create a singleton
    /// </summary>
    private void Awake() {
        if (objManager == null) {
            objManager = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// On Game Start puts the first objective Active
    /// </summary>
    private void Start() {
        CheckManager();
        if (objList.Count > 0) {
            hasObjectives = true;
            StartFirstObjective();
        }
    }

    private void Update() {
        if (hasObjectives) {
            UpdateActiveObjectives();
        } else {
            if (!controlInformation) {
                Debug.Log("All Objectives have been completed!");
                controlInformation = !controlInformation;
            }
        }
    }

    /// <summary>
    /// Updates the active Objective
    /// </summary>
    private void UpdateActiveObjectives() {
        if (activeObjective.CheckOBjectiveStatus()) {
            if (hasObjective()) {
                currentOjectiveID++;
                activeObjective = objList[currentOjectiveID].EnableObjective();
            } else {
                hasObjectives = false;
            }
        }
    }

    /// <summary>
    /// Function that Starts the first Objective
    /// </summary>
    private void StartFirstObjective() {
        activeObjective = objList[currentOjectiveID].EnableObjective();
    }

    /// <summary>
    /// Function that controls if there is objectives
    /// </summary>
    /// <returns></returns>
    private bool hasObjective() {
        if ((currentOjectiveID) < objList.Count - 1) {
            return true;
        }
        return false;
    }


    public void ConfirmKill(string objectTag) {
        if (activeObjective.gameobjectTag == objectTag) {
            activeObjective.EnemyKilled(objectTag);
        }
    }



    // ----------------------------- Editor Window Settings ------------------------ \\

    public void AddNewObjective(Objective obj) {
        objList.Add(obj);
    }

    public int UpdateList(GameObject parent) {
        Objective[] temp = parent.GetComponentsInChildren<Objective>();
        if (temp.Length > 0) {
            foreach (Objective q in temp) {
                bool exists = false;
                foreach (Objective b in objList) {
                    if (q.id == b.id) {
                        exists = true;
                        break;
                    }
                }
                if (!exists) {
                    objList.Add(q);
                }
            }
        } else {
            objList = new List<Objective>();
            return 0;
        }
        return temp.Length;
    }

    public bool CheckIfObjectiveExists(Objective obj) {
        foreach (Objective b in objList) {
            if (obj.internalTitle == b.internalTitle) {
                return true;
            }
        }
        return false;
    }

    public List<Objective> GetObjectiveList() {
        return objList;
    }

    public Objective GetObjective(int id) {
        foreach (Objective b in objList) {
            if (b.id == id) {
                return b;
            }
        }
        return null;
    }

    public void UpdateObjective(Objective obj) {
        UpdateInternalObjective(GetObjective(obj.id), obj);
        UpdateGameObjectName(obj);

    }

    private void UpdateGameObjectName(Objective obj) {
        obj.GetComponent<Transform>().name = "[" + obj.id.ToString() + "]quest(" + Creator.SetInternalName(obj.internalTitle) + ")";
    }

    private void UpdateInternalObjective(Objective _internal, Objective _external) {
        _internal = _external;
    }

    /// <summary>
    /// Function that Updates The Current objective List
    /// </summary>
    public void CheckManager() {
        objList = objList.Where(x => x != null).ToList();
    }
}
