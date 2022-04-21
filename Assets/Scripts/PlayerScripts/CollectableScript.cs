using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableScript : MonoBehaviour
{

    private void Start()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MovementComponent mc = other.GetComponent<MovementComponent>();
            mc.AddPoint();
            gameObject.SetActive(false);
        }
    }
}
