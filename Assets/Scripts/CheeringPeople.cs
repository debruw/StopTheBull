using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeringPeople : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Animator>().SetInteger("Cheer", Random.Range(0, 3));
    }
}
