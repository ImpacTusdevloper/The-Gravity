using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GBodySimulator : MonoBehaviour
{
    bool canContinue = true;
    void Awake()
    {
        Time.fixedDeltaTime = Universe.physicsTimeStep;
    }

    void FixedUpdate()
    {
        //if(canContinue)
            //StartCoroutine(GravityCalc());
        
        //Update velocity for all bodies
        foreach(Body body in Universe.allBodies)
        {
            foreach(Body otherBody in Universe.allBodies)
            {
                if(otherBody == body)
                    continue;
                
                body.CalculateForce(otherBody);
            }
        }
        
    }

    private IEnumerator GravityCalc()
    {
        canContinue = false;
        //int i = 0;
        float startTime = Time.realtimeSinceStartup;
        float timeLimit = Time.fixedDeltaTime;

        //Update velocity for all bodies
        foreach(Body body in Universe.allBodies)
        {
            foreach (Body otherBody in Universe.allBodies)
            {
                if(otherBody == body)
                    continue;
                
                body.CalculateForce(otherBody);

                if(Time.realtimeSinceStartup - startTime > timeLimit)
                {
                    //i++;
                    //Debug.Log(i);
                    startTime = Time.realtimeSinceStartup;
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        canContinue = true;
    }
}
