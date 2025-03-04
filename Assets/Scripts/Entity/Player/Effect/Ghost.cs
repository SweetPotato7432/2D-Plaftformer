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
        currentGhost.transform.position = transform.position;
        currentGhost.transform.localScale = transform.localScale;
        currentGhost.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        currentGhost.GetComponent<SpriteRenderer>().flipX = GetComponent<SpriteRenderer>().flipX;
        currentGhost.SetActive(true);
        StartCoroutine(SetDisableGhost(currentGhost));
    }

    IEnumerator SetDisableGhost(GameObject ghost)
    {
        yield return new WaitForSeconds(.3f);
        GhostPoolManager.Instance.ReturnGhost(ghost);
        ghost.SetActive(false);
    }
}