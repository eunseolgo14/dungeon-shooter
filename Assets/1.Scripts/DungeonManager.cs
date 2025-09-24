using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    

    public Button m_BackBtn = null;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayBGM("Battle");

        if (m_BackBtn != null)
        {
            m_BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(2);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
