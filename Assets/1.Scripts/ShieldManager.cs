using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "HeroBullet")
        {
            //����� �ҷ��� �浹 ��ȿó��
            return;
        }
        else if (collision.gameObject.tag == "EnemyBullet" || collision.gameObject.tag == "BossBullet")
        {
            //print("���� �Ѿ� �浹");
            Destroy(collision.gameObject);
        }

    }


}
