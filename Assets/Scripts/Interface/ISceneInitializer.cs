using System.Collections;
using UnityEngine;

public interface ISceneInitializer
{
    IEnumerator InitializeScene(); // 초기화 완료 후 yield break
}
