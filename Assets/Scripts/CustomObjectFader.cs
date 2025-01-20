using System;
using UnityEngine;

public class CustomObjectFader : MonoBehaviour
{
    public bool DoFade = false;
    public float fadeSpeed = 20f, fadeAmount = 0.3f;
    private float originalOpacity;
    private Material[] mats;
    void Start()
    {
        mats = GetComponent<Renderer>().materials;
        foreach (Material mat in mats)
            originalOpacity = mat.color.a;
    }

    void Update()
    {
        foreach (Material mat in mats)
        {
            if (DoFade)
                FadeNow(mat);
            else
                ResetFade(mat);
        }
    }

    private void FadeNow(Material mat)
    {
        Color currentColor = mat.color;
        Color smoothColor = new(currentColor.r, currentColor.g, currentColor.b, Mathf.Lerp(currentColor.a, fadeAmount, fadeSpeed * Time.deltaTime));
        mat.color = smoothColor;
    }

    private void ResetFade(Material mat)
    {
        Color currentColor = mat.color;
        Color smoothColor = new(currentColor.r, currentColor.g, currentColor.b, Mathf.Lerp(currentColor.a, originalOpacity, fadeSpeed * Time.deltaTime));
        mat.color = smoothColor;
    }
}
