using Unity.VisualScripting;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        Material mat = GetComponentInChildren<MeshRenderer>().material;

        Color col = new Color();
        
        col.r = Random.Range(0f, 1f);
        col.g = Random.Range(0f, 1f);
        col.b = Random.Range(0f, 1f);
        float h;
        float s;
        float v;
        Color.RGBToHSV(col, out h, out s, out v);
        v = 1f;
        mat.color = col;

    }
}
