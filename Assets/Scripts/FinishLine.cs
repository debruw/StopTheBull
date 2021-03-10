using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bull"))
        {
            GameManager.Instance.BullAnimator.SetTrigger("Attack");
            GameManager.Instance.BullAnimator.gameObject.transform.DOMoveZ(
                GameManager.Instance.BullAnimator.gameObject.transform.position.z + 100, 5);
            GameManager.Instance.playerController.OnFinishLine();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bull"))
        {
            StartCoroutine(GameManager.Instance.WaitAndGameLose());
        }
    }
}
