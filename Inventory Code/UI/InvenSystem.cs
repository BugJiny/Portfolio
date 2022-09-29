//#define DEBUG
#undef DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InvenSystem : MonoBehaviour
{

	private static InvenSystem instance = null;

	public List<GameObject> m_AllSlot;  //모든 슬롯을 관리해줄 리스트
	public RectTransform m_InvenRect;   //인벤토리 Rect
	public GameObject m_slotPrefab;     //슬롯 프리팹

	[SerializeField]
	private GameObject m_tooltip;        //툴팁 오브젝트.


	private int m_slotX = 4;                // 슬롯 x의 개수
	private int m_slotY = 6;                 // 슬롯 y의 개수
	private int m_slotGapX = 40;                   // 슬롯사이의 X간격.
	private int m_slotGapY = 35;                    // 슬롯사이의 Y간격.


	private int m_slotCount = 0;
	// Start is called before the first frame update


	private void Awake()
	{

		if(null == instance)
		{
			instance = this;

			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}




#if(DEBUG)
		int item_index = 0;
#endif
		for (int y = 0; y < m_slotY; y++)
		{
			for (int x = 0; x < m_slotX; x++)
			{
				m_slotCount++;

#if(DEBUG)
				if(m_slotCount == 4 || m_slotCount == 10)
				{
					item_index = 0;
				}
#endif

				// 슬롯프리팹을 복사함.
				GameObject slot = Instantiate(m_slotPrefab) as GameObject;

				// 슬롯의 RectTransform을 가져온다 -> 슬롯의 좌표를 설정해 줘야하기 때문에.
				RectTransform slotRect = slot.GetComponent<RectTransform>();

				//이름 셋팅
				slot.name = "ItemSlot" + m_slotCount;



				//좌표 셋팅.
				slotRect.localPosition = new Vector3((1112 + (m_slotGapX * (x + 1)) + slotRect.rect.width * x), (928 - (m_slotGapY * (y + 1)) - slotRect.rect.height * y), 0);

#if (DEBUG)
				if(m_slotCount < 4)
				{
					slot.GetComponent<ItemType>().SetisSlot(true);
					slot.GetComponent<ItemType>().SetImageIndex(item_index);
					slot.GetComponent<ItemType>().SetAmount(10);
					slot.GetComponent<ItemType>().item = (ItemType.Item)1;

				}
				else if(m_slotCount >= 4 && m_slotCount < 10)
				{

					slot.GetComponent<ItemType>().SetisSlot(true);
					slot.GetComponent<ItemType>().SetImageIndex(item_index);
					slot.GetComponent<ItemType>().SetAmount(10);
					slot.GetComponent<ItemType>().item = (ItemType.Item)2;
				}
				else if (m_slotCount >= 10 && m_slotCount < 13)
				{
					slot.GetComponent<ItemType>().SetisSlot(true);
					slot.GetComponent<ItemType>().SetImageIndex(item_index);
					slot.GetComponent<ItemType>().SetAmount(10);
					slot.GetComponent<ItemType>().item = (ItemType.Item)3;
				}

#else
				slot.GetComponent<ItemType>().SetisSlot(false);
				slot.GetComponent<ItemType>().SetImageIndex(0);
				slot.GetComponent<ItemType>().SetAmount(0);
				slot.GetComponent<ItemType>().item = ItemType.Item.DEFAULT;


				slot.GetComponent<ItemType>().SetSlotType(ItemType.SlotType.INVEN);

#endif

				slot.transform.parent = transform; //현재 슬롯의 부모를 설정.
				m_AllSlot.Add(slot);
#if (DEBUG)
				item_index++;
#endif
			}
		}


	}

	public static InvenSystem Instance
	{
		get 
		{
			if(null==instance)
			{
				return null;
			}
			return instance;


		}
	}

	public void ShowTooltip(Vector3 _pos, ItemType.Item type,int index)
	{
		//좌표 셋팅
		_pos.x += 100f;
		_pos.y -= 250f;
		m_tooltip.GetComponent<RectTransform>().anchoredPosition = _pos;

		//툴팁안에 텍스트 설정.

		if (type == ItemType.Item.WEAPON)
		{

			switch (index)
			{
				case 0:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "도끼";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "능력:공격력+20";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "도끼다";
					break;
				case 1:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "메이스";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "능력:공격력+15";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "메이스다";
					break;
				case 2:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "스태프";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "능력:공격력+8";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "스태프다";
					break;
				case 3:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "검";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "능력:공격력+10";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "검이다";
					break;
			}


		}
		else if(type ==ItemType.Item.ARMOR)
		{

			switch (index)
			{
				case 0:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "신발";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "속도+3";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "신발이다";
					break;
				case 1:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "갑옷";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "방어력+10";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "갑옷이다";

					break;
				case 2:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "장갑";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "치명+10";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "장갑이다";
					break;
				case 3:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "투구";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "방어+5,치명+2";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "투구이다";
					break;
				case 4:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "하의";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "방어+5,속도+1";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "하의다";
					break;
				case 5:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "어깨장식";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "방어+3,치명+8";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "어깨장식이다.";
					break;
			}

		}
		else if (type == ItemType.Item.ITEM)
		{

			switch (index)
			{
				case 0:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "빵";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "레어장비를 만드는 재료(신발,장갑)";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "빵이다";
					break;
				case 1:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "철";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "레어장비를 만드는 재료(갑옷,하의)";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "철이다";
					break;
				case 2:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "꽃";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "레어장비를 만드는 재료(머리,어깨)";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "꽃이다";
					break;
			}

		}


		m_tooltip.SetActive(true);


	}

	public void HideTooltip()
	{
		m_tooltip.SetActive(false);
	}
}
