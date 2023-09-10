using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Universe
{
    public const float gravityConst = 10f;
    public const float physicsTimeStep = 0.02f;
    public static List<Body> allBodies = new List<Body>();
}
