using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MouseEvent : MonoBehaviour
{

	public GameObject selectObj;    //어떤 이미지가 선택이 되었는지.
	public Canvas m_canvas;


	private GraphicRaycaster m_gr;        //위치1
	private PointerEventData m_ped;       

	private GraphicRaycaster m_gr2;       //위치2
	private PointerEventData m_ped2;


	private bool isDrag = false;          //드래그중인가
	private bool notSlot = false;         //슬롯말고 다른곳을 눌렀다면.

	private ItemType m_currentslot;      //현재슬롯정보
	private ItemType m_nextslot;         //다음슬롯정보

	private ItemType tempslot;           //아이템을 바꿀때 임시적으로 쓸 변수


	public void OnPointerDown()
	{

		if (!InvenController.isInven && !UserInfoController.isInfo)
			return;

		// 왼쪽은 전체이동
		if (Input.GetMouseButtonDown(0))
		{
			List<RaycastResult> results = new List<RaycastResult>();
			m_gr.Raycast(m_ped, results);
			notSlot = false;

			if (results.Count > 0)
			{
				Debug.Log(results[0]);


				//if (results[0].gameObject.name == "Text")
				//{
				//	Debug.Log("조합버튼을 눌렀습니다.");
				//	notSlot = true;
				//	return;
				//}


				if (results[0].gameObject.name == "Background")
				{
					Debug.Log("클릭Down)이 올바르지 않습니다.(배경)");
					notSlot = true;
					isDrag = false;
					return;
				}
				else if(results[0].gameObject.name == "UserStatPanel")
				{
					Debug.Log("클릭(Down)이 올바르지 않습니다.(유저패널)");
					notSlot = true;
					isDrag = false;
					return;
				}
				else if(results[0].gameObject.name == "ItemPanel")
				{
					Debug.Log("클릭(Down)이 올바르지 않습니다.(아이템 패널)");
					notSlot = true;
					isDrag = false;
					return;
				}
				else if (results[0].gameObject.transform.parent.name == "Interaction")
				{
					Debug.Log("클릭(Up)이 올바르지 않습니다.(상호작용 버튼)");
					notSlot = true;
					isDrag = false;
					return;
				}

				m_currentslot = results[0].gameObject.transform.parent.gameObject.GetComponent<ItemType>();   //슬롯을 클릭했을때 클릭한 오브젝트의 슬롯정보를 가져옴.



				//해당 슬롯에 아이템이 있다면.
				if (m_currentslot.GetisSlot())  
				{
					m_currentslot.transform.Find("Highlight Image").gameObject.SetActive(true);  //현재 선택한 슬롯의 하이라이트 이미지를 켜준다.


					//아이템이 선택되었을때 이미지 오브젝트를 활성화하고 현재 마우스 위치값과 선택된 이미지를 가져온다.
					selectObj.SetActive(true);     
					selectObj.transform.position = Input.mousePosition;
					selectObj.GetComponent<Image>().sprite = m_currentslot.GetItemImage();  


					//selectObj.transform.GetSiblingIndex();
					selectObj.transform.SetAsLastSibling();     //레이어를 최상단으로 올려준다.


					isDrag = true;
					Debug.Log("해당 칸에는 아이템이있음.");
				}
				else
				{
					isDrag = false;
					Debug.Log("없음");
				}
			}


		}
	}

	public void OnPointDrag()
	{
		if (!isDrag) return;

		if (Input.GetMouseButton(0))
		{
			selectObj.transform.position = Input.mousePosition;
		}

	}

	private void OnPointerUp()
	{
		if (!InvenController.isInven && !UserInfoController.isInfo)
			return;

		if (Input.GetMouseButtonUp(0))
		{

			//아이템 이동에 관한 부분. 
			//해당 Slot이 비어있는가? 비어있으면 해당슬롯으로 현재의 아이템 정보를 있는그대로 넘긴다.
			//해당 Slot이 비어있지않고 아이템이 있다면 두 아이템을 스왑한다.

			//선택된 이미지의 오브젝트를 비활성화 해준다.
			selectObj.SetActive(false);
			isDrag = false;


			if (!notSlot && m_currentslot != null)  //슬롯이 아닌곳을 눌렀면 하이라이트 이미지를 꺼준다.
			{
				m_currentslot.transform.Find("Highlight Image").gameObject.SetActive(false);
			}


			List<RaycastResult> results = new List<RaycastResult>();
			m_ped2.position = Input.mousePosition;
			m_gr2.Raycast(m_ped2, results);

			if (results.Count > 0)
			{
				if (results[0].gameObject.name == "Background")
				{
					Debug.Log("클릭(Up)이 올바르지 않습니다.");
					return;
				}
				else if (results[0].gameObject.name == "UserStatPanel")
				{
					Debug.Log("클릭(Up)이 올바르지 않습니다.(유저패널)");
					return;
				}
				else if (results[0].gameObject.name == "ItemPanel")
				{
					Debug.Log("클릭(Up)이 올바르지 않습니다.(아이템 패널)");
					return;
				}
				else if (results[0].gameObject.transform.parent.name == "Interaction")
				{
					Debug.Log("클릭(Up)이 올바르지 않습니다.(상호작용 버튼)");
					return;
				}
				

				m_nextslot = results[0].gameObject.transform.parent.gameObject.GetComponent<ItemType>();

				if (m_currentslot == m_nextslot)  //같은 곳을 클릭했다면.
					return;


				//만약 인벤 슬롯에서 장비 슬롯으로 이동하는 것이라면.
				if (m_currentslot.GetSlotType() == ItemType.SlotType.INVEN && m_nextslot.GetSlotType() == ItemType.SlotType.USERINFO)
				{
					if(m_currentslot.item ==ItemType.Item.ITEM)
					{
						Debug.Log("재료 아이템은 장비창에 등록할 수 없습니다.");
						return;
					}
					else if(m_currentslot.item == ItemType.Item.WEAPON && m_nextslot.item == ItemType.Item.ARMOR)
					{
						Debug.Log("무기 아이템을 방어구 슬롯에 장착 할 수 없습니다.");
						return;
					}
					else if(m_currentslot.item ==ItemType.Item.ARMOR)
					{

						if(m_nextslot.item == ItemType.Item.WEAPON)
						{
							Debug.Log("방어구 아이템을 무기 슬롯에 장착 할 수 없습니다.");
							return;
						}
						else if(m_nextslot.item ==ItemType.Item.ARMOR && m_currentslot.GetImageIndex() != m_nextslot.GetImageIndex())
						{
							Debug.Log("방어구 종류와 장착 슬롯이 맞지 않습니다.");
							return;
						}
					}
				}  
				else if (m_currentslot.GetSlotType() == ItemType.SlotType.USERINFO && m_nextslot.GetSlotType() ==ItemType.SlotType.USERINFO)  //장비창에서 장비창을 이동하는 것이라면.
				{

					Debug.Log("해당 아이템을 다른 장비 슬롯으로 옮길 수 없습니다.");
					return;


				}


				if (m_nextslot.GetisSlot() )   //아이템이 있다면
				{
					if (m_currentslot.item == m_nextslot.item && m_currentslot.GetImageIndex() == m_nextslot.GetImageIndex())      //두슬롯의 아이템이 같다면 => 합치기
					{
						if(m_currentslot.GetSlotType() == ItemType.SlotType.INVEN && m_nextslot.GetSlotType() == ItemType.SlotType.USERINFO)
						{
							Debug.Log("해당 아이템이 이미 장비되어 있습니다. 장비를 해제후 시도해 주세요...");
							return;
						}

						ChangeUserStat(m_currentslot, m_nextslot);

						int temp = m_currentslot.GetAmount() + m_nextslot.GetAmount();
						m_nextslot.SetAmount(temp);
						m_currentslot.InfoClear();  //현재 슬롯에 있는 정보는 초기화.
						

					}
					else            //두 슬롯의 아이템이 같지 않다면 => 스왑.
					{
						
						if(m_currentslot.GetAmount() > 1)
						{
							Debug.Log("개수가 많아서 이동할 수 없습니다. 장착장비를 해제후 시도해 주세요.");
						}
						else
						{
							ChangeUserStat(m_currentslot, m_nextslot);

							//스왑 코드
							tempslot.SetImageIndex(m_currentslot.GetImageIndex());
							tempslot.SetItemImage(m_currentslot.GetItemImage());
							tempslot.SetAmount(m_currentslot.GetAmount());
							tempslot.item = m_currentslot.item;

							m_currentslot.SetImageIndex(m_nextslot.GetImageIndex());
							m_currentslot.SetItemImage(m_nextslot.GetItemImage());
							m_currentslot.SetAmount(m_nextslot.GetAmount());
							m_currentslot.item = m_nextslot.item;


							m_nextslot.SetImageIndex(tempslot.GetImageIndex());
							m_nextslot.SetItemImage(tempslot.GetItemImage());
							m_nextslot.SetAmount(tempslot.GetAmount());
							m_nextslot.item = tempslot.item;


						}

					

						tempslot.InfoClear();

					}


					return;
				}
				else                         //없을경우엔 아이템을 해당 슬롯으로 옮긴다.
				{

					if( m_currentslot.GetSlotType() ==ItemType.SlotType.INVEN &&m_nextslot.GetSlotType() == ItemType.SlotType.USERINFO)  //인벤에서 장비 창으로 이동할 경우.
					{

						//유정 능력치 변화.
						ChangeUserStat(m_currentslot, m_nextslot);

						m_nextslot.SetisSlot(true);
						m_nextslot.SetImageIndex(m_currentslot.GetImageIndex());
						m_nextslot.SetItemImage(m_currentslot.GetItemImage());
						m_nextslot.SetAmount(1);
						m_nextslot.item = m_currentslot.item;


						int num = m_currentslot.GetAmount();
						num -= 1;
						if(num ==0)
						{
							m_currentslot.InfoClear();
						}
						else
						{
							m_currentslot.SetAmount(num);
						}

						

						return;
					}

					ChangeUserStat(m_currentslot, m_nextslot);

					m_nextslot.SetisSlot(true);
					m_nextslot.SetImageIndex(m_currentslot.GetImageIndex());
					m_nextslot.SetItemImage(m_currentslot.GetItemImage());
					m_nextslot.SetAmount(m_currentslot.GetAmount());
					m_nextslot.item = m_currentslot.item;

					if(m_currentslot.GetSlotType() == ItemType.SlotType.USERINFO)
					{
						m_currentslot.SetisSlot(false);
						m_currentslot.SetItemImage(null);
						m_currentslot.SetAmount(0);
					}
					else
						m_currentslot.InfoClear();


					


					return;
				}

			}


		}


	}


	private void ChangeUserStat(ItemType current, ItemType next)
	{

		if(current.GetSlotType() == ItemType.SlotType.USERINFO)  //장비창에서 아이템을 뺄경우.
		{


			if(!next.GetisSlot())
			{
				if (current.item == ItemType.Item.WEAPON)
				{
					switch (current.GetImageIndex())
					{
						case 0:
							PlayerMove.m_status.addAtk -= 20.0f;
							break;
						case 1:
							PlayerMove.m_status.addAtk -= 15.0f;
							break;
						case 2:
							PlayerMove.m_status.addAtk -= 8.0f;
							break;
						case 3:
							PlayerMove.m_status.addAtk -= 10.0f;
							break;
					}


				}
				else if (current.item == ItemType.Item.ARMOR)
				{

					switch (current.GetImageIndex())
					{
						case 0:
							PlayerMove.m_status.addSpd -= 3.0f;
							break;
						case 1:
							PlayerMove.m_status.addDef -= 10.0f;
							break;
						case 2:
							PlayerMove.m_status.addCri -= 10.0f;
							break;
						case 3:
							PlayerMove.m_status.addDef -= 5.0f;
							PlayerMove.m_status.addCri -= 2.0f;
							break;
						case 4:
							PlayerMove.m_status.addDef -= 5.0f;
							PlayerMove.m_status.addSpd -= 1.0f;
							break;
						case 5:
							PlayerMove.m_status.addDef -= 3.0f;
							PlayerMove.m_status.addCri -= 8.0f;
							break;
					}

				}
			}
			else
			{
				if (current.item == ItemType.Item.WEAPON)
				{

					if(current.GetImageIndex() ==next.GetImageIndex())
					{
						switch (current.GetImageIndex())
						{
							case 0:
								PlayerMove.m_status.addAtk -= 20.0f;
								break;
							case 1:
								PlayerMove.m_status.addAtk -= 15.0f;
								break;
							case 2:
								PlayerMove.m_status.addAtk -= 8.0f;
								break;
							case 3:
								PlayerMove.m_status.addAtk -= 10.0f;
								break;
						}
					}
					else
					{
						switch (current.GetImageIndex())
						{
							case 0:
								PlayerMove.m_status.addAtk -= 20.0f;
								break;
							case 1:
								PlayerMove.m_status.addAtk -= 15.0f;
								break;
							case 2:
								PlayerMove.m_status.addAtk -= 8.0f;
								break;
							case 3:
								PlayerMove.m_status.addAtk -= 10.0f;
								break;
						}


						switch (next.GetImageIndex())
						{
							case 0:
								PlayerMove.m_status.addAtk += 20.0f;
								break;
							case 1:
								PlayerMove.m_status.addAtk += 15.0f;
								break;
							case 2:
								PlayerMove.m_status.addAtk += 8.0f;
								break;
							case 3:
								PlayerMove.m_status.addAtk += 10.0f;
								break;
						}

					}

					

				

				}
				else if (current.item == ItemType.Item.ARMOR)
				{

					switch (current.GetImageIndex())
					{
						case 0:
							PlayerMove.m_status.addSpd -= 3.0f;
							break;
						case 1:
							PlayerMove.m_status.addDef -= 10.0f;
							break;
						case 2:
							PlayerMove.m_status.addCri -= 10.0f;
							break;
						case 3:
							PlayerMove.m_status.addDef -= 5.0f;
							PlayerMove.m_status.addCri -= 2.0f;
							break;
						case 4:
							PlayerMove.m_status.addDef -= 5.0f;
							PlayerMove.m_status.addSpd -= 1.0f;
							break;
						case 5:
							PlayerMove.m_status.addDef -= 3.0f;
							PlayerMove.m_status.addCri -= 8.0f;
							break;
					}

				}
			}



			
		}
		else if( !next.GetisSlot() && current.GetSlotType() == ItemType.SlotType.INVEN)        //장비창(없음)에 값 셋팅
		{

			if (current.item == ItemType.Item.WEAPON)
			{
				switch (current.GetImageIndex())
				{
					case 0:
						PlayerMove.m_status.addAtk += 20.0f;
						break;
					case 1:
						PlayerMove.m_status.addAtk += 15.0f;
						break;
					case 2:
						PlayerMove.m_status.addAtk += 8.0f;
						break;
					case 3:
						PlayerMove.m_status.addAtk += 10.0f;
						break;
				}


			}
			else if (current.item == ItemType.Item.ARMOR)
			{

				switch (current.GetImageIndex())
				{
					case 0:
						PlayerMove.m_status.addSpd += 3.0f;
						break;
					case 1:
						PlayerMove.m_status.addDef += 10.0f;
						break;
					case 2:
						PlayerMove.m_status.addCri += 10.0f;
						break;
					case 3:
						PlayerMove.m_status.addDef += 5.0f;
						PlayerMove.m_status.addCri += 2.0f;
						break;
					case 4:
						PlayerMove.m_status.addDef += 5.0f;
						PlayerMove.m_status.addSpd += 1.0f;
						break;
					case 5:
						PlayerMove.m_status.addDef += 3.0f;
						PlayerMove.m_status.addCri += 8.0f;
						break;
				}

			}


		}




	}


	// Start is called before the first frame update
	void Start()
    {

		m_gr = m_canvas.GetComponent<GraphicRaycaster>();
		m_ped = new PointerEventData(null);

		m_gr2 = m_canvas.GetComponent<GraphicRaycaster>();
		m_ped2 = new PointerEventData(null);

		m_currentslot = new ItemType();
		m_nextslot = new ItemType();

		tempslot = new ItemType();

	}

    // Update is called once per frame
    void Update()
    {
		m_ped.position = Input.mousePosition;

		OnPointerDown();
		OnPointDrag();
		OnPointerUp();
	}
}
