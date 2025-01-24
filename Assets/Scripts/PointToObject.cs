using System.Linq;
using UnityEngine;

public class PointToObject : MonoBehaviour
{
    public float maxDistance = 100f;
    public Color NearColor = Color.yellow;
    public Color FarColor = Color.red;
    public GameObject objectToLookAt;
    private GameObject childArrow;
    private Material[] childArrowMat = new Material[2];

    private void Start()
    {
        childArrow = transform.GetChild(0).gameObject;
        var mats = childArrow.GetComponentInChildren<Renderer>().materials;
        childArrowMat = mats;
    }

    private void Update()
    {
        this.transform.LookAt(objectToLookAt.transform);
        var distance = (objectToLookAt.transform.position - transform.position).magnitude;
        var lerp = distance / maxDistance;
        var onePlusLerp = 1 + lerp;
        childArrow.transform.localScale = new Vector3(onePlusLerp, onePlusLerp, onePlusLerp);

        foreach (var mat in childArrowMat)
            mat.color = Color.Lerp(NearColor, FarColor, Mathf.Clamp(lerp, 0f, 1f));
    }
}
