using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Lidar : MonoBehaviour
{

    private VehiclePathController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = transform.parent.gameObject.GetComponent<VehiclePathController>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Building"))
        {
            controller.isBraking = true;

        }

      

    }

    private void OnTriggerExit(Collider other)
    {
        controller.isBraking = false;
    }
}
