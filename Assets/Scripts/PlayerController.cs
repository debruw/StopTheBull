using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Touch touch;
    Vector2 firstPressPos;
    public float m_moveSpeed = 10;
    private Vector3 translation;
    public float Xspeed = 25f, limit;
    public GameObject cylinderPrefab; //assumed to be 1m x 1m x 2m default unity cylinder to make calculations easy
    public List<GameObject> SpawnPoints;
    public int PeopleCount;
    public GameObject World;

    private void Start()
    {
        GetComponentInChildren<Animator>().SetTrigger("HoldRope");
    }

    void Update()
    {
        World.transform.position -= Vector3.forward * m_moveSpeed * Time.deltaTime;

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            translation = new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * Xspeed;

            transform.Rotate(-translation);
            transform.localEulerAngles = new Vector3(0, Mathf.Clamp(transform.localEulerAngles.y, 90 - limit, 90 + limit), 0);
        }

#elif UNITY_IOS || UNITY_ANDROID

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                transform.localEulerAngles = new Vector3(0, Mathf.Clamp(transform.localEulerAngles.y + touch.deltaPosition.x * /*0.01f*/ Time.deltaTime * Xspeed, 90 - limit, 90 + limit), 0);
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("People"))
        {
            rand = Random.Range(0, SpawnPoints.Count);
            Instantiate(particlePuff, other.transform.position, Quaternion.identity);
            other.gameObject.transform.parent = SpawnPoints[rand].transform;            
            other.gameObject.transform.localPosition = Vector3.zero;
            other.gameObject.GetComponentInChildren<ParticleSystem>().Play();
            CreateCylinderBetweenPoints(transform.position, new Vector3(SpawnPoints[rand].transform.position.x, .8f, SpawnPoints[rand].transform.position.z), .08f, SpawnPoints[rand].transform);
            SpawnPoints.Remove(SpawnPoints[rand]);
            other.gameObject.GetComponent<Collider>().enabled = false;
            other.GetComponent<Animator>().SetTrigger("HoldRope");
            PeopleCount++;
        }
    }

    void CreateCylinderBetweenPoints(Vector3 start, Vector3 end, float width, Transform tr)
    {
        var offset = end - start;
        var scale = new Vector3(width, offset.magnitude / 2.0f, width);
        var position = start + (offset / 2.0f);

        var cylinder = Instantiate(cylinderPrefab, position, Quaternion.identity, tr);
        cylinder.transform.up = offset;
        cylinder.transform.localScale = scale;
    }
}
