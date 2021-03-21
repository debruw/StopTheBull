using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class People : MonoBehaviour
{
    public ParticleSystem[] Dusts;
    public GameObject[] Trails;
    public Animator m_animator;
    public bool isTaken;
    public GameObject myRope;
    public GameObject StickMen;
    Rigidbody[] ragdollRigies;
    Collider[] ragdollcollies;

    private void Start()
    {
        m_animator.SetInteger("Idle", Random.Range(0, 4));
        ragdollcollies = StickMen.GetComponentsInChildren<Collider>();
        ragdollRigies = StickMen.GetComponentsInChildren<Rigidbody>();
    }

    public void OpenRagdoll()
    {
        m_animator.enabled = false;
        foreach (Collider item in ragdollcollies)
        {
            item.enabled = true;
        }
        foreach (Rigidbody item in ragdollRigies)
        {
            item.isKinematic = false;
            item.useGravity = true; 
            item.AddForce(new Vector3(0, 10, 40), ForceMode.Impulse);
        }
    }

    public void HoldTheRope(GameObject rand)
    {
        isTaken = true;
        myRope = rand;
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
            transform.parent = null;
            GameManager.Instance.playerController.AddRopeBack(myRope);
            if (myRope != null)
            {
                myRope.GetComponent<MeshRenderer>().enabled = false;
            }
            m_animator.SetTrigger("Fall");
            GameManager.Instance.playerController.CollectedPeoples.Remove(this);
            GameManager.Instance.playerController.LosePeople();
            Destroy(gameObject, 3);
        }
    }

}
