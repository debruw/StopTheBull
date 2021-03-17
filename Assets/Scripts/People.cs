using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class People : MonoBehaviour
{
    public ParticleSystem[] Dusts;
    public GameObject[] Trails;
    public Animator m_animator;
    public bool isTaken;
    int myRopeNumber;

    private void Start()
    {
        m_animator.SetInteger("Idle", Random.Range(0, 4));
    }

    public void HoldTheRope(int rand)
    {
        isTaken = true;
        myRopeNumber = rand;
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
        foreach (ParticleSystem item in Dusts)
        {
            item.Play();
        }
        foreach (GameObject item in Trails)
        {
            item.SetActive(true);
        }
        //GetComponent<Collider>().enabled = false;
        m_animator.SetTrigger("HoldRope");
    }

    public void CloseDust()
    {
        foreach (ParticleSystem item in Dusts)
        {
            item.Stop();
        }
        foreach (GameObject item in Trails)
        {
            item.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log(myRopeNumber);
            transform.parent = null;
            GameManager.Instance.playerController.Ropes[myRopeNumber].SetActive(false);
            m_animator.SetTrigger("Fall");
            GameManager.Instance.playerController.CollectedPeoples.Remove(this);
            GameManager.Instance.playerController.LosePeople();
            GameManager.Instance.playerController.AddRopeBack(myRopeNumber);
            Destroy(gameObject, 3);
        }
    }

}
