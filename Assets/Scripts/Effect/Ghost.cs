using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour,IPoolable

{
    public Queue<GameObject> RootQueue { get; set; }

    public void ReturnPool()
    {

    }

    private void OnEnable()
    {
        StartCoroutine(SetDisableGhost(gameObject));
    }

    IEnumerator SetDisableGhost(GameObject ghost)
    {
        SpriteRenderer spriteRenderer = ghost.GetComponent<SpriteRenderer>();
        Tween tween = spriteRenderer.DOFade(0f, .3f);
        yield return tween.WaitForCompletion();
        PoolManager.ClaimReturnPool(gameObject);
        ghost.SetActive(false);
    }
}
