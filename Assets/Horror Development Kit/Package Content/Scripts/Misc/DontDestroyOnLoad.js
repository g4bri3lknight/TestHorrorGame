// Make this game object and all its transform children
	// survive when loading a new scene.
	function Awake () {
		DontDestroyOnLoad (transform.gameObject);
	}