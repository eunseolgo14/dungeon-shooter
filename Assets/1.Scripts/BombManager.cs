using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Bomb_State
{ 
    Idle,
    CountDown,
    Explode,
    Destroy
}

public class BombManager : MonoBehaviour
{
    //------ (1) 생성 후 포물 곡선을 그리며 이동하는 것 관련 변수 

    //날아가야할 랜덤 목표 위치
    public Transform m_Target;
    //생성된 폭탄(this)의 시작 위치
    private Vector3 m_StartPosition;

    //날아갈 속도
    public float m_Speed = 10;
    //포물 곡선의 높이
    public float m_HeightArc = 1;
    //목표 지점 도착 확인용 변수
    private bool m_IsArrive;

    //------ (2) 플레이어와의 거리 측정, 상태 애니 전환용 변수
    
    //거리를 감지할 플레이어 동적 할당용 변수 
    public GameObject m_RefHero;
    //폭발 카운트다운을 시작할 거리
    float m_ExplosionLimit = 3.0f;
    //폭탄과 플레이어 사이의 거리
    float m_PlayerDist = 0.0f;
    //카운트다운 산태 전환 여부 함수
    //bool m_IsCountDown = false;
    //제한시간
    float m_CountDown = 0.6f;
    //제한 시간 출력용 텍스트 변수
    public Text m_CountTxt;
    //플레이어가 폭탄 데미지를 입을 거리
    float m_CiriticalDist = 3.2f;

    //애니메이션 연출 위한 애니메이러 컴포넌트
    Animator m_Anim = null;
    Bomb_State m_BombState = Bomb_State.Idle;

    void Start()
    {
        m_StartPosition = transform.position;
        m_RefHero = GameObject.FindGameObjectWithTag("Hero");
        m_Anim = GetComponent<Animator>();

        if (m_RefHero == null)
        {
            Debug.Log("히어로를 찾지 못했습니다");
        }

        m_IsArrive = false;
        m_BombState = Bomb_State.Idle;
    }

    void Update()
    {
        //도착했다면 업데이트 함수 실행
        if (m_IsArrive == true)
        {
            BombUpdate();
        }

        //도착 하지 않았다면 포물선으로 날라가는 연출 시작
        else  
        {
            FlyParabola();
        }
        //이걸 업데이트에서 그떄그떄 한 게 아니라 정보값이 달랐던 것
        m_PlayerDist = (this.transform.position - m_RefHero.transform.position).magnitude;
        //print(m_PlayerDist);
    }

    //포물선 그리는 연출 함수
    void FlyParabola()
    {
        if (m_Target == null)
        {
            return;
        }

        //목표지점까지의 x축 상의 거기 구하기
        float m_startX = m_StartPosition.x;
        float m_targetX = m_Target.position.x;
        float m_distance = m_targetX - m_startX;

        float m_nestStepX = Mathf.MoveTowards(transform.position.x, m_targetX, m_Speed * Time.deltaTime);
        float baseY = Mathf.Lerp(m_StartPosition.y, m_Target.position.y, (m_nestStepX - m_startX) / m_distance);

        float arc = m_HeightArc * (m_nestStepX - m_startX) * (m_nestStepX - m_targetX) / (-0.25f * m_distance * m_distance);

        Vector3 nextPosition = new Vector3(m_nestStepX, baseY + arc, transform.position.z);

        transform.position = nextPosition;

        if (nextPosition == m_Target.position)
            m_IsArrive = true;
    }

    //플레이어와의 거리를 측정하는 함수
    void BombUpdate()
    {
        if (m_RefHero == null)
        {
            return;
        }

        switch (m_BombState)
        {
            case Bomb_State.Idle:

                //폭탄 자신과 플레이어 사이의 거리를 계산한다.
                //m_PlayerDist = (this.transform.position - m_RefHero.transform.position).magnitude;// Vector3.Distance(this.transform.position, m_RefHero.transform.position);

                //폭탄 폭파 범위 안에 들어왔습니다.
                if (m_PlayerDist < m_ExplosionLimit)
                {
                    m_BombState = Bomb_State.CountDown;
                    //빙글빙글 돌아가는 애니메이션 전환
                    m_Anim.SetTrigger("IsLit");
                    //카운트다운 텍스트 활성화
                    m_CountTxt.gameObject.SetActive(true);
                }

                break;


            case Bomb_State.CountDown:

                if ( 0 <= m_CountDown)
                {
                    m_CountDown -= Time.deltaTime;
                    m_CountTxt.text = m_CountDown.ToString("N1");

                    if (m_CountDown <= 0.0f)
                    {
                        //카운트다운 텍스트 비활성화
                        m_CountTxt.gameObject.SetActive(false);

                        m_BombState = Bomb_State.Explode;
                        //빙글빙글 돌아가는 애니메이션 전환
                        m_Anim.SetTrigger("IsExlopde");
                    }
                }
                break;

            case Bomb_State.Explode:

                print("플레이어와의 거리: " + m_PlayerDist);
                //플레이어가 피해 범위 안에 들어와있으면
                if (m_PlayerDist <= m_CiriticalDist)
                {
                    print("데미지를 입는 번위 안입니다." + this.transform.position + " : " + m_RefHero.transform.position);
                    //플레이어에게 데미지 입히기
                    m_RefHero.GetComponent<HeroManager>().TakeDamage(10);
                    //Destroy(this.gameObject, 0.7f);
                }
                else
                {
                    print("피해 거리 밖입니다." + this.transform.position + " : " + m_RefHero.transform.position);
                }

                m_BombState = Bomb_State.Destroy; //제거를 기다리는 Destroy로 전환 
                Destroy(this.gameObject, 0.7f);

                break;
            default:
                //Destroy는 여기 걸려져서 남은 시간동안 매 프레임 break당한다.
                break;
        }

        
    }



}
