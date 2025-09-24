using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class DoorManager : MonoBehaviour
{
    //�÷��̾ ���� ��Ҵ��� �Ǻ��ϱ� ���� ���� 

    HeroManager m_refHero;
    float m_MinX = -3.0f;
    float m_MaxX = 3.0f;
    //float m_MinY = 12.0f;
    float m_MaxY = 10.0f;

    //x�����δ� m_MinX�� m_MaxX���̿� �־�� ������
    //y�����δ� m_MinY���� �۱⸸ �ϸ� �ȴ�(�� ���� �۾����°� ���������� ����)


    //�ȳ� �ؽ�Ʈ ����� ����
    bool m_IsTouch = false;
    bool m_IsOnOff = false;
    public GameObject m_HelpTxt = null;
    float m_ShowTime = 0.5f;

    //���� �� ��ȯ�� ����


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
   
    //OnCollisionEnter2D�� �� �� ��� ���� ��ħ ���� �Լ�
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
