using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("-----Flash ����� ����-----")]
    public GameObject m_HeaderObj = null;
    float m_waitTime = 0.0f;

    [Header("-----��ư ����� ����-----")]
    public Button m_ReplayBtn = null;
    public Button m_QuitBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        m_waitTime = 0.5f;

        if (m_ReplayBtn != null)
        {
            m_ReplayBtn.onClick.AddListener(() =>
            {
                //�κ�� ����
                SceneManager.LoadScene(2);
            });
        }
        if (m_QuitBtn != null)
        {
            m_QuitBtn.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
                //#if ��ó�������� ������ => ������ ���� / �������� => �������� ����
            });
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (0 <= m_waitTime)
        {
            m_waitTime -= Time.deltaTime;

            if (m_waitTime < 0)
            {
                m_HeaderObj.SetActive(!m_HeaderObj.activeSelf);
                m_waitTime = 0.5f;
            }
        }

    }
}
