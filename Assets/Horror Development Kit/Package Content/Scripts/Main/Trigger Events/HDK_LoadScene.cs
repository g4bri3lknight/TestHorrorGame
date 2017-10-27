//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;

public class HDK_LoadScene : MonoBehaviour {

    public string SceneName;
    GameObject LoadingPrefab;

    void Start()
    {
        LoadingPrefab = GameObject.Find("Player").GetComponent<GameSceneManager>()._loadingScreen;
    }

    void OnTriggerEnter (Collider col)
    {		
		if (col.tag == "Player")
        {
            HDK_LoadingScreen.LoadScene(SceneName, LoadingPrefab);
        }
	}
}