//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HDK_UITextManager : MonoBehaviour
{
    Text mainText;

    void Start()
    {
        mainText = GameObject.Find("itemInteraction").GetComponent<Text>();
    }

    public void ShowTextInfo(string text)
    {
        StartCoroutine(ShowText(text));
    }

    IEnumerator ShowText(string text)
    {
        mainText.text = text;
        yield return new WaitForSeconds(2f);
        mainText.text = null;
    }
}