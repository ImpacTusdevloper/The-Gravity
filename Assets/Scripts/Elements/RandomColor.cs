using UnityEngine;

public class RandomColor : MonoBehaviour
{
    public bool bright = false;
    // Start is called before the first frame update
    void Start()
    {
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        if(bright)
        {
            Color col = Random.ColorHSV(0f, 1f, 0.8f, 1f, 1f, 1f);
            mat.color = col;
        }
        else
        {
            Color col = Random.ColorHSV();
            mat.color = col;
        }

    }
}
