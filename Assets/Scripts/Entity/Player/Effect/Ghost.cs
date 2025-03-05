using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float ghostDistance = 0.5f; // �ܻ��� ������ �ּ� �̵� �Ÿ�

    public bool makeGhost;

    private Vector3 lastGhostPosition; // ������ �ܻ��� ������ ��ġ

    void Start()
    {
        Material whiteMaterial = new Material(Shader.Find("Custom/GhostWhiteShader"));
        GetComponent<SpriteRenderer>().material = whiteMaterial;
        lastGhostPosition = transform.position; // ���� �� ���� ��ġ ����
    }

    void FixedUpdate()
    {
        if (makeGhost)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastGhostPosition);

            if (distanceMoved >= ghostDistance)
            {
                CreateGhost();
                lastGhostPosition = transform.position; // ������ ���� ��ġ ������Ʈ
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