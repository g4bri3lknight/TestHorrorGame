using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HDK_LightFlicker : MonoBehaviour {  // This name 'LightFlickerPulse' is a convenient name for search results.

    public bool checkAssigned; // The option to continuously update the checkAssigned function (incase anything changes). Recommended off.
    public bool displayScriptErrors = true; // Displays script versions of errors so that Unity's errors don't spam and slow down your fps to an extent. Recommended on/

    public enum UseType {Color, Intensity, Range, Off}  // The way you want to affect the light in a nice drop down menu!
	public UseType useType;  // The inspector view of the UseTypes!
	
	public enum WaveFunction {Noise, Sin, Tri, Sqr, Saw, SawInv}  // The type of function you want to use in a drop down menu!
	public WaveFunction waveFunction;  // The inspector view of the WaveFunctions!

	public bool noiseUpdatePF = true;  // Like a vsync for the rate of update, doesn't affect FPS!  **PF = Per Frame!
	public bool noiseSmoothFade;  // Use this if you slow down the rate a lot!
	public float noiseSmoothSpeed = 4;  // To smooth the noise function to what speed you like!
	
	public float timescaleMin = 0.1f;  // The capping range to stop the script at pause!
	
	public float coreBase = 0.8f;  // The strength of the light, changes from the useTypes!
	public float amplitude = 0.4f;  // The measurement of how intense the functions are going to work!
	public float phase;  // Changes the time from the start of the first frame
	public float frequency = 0.5f;  // The measurement of time between the cycles, use this to vsync the noise rate too!

	public bool useOutsideLight;  // The option to enable or disable the use of an or many OutsideLight(s)
	public List<Light> outsideLight;  // The array to hold all of the lights you want to affect
	
	private Color mainColor;  // Hidden variable to remember the single lights colour on the gameObject
	private float mainValueI;  // Hidden variable to remember the single lights intensity on the gameObject
	private float mainValueR;  // Hidden variable to remember the single lights range on the gameObject

    [HideInInspector]
    public List<Color> mainOutColor;  // Hidden variables to remember the single light(s) colours on different gameObjects
    [HideInInspector]
    public List<float> mainOutValueI;  // Hidden variables to remember the single light(s) intensities on different gameObjects
    [HideInInspector]
    public List<float> mainOutValueR;  // Hidden variables to remember the single light(s) ranges on different gameObjects
	
	private Light lightS;  // Hidden variable on the single gameObjects light component!
	
	private bool error1;  // Hidden variable remembering if you had the error1 or not
	private bool error2;  // Hidden variable remembering if you had the error2 or not
	private bool assigned;  // Hidden variable to make sure you have assigned everything, almost an on/off switch to control if it works or not!
	
	private float x;  // Hidden variable used for the outcome calculation
	private float y;  // Hidden variable used for the outcome calculation
	private float z;  // Hidden variable used for the outcome calculation

	private float noiseD;  // Hidden variable keeping track of the delays timer to cap the noise WaveFunction
	private bool noiseGo;  // Hidden variable to ask it the noise function can re-randomise again
	
	
	void Update(){
        phase = Mathf.Clamp01(phase);
            if (waveFunction == WaveFunction.Noise && !noiseUpdatePF)
            {
                if (noiseD > frequency)
                {
                    noiseD = 0;
                    noiseGo = true;
                }
                else
                {
                    noiseD += Time.deltaTime;
                    noiseGo = false;
                }
            }

            if (lightS == null && !useOutsideLight)
            {
                lightS = gameObject.GetComponent<Light>();
            }
            if (checkAssigned || !assigned)
            {
                CheckAssigned();
            }

            if (((!error1 && !useOutsideLight) || (!error2 && useOutsideLight)) && assigned && useType != UseType.Off && Time.timeScale > timescaleMin)
            {
                CalWaveFunc();
                if (useOutsideLight)
                {
                    for (int i = 0; i < outsideLight.Count; i++)
                    {
                        if (useType == UseType.Color)
                        {
                            outsideLight[i].color = mainOutColor[i] * (z);
                        }
                        else if (useType == UseType.Intensity)
                        {
                            outsideLight[i].intensity = mainOutValueI[i] * (z);
                        }
                        else if (useType == UseType.Range)
                        {
                            outsideLight[i].range = mainOutValueR[i] * (z);
                        }
                        if (mainOutValueI[i] != outsideLight[i].intensity && (useType != UseType.Intensity))
                        {
                            outsideLight[i].intensity = mainOutValueI[i];
                        }
                        if (mainOutValueR[i] != outsideLight[i].range && (useType != UseType.Range))
                        {
                            outsideLight[i].range = mainOutValueR[i];
                        }
                        if (mainOutColor[i] != outsideLight[i].color && (useType != UseType.Color))
                        {
                            outsideLight[i].color = mainOutColor[i];
                        }
                    }
                }
                else
                {
                    if (useType == UseType.Color)
                    {
                        lightS.color = mainColor * (z);
                    }
                    else if (useType == UseType.Intensity)
                    {
                        lightS.intensity = mainValueI * (z);
                    }
                    else if (useType == UseType.Range)
                    {
                        lightS.range = mainValueR * (z);
                    }
                }
            }
            if (!useOutsideLight && !error1)
            {
                if (mainValueI != lightS.intensity && (useType != UseType.Intensity))
                {
                    lightS.intensity = mainValueI;
                }
                if (mainValueR != lightS.range && (useType != UseType.Range))
                {
                    lightS.range = mainValueR;
                }
                if (mainColor != lightS.color && (useType != UseType.Color))
                {
                    lightS.color = mainColor;
                }
            }
	}

    void CheckAssigned ()
    {
        if (!GetComponent<Light>() && displayScriptErrors && !error1 && !useOutsideLight)
        {
            Debug.LogError("ERROR MISSING COMPONENT: The gameObject " + "'" + gameObject.name + "'" + " does not have a Light component attached to it. If you wish to use a Light from another gameObject, please select the boolean variable 'Use Outside Light' for this feature.", gameObject);
            error1 = true;
        }
        else if (!useOutsideLight && (!error1 || GetComponent<Light>()))
        {
            mainColor = GetComponent<Light>().color;
            mainValueI = GetComponent<Light>().intensity;
            mainValueR = GetComponent<Light>().range;
            if (error1)
            {
                Debug.Log("FIXED MISSING COMPONENT: The missing Light component on the gameObject " + "'" + gameObject.name + "'" + " has been successfully re-established!", gameObject);
                error1 = false;
            }
            assigned = true;
        }

        if (useOutsideLight)
        {
            if (mainOutColor.Count > 0)
            {
                mainOutColor.RemoveRange(0, mainOutColor.Count);
                mainOutValueI.RemoveRange(0, mainOutValueI.Count);
                mainOutValueR.RemoveRange(0, mainOutValueR.Count);
            }
            if (OutsideLight() && error2)
            {
                error2 = false;
                Debug.Log("FIXED EMPTY VARIABLE:  The gameObject " + "'" + gameObject.name + "'" + " has re-established an index with missing Light component!", gameObject);
            }
            if (!error2)
            {
                assigned = true;
            }
        }
    }

    bool OutsideLight ()
    {
        for (int i = 0; i < outsideLight.Count; i++)
        {
            if (!outsideLight[i] && displayScriptErrors && !error2)
            {
                error2 = true;
                Debug.LogError("ERROR EMPTY VARIABLE: The gameObject " + "'" + gameObject.name + "'" + " does not have a Light component attached to the variable 'Outside Light' index number " + "[ " + i + " ], to fix this either, assign a gameObject with a Light component in this space or, lower the size amount to fit your assigned lights. If you do not wish to use this feature, un-tick the boolean variable 'Use Outside Light.'", gameObject);
                return false;
            }
            else if(outsideLight[i] || !error2)
            {
                mainOutColor.Add(outsideLight[i].color);
                mainOutValueI.Add(outsideLight[i].intensity);
                mainOutValueR.Add(outsideLight[i].range);
            }
            else
            {
                return false;
            }
        }
        return true;
    }
	
	
	void CalWaveFunc(){
		
		x = (Time.time + phase)*frequency;
		x = x - Mathf.Floor(x);
		
		if(waveFunction == WaveFunction.Sin){
			y = Mathf.Sin(x*2*Mathf.PI);
		}else if(waveFunction == WaveFunction.Tri){
			if(x<0.5f){
				y=4.0f*x-1.0f;
			}else{
				y=-4.0f*x-3.0f;
			}
		}else if(waveFunction == WaveFunction.Sqr){
			if(x<0.5f){
				y=1.0f;
			}else{
				y=-1.0f;
			}
		}else if(waveFunction == WaveFunction.Saw){
			y=x;
		}else if(waveFunction == WaveFunction.SawInv){
			y=1.0f-x;
		}else if(waveFunction == WaveFunction.Noise){
			if(noiseGo || noiseUpdatePF){
				y= 1-(Random.value*2);
			}
		}
		if(noiseSmoothFade && waveFunction == WaveFunction.Noise){
			z = Mathf.Lerp(z, (y*amplitude)+coreBase, Time.deltaTime *noiseSmoothSpeed);
		}else{
			z = (y*amplitude)+coreBase;
		}
	}
}
/// This script is free to use and may be modified to your likings.
/// Self distribution is not permitted. 
/// Creditation for your use of this script is mandatory.
// Developed and distributed by Molten Metal Studios.

