using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float ghostDistance = 0.5f; // 잔상을 생성할 최소 이동 거리

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
        GameObject currentGhost = GhostPoolManager.Instance.GetGhost();
        currentGhost.transform.position = transform.position;
        currentGhost.transform.localScale = transform.localScale;
        currentGhost.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        currentGhost.SetActive(true);
        StartCoroutine(SetDisableGhost(currentGhost));
    }

    IEnumerator SetDisableGhost(GameObject ghost)
    {
        yield return new WaitForSeconds(1f);
        GhostPoolManager.Instance.ReturnGhost(ghost);
        ghost.SetActive(false);
    }
}