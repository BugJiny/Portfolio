//#define DEBUG
#undef DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InvenSystem : MonoBehaviour
{

	private static InvenSystem instance = null;

	public List<GameObject> m_AllSlot;  //��� ������ �������� ����Ʈ
	public RectTransform m_InvenRect;   //�κ��丮 Rect
	public GameObject m_slotPrefab;     //���� ������

	[SerializeField]
	private GameObject m_tooltip;        //���� ������Ʈ.


	private int m_slotX = 4;                // ���� x�� ����
	private int m_slotY = 6;                 // ���� y�� ����
	private int m_slotGapX = 40;                   // ���Ի����� X����.
	private int m_slotGapY = 35;                    // ���Ի����� Y����.


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

				// ������������ ������.
				GameObject slot = Instantiate(m_slotPrefab) as GameObject;

				// ������ RectTransform�� �����´� -> ������ ��ǥ�� ������ ����ϱ� ������.
				RectTransform slotRect = slot.GetComponent<RectTransform>();

				//�̸� ����
				slot.name = "ItemSlot" + m_slotCount;



				//��ǥ ����.
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

				slot.transform.parent = transform; //���� ������ �θ� ����.
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
		//��ǥ ����
		_pos.x += 100f;
		_pos.y -= 250f;
		m_tooltip.GetComponent<RectTransform>().anchoredPosition = _pos;

		//�����ȿ� �ؽ�Ʈ ����.

		if (type == ItemType.Item.WEAPON)
		{

			switch (index)
			{
				case 0:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "����";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "�ɷ�:���ݷ�+20";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "������";
					break;
				case 1:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "���̽�";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "�ɷ�:���ݷ�+15";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "���̽���";
					break;
				case 2:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "������";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "�ɷ�:���ݷ�+8";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "��������";
					break;
				case 3:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "��";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "�ɷ�:���ݷ�+10";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "���̴�";
					break;
			}


		}
		else if(type ==ItemType.Item.ARMOR)
		{

			switch (index)
			{
				case 0:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "�Ź�";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "�ӵ�+3";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "�Ź��̴�";
					break;
				case 1:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "����";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "����+10";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "�����̴�";

					break;
				case 2:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "�尩";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "ġ��+10";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "�尩�̴�";
					break;
				case 3:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "����";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "���+5,ġ��+2";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "�����̴�";
					break;
				case 4:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "����";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "���+5,�ӵ�+1";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "���Ǵ�";
					break;
				case 5:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "������";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "���+3,ġ��+8";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "�������̴�.";
					break;
			}

		}
		else if (type == ItemType.Item.ITEM)
		{

			switch (index)
			{
				case 0:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "��";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "������� ����� ���(�Ź�,�尩)";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "���̴�";
					break;
				case 1:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "ö";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "������� ����� ���(����,����)";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "ö�̴�";
					break;
				case 2:
					m_tooltip.transform.GetChild(0).gameObject.GetComponent<Text>().text = "��";
					m_tooltip.transform.GetChild(1).gameObject.GetComponent<Text>().text = "������� ����� ���(�Ӹ�,���)";
					m_tooltip.transform.GetChild(2).gameObject.GetComponent<Text>().text = "���̴�";
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
