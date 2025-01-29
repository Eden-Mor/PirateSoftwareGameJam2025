using UnityEngine;

[RequireComponent(typeof(Light))]
public class DayNightCycle : MonoBehaviour
{

    [SerializeField] private Color dayColor = Color.HSVToRGB(44 / 256, 16 / 256, 1);
    [SerializeField] private Color nightColor = Color.HSVToRGB(221 / 256, 1, 55 / 256);
    [SerializeField] private float daySpeed = 2f;
    [SerializeField] private float nightSpeed = 6f;

    private Light lightComp;

    private void Start()
    {
        lightComp = GetComponent<Light>();
    }

    void Update()
    {
        var isDay = transform.position.y >= -1.4;
        float speed = isDay ? daySpeed : nightSpeed;

        transform.RotateAround(Vector3.zero, Vector3.right, speed * Time.deltaTime);
        transform.LookAt(Vector3.zero);

        var eulerX = this.transform.rotation.eulerAngles.x + 90f;
        if (eulerX >= 360f)
            eulerX -= 360f;

        var scale = Mathf.Clamp01((eulerX / 180f * 2f) - 0.5f);

        lightComp.color = Color.Lerp(nightColor, dayColor, scale);
        lightComp.intensity = scale;
    }
}