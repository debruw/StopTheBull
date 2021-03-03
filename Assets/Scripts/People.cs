using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class People : MonoBehaviour
{
    public ParticleSystem[] Dusts;
    public Animator m_animator;

    public void HoldTheRope()
    {
        transform.localPosition = Vector3.zero;
        foreach (ParticleSystem item in Dusts)
        {
            item.Play();
        }

        GetComponent<Collider>().enabled = false;
        m_animator.SetTrigger("HoldRope");
    }

    public void CloseDust()
    {
        foreach (ParticleSystem item in Dusts)
        {
            item.Stop();
        }
    }
}
