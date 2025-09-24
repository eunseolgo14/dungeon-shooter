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
    //------ (1) ���� �� ���� ��� �׸��� �̵��ϴ� �� ���� ���� 

    //���ư����� ���� ��ǥ ��ġ
    public Transform m_Target;
    //������ ��ź(this)�� ���� ��ġ
    private Vector3 m_StartPosition;

    //���ư� �ӵ�
    public float m_Speed = 10;
    //���� ��� ����
    public float m_HeightArc = 1;
    //��ǥ ���� ���� Ȯ�ο� ����
    private bool m_IsArrive;

    //------ (2) �÷��̾���� �Ÿ� ����, ���� �ִ� ��ȯ�� ����
    
    //�Ÿ��� ������ �÷��̾� ���� �Ҵ�� ���� 
    public GameObject m_RefHero;
    //���� ī��Ʈ�ٿ��� ������ �Ÿ�
    float m_ExplosionLimit = 3.0f;
    //��ź�� �÷��̾� ������ �Ÿ�
    float m_PlayerDist = 0.0f;
    //ī��Ʈ�ٿ� ���� ��ȯ ���� �Լ�
    //bool m_IsCountDown = false;
    //���ѽð�
    float m_CountDown = 0.6f;
    //���� �ð� ��¿� �ؽ�Ʈ ����
    public Text m_CountTxt;
    //�÷��̾ ��ź �������� ���� �Ÿ�
    float m_CiriticalDist = 3.2f;

    //�ִϸ��̼� ���� ���� �ִϸ��̷� ������Ʈ
    Animator m_Anim = null;
    Bomb_State m_BombState = Bomb_State.Idle;

    void Start()
    {
        m_StartPosition = transform.position;
        m_RefHero = GameObject.FindGameObjectWithTag("Hero");
        m_Anim = GetComponent<Animator>();

        if (m_RefHero == null)
        {
            Debug.Log("����θ� ã�� ���߽��ϴ�");
        }

        m_IsArrive = false;
        m_BombState = Bomb_State.Idle;
    }

    void Update()
    {
        //�����ߴٸ� ������Ʈ �Լ� ����
        if (m_IsArrive == true)
        {
            BombUpdate();
        }

        //���� ���� �ʾҴٸ� ���������� ���󰡴� ���� ����
        else  
        {
            FlyParabola();
        }
        //�̰� ������Ʈ���� �׋��׋� �� �� �ƴ϶� �������� �޶��� ��
        m_PlayerDist = (this.transform.position - m_RefHero.transform.position).magnitude;
        //print(m_PlayerDist);
    }

    //������ �׸��� ���� �Լ�
    void FlyParabola()
    {
        if (m_Target == null)
        {
            return;
        }

        //��ǥ���������� x�� ���� �ű� ���ϱ�
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

    //�÷��̾���� �Ÿ��� �����ϴ� �Լ�
    void BombUpdate()
    {
        if (m_RefHero == null)
        {
            return;
        }

        switch (m_BombState)
        {
            case Bomb_State.Idle:

                //��ź �ڽŰ� �÷��̾� ������ �Ÿ��� ����Ѵ�.
                //m_PlayerDist = (this.transform.position - m_RefHero.transform.position).magnitude;// Vector3.Distance(this.transform.position, m_RefHero.transform.position);

                //��ź ���� ���� �ȿ� ���Խ��ϴ�.
                if (m_PlayerDist < m_ExplosionLimit)
                {
                    m_BombState = Bomb_State.CountDown;
                    //���ۺ��� ���ư��� �ִϸ��̼� ��ȯ
                    m_Anim.SetTrigger("IsLit");
                    //ī��Ʈ�ٿ� �ؽ�Ʈ Ȱ��ȭ
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
                        //ī��Ʈ�ٿ� �ؽ�Ʈ ��Ȱ��ȭ
                        m_CountTxt.gameObject.SetActive(false);

                        m_BombState = Bomb_State.Explode;
                        //���ۺ��� ���ư��� �ִϸ��̼� ��ȯ
                        m_Anim.SetTrigger("IsExlopde");
                    }
                }
                break;

            case Bomb_State.Explode:

                print("�÷��̾���� �Ÿ�: " + m_PlayerDist);
                //�÷��̾ ���� ���� �ȿ� ����������
                if (m_PlayerDist <= m_CiriticalDist)
                {
                    print("�������� �Դ� ���� ���Դϴ�." + this.transform.position + " : " + m_RefHero.transform.position);
                    //�÷��̾�� ������ ������
                    m_RefHero.GetComponent<HeroManager>().TakeDamage(10);
                    //Destroy(this.gameObject, 0.7f);
                }
                else
                {
                    print("���� �Ÿ� ���Դϴ�." + this.transform.position + " : " + m_RefHero.transform.position);
                }

                m_BombState = Bomb_State.Destroy; //���Ÿ� ��ٸ��� Destroy�� ��ȯ 
                Destroy(this.gameObject, 0.7f);

                break;
            default:
                //Destroy�� ���� �ɷ����� ���� �ð����� �� ������ break���Ѵ�.
                break;
        }

        
    }



}
