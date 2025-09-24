using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("-----Flash 연출용 변수-----")]
    public GameObject m_HeaderObj = null;
    float m_waitTime = 0.0f;

    [Header("-----버튼 연결용 변수-----")]
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
                //로비씬 연결
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
                //#if 전처리문으로 에디터 => 에디터 종료 / 실행파일 => 실행파일 종료
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
