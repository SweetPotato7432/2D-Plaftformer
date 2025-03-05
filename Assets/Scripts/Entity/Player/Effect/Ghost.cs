using DG.Tweening;
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
        Material whiteMaterial = new Material(Shader.Find("Custom/GhostWhiteShader"));
        GetComponent<SpriteRenderer>().material = whiteMaterial;
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
        SpriteRenderer spriteRenderer = currentGhost.GetComponent<SpriteRenderer>();
        currentGhost.transform.position = transform.position;
        currentGhost.transform.localScale = transform.localScale;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        spriteRenderer.sprite = GetComponent<SpriteRenderer>().sprite;
        spriteRenderer.flipX = GetComponent<SpriteRenderer>().flipX;
        currentGhost.SetActive(true);
        StartCoroutine(SetDisableGhost(currentGhost));
    }

    IEnumerator SetDisableGhost(GameObject ghost)
    {
        SpriteRenderer spriteRenderer = ghost.GetComponent<SpriteRenderer>();
        Tween tween = spriteRenderer.DOFade(0f, .3f);
        yield return tween.WaitForCompletion();
        //yield return new WaitForSeconds(.3f);
        GhostPoolManager.Instance.ReturnGhost(ghost);
        ghost.SetActive(false);
    }
}