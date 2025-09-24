using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemManager : MonoBehaviour
{
    float m_WaitTime = 0.6f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < m_WaitTime)
        {
            m_WaitTime -= Time.deltaTime;
            if (m_WaitTime <= 0.0f)
            {
                this.GetComponent<SpriteRenderer>().enabled = true;
                this.GetComponent<BoxCollider2D>().enabled = true;
            }
            
        }

       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("Hero") == false)
        {
            //플레이어가 아닌 다른 콜라이더와의 충돌은 무시
            return;
        }

        switch (this.tag)
        {
            case "BlueGem":
                GlobalValue.g_UserBlueGem++;
                PlayerPrefs.SetInt("BlueGem", GlobalValue.g_UserBlueGem);
                Destroy(this.gameObject);
                break;

            case "GreenGem":
                GlobalValue.g_UserGreenGem++;
                PlayerPrefs.SetInt("GreenGem", GlobalValue.g_UserGreenGem);
                Destroy(this.gameObject);
                break;

            case "RedGem":
                GlobalValue.g_UserRedGem++;
                PlayerPrefs.SetInt("RedGem", GlobalValue.g_UserRedGem);
                Destroy(this.gameObject);
                break;

        }

        //리프레쉬 판넬
        GameObject.FindObjectOfType<UI_Manager>().RefreshAcquisition();

        //ui에 출력 해야함
        //저장되어야 하는 값들
        //1.위의 4가지 재화의 보유값 => 글로벌 벨류 세이브 함수
        //2.보유 액티브 스킬의 종류와 갯수
        //3.보유 패시브 스킬의 종류와 등급 => 스킬 아이템 세이브 함수

        
    }
}
