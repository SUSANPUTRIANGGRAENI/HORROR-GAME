using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleCollision : MonoBehaviour
{

    private CharacterController controller;
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Menabrak: " + hit.gameObject.name);

        if (hit.gameObject.CompareTag("Musuh"))
        {
            Debug.Log("Menabrak musuh!");
        }
    }

}
