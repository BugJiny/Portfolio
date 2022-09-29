using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoSystem : MonoBehaviour
{

	
	public List<GameObject> m_AllSlot;  //��� ������ �������� ����Ʈ
	public RectTransform m_userInfoRect;   //�ռ�â Rect
	public GameObject m_slotPrefab;     //���� ������


	private string[] m_slotName = {"����" ,"�Ź�", "����", "�尩", "�Ӹ�", "����", "���" };

	private int m_slotX = 2;                // ���� x�� ����
	private int m_slotY = 3;                 // ���� y�� ����
	private int m_slotGapX = 250;                   // ���Ի����� X����.
	private int m_slotGapY = 180;                    // ���Ի����� Y����.

	private int m_slotCount = 0;

	private void Awake()
	{

		//���� ����
		m_slotCount++;

		// ������������ ������.
		GameObject slot = Instantiate(m_slotPrefab) as GameObject;

		// ������ RectTransform�� �����´� -> ������ ��ǥ�� ������ ����ϱ� ������.
		RectTransform slotRect = slot.GetComponent<RectTransform>();

		//�̸� ����
		slot.name = "ItemSlot" + m_slotCount;

		// -381 294       0 270
		//��ǥ ����.
		slotRect.localPosition = new Vector3(672f , 826f , 0f);

		slot.transform.GetChild(4).gameObject.SetActive(true);  //���â���� �ؿ� ���� �ؽ�Ʈ Ȱ��ȭ.
		slot.transform.GetChild(4).gameObject.GetComponent<Text>().text = m_slotName[m_slotCount - 1];
		slot.GetComponent<ItemType>().SetSlotType(ItemType.SlotType.USERINFO);  //slot�� UserInfo�Ҵ�.

		slot.transform.parent = transform.Find("ItemPanel").transform; //���� ������ �θ� ����.
		m_AllSlot.Add(slot);


		//�� ����.
		for (int y = 0; y < m_slotY; y++)
		{
			for (int x = 0; x < m_slotX; x++)
			{
				m_slotCount++;


				// ������������ ������.
				slot = Instantiate(m_slotPrefab) as GameObject;

				// ������ RectTransform�� �����´� -> ������ ��ǥ�� ������ ����ϱ� ������.
				slotRect = slot.GetComponent<RectTransform>();

				//�̸� ����
				slot.name = "ItemSlot" + m_slotCount;

				slot.GetComponent<ItemType>().SetisSlot(false);
				slot.GetComponent<ItemType>().SetImageIndex(0);
				slot.GetComponent<ItemType>().SetAmount(0);
				slot.GetComponent<ItemType>().item = ItemType.Item.WEAPON;

				//��ǥ ����.
				slotRect.localPosition = new Vector3((291 + (m_slotGapX * (x + 1)) ), (850 - (m_slotGapY * (y+1) ) ), 0);


				slot.GetComponent<ItemType>().SetisSlot(false);
				slot.GetComponent<ItemType>().SetImageIndex(m_slotCount-2);
				slot.GetComponent<ItemType>().SetAmount(0);
				slot.GetComponent<ItemType>().item = ItemType.Item.ARMOR;


				
				slot.transform.GetChild(4).gameObject.SetActive(true);  //���â���� �ؿ� ���� �ؽ�Ʈ Ȱ��ȭ.
				slot.transform.GetChild(4).gameObject.GetComponent<Text>().text = m_slotName[m_slotCount - 1];
				slot.GetComponent<ItemType>().SetSlotType(ItemType.SlotType.USERINFO);  //slot�� UserInfo�Ҵ�.



				slot.transform.parent = transform.Find("ItemPanel").transform; //���� ������ �θ� ����.
				m_AllSlot.Add(slot);

			}
		}





	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
