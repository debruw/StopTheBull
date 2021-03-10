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
        }
    }

    public void ThrowBody()
    {
        if (throwingBody != null)
        {
            throwingBody.AddForce(0, 10, 20, ForceMode.VelocityChange);
            throwingBody = null;
        }
    }
}
