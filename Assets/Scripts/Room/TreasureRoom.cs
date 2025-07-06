using System.Collections.Generic;
using UnityEngine;

public class TreasureRoom : Room
{
    [System.Serializable]
    public struct ItemSpawnInfo
    {
        public Vector3 localSpawnpoint;
        [HideInInspector]
        public Vector3 globalSpawnPoint;
    }

    [Header("ItemSpawn")]
    [SerializeField]
    ItemSpawnInfo ItemSpawn;

    public override void Awake()
    {
        base.Awake();

        ItemSpawn.globalSpawnPoint = ItemSpawn.localSpawnpoint + transform.position;

        PassiveItem passiveItem = PassiveItemPoolManager.Instance.GetPassiveItem();

        passiveItem.transform.position = ItemSpawn.globalSpawnPoint;

        // 아이템 개수에 맞게 수정
        int len = GameManager.Instance.PassiveItemLength();

        int rarity = passiveItem.CalculateRarityFromPassiveItem();

        List<int> candidateIds = GameManager.Instance.passiveItemRarityGroups[rarity];
        int selectedItemId = candidateIds[Random.Range(0, candidateIds.Count)];

        Debug.Log($"r : {rarity}, id : {selectedItemId}");

        passiveItem.InitalizePassiveItem(selectedItemId);
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        {
            Gizmos.color = Color.yellow;
            float size = .3f;

            Vector3 globalSpawnpointPos = (Application.isPlaying) ? ItemSpawn.globalSpawnPoint : ItemSpawn.localSpawnpoint + transform.position;
            Gizmos.DrawLine(globalSpawnpointPos - Vector3.up * size, globalSpawnpointPos + Vector3.up * size);
            Gizmos.DrawLine(globalSpawnpointPos - Vector3.left * size, globalSpawnpointPos + Vector3.left * size);
        }
    }
}
