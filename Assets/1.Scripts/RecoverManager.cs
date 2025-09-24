using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecoverManager : MonoBehaviour
{
    public GameObject m_HpTextObj = null;
    public Text m_HP = null;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEndEvent()
    {
        Destroy(this.gameObject);
    }
}
