using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinUpTxtManager : MonoBehaviour
{
    float m_ShowTime = 0.0f;         
    public Text m_CoinTxt = null;  

    float MvSpeed = 100f;    
    float AlphaSpeed = 1.0f / (1.0f - 0.4f);
    //alpha 0.4(0.0f)초부터 연출 1.0초(1.0f)까지

    Vector3 m_CurPos;   
    Color m_Color;     


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_ShowTime += Time.deltaTime;

        if (m_ShowTime < 1.05f)
        {
            m_CurPos = m_CoinTxt.transform.position;
            m_CurPos.y += Time.deltaTime * MvSpeed;
            m_CoinTxt.transform.position = m_CurPos;
        }

        if (0.4f < m_ShowTime)
        {
            m_Color = m_CoinTxt.color;
            m_Color.a -= (Time.deltaTime * AlphaSpeed);
            if (m_Color.a < 0.0f)
                m_Color.a = 0.0f;
            m_CoinTxt.color = m_Color;
        }

        if (1.05f < m_ShowTime)
        {
            Destroy(this.gameObject);
        }

    }

    public void InitCoinTxt(float a_Damage, Color a_Color)
    {
        if (m_CoinTxt == null)
            m_CoinTxt = this.GetComponentInChildren<Text>();

        if (a_Damage <= 0.0f)
        {
            int a_Dmg = (int)Mathf.Abs(a_Damage);   //절대값 함수
            m_CoinTxt.text = "- " + a_Dmg;
        }
        else
        {
            m_CoinTxt.text = "+ " + (int)a_Damage;
        }

        a_Color.a = 1.0f;
        m_CoinTxt.color = a_Color;
    }//public void InitDamage(float a_Damage, Color a_Color)
}
