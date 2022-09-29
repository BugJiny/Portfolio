using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoSystem : MonoBehaviour
{

	
	public List<GameObject> m_AllSlot;  //모든 슬롯을 관리해줄 리스트
	public RectTransform m_userInfoRect;   //합성창 Rect
	public GameObject m_slotPrefab;     //슬롯 프리팹


	private string[] m_slotName = {"무기" ,"신발", "갑옷", "장갑", "머리", "하의", "숄더" };

	private int m_slotX = 2;                // 슬롯 x의 개수
	private int m_slotY = 3;                 // 슬롯 y의 개수
	private int m_slotGapX = 250;                   // 슬롯사이의 X간격.
	private int m_slotGapY = 180;                    // 슬롯사이의 Y간격.

	private int m_slotCount = 0;

	private void Awake()
	{

		//무기 슬롯
		m_slotCount++;

		// 슬롯프리팹을 복사함.
		GameObject slot = Instantiate(m_slotPrefab) as GameObject;

		// 슬롯의 RectTransform을 가져온다 -> 슬롯의 좌표를 설정해 줘야하기 때문에.
		RectTransform slotRect = slot.GetComponent<RectTransform>();

		//이름 셋팅
		slot.name = "ItemSlot" + m_slotCount;

		// -381 294       0 270
		//좌표 셋팅.
		slotRect.localPosition = new Vector3(672f , 826f , 0f);

		slot.transform.GetChild(4).gameObject.SetActive(true);  //장비창에서 밑에 보일 텍스트 활성화.
		slot.transform.GetChild(4).gameObject.GetComponent<Text>().text = m_slotName[m_slotCount - 1];
		slot.GetComponent<ItemType>().SetSlotType(ItemType.SlotType.USERINFO);  //slot에 UserInfo할당.

		slot.transform.parent = transform.Find("ItemPanel").transform; //현재 슬롯의 부모를 설정.
		m_AllSlot.Add(slot);


		//방어구 슬롯.
		for (int y = 0; y < m_slotY; y++)
		{
			for (int x = 0; x < m_slotX; x++)
			{
				m_slotCount++;


				// 슬롯프리팹을 복사함.
				slot = Instantiate(m_slotPrefab) as GameObject;

				// 슬롯의 RectTransform을 가져온다 -> 슬롯의 좌표를 설정해 줘야하기 때문에.
				slotRect = slot.GetComponent<RectTransform>();

				//이름 셋팅
				slot.name = "ItemSlot" + m_slotCount;

				slot.GetComponent<ItemType>().SetisSlot(false);
				slot.GetComponent<ItemType>().SetImageIndex(0);
				slot.GetComponent<ItemType>().SetAmount(0);
				slot.GetComponent<ItemType>().item = ItemType.Item.WEAPON;

				//좌표 셋팅.
				slotRect.localPosition = new Vector3((291 + (m_slotGapX * (x + 1)) ), (850 - (m_slotGapY * (y+1) ) ), 0);


				slot.GetComponent<ItemType>().SetisSlot(false);
				slot.GetComponent<ItemType>().SetImageIndex(m_slotCount-2);
				slot.GetComponent<ItemType>().SetAmount(0);
				slot.GetComponent<ItemType>().item = ItemType.Item.ARMOR;


				
				slot.transform.GetChild(4).gameObject.SetActive(true);  //장비창에서 밑에 보일 텍스트 활성화.
				slot.transform.GetChild(4).gameObject.GetComponent<Text>().text = m_slotName[m_slotCount - 1];
				slot.GetComponent<ItemType>().SetSlotType(ItemType.SlotType.USERINFO);  //slot에 UserInfo할당.



				slot.transform.parent = transform.Find("ItemPanel").transform; //현재 슬롯의 부모를 설정.
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
