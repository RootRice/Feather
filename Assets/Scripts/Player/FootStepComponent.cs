using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepComponent : MonoBehaviour
{
	public ParticleSystem LeftFootParticles;
	public ParticleSystem RightFootParticles;
	public ParticleSystem DashParticles;
	public ParticleSystem JumpParticles;
	public ParticleSystem AirDashParticles;
	public ParticleSystem DoubleJumpParticles;
	public ParticleSystem LandParticles;
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
