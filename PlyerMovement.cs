using UnityEngine;
using System;

public class PlyerMovement : MonoBehaviour
{
    N_CameraChange camerachange;
    N_GameManager GameManager;
    N_PlayerHealth playerHealth;

    public float speed = 6f;
    public Rigidbody playerRb;
    public Animator anim;

    Vector3 movement;
    int floorMask;
    float camRayLength = 100f;

    public bool TurningOn = true;

    void Awake()
    {
        playerHealth = GetComponent<N_PlayerHealth>();
        GameManager = GameObject.Find("GameManager").GetComponent<N_GameManager>();
        floorMask = LayerMask.GetMask("Floor");
        camerachange = GetComponent<N_CameraChange>();
    }

    void FixedUpdate()
    {
        if (GameManager.GameStop) return;

        float h = Input.GetAxisRaw("Horizontal"); // 좌:-1 , 우:1
        float v = Input.GetAxisRaw("Vertical"); //하:-1 , 상:1

        if(!camerachange.change) Move(h,v); 
        else if(camerachange.change) Move2(h, v);


        if (TurningOn) Turning();
        Animating(h, v);
    }


    void Animating(float h , float v)
    {
        bool walking = (h != 0f) || (v != 0f); // 이동 중인 상태라면 
        anim.SetBool("Isworking", walking); // 걷는 애니메이션 실행 
    }


    void Turning() // Player의 회전을 담당하는 함수
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition); // 마우스 위치에 대한 Ray

        RaycastHit floorHit;

        // 기준점이 되는 레이져 , 레이저가 닿았을때 정보를 저장할 오브젝트 , 레이져의 길이 , 어떤 레이어 마스크를 감지할 것인가 
        if (Physics.Raycast(camRay, out floorHit, camRayLength , floorMask)) 
        {
            Vector3 playerToMouse = floorHit.point - transform.position; // playerToMouse는 Player가 마우스가 있는 위치(floorMask가 있는 경우에만)의 방향
            playerToMouse.y = 0f;

            Quaternion newPos = Quaternion.LookRotation(playerToMouse); // 회전 값을 변경하기 위해 Quaternion 형태로 선언 , newPos는 현재 마우스 방향에 대한 회전값
            playerRb.MoveRotation(newPos); // Rigidbody.MoveRotation를 이용해 회전값을 변경

        }

    }

    void Move(float h , float v) // 3인칭 이동방법
    {
        // 전달 된 값에 따라서 vector의 방향을 정한다
         movement.Set(h, 0f, v);
        
        // 모든 방향에 대해 크기를 '1'로 만들고 (normalized)
        // verctor의 크기를 정한다
        movement = movement.normalized * speed * Time.deltaTime;

        // vector의 크기와 방향이 결정이 되었으니 , 다음 프레임에서 위치를 정한다.
        playerRb.MovePosition(transform.position + movement);
    }

    void Move2(float h, float v) // 1인칭 이동 방법
    {
        movement = (transform.forward * v + transform.right * h) * speed * Time.deltaTime;

        playerRb.MovePosition(transform.position + movement);
    }


}
