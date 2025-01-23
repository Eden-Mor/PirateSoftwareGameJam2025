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
    private bool isFaded = false;

    void Start()
    {
        mats = GetComponentInChildren<Renderer>().materials;
        originalOpacity = 100f;
    }

    private IEnumerator FadeNow()
    {
        while (doFade == true)
        {
            Color? smoothColor = null;

            foreach (Material mat in mats)
            {
                Color currentColor = mat.GetColor("_Base_Color");
                smoothColor = new(currentColor.r, currentColor.g, currentColor.b, Mathf.Lerp(currentColor.a, fadeAmount, fadeSpeed * Time.deltaTime));
                mat.SetColor("_Base_Color", smoothColor.Value);
            }


            yield return 0;

            if (!smoothColor.HasValue || smoothColor.Value.a == fadeAmount)
                doFade = null;
        }
    }

    private IEnumerator ResetFade()
    {
        while (!(doFade == true))
        {
            Color? smoothColor = null;

            foreach (Material mat in mats)
            {
                Color currentColor = mat.GetColor("_Base_Color");
                smoothColor = new(currentColor.r, currentColor.g, currentColor.b, Mathf.Lerp(currentColor.a, originalOpacity, fadeSpeed * Time.deltaTime));
                mat.SetColor("_Base_Color", smoothColor.Value);
            }

            yield return 0;

            if (!smoothColor.HasValue || smoothColor.Value.a == originalOpacity)
                doFade = null;
        }
    }

    public bool IsFaded()
        => isFaded;

    public void Reveal()
    {
        isFaded = false;

        //If its already false, do nothing
        if (!(doFade == true))
            return;

        doFade = false;
        StartCoroutine(ResetFade());
    }

    public void Fade()
    {
        isFaded = true;

        //If its already true, do nothing
        if (doFade == true)
            return;

        doFade = true;
        StartCoroutine(FadeNow());
    }
}
