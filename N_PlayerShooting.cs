using UnityEngine;

public class N_PlayerShooting : MonoBehaviour
{
    LineRenderer bulletLine;
    Light light;
    AudioSource audio;
    N_GameManager GameManager;

    public float bulletLength = 100f; // 총알 길이 
    int bulletDamage = 20; // 총알의 데미지
    int Layer;

    float bulletDelay = 0.1f;

    Ray ray;
    RaycastHit rayhit;

    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<N_GameManager>();
            
        bulletLine = GetComponent<LineRenderer>();
        light = GetComponent<Light>();
        audio = GetComponent<AudioSource>();

        bulletDelay = 0.1f;

        Layer = LayerMask.GetMask("Shootable");
    }

    void FixedUpdate()
    {
        if (GameManager.GameStop || GameManager.UICheck) return; // 게임이 정지 상태면 총 쏘는걸 실행하지 않는다 , 마우스가 ui위에 올라가 있다면 실행 안함

        if (bulletDelay > 0) // 총알 쐇을때 딜레이가 0이 아니라면 리턴 함
        {
            bulletDelay -= Time.deltaTime;
            bulletEnd();
            return;
        }

        if (Input.GetMouseButton(0) && !GameManager.UICheck)
        {
            shooting();
        }
    }

    public void shooting() // 총알 궤적 구현
    {
        if (bulletDelay > 0) return;

        bulletLine.enabled = true; // 라인렌더러를 활성화하여 총알 선이 눈에 보이도록 만든다
        light.enabled = true; // Light컴포넌트를 활성화 새서 총알 빛이 보이게 만든다
        audio.Play(); // 총 쏘는 소리를 활성화

        ray.origin = transform.position; // Ray의 발사 위치를 현재 오브젝트 위치에서 발사 하도록 만든다
        ray.direction = transform.forward; // Ray의 발사 방향을 현재 오브젝트의 z축으로 발사하도록 고정한다

        bulletLine.SetPosition(0, transform.position); // 총알 발사 위치를 현재 총 위치로 재조정한다 


        if (Physics.Raycast(ray , out rayhit , bulletLength , Layer)) // Ray를 발사 했을때 적 레이어에 충돌한다면
        {
            bulletLine.SetPosition(1, rayhit.point); // 총알의 길이를 딱 적이 맞은 위치까지만 구현을 한다

            N_EnemyHealth enemyHealth = rayhit.collider.GetComponent<N_EnemyHealth>(); // 총을 적이 맞았다면 스크립트 연결 
            if (enemyHealth != null) enemyHealth.TakeDamge(bulletDamage , rayhit.transform.position); // 만약 맞은 대상안에 스크립트가 있다면 대상의 hp를 깎는다.


        }

        else // Ray가 적에 맞지 않았다면 그냥 최대 사거리(100f)까지 선을 구현한다.
        {
            bulletLine.SetPosition(1 , ray.origin + ray.direction * bulletLength);

            // bulletLine.SetPosition(1 , ray.direction * bulletLength); 이 코드도 정상적으로 실행이 되던데 차이점이 뭔지 선생님에게 물어보기
              
        }

        bulletDelay = 0.1f; // 총알 딜레이 재설정
        
    }


    void bulletEnd() // 총알이 발사 후 비활성화 해야하는 것
    {
        bulletLine.enabled = false;
        light.enabled = false;
    }


    void OnDisable() // 만야 게임이 끝났는데 총알 궤적이 안 없어질 경우
    {
        bulletLine.enabled = false;
    }
}
