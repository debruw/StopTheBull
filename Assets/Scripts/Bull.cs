using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bull : MonoBehaviour
{
    public Animator m_animator;
    Rigidbody throwingBody;
    public void StandUpOver()
    {
        GameManager.Instance.isGameStarted = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("People"))
        {
            m_animator.SetTrigger("Attack");
            throwingBody = other.GetComponent<Rigidbody>();
            throwingBody.GetComponent<People>().CloseDust();
        }
        else if (other.CompareTag("Obstacle"))
        {
            m_animator.SetTrigger("Attack");
        }
    }

    public void ThrowBody()
    {
        if (throwingBody != null)
        {
            throwingBody.gameObject.GetComponent<People>().OpenRagdoll();            
            Destroy(throwingBody, 3);
            throwingBody = null;
        }
    }
}
