using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class VarManager : MonoBehaviour
{

    public Slider SatisfactionVar;
    public float timeRemaining;
    public float maxTime = 20.0f;
    public GameObject globalVolume0;
    public GameObject globalVolume1;
    private CustomPostProcessVolumeComponent color;
    public float decreaseRatio = 0.5f;

    public float thresholdValue = 0.5f;
    public SceneLoader sceneManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //clonedProfile = Instantiate(globalVolume.profile);
        timeRemaining = maxTime;
        //globalVolume.profile = clonedProfile;
        //clonedProfile.TryGet<ColorAdjustments>(out color);
        //color = Instantiate(color);
        //globalVolume = Camera.main.GetComponentInChildren<Volume>();
        globalVolume0.GetComponent<Volume>().weight = 0.0f;
        globalVolume1.GetComponent<Volume>().weight = 1.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= decreaseRatio*Time.deltaTime;
            SatisfactionVar.value = timeRemaining / maxTime;
            //color = globalVolume.GetComponent<CustomPostProcessVolumeComponent>().;
            //clonedProfile.TryGet<ColorAdjustments>(out color);
            //if (color != null)
            //{
            // mappedValue = targetMin + (value-sourceMin) * (targetMax - targetMin) / (sourceMax - sourceMin)
            // 100 = 200x
            // 100 - 0 == 100x - -100x
            // 
            //color.Override();
            //color.saturation.value = SatisfactionVar.value*100;
            //clonedProfile.components[0] = color; 

            // Value/minValue - 1
            if (SatisfactionVar.value > thresholdValue)
            {
                globalVolume1.GetComponent<Volume>().weight = SatisfactionVar.value/thresholdValue - 1;
            }
            // 1 - Value/maxValue
            else
            {
                globalVolume0.GetComponent<Volume>().weight =  1 - SatisfactionVar.value/thresholdValue;
            }
                
        }
        else {
            sceneManager.ReloadCurrentScene();
        }
    }
}
