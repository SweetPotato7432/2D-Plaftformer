using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float ghostDistance = 0.5f; // 잔상을 생성할 최소 이동 거리
    public GameObject ghost;
    public bool makeGhost;

    private Vector3 lastGhostPosition; // 마지막 잔상이 생성된 위치

    void Start()
    {
        lastGhostPosition = transform.position; // 시작 시 현재 위치 저장
    }

    void FixedUpdate()
    {
        if (makeGhost)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastGhostPosition);

            if (distanceMoved >= ghostDistance)
            {
                CreateGhost();
                lastGhostPosition = transform.position; // 마지막 생성 위치 업데이트
            }
        }
    }

    void CreateGhost()
    {
        GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation);
        currentGhost.transform.localScale = transform.localScale;
        currentGhost.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        Destroy(currentGhost, 1f);
    }
}