using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInfo : ScriptableObject
{
    private int sceneIndex;

    private string sceneName;

    public int SceneIndex
    {
        get
        {
            return this.sceneIndex;
        }
        set
        {
            sceneIndex = value;
        }
    }

    public string SceneName
    {
        get
        {
            return this.sceneName;
        }
        set
        {
            sceneName = value;
        }
    }
}
