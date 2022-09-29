using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvenController : MonoBehaviour
{
	public static bool isInven;


	public GameObject m_Inventory;

	
	// Start is called before the first frame update
	void Start()
    {
		isInven = false;

	}

    // Update is called once per frame
    void Update()
    {

		if (Input.GetKeyDown(KeyCode.I))
		{
			if (isInven)
				isInven = false;
			else
				isInven = true;
		}


		if (isInven)
		{
			m_Inventory.SetActive(true);
		}
		else
		{
			InvenSystem.Instance.HideTooltip();
			m_Inventory.SetActive(false);
		}

	}
}
