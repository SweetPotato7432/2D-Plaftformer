using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    public LayerMask passengerMask;
    ////단순 이동
    //public Vector3 move;

    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    public float speed;
    // 플랫폼 움직임 순환 여부
    public bool cyclic;
    // 플랫폼 정지 후 대기
    public float waitTime;
    [Range(0,2)]
    public float easeAmount;

    

    int fromWaypointIndex;
    float percentBetweenWaypoints; // 0~1
    float nextMoveTime;

    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        

        base.Start();

        //// 플랫폼 크기에 비례해 레이 개수가 적다면 캐릭터가 밑으로 빠지거나 비정상적으로 행동하는 경우가 발생한다.
        //// 따라서 플랫폼 크기에 비례해 레이개수를 동적으로 조절하여 이상현상을 최소화 시킨다.(최소 4개, 최대 15개)
        //// 플랫폼 크기에 따라 레이 개수 동적으로 조절

        //// *중요!) 15개도 모자라는 경우가 생길수도 있으므로, 발판의 크기에 따라 최대치를 없애는 방향도 생각해볼것
        //verticalRayCount = Mathf.Clamp((int)(collider.bounds.size.x * 5), 4, 15);
        //horizontalRayCount = Mathf.Clamp((int)(collider.bounds.size.y * 5), 4, 15);
        //// 레이의 개수를 다시 지정했으니 다시 한번 레이의 간격 설정
        //CalculateRaySpacing();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for(int i = 0; i<localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateRaycastOrigins();

        //Vector3 velocity = move * Time.fixedDeltaTime;

        if (localWaypoints.Length > 0)
        {
            Vector3 velocity = CalculatePlatformMovement();

            CalculatePassengersMovement(velocity);

            MovePassengers(true);
            transform.Translate(velocity);
            MovePassengers(false);
        }


    }

    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    Vector3 CalculatePlatformMovement()
    {
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;

        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.fixedDeltaTime * speed/distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWayPoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex],globalWaypoints[toWaypointIndex], easedPercentBetweenWayPoints);

        if(percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            if (!cyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    // 배열을 모두 돌았으면 배열을 뒤집어서 다시 순환
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time+waitTime;
            
        }

        return newPos-transform.position;
    }

    void MovePassengers(bool beforeMovePlatform)
    {
        foreach (PassengerMovement passenger in passengerMovement)
        {
            if (!passengerDictionary.ContainsKey(passenger.transform))    
            {
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
            }
            if(passenger.moveBeforePlatform == beforeMovePlatform)
            {
                Vector3 passengerVelocity = passenger.velocity;
                passengerVelocity.y += (passenger.standingOnPlatform ? -skinWidth * 2 : 0); // 추가 보정
                passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }

    // 플랫폼에 타있는 승객 이동정보 계산
    void CalculatePassengersMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        // 수직이동 플랫폼
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;


            for (int i = 0; i < verticalRayCount; i++)
            {
                // 캐릭터가 점프 or 낙하 중일때 raycast가 시작될 부분 설정
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);


                if (hit&&hit.distance !=0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);

                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        //hit.transform.Translate(new Vector3(pushX, pushY));
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY==1, true));

                    }

                }

            }
        }

        // 수평 이동 플랫폼
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;


            for (int i = 0; i < horizontalRayCount; i++)
            {
                // 캐릭터가 점프 or 낙하 중일때 raycast가 시작될 부분 설정
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);


                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);

                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = -skinWidth;

                        //hit.transform.Translate(new Vector3(pushX, pushY));

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }

                }
            }
        }

        // 수평으로 이동하는 플랫폼과 아래로 내려가는 플랫폼에 플레이어가 존재할때
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = skinWidth * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.red);


                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        pushY -= (hit.distance - skinWidth);


                        //hit.transform.Translate(new Vector3(pushX, pushY));

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));

                    }
                }
            }
        }

    }

    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform,Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }

    private void OnDrawGizmos()
    {
        if(localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = .3f;

            for(int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos-Vector3.up*size, globalWaypointPos+Vector3.up*size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);

            }
        }
    }
}
