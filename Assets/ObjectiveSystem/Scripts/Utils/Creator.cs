using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creator {
    public static GameObject CreateObjectiveMain() {
        GameObject _object = new GameObject();
        _object.name = ObjectiveManagerConstants.objHolderName;

        return _object;
    }

    public static GameObject CreateGameObject(GameObject parent, string name) {
        GameObject _object = new GameObject();
        _object.name = name;

        _object.transform.parent = parent.transform;

        return _object;
    }

    public static string SetInternalName(string title) {
        if (title != null)
            return title.Replace(" ", "_");
        else
            throw new System.Exception("Internal Name Can't be null.");
    }

}
