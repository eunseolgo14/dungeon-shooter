using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Test_Manager : MonoBehaviour
{
   
    //List<Node> FinalNodeList;
   //public Monstate m_State = Monstate.Chasing;

    //¿Ãµø
    public float speed = 100f;
    public int idx = 0;
    public bool isAuto = false;

    void Start()
    {
    }

    void Update()
    {
        print(Time.deltaTime);
        if (isAuto == false)
        {
            return;
        }
        if (PathFinding.inst.FinalNodeList.Count  == idx) 
        {
            return;
        }
        Vector3 a_TargetPos = new Vector3(PathFinding.inst.FinalNodeList[idx].x, PathFinding.inst.FinalNodeList[idx].y, 0.0f);
        transform.position = Vector3.MoveTowards(transform.position, a_TargetPos, speed * Time.deltaTime ) ;

        if ((a_TargetPos.x - 0.2f < transform.position.x && transform.position.x < a_TargetPos.x + 0.2f) &&
            (a_TargetPos.y - 0.2f < transform.position.y && transform.position.y < a_TargetPos.y + 0.2f))
        {
            transform.position = a_TargetPos;
            idx++;
            
           //m_State = Monstate.Arriving;
            
            return;
        }


    }



    public void AutoMove()
    {
        isAuto = true;
    }
}
