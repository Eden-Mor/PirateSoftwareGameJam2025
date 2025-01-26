using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HonkAndBrake : MonoBehaviour
{
    private VehiclePathController controller;
    public AudioSource honkSound;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<VehiclePathController>();
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Driveable Taxi")
        {
            if (!honkSound.isPlaying)
            {
                honkSound.Play();
            }

            controller.speed = 0f;
        }



    }
    private void OnTriggerExit(Collider other)
    {
        controller.speed = controller.maxSpeed;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
