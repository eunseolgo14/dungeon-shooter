using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class DoorManager : MonoBehaviour
{
    //플레이어가 문과 닿았는지 판별하기 위한 변수 

    HeroManager m_refHero;
    float m_MinX = -3.0f;
    float m_MaxX = 3.0f;
    //float m_MinY = 12.0f;
    float m_MaxY = 10.0f;

    //x축으로는 m_MinX와 m_MaxX사이에 있어야 하지만
    //y축으로는 m_MinY보다 작기만 하면 된다(그 보다 작아지는건 물리적으로 막힘)


    //안내 텍스트 연출용 변수
    bool m_IsTouch = false;
    bool m_IsOnOff = false;
    public GameObject m_HelpTxt = null;
    float m_ShowTime = 0.5f;

    //다음 씬 전환용 변수


    // Start is called before the first frame update
    void Start()
    {
        m_refHero = GameObject.FindObjectOfType<HeroManager>();
        m_ShowTime = 0.5f;
    }


    // Update is called once per frame
    void Update()
    {
        if (TouchState(m_refHero.transform.position) == true)
        {
            m_IsTouch = true;
            m_HelpTxt.gameObject.SetActive(m_IsTouch);
        }
        else if (TouchState(m_refHero.transform.position) == false)
        {
            m_IsTouch = false;
            m_HelpTxt.gameObject.SetActive(m_IsTouch);
        }

        if (m_IsTouch == true && Input.GetKeyDown(KeyCode.R) == true)
        {
            LoadingSceneManeger.LoadScene("BossStageScene");
        }
    }
   
    //OnCollisionEnter2D를 쓸 수 없어서 만든 겹침 감지 함수
    bool TouchState(Vector3 a_Pos)
    {
        if (m_MinX < a_Pos.x && a_Pos.x < m_MaxX && m_MaxY < a_Pos.y)
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}
