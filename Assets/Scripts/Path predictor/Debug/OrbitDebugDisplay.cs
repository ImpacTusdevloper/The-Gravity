using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor;

//using System.Diagnostics;
using UnityEngine;

[ExecuteInEditMode]
public class OrbitDebugDisplay : MonoBehaviour 
{
    [Range(0, 20000)] public int numSteps = 1000;
    public float timeStep = 0.1f;
    public bool usePhysicsTimeStep;

    public bool relativeToBody;
    public Body centralBody;
    public float width = 100;
    public bool useThickLines;
    /*
    void Start()
    {
        if (Application.isPlaying)
            HideOrbits();
    }

    void Update()
    {
        if (!Application.isPlaying && !EditorApplication.isPaused)
            DrawOrbits();
    }
    */
    void DrawOrbits()
    {
        Body[] bodies = Universe.allBodies.ToArray();
        if(bodies.Length <= 0)
            return;
        var virtualBodies = new VirtualBody[bodies.Length];
        var drawPoints = new Vector3[bodies.Length][];
        int referenceFrameIndex = 0;
        Vector3 referenceBodyInitialPosition = Vector3.zero;
        OnValidate();

        // Initialize virtual bodies (don't want to move the actual bodies)
        for (int i = 0; i < virtualBodies.Length; i++) 
        {
            virtualBodies[i] = new VirtualBody (bodies[i]);
            drawPoints[i] = new Vector3[numSteps];
            virtualBodies[i].collided = false;

            if (bodies[i] == centralBody && relativeToBody) {
                referenceFrameIndex = i;
                referenceBodyInitialPosition = virtualBodies[i].position;
            }
        }
        // Simulate
        for (int step = 0; step < numSteps; step++) 
        {
            Vector3 referenceBodyPosition = (relativeToBody) ? virtualBodies[referenceFrameIndex].position : Vector3.zero;

            //Collision Check
            //CollisionCheck(virtualBodies);

            // Update velocities
            for (int i = 0; i < virtualBodies.Length; i++) 
            {
                /*
                if(virtualBodies[i].collided)
                {
                    virtualBodies[i].velocity = Vector3.zero;
                    continue;
                }
                */
                virtualBodies[i].velocity += CalculateAcceleration (i, virtualBodies) * timeStep;
            }

            // Update positions
            for (int i = 0; i < virtualBodies.Length; i++) 
            {
                VirtualBody curBody = virtualBodies[i];
                Vector3 newPos = curBody.position + curBody.velocity * timeStep;
                curBody.position = newPos;
                //Update position of visual body(shadow)
                if(bodies[i].visObj != null)
                    bodies[i].visObj.transform.position = newPos + 
                        (newPos - curBody.position).normalized * curBody.radius;

                if (relativeToBody)
                {
                    var referenceFrameOffset = referenceBodyPosition - referenceBodyInitialPosition;
                    newPos -= referenceFrameOffset;
                }
                if (relativeToBody && i == referenceFrameIndex)
                {
                    newPos = referenceBodyInitialPosition;
                }

                drawPoints[i][step] = newPos;
            }
        }

        // Draw paths and Gizmos
        for (int bodyIndex = 0; bodyIndex < virtualBodies.Length; bodyIndex++) 
        {
            Body curBody = bodies[bodyIndex];
            Vector3[] curPoint = drawPoints[bodyIndex];
            var pathColour = curBody.gameObject.
            GetComponentInChildren<MeshRenderer>().sharedMaterial.color;

            if(useThickLines)
            {
                var lineRenderer = curBody.gameObject.GetComponentInChildren<LineRenderer>();
                lineRenderer.enabled = true;
                lineRenderer.positionCount = curPoint.Length;
                lineRenderer.SetPositions (curPoint);
                lineRenderer.startColor = pathColour;
                lineRenderer.endColor = pathColour;
                lineRenderer.widthMultiplier = width;
            } 
            
            else
            {
                for(int i = 0; i < curPoint.Length - 1; i++) 
                {
                    Debug.DrawLine(curPoint[i], curPoint[i + 1], pathColour);
                }

                // Hide renderer
                var lineRenderer = curBody.gameObject.
                    GetComponentInChildren<LineRenderer>();
                if (lineRenderer)
                    lineRenderer.enabled = false;
            }
        }
    }

    Vector3 CalculateAcceleration(int i, VirtualBody[] virtualBodies)
    {
        Vector3 acceleration = Vector3.zero;
        for (int j = 0; j < virtualBodies.Length; j++) {
            if (i == j) {
                continue;
            }
            Vector3 forceDir = (virtualBodies[j].position - virtualBodies[i].position).normalized;
            float sqrDst = (virtualBodies[j].position - virtualBodies[i].position).sqrMagnitude;
            acceleration += forceDir * Universe.gravityConst * virtualBodies[j].mass / sqrDst;
        }
        return acceleration;
    }

    void HideOrbits()
    {
        Body[] bodies = Universe.allBodies.ToArray();

        // Draw paths
        for (int bodyIndex = 0; bodyIndex < bodies.Length; bodyIndex++) 
        {
            var lineRenderer = bodies[bodyIndex].gameObject.
            GetComponentInChildren<LineRenderer>();
            lineRenderer.positionCount = 0;
        }
    }

    private void CollisionCheck(VirtualBody[] virtualBodies)
    {
        List<VirtualBody> collidedBody = new List<VirtualBody>();
        foreach(VirtualBody body in virtualBodies)
        {
            foreach(VirtualBody otherBody in virtualBodies)
            {
                if(otherBody == body || otherBody.collided && body.collided)
                    continue;

                float minDist = otherBody.radius + body.radius;
                if((otherBody.position - body.position).sqrMagnitude <= minDist * minDist)
                    collidedBody.Add(body);
            }
        }

        foreach(VirtualBody virBody in collidedBody)
        {
            if(virBody.collided)
                continue;
            if(virBody.body.colShadow == null)
            {
                virBody.body.colShadow = Instantiate(virBody.body.visObj, virBody.position, 
                    virBody.body.visObj.transform.rotation);
            }
            virBody.body.colShadow.transform.position = virBody.position;
            virBody.collided = true;
        }
    }

    void OnValidate()
    {
        if (usePhysicsTimeStep)
            timeStep = Universe.physicsTimeStep;
    }

    class VirtualBody
    {
        public Vector3 position;
        public Vector3 velocity;
        public float mass;
        public float radius;
        public bool collided = false;
        public Body body;
        //public GameObject colShadow;
        public VirtualBody (Body _body)
        {
            if(_body == null)
                return;
            position = _body.transform.position;
            velocity = _body.initialVelocity;
            mass = _body.mass;
            radius = _body.radius;
            collided = false;
            body = _body;
        }
    }
}