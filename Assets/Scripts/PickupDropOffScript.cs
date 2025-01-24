using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupDropOffScript : MonoBehaviour
{
    //False for drop off
    public bool pickupPoint = true;
    public GameObject waitingPerson;
    private Vector3 relativePos = Vector3.zero;
    public bool isTriggered = false;

    private void OnTriggerStay(Collider collision)
    {
        if (collision.attachedRigidbody.velocity.magnitude >= 1f || isTriggered)
            return;

        isTriggered = true;

        if (pickupPoint)
            StartCoroutine(PickupPerson());
        else
            StartCoroutine(DropOffPerson());
    }

    private IEnumerator DropOffPerson()
    {

        //Play dropoff anim
        yield return new WaitForSeconds(1);
        //EventManager.Player.OnReviewStop.Get().Invoke();
        waitingPerson.transform.position = this.transform.position - relativePos;
        waitingPerson.gameObject.SetActive(true);


        //Reset
        //Pick new point
        this.transform.position = this.transform.position + new Vector3(0f,0f,10f);
        pickupPoint = true;
        yield return new WaitForSeconds(1);

        isTriggered = false;

        //PICK A NEW waitingPerson that is different from current waitingPerson

    }

    private IEnumerator PickupPerson()
    {
        //Play pickup anim

        yield return new WaitForSeconds(2);
        relativePos = waitingPerson.transform.InverseTransformPoint(transform.position);
        waitingPerson.SetActive(false);


        //EventManager.Player.OnReviewStart.Get().Invoke();
        pickupPoint = false;

        //THIS IS WHERE YOU WOULD MOVE THIS OBJECT SOMEWHERE ELSE
        this.transform.position = this.transform.position + new Vector3(0f,0f,10f);
        yield return new WaitForSeconds(2);

        isTriggered = false;
    }
}
