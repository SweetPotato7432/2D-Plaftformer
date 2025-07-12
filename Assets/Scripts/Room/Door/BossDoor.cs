using UnityEngine;
using UnityEngine.AddressableAssets;

public class BossDoor : MonoBehaviour
{
    public GameObject popup;

    public string sceneName;



    public void ChangeScene()
    {

        Debug.Log(MySceneManager.Instance == null ? "MySceneManager is NULL" : "MySceneManager is OK");

        MySceneManager.Instance.ChangeScene(sceneName);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 부딪혔을때, 아이템의 상세 설명이 나오고, 활성화 키를 누르면 상호작용.
        // 플레이어와 부딪힐때는 상세 팝업창 출현
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            popup.SetActive(true);

            PlayerInput.OnInteractionSceneChange -= ChangeScene;
            PlayerInput.OnInteractionSceneChange += ChangeScene;


            //switch (effectType)
            //{
            //    case DropItemInfo.EffectType.Heal:
            //        player.TakeHeal(stat.effectStatus);
            //        DropItemPoolManager.Instance.ReturnDropItem(this);
            //        break;
            //    case DropItemInfo.EffectType.Gold:
            //        break;
            //}
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            PlayerInput.OnInteractionSceneChange -= ChangeScene;
            popup.SetActive(false);
        }
    }
}
