#pragma strict

var LaserOrigin: Transform; // Put the laser (line renderer) object here.

var LaserTarget: Transform; //The point light object.
function Update () {

    var ray =  new Ray(LaserOrigin.transform.position, LaserOrigin.transform.forward); //Casts a new ray

    var hit: RaycastHit;

      

    if(Physics.Raycast(ray, hit, Mathf.Infinity))

    {

         

        LaserTarget.transform.position = hit.point; //Positions the light at the same point the laser hits something.

    }

      
}