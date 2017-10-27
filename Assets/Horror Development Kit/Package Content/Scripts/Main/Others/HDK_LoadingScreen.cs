//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
 
public class HDK_LoadingScreen : MonoBehaviour
{
	private static HDK_LoadingScreen instance = null;     // LoadingScreen script access
    public Image background;
	public Image topPanel;
	public Image downPanel;
    public Text status;
    public float animationSpeed = 1.25f;
    private AsyncOperation loadingProcess;    // Scene loading process

    // Load a new scene
    public static void LoadScene(string sceneName, GameObject loadingPrefab)
    {
        // If there isn't a LoadingScreen, then create a new one
        if (instance == null)
        {
			instance = Instantiate(loadingPrefab.GetComponent<HDK_LoadingScreen>());
			// Don't destroy loading screen while it's loading
            DontDestroyOnLoad(instance.gameObject);
        }
         
        // Enable loading screen
        instance.gameObject.SetActive(true);
        // Start loading between scenes (Background process. That's why there is an Async)
        instance.loadingProcess = SceneManager.LoadSceneAsync(sceneName);
        // Don't switch scene even after loading is completed
        instance.loadingProcess.allowSceneActivation = false;
    }
 
    void Awake()
    {
        // Set loading screen invisible at first (panel alpha color)
        Color c = background.color;
        c.a = 0f;
		background.color = c;

		Color c2 = topPanel.color;
		c2.a = 0f;
		topPanel.color = c2;

		Color c3 = downPanel.color;
		c3.a = 0f;
		downPanel.color = c3;
         
		c = status.color;
        c.a = 0f;
		status.color = c;
    }
 
    void Update()
    {
        // Update loading status
		status.text = (loadingProcess.progress * 100f) + "%";
         
        // If loading is complete
        if (loadingProcess.isDone)
        {
            // Fade out
			Color c = background.color;
            c.a -= animationSpeed * Time.deltaTime;
			background.color = c;

			Color c2 = topPanel.color;
			c2.a -= animationSpeed * Time.deltaTime;
			topPanel.color = c2;

			Color c3 = downPanel.color;
			c3.a -= animationSpeed * Time.deltaTime;
			downPanel.color = c3;
             
			c = status.color;
            c.a -= animationSpeed * Time.deltaTime;
			status.color = c;
             
            // If fade out is complete, then disable the object
            if (c.a <= 0)
            {
                gameObject.SetActive(false);
            }
        }
        else // If loading proccess isn't completed
        {
            // Start Fade in
			Color c = background.color;
            c.a += animationSpeed * Time.deltaTime;
			background.color = c;

			Color c2 = topPanel.color;
			c2.a += animationSpeed * Time.deltaTime;
			topPanel.color = c2;

			Color c3 = downPanel.color;
			c3.a += animationSpeed * Time.deltaTime;
			downPanel.color = c3;
             
			c = status.color;
            c.a += animationSpeed * Time.deltaTime;
			status.color = c;
             
            // If loading screen is visible
            if (c.a >= 1)
            {
                // We're good to go. New scene is on! :)
                loadingProcess.allowSceneActivation = true;
            }
        }
    }
}