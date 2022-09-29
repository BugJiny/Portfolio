using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfoController : MonoBehaviour
{

	public static bool isInfo;


	public GameObject m_userInfo;

	// Start is called before the first frame update
	void Start()
    {
		isInfo = false;
	}

    // Update is called once per frame
    void Update()
    {

		if (Input.GetKeyDown(KeyCode.P))
		{
			if (isInfo)
				isInfo = false;
			else
				isInfo = true;
		}


		if (isInfo)
		{
			m_userInfo.SetActive(true);
		}
		else
		{
			m_userInfo.SetActive(false);
		}

	}
}
