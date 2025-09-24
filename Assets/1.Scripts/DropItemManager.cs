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
            //�÷��̾ �ƴ� �ٸ� �ݶ��̴����� �浹�� ����
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

        //�������� �ǳ�
        GameObject.FindObjectOfType<UI_Manager>().RefreshAcquisition();

        //ui�� ��� �ؾ���
        //����Ǿ�� �ϴ� ����
        //1.���� 4���� ��ȭ�� ������ => �۷ι� ���� ���̺� �Լ�
        //2.���� ��Ƽ�� ��ų�� ������ ����
        //3.���� �нú� ��ų�� ������ ��� => ��ų ������ ���̺� �Լ�

        
    }
}
