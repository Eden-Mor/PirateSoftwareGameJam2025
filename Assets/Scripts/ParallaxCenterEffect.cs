using System;
using UnityEngine;

public class ParallaxCenterEffect : MonoBehaviour
{
    public Vector2[] parallaxScales;
    public float smoothing = 1f;
    public bool clampByScreenSize = false;

    private Vector3 screenCenter;

    void Start()
    {
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    void Update()
    {
        Vector3 currentMousePosition = clampByScreenSize ? ClampMousePosition(Input.mousePosition) : Input.mousePosition;
        Vector3 mouseDelta = currentMousePosition - screenCenter;

        for (int i = 0; i < parallaxScales.Length; i++)
        {
            Vector3 parallax = new(mouseDelta.x * parallaxScales[i].x, mouseDelta.y * parallaxScales[i].y, 0);
            transform.GetChild(i).localPosition = (smoothing * parallax) / 100f;
        }
    }

    Vector3 ClampMousePosition(Vector3 mousePosition)
    {
        float clampedX = Mathf.Clamp(mousePosition.x, 0, Screen.width);
        float clampedY = Mathf.Clamp(mousePosition.y, 0, Screen.height);
        return new Vector3(clampedX, clampedY, mousePosition.z);
    }
}