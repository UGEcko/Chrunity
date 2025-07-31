using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChromaObject : MonoBehaviour
{
    public ChromaObjectType objectType = ChromaObjectType.Cube;
    public string objectNameOverride = "";
    public string objectMaterial = "";
    public string track = "";
    public int lightID = 0;
    public int lightType = 0;
    
    public enum ChromaObjectType
    {
        Dummy,
        Cube,
        Sphere,
        Capsule,
        Cylinder,
        Plane,
        Quad,
        Triangle
    }
}
