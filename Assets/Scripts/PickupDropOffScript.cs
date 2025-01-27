using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickupDropOffScript : MonoBehaviour
{
    private Vector3 relativePos = Vector3.zero;
    private List<GameObject> waitingPeople = new();
    private Transform parent;

    //False for drop off
    public bool pickupPoint = true;
    public GameObject[] customers;
    public bool isTriggered = false;
    public World worldScript;

    [SerializeField] private AudioSource carDoorOpen;
    [SerializeField] private AudioSource carDoorClose;

    private void Start()
    {
        parent = transform.parent;
        SetupFirstPickup();
    }

    private void SetupFirstPickup()
    {
        var newWaitingPerson = GetNewRandomPersonAtLocation(transform);
        waitingPeople.Add(newWaitingPerson);
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag != "Player" || collision.attachedRigidbody.velocity.magnitude >= 1f || isTriggered)
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
        carDoorClose.Play();
        yield return new WaitForSeconds(1);
        EventManager.Player.OnReviewStop.Get().Invoke();
        var waitingPerson = waitingPeople.Last();
        waitingPerson.transform.position = this.transform.position - relativePos;
        waitingPerson.gameObject.SetActive(true);


        //Reset
        //Pick new point
        this.transform.position = GetRandomPointOnMap().position;
        pickupPoint = true;
        var newWaitingPerson = GetNewRandomPersonAtLocation(transform);
        waitingPeople.Add(newWaitingPerson);

        yield return new WaitForSeconds(1);

        isTriggered = false;
    }

    private IEnumerator PickupPerson()
    {
        if (waitingPeople.Count >= 2)
        {
            var previousWaitingPerson = waitingPeople.First();
            waitingPeople.Remove(previousWaitingPerson);
            Destroy(previousWaitingPerson.gameObject);
        }
        
        //Play pickup anim

        carDoorOpen.Play();
        yield return new WaitForSeconds(1);

        var waitingPerson = waitingPeople.First();
        relativePos = waitingPerson.transform.InverseTransformPoint(transform.position);
        waitingPerson.SetActive(false);


        EventManager.Player.OnReviewStart.Get().Invoke();
        pickupPoint = false;

        this.transform.position = GetRandomPointOnMap().position;

        yield return new WaitForSeconds(1);

        isTriggered = false;
    }

    private GameObject GetNewRandomPersonAtLocation(Transform pos)
    {
        var person = Instantiate(customers.ElementAt(UnityEngine.Random.Range(0, customers.Length)), pos.position, new Quaternion(0f, 90f, 0f, 0f), parent);
        person.transform.localScale = new Vector3(2f,2f,2f);
        return person;
    }

    private Transform GetRandomPointOnMap()
    {
        var tile = worldScript.GetRandomRoadTile();
        var points = tile.GetTilePickupPoints();
        if (points == null || points.Length <= 0)
        {
            Debug.LogError("NO PICKUP POINTS, HOW DID THIS HAPPEN?");
            return this.transform;
        }

        var chosenPoint = points[UnityEngine.Random.Range(0, points.Length)];
        return chosenPoint;
    }
}
