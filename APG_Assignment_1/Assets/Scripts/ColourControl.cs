using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourControl : MonoBehaviour
{
    //public List<GameObject> paintable;

    [Header("Station")]
    public Material buildingMat;
    public Material buildingBaseMat;
    public Material roofMat;
    public Material doorMat;
    public Material doorKnobMat;
    public Material windowGlassMat;
    public Material windowPaneMat;
    public Material cloudMat;

    public Gradient buildingColours;
    public Gradient cloudColours;
    public Gradient trainColours;



    public void Paint()
    {
        // TODO: limit colours to a section of the palette?

        GameObject[] paintable = GameObject.FindGameObjectsWithTag("Paintable");
        Debug.Log(paintable.Length);

        foreach (GameObject go in paintable)
        {
            foreach (Material m in go.GetComponent<MeshRenderer>().materials)
            {
                if (m.name.ToLower().Contains("cloud"))
                {
                    m.color = cloudColours.Evaluate(Random.Range(0f, 1f));
                }
                else if (m.name.ToLower().Contains("train") || m.name.ToLower().Contains("wheels"))
                {
                    m.color = trainColours.Evaluate(Random.Range(0f, 1f));
                }
                else if (!m.name.ToLower().Contains("glass"))
                {
                    m.color = buildingColours.Evaluate(Random.Range(0f, 1f));
                }
            }
        }
    }

}
