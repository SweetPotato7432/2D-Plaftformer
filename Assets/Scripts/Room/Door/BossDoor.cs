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
        // �÷��̾�� �ε�������, �������� �� ������ ������, Ȱ��ȭ Ű�� ������ ��ȣ�ۿ�.
        // �÷��̾�� �ε������� �� �˾�â ����
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
