using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Transactions;
using UnityEngine;

public class Lidar : MonoBehaviour
{
    public CapsuleCollider capColider;
    private VehiclePathController controller;
    private Collider sourceCollider;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<VehiclePathController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Building"))
        {
            controller.isBraking = true;
            sourceCollider = other;
        }

        if (other.gameObject.name == "Lidar")
        {
            controller.isBraking = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        controller.isBraking = false;
    }

    private void Update()
    {
        if (controller.isBraking && !sourceCollider)
        {
            controller.isBraking = false;
        }

    }
}
