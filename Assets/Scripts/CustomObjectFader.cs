using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class CustomObjectFader : MonoBehaviour
{
    public float fadeSpeed = 20f, fadeAmount = 0.3f;
    private float originalOpacity;
    private Material[] mats;

    private bool? doFade = false;

    void Start()
    {
        mats = GetComponentInChildren<Renderer>().materials;
        foreach (Material mat in mats)
        {
            originalOpacity = mat.color.a;
        }
    }

    private IEnumerator FadeNow()
    {
        while (doFade == true)
        {
            foreach (Material mat in mats)
            {

                Color currentColor = mat.color;
                Color smoothColor = new(currentColor.r, currentColor.g, currentColor.b, Mathf.Lerp(currentColor.a, fadeAmount, fadeSpeed * Time.deltaTime));
                mat.color = smoothColor;
            }

            yield return 0;

            if (mats.First().color.a == originalOpacity)
                doFade = null;
        }
    }

    private IEnumerator ResetFade()
    {
        while (!(doFade == true))
        {
            foreach (Material mat in mats)
            {
                Color currentColor = mat.color;
                Color smoothColor = new(currentColor.r, currentColor.g, currentColor.b, Mathf.Lerp(currentColor.a, originalOpacity, fadeSpeed * Time.deltaTime));
                mat.color = smoothColor;
            }

            yield return 0;

            if (mats.First().color.a == originalOpacity)
                doFade = null;
        }
    }

    public bool IsFading()
        => doFade == true;

    public void Reveal()
    {
        //If its already false, do nothing
        if (!(doFade == true))
            return;

        doFade = false;
        StartCoroutine(ResetFade());
    }

    public void Fade()
    {
        //If its already true, do nothing
        if (doFade == true)
            return;

        doFade = true;
        StartCoroutine(FadeNow());
    }
}
