using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler,IDragHandler,IPointerEnterHandler,IPointerExitHandler
{
	

	private Vector3 m_defaultPos;
	private Vector3 m_currentPos;

	private PrefabInfo m_info;

	public void OnBeginDrag(PointerEventData eventData)
	{
		if(SkillSet.isDrag)
			m_defaultPos = this.transform.position;


	}
	public void OnDrag(PointerEventData eventData)
	{

		if (SkillSet.isDrag)
		{
			m_currentPos = Input.mousePosition;
			this.transform.position = m_currentPos;
		}
		
	}

	

	public void OnEndDrag(PointerEventData eventData)
	{
		if (SkillSet.isDrag)
		{
			this.transform.position = m_currentPos;
		}
	}

	

	public void OnPointerEnter(PointerEventData eventData)
	{

		m_info = this.GetComponent<PrefabInfo>();

		if (!SkillSet.isDrag)
		{
			//툴팁을 수정해야함.

			if(m_info.info.currentlevel ==m_info.info.point)
			{
				this.transform.Find("TooTipPanel").transform.Find("Name").GetComponent<Text>().text = $"{m_info.info.name}(Max)";
			}
			else
			{
				this.transform.Find("TooTipPanel").transform.Find("Name").GetComponent<Text>().text = $"{m_info.info.name}({m_info.info.currentlevel})";
			}
			


			this.transform.Find("TooTipPanel").transform.Find("LinkSkill").GetComponent<Text>().text = "링크된스킬:";

			foreach(string name in m_info.info.linkSkillNameList)
			{
				this.transform.Find("TooTipPanel").transform.Find("LinkSkill").GetComponent<Text>().text += name ;
				this.transform.Find("TooTipPanel").transform.Find("LinkSkill").GetComponent<Text>().text += ",";
			}

			this.transform.Find("TooTipPanel").gameObject.SetActive(true);  //툴팁활성화.

			
		}
			


	}

	public void OnPointerExit(PointerEventData eventData)
	{

		if (!SkillSet.isDrag)
			this.transform.Find("TooTipPanel").gameObject.SetActive(false);  //툴팁 비활성화.

	}
}
