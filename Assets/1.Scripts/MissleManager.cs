using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissleManager : MonoBehaviour
{
    //유도탄 변수
    float m_MoveSpeed = 8.0f;   //이동 속도
    float m_RotSpeed = 200.0f;  //회전 속도

    [HideInInspector] public GameObject Target_Obj = null;  //타겟 참조 변수
    [SerializeField] Vector3 m_TargetPos = Vector3.zero;
    EnemyManager m_RefMonMgr = null;
    Vector3 m_DesiredDir;   //타겟을 향하는 방향 벡터 변수
                            //유도탄 변수

    //애니메이션 연출용 변수
    Animator m_Anim = null;
    bool m_isHit = false;

    int m_idx = -1;
    void Start()
    {
        m_Anim = GetComponent<Animator>();
        Destroy(this.gameObject, 8.0f);
        m_idx = SceneManager.GetActiveScene().buildIndex;

    }

    // Update is called once per frame
    void Update()
    {
        if (Target_Obj == null)
        {
            FindTarget(); //추적 대상을 탐색하는 함수
        }

        if (Target_Obj != null && m_isHit == false)
        {
            BulletHoming(); //추적 대상을 향해 이동하는 행동 패턴 함수
        }
        else if (Target_Obj == null )//추적 중 타겟이 사망했거나 없어짐 => 오른쪽으로 계속 이동
        {
            transform.Translate(transform.right * m_MoveSpeed * Time.deltaTime, Space.World);
            //rig.velocity = transform.right * m_MoveSpeed;
        }

        //if (CameraResolution.m_ScreenWMax.x + 5.0f < transform.position.x ||
        //    CameraResolution.m_ScreenWMin.x - 5.0f > transform.position.x ||
        //    CameraResolution.m_ScreenWMax.y + 5.0f < transform.position.y ||
        //    CameraResolution.m_ScreenWMin.y - 5.0f > transform.position.y)
        //{
        //    Destroy(this.gameObject);
        //}
    }
    void FindTarget() //추적 대상을 잡는 함수
    {
        

        if (m_idx == 4) //스테이지 1이라면
        {
            GameObject[] a_EnemyList = GameObject.FindGameObjectsWithTag("Monster");

            if (a_EnemyList.Length <= 0)
            {
                return;
            }

            GameObject a_Find_Mon = null;
            //float a_CacDist = 0.0f;
            //Vector3 a_CacVec = Vector3.zero;
            

            for (int ii = 0; ii < a_EnemyList.Length; ii++)
            {
                m_RefMonMgr = a_EnemyList[ii].GetComponent<EnemyManager>();

                //한 유도탄이 몬스터 객체를 추적중이라면 걔는 추적 가능 대상에서 제외
                if (m_RefMonMgr != null && m_RefMonMgr.m_HomingMissile != null)
                {
                    continue;
                }

                a_Find_Mon = a_EnemyList[ii].gameObject;
                m_RefMonMgr.m_HomingMissile = this.gameObject;//null => this, 추정 대상으로 추후 타겟팅 되지 않음
                break;
            }

            Target_Obj = a_Find_Mon;
            //m_TargetPos = Target_Obj.transform.position;
        }
        else if (m_idx == 5) //보스 스테이지라면
        {
            Target_Obj = GameObject.FindGameObjectWithTag("Boss");

            UseSkillManager a_SkillMgr = GameObject.FindObjectOfType<UseSkillManager>();

            if (a_SkillMgr != null && m_TargetPos == Vector3.zero)
            {
                for (int i = 0; i < a_SkillMgr.m_BossPoints.Count; i++) //4보다 작다 => 0, 1, 2, 3 => 4번
                {
                    if (m_TargetPos != Vector3.zero)
                    {
                        return;
                    }
                    int a_Rand = Random.Range(0, a_SkillMgr.m_BossPoints.Count); //0~3번 사이에
                    m_TargetPos = a_SkillMgr.m_BossPoints[a_Rand];
                    //Debug.Log("타겟포스:" + m_TargetPos + " /리스트 번호:" + a_Rand + " /남은 리스트 갯수:" + (a_SkillMgr.m_BossPoints.Count - 1));
                    a_SkillMgr.m_BossPoints.RemoveAt(a_Rand);
                }
            }
        }



    }

    void BulletHoming() //추적 대상을 향해 이동하는 함수
    {
        if (m_idx == 4 && Target_Obj != null)
        {
            m_TargetPos = Target_Obj.transform.position;
        }
        else if (m_idx == 5)
        {

        }

            //타겟을 향한 방향 벡터 구하기 
            m_DesiredDir = m_TargetPos - transform.position;
        m_DesiredDir.z = 0.0f;
        m_DesiredDir.Normalize();

        //유도탄을 타겟을 향해 회전시키기
        float angle = Mathf.Atan2(m_DesiredDir.y, m_DesiredDir.x) * Mathf.Rad2Deg;
        //보간을 주어 부드럽게 회전을 시키는 기능
        Quaternion targetRot = Quaternion.AngleAxis(angle, Vector3.forward);
        //현재 회전값에서 복표 회전값까지 이 속력으로 천천히 
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, m_RotSpeed * Time.deltaTime);

        //유도탄이 바라보는 방향을 향해 움직이기
        transform.Translate(transform.right * m_MoveSpeed * Time.deltaTime, Space.World);

        

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //유도탄 필드몹과 충돌
        if (collision.tag == "Monster")
        {
            m_isHit = true;
            if (m_Anim != null)
            {
                m_Anim.SetTrigger("IsHit");
            }
            m_RefMonMgr.TakeDamage(50);
            //Destroy(this.gameObject, 0.95f);
        }
        //유도탄 보스몹과 충돌
        else if (collision.tag == "Boss")
        {
            m_isHit = true;
            if (m_Anim != null)
            {
                m_Anim.SetTrigger("IsHit");
            }
            BossManager a_BsMgr = collision.GetComponent<BossManager>();
            a_BsMgr.TakeDamage(50f);
            //Destroy(this.gameObject, 1.0f);
        }
    }

    public void Disappear()
    {
        Destroy(gameObject);
    }

    void SetBossPoint()
    { 
    
    }
}
