using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float ghostDistance = 0.5f; // �ܻ��� ������ �ּ� �̵� �Ÿ�
    public GameObject ghost;
    public bool makeGhost;

    private Vector3 lastGhostPosition; // ������ �ܻ��� ������ ��ġ

    void Start()
    {
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
        GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation);
        currentGhost.transform.localScale = transform.localScale;
        currentGhost.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        Destroy(currentGhost, 1f);
    }
}