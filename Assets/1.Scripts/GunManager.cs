using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    //public Vector3 L_Arm_Pos = Vector3.zero;
    //public Vector3 R_Arm_Pos = Vector3.zero;
    //public Vector3 Gun_Pos = Vector3.zero;
    //public Vector3 Shoot_Pos = Vector3.zero;

    //public Transform ArmPivotObj = null;
    //public GameObject GunObj = null;
    //public GameObject GunObj = null;

    //public SpriteRenderer Hand_Sprite = null;
    //public SpriteRenderer Gun_Sprite = null;

    public Sprite[] m_HeroHands = null;

    HeroManager m_RefHero = null;

    // Start is called before the first frame update
    void Start()
    {
        m_RefHero = GameObject.FindObjectOfType<HeroManager>();

        if (GlobalValue.g_HeroType == HeroType.FirstHero)
        {
            for (int i = 0; i < m_RefHero.m_Hands.Length; i++)
            {
                //첫번째 히어로 손 스프라이트 넣어주기
                m_RefHero.m_Hands[i].sprite = m_HeroHands[0];
            }
        }
        else if (this.gameObject.name.Contains("ArmPivot_STG44") == true)
        {
            for (int i = 0; i < m_RefHero.m_Hands.Length; i++)
            {
                //첫번째 히어로 손 스프라이트 넣어주기
                m_RefHero.m_Hands[i].sprite = m_HeroHands[1];
            }
        }
        else if (this.gameObject.name.Contains("ArmPivot_M1A1") == true)
        {
            for (int i = 0; i < m_RefHero.m_Hands.Length; i++)
            {
                //첫번째 히어로 손 스프라이트 넣어주기
                m_RefHero.m_Hands[i].sprite = m_HeroHands[2];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
