using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeringPeople : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Animator>().SetTrigger("Cheer" + Random.Range(0, 2));
    }
}
