using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarCameraScript : MonoBehaviour
{
    public Transform car;

    [Header("Camera Settings")]
    public float distance = 6.4f;
    public float height = 1.4f;
    public float rotationDamping = 3.0f;
    public float heightDamping = 2.0f;
    public float zoomRatio = 0.5f;
    public float defaultFOV = 60f;

    [Space(10)]
    [Header("Fade Options")]
    public int raycastMaxHits = 10;
    public Transform raycastFadeLocation;


    private List<CustomObjectFader> faders = new();
    private Vector3 rotationVector;

    void LateUpdate()
    {
        float wantedAngle = rotationVector.y;
        float wantedHeight = car.position.y + height;
        float myAngle = transform.eulerAngles.y;
        float myHeight = transform.position.y;

        myAngle = Mathf.LerpAngle(myAngle, wantedAngle, rotationDamping * Time.deltaTime);
        myHeight = Mathf.Lerp(myHeight, wantedHeight, heightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0, myAngle, 0);
        transform.position = car.position;
        transform.position -= currentRotation * Vector3.forward * distance;
        Vector3 temp = transform.position; //temporary variable so Unity doesn't complain
        temp.y = myHeight;
        transform.position = temp;
        transform.LookAt(car);
    }

    void FixedUpdate()
    {
        Vector3 temp = rotationVector;
        temp.y = car.eulerAngles.y;
        rotationVector = temp;

        float acc = car.GetComponent<Rigidbody>().velocity.magnitude;
        GetComponent<Camera>().fieldOfView = defaultFOV + acc * zoomRatio * Time.deltaTime;  //he removed * Time.deltaTime but it works better if you leave it like this.

        FadeHandler();
    }

    private void FadeHandler()
    {
        Vector3 dir = raycastFadeLocation.transform.position - transform.position;
        Ray ray = new(transform.position, dir);

        var hits = new RaycastHit[raycastMaxHits];
        if (Physics.RaycastNonAlloc(ray, hits) == 0)
            return;

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider == null)
                continue;

            if (hit.collider.gameObject == car.gameObject)
            {
                if (i != 1)
                    continue;

                for (int j = faders.Count - 1; j >= 0; j--)
                {
                    CustomObjectFader fadeIn = faders[j];
                    fadeIn.Reveal();
                    faders.RemoveAt(j);
                }
            }
            else 
            {
                var fader = hit.collider.gameObject.GetComponentInChildren<CustomObjectFader>();
                if (fader == null || fader.IsFading())
                    continue;

                faders.Add(fader);
                fader.Fade();
            }
        }
    }
}