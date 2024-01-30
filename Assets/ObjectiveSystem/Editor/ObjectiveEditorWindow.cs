using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;

public class ObjectiveEditorWindow : EditorWindow {

    public object source;
    //Objects
    public object _objective;

    //GameObjects
    private GameObject _objectiveManager;
    public GameObject[] targets;

    // objectiveManager
    ObjectiveManager objManager;

    // Fields Options  
    public int id;                          // Set the ID of the objective
    public string internalTitle = "";       // Internal objective Naming
    public string objTitle;                 // External objective Naming
    public string description;              // Description of the objective
    public string descriptionInProgress;    // Description of the objective in Progress

    public ObjectiveType objectiveType;     // Set the Type of objective
    public int amount = 1;                  // Set the amount to finish the objective
    public float timeToInteract = 3.0f;     // Set the amount of time needed to interact
    public KeyCode interactiveKey;          // Set the interaction key
    public bool setAutomaticColliders = false;

    //Editor Configurations
    public Vector2 scrollPosition = Vector2.zero;
    public Vector2 mainScroolPosition = Vector2.zero;
    public bool view = false;
    private bool enableReview = false;
    private bool isToString = false;

    private ObjectiveGoal objectiveGoal = new ObjectiveGoal();

    private Objective temporaryDisplay;
    private string _GameTag = "";


    // NEW STUFF

    private GameObject _main;

    private int selectedTab = 0;


    [MenuItem("Objective System/Objective Manager", false, 1)]
    public static void ShowWindow() {
        var window = EditorWindow.GetWindow(typeof(ObjectiveEditorWindow), true, "Objective Manager");
        window.position = new Rect(Screen.width / 2 + 0.5f, Screen.height / 2 + 0.5f, 1024, 500);
    }

    private void Awake() {
        // Update or Create objective List Object
        SetObjectiveMainObj();
        // Update or Create objective Manager
        SetObjectiveManager();
        // Update objective List
        UpdateObjectiveList();
        // TO BE DECIDED
        //SetPlayerobjectiveScript();
    }


    #region GUI & GUI BUTTONS

    // ---------------------------------------- GUI ---------------------------------------- \\

    void OnGUI() {

        //DisplayToolbar();

        GUILayout.Box("Objective Manager", GUILayout.Width(1024));
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical(GUILayout.Width(10));
        UIButtons();
        UI_DisplayObjectiveList();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        mainScroolPosition = EditorGUILayout.BeginScrollView(mainScroolPosition, false, false, GUILayout.Height(0));
        if (enableReview) {
            UI_UpdateObjective(temporaryDisplay);
        } else {
            UI_CreateObjective();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }


    // ---------------------------------------- GUI_Buttons ---------------------------------------- \\

    private void UIButtons() {
        GUILayout.Box("Options", GUILayout.Width(200));
        if (GUILayout.Button("Create New Objective", GUILayout.Width(200))) {
            enableReview = false;
            ClearInfo();
        }
        GUI.enabled = false;
        if (GUILayout.Button("Save", GUILayout.Width(200))) {
            //TODO
        }
        if (GUILayout.Button("Load", GUILayout.Width(200))) {
            //TODO
        }
        GUI.enabled = true;
    }

    // ---------------------------------------- GUI_Buttons (END) ---------------------------------------- \\

    /// <summary>
    /// Function that Creates an Objective
    /// </summary>
    private void UI_CreateObjective() {
        EditorGUILayout.HelpBox("System Mode Set to Linear, use this method for linear story, one quest at a time.", MessageType.Info);
        GUILayout.Label("New Objective Information", EditorStyles.boldLabel);

        // Show up if the Quest System is set to Linear
        EditorGUI.BeginDisabledGroup(true);
        id = EditorGUILayout.IntField("ID (auto)", id);
        EditorGUI.EndDisabledGroup();
        internalTitle = EditorGUILayout.TextField("Internal Title", internalTitle);
        objTitle = EditorGUILayout.TextField("Title", objTitle);
        description = EditorGUILayout.TextField("Description", description);
        descriptionInProgress = EditorGUILayout.TextField("Description Progress", descriptionInProgress);
        EditorGUILayout.Space();
        GUILayout.Label("Objective Details", EditorStyles.boldLabel);
        objectiveType = (ObjectiveType)EditorGUILayout.EnumPopup("Type", objectiveType);
        if (objectiveType == ObjectiveType.Kill) {
            isToString = true;
        } else {
            isToString = false;
        }
        if (objectiveType == ObjectiveType.Reach || objectiveType == ObjectiveType.Action) {
            setAutomaticColliders = EditorGUILayout.Toggle("Enable Automatic Colliders", setAutomaticColliders);
        }
        if (setAutomaticColliders) {
            EditorGUILayout.HelpBox("(Experimental) This might not work as intended. Use Manual Colliders, is Prefebly.", MessageType.Warning);
        }
        if (objectiveType != ObjectiveType.Action) {
            if (objectiveType == ObjectiveType.Kill) {
                amount = EditorGUILayout.IntField("Amount", amount);
            }
            Objective();
        } else {
            interactiveKey = (KeyCode)EditorGUILayout.EnumPopup("Action Key", interactiveKey);
            timeToInteract = EditorGUILayout.FloatField("Time to Interact", timeToInteract);
            Action();
        }
        if (internalTitle.Length < 3) {
            EditorGUI.BeginDisabledGroup(true);
        } else {
            EditorGUI.BeginDisabledGroup(false);
        }
        CreateObjective(null);
        EditorGUI.BeginDisabledGroup(false);

    }


    private void UI_UpdateObjective(Objective q) {
        EditorGUILayout.HelpBox("Change the Settings of the Objective.", MessageType.Info);

        GUILayout.Label("Objective '" + q.title + "'", EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        q.id = EditorGUILayout.IntField("ID (auto)", q.id);
        EditorGUI.EndDisabledGroup();
        q.internalTitle = EditorGUILayout.TextField("Internal Title", q.internalTitle);
        q.title = EditorGUILayout.TextField("Title", q.title);
        q.description = EditorGUILayout.TextField("description", q.description);
        q.descriptionInProgress = EditorGUILayout.TextField("description Progress", q.descriptionInProgress);
        EditorGUILayout.Space();
        GUILayout.Label("Objective Details", EditorStyles.boldLabel);
        q.objGoal.objType = (ObjectiveType)EditorGUILayout.EnumPopup("Type", q.objGoal.objType);

        if (q.objGoal.objType == ObjectiveType.Kill) {
            isToString = true;
        } else {
            isToString = false;
        }

        if (q.objGoal.objType == ObjectiveType.Reach || q.objGoal.objType == ObjectiveType.Action) {
            setAutomaticColliders = EditorGUILayout.Toggle("Enable Automatic Colliders", setAutomaticColliders);
        }

        if(setAutomaticColliders) {
            EditorGUILayout.HelpBox("(Experimental) This might not work as intended. Use Manual Colliders, is Prefebly.", MessageType.Warning);
        }
        if(q.objGoal.objType != ObjectiveType.Action) {
            if(q.objGoal.objType == ObjectiveType.Kill) {
                q.objGoal.requiredAmount = EditorGUILayout.IntField("Amount", q.objGoal.requiredAmount);
            }
            Objective(q);
        } else {
            q.key = (KeyCode)EditorGUILayout.EnumPopup("Action Key", q.key);
            q.timeToInteract = EditorGUILayout.FloatField("Time to Interact", q.timeToInteract);
            Action();
        }
        if (q.internalTitle.Length < 3) {
            EditorGUI.BeginDisabledGroup(true);
        } else {
            EditorGUI.BeginDisabledGroup(false);
        }
        //CreateObjective(null);
        EditorGUI.BeginDisabledGroup(false);
        /*
        q.objGoal.requiredAmount = EditorGUILayout.IntField("Amount", q.objGoal.requiredAmount);
        Objective();*/
        UpdateQuest(q);
    }

    private void UpdateQuest(Objective q) {
        if (GUILayout.Button("Save")) {
            objManager.UpdateObjective(q);
        }
    }

    private void UI_DisplayObjectiveList() {
        EditorGUILayout.BeginVertical();
        GUILayout.Box("Current Objective List", GUILayout.Width(200));
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(200));
        //Stuff Here
        ShowList();
        //TO FIX
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DisplayToolbar() {
        string[] tabs = { "File", "Teste" };
        selectedTab = GUILayout.Toolbar(selectedTab, tabs);
    }


    // ---------------------------------------- GUI(END) ---------------------------------------- \\
    #endregion

    #region UI RELATED

    /// <summary>
    /// Update the current Objective List
    /// </summary>
    /// <returns>The current Number of Objectives</returns>
    private int UpdateObjectiveList() {
        id = objManager.UpdateList(_main);
        return id;
    }

    /// <summary>
    /// Function that shows the List of Objectives
    /// </summary>
    private void ShowList() {
        if (objManager.GetObjectiveList().Count > 0) {
            foreach (Objective q in objManager.GetObjectiveList()) {
                if (GUILayout.Button("[" + q.id + "] " + q.internalTitle, GUILayout.Width(190))) {
                    enableReview = true;
                    temporaryDisplay = q;
                    UpdateTargets(q);
                }
            }
        } else {
            GUILayout.Label("No Objectives have been created yet.");
        }
    }

    private void UpdateTargets(Objective q) {
        targets = q.objectives;
    }

    private void Action() {
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty actions = so.FindProperty("targets");

        EditorGUILayout.PropertyField(actions, true, GUILayout.Width(810));
        so.ApplyModifiedProperties();
    }

    #endregion

    /// <summary>
    /// Function that controls the Type and the Objective
    /// Further Better documentantion needed.
    /// </summary>
    private void Objective() {
        if (isToString) {
            _GameTag = EditorGUILayout.TextField("Target Tag:", _GameTag);
            if (GUILayout.Button("Search Tag")) {
                if (_GameTag.Length > 0) {
                    _GameTag = GetTag(_GameTag);
                    Debug.Log("output: " + _GameTag);
                    if (_GameTag != "None") {
                        GameObject[] temp = GameObject.FindGameObjectsWithTag(_GameTag);
                        if (temp.Length > 0) {
                            //Debug.LogWarning(ErrorList.info1 + temp.Length + " Objects with the " + _GameTag + " tag");
                        } else {
                            //Debug.LogError(ErrorList.erro2);
                        }
                    }
                } else {
                    //Debug.LogError(ErrorList.erro1);
                }
            }
        } else {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Objective");
            _objective = EditorGUILayout.ObjectField((Object)_objective, typeof(Object), true, GUILayout.Width(658));
            EditorGUILayout.EndHorizontal();
        }
    }

    private void Objective(Objective q) {
        if (isToString) {
            _GameTag = EditorGUILayout.TextField("Target Tag:", _GameTag);
            if (GUILayout.Button("Search Tag")) {
                if (_GameTag.Length > 0) {
                    _GameTag = GetTag(_GameTag);
                    Debug.Log("output: " + _GameTag);
                    if (_GameTag != "None") {
                        GameObject[] temp = GameObject.FindGameObjectsWithTag(_GameTag);
                        if (temp.Length > 0) {
                            //Debug.LogWarning(ErrorList.info1 + temp.Length + " Objects with the " + _GameTag + " tag");
                        } else {
                            //Debug.LogError(ErrorList.erro2);
                        }
                    }
                } else {
                    //Debug.LogError(ErrorList.erro1);
                }
            }
        } else {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Objective");
            q.objective = EditorGUILayout.ObjectField((Object)q.objective, typeof(Object), true, GUILayout.Width(658)) as GameObject;
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// Fazer um script, que recebe um gameobject (qualquer neste caso relacionado com um "inimigo"
    /// Este script tera de ir buscar todos os objectos da mesma familia (idênticos) e "marca-los" como objective.
    /// Assim que a objective esteja completa, retirar de TODOS a marca objective.
    /// </summary>

    private void CreateObjective(Objective q) {
        if (!enableReview) {
            if (GUILayout.Button("Create")) {
                string tempTitle = "[" + id.ToString() + "]objective(" + Creator.SetInternalName(internalTitle) + ")";
                GameObject temp = Creator.CreateGameObject(_main, tempTitle);

                Objective newObjective = temp.AddComponent(typeof(Objective)) as Objective;

                newObjective.id = id;
                newObjective.internalTitle = internalTitle;
                newObjective.title = objTitle;
                newObjective.description = description;
                newObjective.descriptionInProgress = descriptionInProgress;

                objectiveGoal.objType = objectiveType;
                objectiveGoal.requiredAmount = amount;

                newObjective.objGoal = objectiveGoal;

                if (objectiveType == ObjectiveType.Reach) {
                    newObjective.objective = (GameObject)_objective;
                    newObjective.setAutomaticColliders = setAutomaticColliders;
                } else if (objectiveType == ObjectiveType.Action) {
                    newObjective.objectives = targets;
                    newObjective.timeToInteract = timeToInteract;
                    newObjective.key = interactiveKey;
                    newObjective.setAutomaticColliders = setAutomaticColliders;
                } else {
                    newObjective.gameobjectTag = _GameTag;
                }

                objManager.AddNewObjective(newObjective);
                UpdateObjectiveList();
                ClearInfo();

            }
        } else {
            if (GUILayout.Button("Save")) {
                UI_UpdateObjective(q);
            }
        }

    }

    void OnInspectorUpdate() {
        Repaint();
    }

    private void ClearInfo() {
        internalTitle = "";
        objTitle = "";
        description = "";
        descriptionInProgress = "";
        amount = 1;
        objectiveType = ObjectiveType.Reach;
    }

    private string GetTag(string tag) {
        string[] temp = InternalEditorUtility.tags;
        for (int i = 0; i < temp.Length; i++) {
            if (temp[i].ToLower() == tag.ToLower()) {
                if (temp[i] != "Untagged")
                    return temp[i];
            }
        }
        return "None";
    }

    // Internal Searches & Stuff

    private void SetObjectiveMainObj() {
        if (!GameObject.Find(ObjectiveManagerConstants.objHolderName)) {
            _main = Creator.CreateObjectiveMain();
        } else {
            _main = GameObject.Find(ObjectiveManagerConstants.objHolderName);
        }
    }

    private void SetObjectiveManager() {
        if (!GameObject.Find(ObjectiveManagerConstants.objManagerName)) {
            _objectiveManager = new GameObject(ObjectiveManagerConstants.objManagerName);
            _objectiveManager.AddComponent<ObjectiveManager>();
            objManager = _objectiveManager.GetComponent<ObjectiveManager>();
            Debug.LogWarning("Creating new Objective Manager");
        } else {
            _objectiveManager = GameObject.Find(ObjectiveManagerConstants.objManagerName);
            objManager = _objectiveManager.GetComponent<ObjectiveManager>();
            objManager.CheckManager();
            Debug.LogWarning("Updating objective Window");
        }
    }
}