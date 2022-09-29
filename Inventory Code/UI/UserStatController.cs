using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserStatController : MonoBehaviour
{

	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

		transform.GetChild(0).GetComponent<Text>().text = "ATK:"+(PlayerMove.m_status.Attack+PlayerMove.m_status.addAtk);
		transform.GetChild(1).GetComponent<Text>().text = "DEF:"+(PlayerMove.m_status.Defence+PlayerMove.m_status.addDef);
		transform.GetChild(2).GetComponent<Text>().text = "SPD:"+(PlayerMove.m_status.Speed+PlayerMove.m_status.addSpd);
		transform.GetChild(3).GetComponent<Text>().text = "CRI:"+(PlayerMove.m_status.Critical+PlayerMove.m_status.addCri);
	}


	



}
