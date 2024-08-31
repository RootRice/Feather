using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FootStepComponent : MonoBehaviour
{
	public VisualEffect LeftFootParticles;
	public VisualEffect RightFootParticles;
    public VisualEffect DashParticles;
	public VisualEffect JumpParticles;
	public VisualEffect AirDashParticles;
    public VisualEffect DoubleJumpParticles;
	public VisualEffect LandParticles;
    void FootstepEvent(int whichfoot)
    {
        Debug.Log("Foot step: " + whichfoot);
        switch (whichfoot)
        {
            case 0:
                LeftFootParticles.Play();
                break;
            case 1:
                RightFootParticles.Play();
                break;
            case 2:
                DashParticles.Play();
                break;
            case 3:
                JumpParticles.Play();
                break;
            case 4:
                AirDashParticles.Play();
                break;
            case 5:
                DoubleJumpParticles.Play();
                break;
            case 6:
                LandParticles.Play();
                break;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
