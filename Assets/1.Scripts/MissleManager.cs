using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissleManager : MonoBehaviour
{
    //����ź ����
    float m_MoveSpeed = 8.0f;   //�̵� �ӵ�
    float m_RotSpeed = 200.0f;  //ȸ�� �ӵ�

    [HideInInspector] public GameObject Target_Obj = null;  //Ÿ�� ���� ����
    [SerializeField] Vector3 m_TargetPos = Vector3.zero;
    EnemyManager m_RefMonMgr = null;
    Vector3 m_DesiredDir;   //Ÿ���� ���ϴ� ���� ���� ����
                            //����ź ����

    //�ִϸ��̼� ����� ����
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
            FindTarget(); //���� ����� Ž���ϴ� �Լ�
        }

        if (Target_Obj != null && m_isHit == false)
        {
            BulletHoming(); //���� ����� ���� �̵��ϴ� �ൿ ���� �Լ�
        }
        else if (Target_Obj == null )//���� �� Ÿ���� ����߰ų� ������ => ���������� ��� �̵�
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
    void FindTarget() //���� ����� ��� �Լ�
    {
        

        if (m_idx == 4) //�������� 1�̶��
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

                //�� ����ź�� ���� ��ü�� �������̶�� �´� ���� ���� ��󿡼� ����
                if (m_RefMonMgr != null && m_RefMonMgr.m_HomingMissile != null)
                {
                    continue;
                }

                a_Find_Mon = a_EnemyList[ii].gameObject;
                m_RefMonMgr.m_HomingMissile = this.gameObject;//null => this, ���� ������� ���� Ÿ���� ���� ����
                break;
            }

            Target_Obj = a_Find_Mon;
            //m_TargetPos = Target_Obj.transform.position;
        }
        else if (m_idx == 5) //���� �����������
        {
            Target_Obj = GameObject.FindGameObjectWithTag("Boss");

            UseSkillManager a_SkillMgr = GameObject.FindObjectOfType<UseSkillManager>();

            if (a_SkillMgr != null && m_TargetPos == Vector3.zero)
            {
                for (int i = 0; i < a_SkillMgr.m_BossPoints.Count; i++) //4���� �۴� => 0, 1, 2, 3 => 4��
                {
                    if (m_TargetPos != Vector3.zero)
                    {
                        return;
                    }
                    int a_Rand = Random.Range(0, a_SkillMgr.m_BossPoints.Count); //0~3�� ���̿�
                    m_TargetPos = a_SkillMgr.m_BossPoints[a_Rand];
                    //Debug.Log("Ÿ������:" + m_TargetPos + " /����Ʈ ��ȣ:" + a_Rand + " /���� ����Ʈ ����:" + (a_SkillMgr.m_BossPoints.Count - 1));
                    a_SkillMgr.m_BossPoints.RemoveAt(a_Rand);
                }
            }
        }



    }

    void BulletHoming() //���� ����� ���� �̵��ϴ� �Լ�
    {
        if (m_idx == 4 && Target_Obj != null)
        {
            m_TargetPos = Target_Obj.transform.position;
        }
        else if (m_idx == 5)
        {

        }

            //Ÿ���� ���� ���� ���� ���ϱ� 
            m_DesiredDir = m_TargetPos - transform.position;
        m_DesiredDir.z = 0.0f;
        m_DesiredDir.Normalize();

        //����ź�� Ÿ���� ���� ȸ����Ű��
        float angle = Mathf.Atan2(m_DesiredDir.y, m_DesiredDir.x) * Mathf.Rad2Deg;
        //������ �־� �ε巴�� ȸ���� ��Ű�� ���
        Quaternion targetRot = Quaternion.AngleAxis(angle, Vector3.forward);
        //���� ȸ�������� ��ǥ ȸ�������� �� �ӷ����� õõ�� 
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, m_RotSpeed * Time.deltaTime);

        //����ź�� �ٶ󺸴� ������ ���� �����̱�
        transform.Translate(transform.right * m_MoveSpeed * Time.deltaTime, Space.World);

        

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //����ź �ʵ���� �浹
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
        //����ź �������� �浹
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
