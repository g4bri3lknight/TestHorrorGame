using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class BreakableObject : MonoBehaviour
{
    [Header("Parameter")]
    [Tooltip("Il prefab che verra'creato quando questo oggetto si distruggera' per simulare la rottura dello stesso.")]
    public GameObject destroyedObject;
    [Space(5)]
    [Tooltip("Il prefab che verra'creato quando questo oggetto si distruggera'. Rappresenta l'oggetto contenuto nella scatola.Esempio: medkit,munizioni,ecc..).\n" +
        "Se e' presente piu'di un elemento viene effettuato un random tra i prefab. Puo'essere vuoto")]
    public List<GameObject> ListOfItemObjectInside;
    [Space(5)]
    [Tooltip("NON VALORIZZARE!Mostra l'oggetto selezionato tramite random!(Per usi futuri...)")]
    public GameObject itemObjectInside;
    [Space(5)]
    [Tooltip("Se TRUE il random includera' la possibilita'che la scatola non dia nessun oggetto")]
    public bool canBeEmpty;
    [Space(10)]
    /*
    [Tooltip("Audio da riprodurre quando l'oggetto viene distrutto")]
    public AudioClip breakAudio;
    [Space(5)]
    */
    [Header("Cursor")]
    [Tooltip("Cursore di default quando non si sta sopra l'oggetto con il mouse")]
    public GameObject defaultCursor;
    [Space(5)]
    [Tooltip("Cursore personalizzato quando si sta sopra l'oggetto con il mouse")]
    public GameObject breakableCursor;

    void Start()
    {
        int rand;
        if (canBeEmpty)
        {
            rand = Random.Range(-1, ListOfItemObjectInside.Count);
        }
        else
        {
            rand = Random.Range(0, ListOfItemObjectInside.Count);
        }

        //-1 viene usato per non generare nessun oggetto all'interno della scatola
        if (rand == -1)
        {
            itemObjectInside = null;
        }
        else
        {
            itemObjectInside = ListOfItemObjectInside[rand];
        }

    }

    public void  OnMouseOver()
    {
        if (breakableCursor)
        {
            breakableCursor.SetActive(true);
            defaultCursor.SetActive(false);
        }
    }

    public void OnMouseExit()
    {
        defaultCursor.SetActive(true);
        breakableCursor.SetActive(false);
    }

    void DestroyIt()
    {

        if (ListOfItemObjectInside != null && ListOfItemObjectInside.Count > 0)
        {
            if (itemObjectInside != null)
            {
                GameObject clone = Instantiate(itemObjectInside, transform.position, transform.rotation);
            }
        }
        
        if (destroyedObject)
        {
            destroyedObject.transform.localScale = this.transform.localScale;
            Instantiate(destroyedObject, transform.position, transform.rotation);
        }

        defaultCursor.SetActive(true);
        breakableCursor.SetActive(false);

        Destroy(this.gameObject);
    }
}