using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{

    // 최적화 하는 방향성
    // 1. 메모리를 사용해 실행을 빠르게
    // 2. 실행을 느리게 해서 메모리를 아끼기
    public Queue<GameObject> RootQueue { get; set; }

    // 풀로 돌아갈때 실행할 코드
    public void ReturnPool();

}
