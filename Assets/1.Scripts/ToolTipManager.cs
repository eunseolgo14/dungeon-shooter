using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipManager : MonoBehaviour
{
    public Text m_SkillNameTxt = null;
    public Text m_SkillExpTxt = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitToolTip(SkillType a_Type)
    {
        if (m_SkillNameTxt == null|| m_SkillExpTxt == null)
        {
            return;
        }

        m_SkillNameTxt.text = "[" + GlobalValue.m_SkillInfoList[(int)a_Type].m_SkillName + "]";
        m_SkillExpTxt.text = GlobalValue.m_SkillInfoList[(int)a_Type].m_SkillExp;
    }
}
