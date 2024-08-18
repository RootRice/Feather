using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningController : MonoBehaviour
{
    MeshRenderer myRenderer;
    [SerializeField] float timeRangeMin;
    [SerializeField] float timeRangeMax;
    float currentTime;
    float nextTime;
    float intenstiy = 0;
    int nameID;
    [SerializeField] float flashSpeed;
    [SerializeField] AnimationCurve curve;
    // Start is called before the first frame update
    void Start()
    {
        myRenderer = GetComponent<MeshRenderer>();
        timeRangeMin = 1; timeRangeMax = 2;
        nextTime = Random.Range(timeRangeMin, timeRangeMax);
        nameID = Shader.PropertyToID("_LightningIntensity");
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTime >= nextTime)
        {
            intenstiy += Time.deltaTime;
            float sinTensity = curve.Evaluate(intenstiy);
            if (intenstiy > 2.5)
            {
                currentTime = 0;
                nextTime = Random.Range(timeRangeMin, timeRangeMax);
                intenstiy = 0;
            }
            myRenderer.sharedMaterial.SetFloat(nameID, sinTensity*2);
            
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }
}
