var FPS:int;
function Start () {
    Do();
}
function Do(){
    while(true){
        yield WaitForSeconds(1);
        GetComponent("Text").text = FPS+" : FPS";
        FPS=0;
    }
}
function Update () {
    FPS++;
}