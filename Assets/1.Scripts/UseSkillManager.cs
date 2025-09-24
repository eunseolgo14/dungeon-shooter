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
            case SkillType.Skill_0: //체력 회복
                {
                    if (m_EffObj != null)
                    {
                        //체력 변수 증가
                        GlobalValue.g_CurrHP += (GlobalValue.g_MaxHP / 10);//(100/10) => 즉 10이 충전됨

                        //max를 넘기지 않게 clamping
                        if (GlobalValue.g_MaxHP < GlobalValue.g_CurrHP)
                        {
                            GlobalValue.g_CurrHP = GlobalValue.g_MaxHP;
                        }
                        //증가된 값 로컬에 저장 => 필요한가?


                        //체력바 ui 리프레쉬(히어로 매니저 스크립트의 RefreshHpBar함수를 이름으로 호출)
                        SendMessage("RefreshHpBar");

                        //hp텍스트에 회복값 대입
                        m_HpTxt.text = "+" + (GlobalValue.g_MaxHP / 10).ToString("N0");

                        m_EffObj.SetActive(true);
                        //타이머 충전
                        m_RecoverDur = 2.0f;
                    }
                }
                break;

            case SkillType.Skill_1: //유도탄
                {
                    //현재 열린 씬의 빌드 번호를 저장
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
                            //Debug.Log("보스 타격점 검색 완료, 총 " + m_BossPoints.Count + "개");
                        }
                    }

                    //--- 4발을 발사
                    Vector3 a_Pos;
                    GameObject a_Clone;

                    //위치: 주인공 기준으로 앞에 두 지점, 뒤에 두 지점
                    for (float yy = 0.5f; yy > -0.55f; yy -= 0.25f) //(0.5) (0.25) (0) (-0.25) (-0.5) => 5번 반복
                    {
                        if (-0.1f < yy && yy < 0.1f) //오차값이 있으니까 범위로
                        {
                            continue; //0이면 건너뛰기 위해서 => 5발에서 4발만 발사!
                        }

                        a_Clone = Instantiate(m_HomingMissile) as GameObject;

                        //Debug.Log(this.transform.rotation.eulerAngles.y);

                        //히어로의 y회전값이 180(-180)일 때(왼쪽을 응시하고 있을 때) => 프리팹의 회전각 변경
                        if (this.transform.rotation.eulerAngles.y == 180.0f || this.transform.rotation.eulerAngles.y == -180.0f)
                        {
                            //Debug.Log("회전합니다");
                            a_Clone.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 180.0f));

                            //a_Clone.GetComponent<SpriteRenderer>().flipX = true;

                            a_Pos = FindObjectOfType<HeroManager>().transform.position + new Vector3(-0.1f, 0.14f, 0);//주인공의 위치를 기준으로

                            if (-0.5f < yy && yy < 0.5f)
                            {
                                a_Pos.x -= 1.0f; //안쪽의 유도탄의 위치는 주인공과 가깝게
                            }
                            else
                            {
                                a_Pos.x -= 1.4f; //바깥쪽의 유도탄 위치는 주인공과 떨어져서
                            }
                            a_Pos.y += yy; //yy증가
                            a_Clone.transform.position = a_Pos;//바뀐 위치 적용
                        }
                        //히어로의 y회전값이 0일 때(오른쪽을 응시하고 있을 때) => 프리팹의 원 회전각 유지
                        else
                        {
                            //Debug.Log("회전하지 않습니다");

                            a_Pos = FindObjectOfType<HeroManager>().transform.position + new Vector3(0.1f, 0.14f, 0);//주인공의 위치를 기준으로

                            if (-0.5f < yy && yy < 0.5f)
                            {
                                a_Pos.x += 1.0f; //안쪽의 유도탄의 위치는 주인공과 가깝게
                            }
                            else
                            {
                                a_Pos.x += 1.4f; //바깥쪽의 유도탄 위치는 주인공과 떨어져서
                            }
                            a_Pos.y += yy; //yy증가
                            a_Clone.transform.position = a_Pos;//바뀐 위치 적용
                        }

                    }

                    
                    //--- 4발을 발사
                }
                break;

            case SkillType.Skill_2: //보호막
                {
                    if (m_ShieldObj != null)
                    {
                        m_ShieldObj.SetActive(true);
                        //타이머 충전
                        m_ShieldDur = 30.0f;
                    }
                }
                break;

            //case SkillType.Skill_3: //체력 회복
            //    break;

            //case SkillType.Skill_4: //빠른 연사
            //    break;

            //case SkillType.Skill_5: //소환수
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

        //체력 회복 텍스트 서서히 위로
        m_CurPos = m_HpTxt.transform.position;
        m_CurPos.y += Time.deltaTime * MvSpeed;
        m_HpTxt.transform.position = m_CurPos;

        m_Color = m_HpTxt.color;
        m_Color.a -= (Time.deltaTime * AlphaSpeed);
        if (m_Color.a < 0.0f)
            m_Color.a = 0.0f;
        m_HpTxt.color = m_Color;

        //특정 시간이 되면 연출 종료
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

        //빙글뱅글 도는 연출
        m_ShieldObj.transform.Rotate(new Vector3(0.0f, 0.0f, -140.0f * Time.deltaTime));

        if (m_ShieldDur <= 0.0f)
        {
            //연출 종료
            m_ShieldObj.SetActive(false);
        }
    }
}
