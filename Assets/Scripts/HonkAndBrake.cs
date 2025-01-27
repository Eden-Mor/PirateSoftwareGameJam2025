using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HonkAndBrake : MonoBehaviour
{
    private VehiclePathController controller;
    public ReviewManager rm;
    public AudioSource honkSound;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<VehiclePathController>();
        rm = GetComponent<ReviewManager>();

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Driveable Taxi" && controller.isPathing)
        {
            if (!honkSound.isPlaying)
            {
                honkSound.Play();
                EventManager.Player.OnCarHonked.Get().Invoke();
            }

            controller.speed = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        controller.speed = controller.maxSpeed;
    }
}
