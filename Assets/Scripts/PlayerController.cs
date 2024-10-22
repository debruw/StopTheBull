﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TapticPlugin;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int NeededPeopleCount;
    int currentPeopleCount;

    Touch touch;
    Vector2 firstPressPos;
    public float m_moveSpeed = 10;
    float DefaultMoveSpeed;
    float moveStep;
    private Vector3 translation, translation2;
    public float Xspeed = 25f, limit;
    public List<GameObject> SpawnPoints;
    public List<GameObject> ObiRopes;
    public GameObject[] RopeParents;
    public GameObject World;
    public ParticleSystem DustParticle1, DustParticle2;
    public List<People> CollectedPeoples;
    public Animator m_animator;
    public ParticleSystem[] RoseParticles;
    float roseParticleTimer, RPTimerRand;

    private void Start()
    {
        DefaultMoveSpeed = m_moveSpeed;
        moveStep = (m_moveSpeed / 2) / (float)NeededPeopleCount;
        RPTimerRand = Random.Range(5, 15);
        m_animator.SetInteger("Idle", 1);
    }

    public GameObject SpawnPointRotater;
    bool isMovementReleased;
    void Update()
    {
        if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver)
        {
            return;
        }
        if (!DustParticle1.isPlaying)
        {
            DustParticle1.Play();
            DustParticle2.Play();
        }

        roseParticleTimer += Time.deltaTime;
        if (roseParticleTimer >= RPTimerRand)
        {
            roseParticleTimer = 0;
            RPTimerRand = Random.Range(3, 6);
            foreach (ParticleSystem item in RoseParticles)
            {
                item.Play();
                SoundManager.Instance.playSound(SoundManager.GameSounds.Pop);
            }

        }


        World.transform.position += Vector3.forward * m_moveSpeed * Time.deltaTime;
        GameManager.Instance.UpdateLevelStateImage();

#if UNITY_EDITOR

        if (Input.GetMouseButton(0))
        {
            if (GameManager.Instance.currentLevel == 1 && GameManager.Instance.tutorialCanvas.activeSelf)
            {
                GameManager.Instance.tutorialCanvas.SetActive(false);
                GameManager.Instance.tutorialCanvas2.SetActive(true);
                Destroy(GameManager.Instance.tutorialCanvas2, 3);
            }
            translation = new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * Xspeed;

            transform.Rotate(-translation);
            transform.localEulerAngles = new Vector3(0, Mathf.Clamp(transform.localEulerAngles.y, 90 - limit, 90 + limit), 0);

            translation2 = new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * (Xspeed / 2);

            SpawnPointRotater.transform.Rotate(-translation2);
            SpawnPointRotater.transform.localEulerAngles = new Vector3(0, Mathf.Clamp(SpawnPointRotater.transform.localEulerAngles.y, 90 - limit, 90 + limit), 0);
        }

#elif UNITY_IOS || UNITY_ANDROID

        if (Input.touchCount > 0)
        {
            if (GameManager.Instance.currentLevel == 1 && GameManager.Instance.tutorialCanvas.activeSelf)
            {
                GameManager.Instance.tutorialCanvas.SetActive(false);
                GameManager.Instance.tutorialCanvas2.SetActive(true);
                Destroy(GameManager.Instance.tutorialCanvas2, 3);
            }
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                isMovementReleased = false;
                transform.localEulerAngles = new Vector3(0, Mathf.Clamp(transform.localEulerAngles.y + touch.deltaPosition.x * 0.05f * Time.deltaTime * -Xspeed, 90 - limit, 90 + limit), 0);
                SpawnPointRotater.transform.localEulerAngles = new Vector3(0, Mathf.Clamp(SpawnPointRotater.transform.localEulerAngles.y + touch.deltaPosition.x * 0.05f * Time.deltaTime * (-Xspeed / 2), 90 - limit, 90 + limit), 0);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isMovementReleased = true;
            }
            else if (touch.phase == TouchPhase.Began)
            {
                //save began touch 2d point
                firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
        }

#endif
    }

    int rand;
    public GameObject particlePuff;
    ParticleSystem[] Dusts;
    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver)
        {
            return;
        }
        if (other.CompareTag("People"))
        {
            if (!other.GetComponent<People>().isTaken)
            {
                rand = Random.Range(0, SpawnPoints.Count);
                Instantiate(particlePuff, other.transform.position, Quaternion.identity);
                Instantiate(particlePuff, SpawnPoints[rand].transform.position, Quaternion.identity);

                other.gameObject.transform.parent = SpawnPoints[rand].transform;
                ObiRopes[rand].GetComponent<MeshRenderer>().enabled = true;

                other.GetComponent<People>().HoldTheRope(ObiRopes[rand]);
                ObiRopes.Remove(ObiRopes[rand]
                    );
                CollectedPeoples.Add(other.GetComponent<People>());

                SpawnPoints.Remove(SpawnPoints[rand]);
                currentPeopleCount++;
                GameManager.Instance.UpdateCurrentPeopleCount(currentPeopleCount);
                if (PlayerPrefs.GetInt("VIBRATION") == 1)
                    TapticManager.Impact(ImpactFeedback.Light);
                SoundManager.Instance.playSound(SoundManager.GameSounds.Pick);

                StartCoroutine(ScaleSpeed(m_moveSpeed, (m_moveSpeed - moveStep), .5f));
            }
        }
        else if (other.CompareTag("Obstacle"))
        {
            OnFinishLine();
            GameManager.Instance.FinishTransform.gameObject.SetActive(false);
            GameManager.Instance.BullAnimator.gameObject.transform.DOMoveZ(
                GameManager.Instance.BullAnimator.gameObject.transform.position.z + 100, 10);
            StartCoroutine(GameManager.Instance.WaitAndGameLose());
        }
    }

    public void AddRopeBack(GameObject rand)
    {
        ObiRopes.Add(rand);
    }

    public void LosePeople()
    {
        currentPeopleCount--;
        GameManager.Instance.UpdateCurrentPeopleCount(currentPeopleCount);

        StartCoroutine(ScaleSpeed(m_moveSpeed, (m_moveSpeed + moveStep), .1f));
    }

    int randm;
    IEnumerator ScaleSpeed(float start, float end, float time)
    {
        float lastTime = Time.realtimeSinceStartup;
        float timer = 0.0f;

        while (timer < time)
        {
            m_moveSpeed = Mathf.Lerp(start, end, timer / time);
            timer += (Time.realtimeSinceStartup - lastTime);
            lastTime = Time.realtimeSinceStartup;
            yield return null;
        }
        m_moveSpeed = end;

        if (m_moveSpeed <= DefaultMoveSpeed / 4)
        {
            GameManager.Instance.BullAnimator.SetTrigger("Walk");
        }

        if (m_moveSpeed <= DefaultMoveSpeed / 2 || currentPeopleCount == NeededPeopleCount)
        {
            foreach (GameObject item in RopeParents)
            {
                item.SetActive(false);
            }
            GameManager.Instance.BullAnimator.SetTrigger("Die");
            m_animator.SetTrigger("Cheer1");
            DustParticle1.Stop();
            DustParticle2.Stop();
            foreach (People item in CollectedPeoples)
            {
                item.CloseDust();
                randm = Random.Range(1, 3);
                item.m_animator.SetTrigger("Cheer" + randm);
            }
            StartCoroutine(GameManager.Instance.WaitAndGameWin());
        }
    }

    public void OnFinishLine()
    {

        m_moveSpeed = 0;
        m_animator.SetTrigger("Fall");
        DustParticle1.Stop();
        DustParticle2.Stop();
        foreach (People item in CollectedPeoples)
        {
            item.m_animator.SetTrigger("Fall");
            item.CloseDust();
        }
        foreach (GameObject item in RopeParents)
        {
            item.SetActive(false);
        }
    }
}
