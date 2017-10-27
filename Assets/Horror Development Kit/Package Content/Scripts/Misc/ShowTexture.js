#pragma strict
// FPS KIT [www.armedunity.com]

var show : boolean;
var textureToShow : Texture2D;

function Start(){
	show = true;
}

function OnGUI () {
	if(show){
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), textureToShow);
	}
}