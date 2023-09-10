using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent (typeof (Rigidbody))]
public class Body : MonoBehaviour
{
    public GameObject visObj;
    public GameObject colShadow = null;
    [SerializeField] private Material shadowMat;
    [Range(0.00001f, 1000f)]public float radius;
    [Range(1f, 1000f)]public float density;
    public Vector3 initialVelocity;
    public string bodyName = "Unnamed";

    public Vector3 curVelocity { get; private set; }
    public float mass {get; private set;}
    Rigidbody bodyRb;

    const float volumeConst = 4/3 * 3.14f;
    void Awake()
    {
        bodyRb = GetComponent<Rigidbody>();
        //colShadow = new GameObject();
    }

    void Update() 
    {
        if(!Application.isPlaying || !Universe.allBodies.Contains(this))
        {
            //mass = density * volume
            //volume of sphere = 4/3 * pi * radius^3
            mass = density * radius;
            bodyRb.mass = mass;
            curVelocity = initialVelocity;
            bodyRb.velocity = curVelocity;
            transform.localScale = Vector3.one * radius * 2;
            //if(visObj == null)
               // SpawnShadow();
            if(!Universe.allBodies.Contains(this))
                Universe.allBodies.Add(this);
        }
    }   

    public void CalculateForce(Body otherBody)
    {
        Vector3 difVector = otherBody.transform.position - transform.position;
        float distSqr = difVector.sqrMagnitude;
        Vector3 dir = difVector.normalized;
        //F = (GravConst*m1*m2) / d^2
        Vector3 force = dir * (Universe.gravityConst * otherBody.mass) / distSqr;
        //F = MA
        //Force == Acceleration of the body(mass of the body is not considered in eq)
        curVelocity += force * Universe.physicsTimeStep;
        bodyRb.velocity = curVelocity;
    }

    private void SpawnShadow()
    {
        //Create a shadow object to predict position
        visObj = GetComponentInChildren<MeshRenderer>().gameObject;
        visObj = Instantiate(visObj, transform.position, transform.rotation);
        visObj.transform.localScale = transform.localScale;
        //DestroyImmediate(visObj.GetComponent<MeshRenderer>());
        Material mat = visObj.GetComponent<MeshRenderer>().material;
        Material instShadow = Material.Instantiate(shadowMat);
        float alpha = instShadow.color.a;
        Color col = mat.color;
        col.a = 0.1f;
        instShadow.color = col;
        visObj.GetComponent<MeshRenderer>().material = instShadow;
    }
}
