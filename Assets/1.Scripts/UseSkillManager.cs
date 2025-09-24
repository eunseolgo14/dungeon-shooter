using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UseSkillManager : MonoBehaviour
{
    [Header("-----Skill_1(Recover)-----")]
    public GameObject m_EffObj = null;
    public float m_RecoverDur = 0.0f;

    public Text m_HpTxt = null;
    float MvSpeed = 1.0f;
    Vector3 m_OriginPos;
    Vector3 m_CurPos;
    float AlphaSpeed = 0.9f;
    Color m_Color;

    [Header("-----Skill_2(Shield)-----")]
    public GameObject m_ShieldObj = null;
    public float m_ShieldDur = 0.0f;

    [Header("-----Skill_3(Missle)-----")]
    public GameObject m_HomingMissile = null;
    //public float m_ShieldDur = 0.0f;
    public List<Vector3> m_BossPoints;


    // Start is called before the first frame update
    void Start()
    {
        m_OriginPos = m_HpTxt.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (0 <= m_RecoverDur)
        {
            Skill_1_Recover_Update();
        }
        if (0 <= m_ShieldDur)
        {
            Skill_2_Shield_Update();
        }
        
    }


    public void UseSkill(SkillType a_Type)
    {
        if (GlobalValue.g_CurrHP <= 0)
        {
            return;
        }

        switch (a_Type)
        {
            case SkillType.Skill_0: //ü�� ȸ��
                {
                    if (m_EffObj != null)
                    {
                        //ü�� ���� ����
                        GlobalValue.g_CurrHP += (GlobalValue.g_MaxHP / 10);//(100/10) => �� 10�� ������

                        //max�� �ѱ��� �ʰ� clamping
                        if (GlobalValue.g_MaxHP < GlobalValue.g_CurrHP)
                        {
                            GlobalValue.g_CurrHP = GlobalValue.g_MaxHP;
                        }
                        //������ �� ���ÿ� ���� => �ʿ��Ѱ�?


                        //ü�¹� ui ��������(����� �Ŵ��� ��ũ��Ʈ�� RefreshHpBar�Լ��� �̸����� ȣ��)
                        SendMessage("RefreshHpBar");

                        //hp�ؽ�Ʈ�� ȸ���� ����
                        m_HpTxt.text = "+" + (GlobalValue.g_MaxHP / 10).ToString("N0");

                        m_EffObj.SetActive(true);
                        //Ÿ�̸� ����
                        m_RecoverDur = 2.0f;
                    }
                }
                break;

            case SkillType.Skill_1: //����ź
                {
                    //���� ���� ���� ���� ��ȣ�� ����
                    int a_idx = SceneManager.GetActiveScene().buildIndex;

                    if (a_idx == 5)
                    {
                        GameObject[] a_AimPoints = GameObject.FindGameObjectsWithTag("BossAimPoints");

                        if (0 < a_AimPoints.Length)
                        {
                            for (int i = 0; i < a_AimPoints.Length; i++)
                            {
                                m_BossPoints.Add(a_AimPoints[i].transform.position);
                            }
                            //Debug.Log("���� Ÿ���� �˻� �Ϸ�, �� " + m_BossPoints.Count + "��");
                        }
                    }

                    //--- 4���� �߻�
                    Vector3 a_Pos;
                    GameObject a_Clone;

                    //��ġ: ���ΰ� �������� �տ� �� ����, �ڿ� �� ����
                    for (float yy = 0.5f; yy > -0.55f; yy -= 0.25f) //(0.5) (0.25) (0) (-0.25) (-0.5) => 5�� �ݺ�
                    {
                        if (-0.1f < yy && yy < 0.1f) //�������� �����ϱ� ������
                        {
                            continue; //0�̸� �ǳʶٱ� ���ؼ� => 5�߿��� 4�߸� �߻�!
                        }

                        a_Clone = Instantiate(m_HomingMissile) as GameObject;

                        //Debug.Log(this.transform.rotation.eulerAngles.y);

                        //������� yȸ������ 180(-180)�� ��(������ �����ϰ� ���� ��) => �������� ȸ���� ����
                        if (this.transform.rotation.eulerAngles.y == 180.0f || this.transform.rotation.eulerAngles.y == -180.0f)
                        {
                            //Debug.Log("ȸ���մϴ�");
                            a_Clone.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 180.0f));

                            //a_Clone.GetComponent<SpriteRenderer>().flipX = true;

                            a_Pos = FindObjectOfType<HeroManager>().transform.position + new Vector3(-0.1f, 0.14f, 0);//���ΰ��� ��ġ�� ��������

                            if (-0.5f < yy && yy < 0.5f)
                            {
                                a_Pos.x -= 1.0f; //������ ����ź�� ��ġ�� ���ΰ��� ������
                            }
                            else
                            {
                                a_Pos.x -= 1.4f; //�ٱ����� ����ź ��ġ�� ���ΰ��� ��������
                            }
                            a_Pos.y += yy; //yy����
                            a_Clone.transform.position = a_Pos;//�ٲ� ��ġ ����
                        }
                        //������� yȸ������ 0�� ��(�������� �����ϰ� ���� ��) => �������� �� ȸ���� ����
                        else
                        {
                            //Debug.Log("ȸ������ �ʽ��ϴ�");

                            a_Pos = FindObjectOfType<HeroManager>().transform.position + new Vector3(0.1f, 0.14f, 0);//���ΰ��� ��ġ�� ��������

                            if (-0.5f < yy && yy < 0.5f)
                            {
                                a_Pos.x += 1.0f; //������ ����ź�� ��ġ�� ���ΰ��� ������
                            }
                            else
                            {
                                a_Pos.x += 1.4f; //�ٱ����� ����ź ��ġ�� ���ΰ��� ��������
                            }
                            a_Pos.y += yy; //yy����
                            a_Clone.transform.position = a_Pos;//�ٲ� ��ġ ����
                        }

                    }

                    
                    //--- 4���� �߻�
                }
                break;

            case SkillType.Skill_2: //��ȣ��
                {
                    if (m_ShieldObj != null)
                    {
                        m_ShieldObj.SetActive(true);
                        //Ÿ�̸� ����
                        m_ShieldDur = 30.0f;
                    }
                }
                break;

            //case SkillType.Skill_3: //ü�� ȸ��
            //    break;

            //case SkillType.Skill_4: //���� ����
            //    break;

            //case SkillType.Skill_5: //��ȯ��
            //    break;
        }
    }

    void Skill_1_Recover_Update()
    {
        if (m_RecoverDur <= 0.0f)
        {
            return;
        }

        m_RecoverDur -= Time.deltaTime;

        //ü�� ȸ�� �ؽ�Ʈ ������ ����
        m_CurPos = m_HpTxt.transform.position;
        m_CurPos.y += Time.deltaTime * MvSpeed;
        m_HpTxt.transform.position = m_CurPos;

        m_Color = m_HpTxt.color;
        m_Color.a -= (Time.deltaTime * AlphaSpeed);
        if (m_Color.a < 0.0f)
            m_Color.a = 0.0f;
        m_HpTxt.color = m_Color;

        //Ư�� �ð��� �Ǹ� ���� ����
        if (m_RecoverDur <= 0.0f)
        {
            //m_HpTxt.gameObject.SetActive(false);
            m_HpTxt.gameObject.transform.position = m_OriginPos;
            m_HpTxt.color = new Color(255, 255, 255, 255);
            m_EffObj.gameObject.SetActive(false);
        }
    }

    void Skill_2_Shield_Update()
    {
        if (m_ShieldDur <= 0.0f)
        {
            return;
        }

        m_ShieldDur -= Time.deltaTime;

        //���۹�� ���� ����
        m_ShieldObj.transform.Rotate(new Vector3(0.0f, 0.0f, -140.0f * Time.deltaTime));

        if (m_ShieldDur <= 0.0f)
        {
            //���� ����
            m_ShieldObj.SetActive(false);
        }
    }
}
