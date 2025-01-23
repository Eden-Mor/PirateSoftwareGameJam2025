using System.Collections;
using UnityEngine;

public class CustomObjectFader : MonoBehaviour
{
    public float fadeSpeed = 20f, fadeAmount = 0.3f;
    private float originalOpacity;
    private Material[] mats;

    private bool isFaded = false;

    void Start()
    {
        mats = GetComponentInChildren<Renderer>().materials;
        originalOpacity = 1f;
        fadeAmount += 0.3f;
    }

    private IEnumerator FadeNow()
    {
        float alpha = 1f;
        Color smoothColor = Color.red;
        Color currentColor = Color.red;
        while (isFaded)
        {
            foreach (Material mat in mats)
            {
                currentColor = mat.GetColor("_Base_Color");
                alpha = Mathf.Lerp(currentColor.a, fadeAmount, fadeSpeed * 0.1f * Time.deltaTime);
                if (alpha >= 0.99f)
                    alpha = 1f;

                smoothColor = new(currentColor.r, currentColor.g, currentColor.b, alpha);
                mat.SetColor("_Base_Color", smoothColor);
            }

            yield return 0;

            if (smoothColor.a <= fadeAmount + 0.02f)
                break;
        }
    }

    private IEnumerator ResetFade()
    {
        float alpha = 100f;
        Color smoothColor = Color.red;
        Color currentColor = Color.red;
        while (!isFaded)
        {
            foreach (Material mat in mats)
            {
                currentColor = mat.GetColor("_Base_Color");
                alpha = Mathf.Lerp(currentColor.a, originalOpacity, fadeSpeed * 0.1f * Time.deltaTime);
                if (alpha >= 0.99f)
                    alpha = 1f;

                smoothColor = new(currentColor.r, currentColor.g, currentColor.b, alpha);
                mat.SetColor("_Base_Color", smoothColor);
            }

            yield return 0;

            if (alpha + 0.01f >= originalOpacity)
                break;
        }
    }

    public bool IsFaded()
        => isFaded;

    public void Reveal()
    {
        if (!isFaded)
            return;

        isFaded = false;

        StartCoroutine(ResetFade());
    }

    public void Fade()
    {
        if (isFaded)
            return;

        isFaded = true;

        StartCoroutine(FadeNow());
    }
}
