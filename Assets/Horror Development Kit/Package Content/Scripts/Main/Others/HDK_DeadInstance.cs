//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HDK_DeadInstance : MonoBehaviour {

	public float secBeforeFade = 3.0f;
	public float fadeTime = 5.0f;
	public Texture fadeTexture;
	private bool  fadeIn = false;
	private float tempTime;
	private float time = 0.0f;
	public AudioClip dieSound;

	void  Start ()
	{		
		AudioSource.PlayClipAtPoint(dieSound, transform.position);
		StartCoroutine (Fade ());
	}

	IEnumerator Fade()
	{
		yield return new WaitForSeconds(secBeforeFade);
		fadeIn = true;
	}

	void  Update ()
	{
		if (fadeIn){
			if(time < fadeTime) time += Time.deltaTime;
			tempTime = Mathf.InverseLerp(0.0f, fadeTime, time);
		}

		if(tempTime >= 1.0f) 
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	void OnGUI ()
	{
		if(fadeIn){
			Color colorT = GUI.color;
			colorT.a = tempTime;
			GUI.color = colorT;
			GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
		}
	}
}