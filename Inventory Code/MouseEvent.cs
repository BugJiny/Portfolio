using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MouseEvent : MonoBehaviour
{

	public GameObject selectObj;    //� �̹����� ������ �Ǿ�����.
	public Canvas m_canvas;


	private GraphicRaycaster m_gr;        //��ġ1
	private PointerEventData m_ped;       

	private GraphicRaycaster m_gr2;       //��ġ2
	private PointerEventData m_ped2;


	private bool isDrag = false;          //�巡�����ΰ�
	private bool notSlot = false;         //���Ը��� �ٸ����� �����ٸ�.

	private ItemType m_currentslot;      //���罽������
	private ItemType m_nextslot;         //������������

	private ItemType tempslot;           //�������� �ٲܶ� �ӽ������� �� ����


	public void OnPointerDown()
	{

		if (!InvenController.isInven && !UserInfoController.isInfo)
			return;

		// ������ ��ü�̵�
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
				//	Debug.Log("���չ�ư�� �������ϴ�.");
				//	notSlot = true;
				//	return;
				//}


				if (results[0].gameObject.name == "Background")
				{
					Debug.Log("Ŭ��Down)�� �ùٸ��� �ʽ��ϴ�.(���)");
					notSlot = true;
					isDrag = false;
					return;
				}
				else if(results[0].gameObject.name == "UserStatPanel")
				{
					Debug.Log("Ŭ��(Down)�� �ùٸ��� �ʽ��ϴ�.(�����г�)");
					notSlot = true;
					isDrag = false;
					return;
				}
				else if(results[0].gameObject.name == "ItemPanel")
				{
					Debug.Log("Ŭ��(Down)�� �ùٸ��� �ʽ��ϴ�.(������ �г�)");
					notSlot = true;
					isDrag = false;
					return;
				}
				else if (results[0].gameObject.transform.parent.name == "Interaction")
				{
					Debug.Log("Ŭ��(Up)�� �ùٸ��� �ʽ��ϴ�.(��ȣ�ۿ� ��ư)");
					notSlot = true;
					isDrag = false;
					return;
				}

				m_currentslot = results[0].gameObject.transform.parent.gameObject.GetComponent<ItemType>();   //������ Ŭ�������� Ŭ���� ������Ʈ�� ���������� ������.



				//�ش� ���Կ� �������� �ִٸ�.
				if (m_currentslot.GetisSlot())  
				{
					m_currentslot.transform.Find("Highlight Image").gameObject.SetActive(true);  //���� ������ ������ ���̶���Ʈ �̹����� ���ش�.


					//�������� ���õǾ����� �̹��� ������Ʈ�� Ȱ��ȭ�ϰ� ���� ���콺 ��ġ���� ���õ� �̹����� �����´�.
					selectObj.SetActive(true);     
					selectObj.transform.position = Input.mousePosition;
					selectObj.GetComponent<Image>().sprite = m_currentslot.GetItemImage();  


					//selectObj.transform.GetSiblingIndex();
					selectObj.transform.SetAsLastSibling();     //���̾ �ֻ������ �÷��ش�.


					isDrag = true;
					Debug.Log("�ش� ĭ���� ������������.");
				}
				else
				{
					isDrag = false;
					Debug.Log("����");
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

			//������ �̵��� ���� �κ�. 
			//�ش� Slot�� ����ִ°�? ��������� �ش罽������ ������ ������ ������ �ִ±״�� �ѱ��.
			//�ش� Slot�� ��������ʰ� �������� �ִٸ� �� �������� �����Ѵ�.

			//���õ� �̹����� ������Ʈ�� ��Ȱ��ȭ ���ش�.
			selectObj.SetActive(false);
			isDrag = false;


			if (!notSlot && m_currentslot != null)  //������ �ƴѰ��� ������ ���̶���Ʈ �̹����� ���ش�.
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
					Debug.Log("Ŭ��(Up)�� �ùٸ��� �ʽ��ϴ�.");
					return;
				}
				else if (results[0].gameObject.name == "UserStatPanel")
				{
					Debug.Log("Ŭ��(Up)�� �ùٸ��� �ʽ��ϴ�.(�����г�)");
					return;
				}
				else if (results[0].gameObject.name == "ItemPanel")
				{
					Debug.Log("Ŭ��(Up)�� �ùٸ��� �ʽ��ϴ�.(������ �г�)");
					return;
				}
				else if (results[0].gameObject.transform.parent.name == "Interaction")
				{
					Debug.Log("Ŭ��(Up)�� �ùٸ��� �ʽ��ϴ�.(��ȣ�ۿ� ��ư)");
					return;
				}
				

				m_nextslot = results[0].gameObject.transform.parent.gameObject.GetComponent<ItemType>();

				if (m_currentslot == m_nextslot)  //���� ���� Ŭ���ߴٸ�.
					return;


				//���� �κ� ���Կ��� ��� �������� �̵��ϴ� ���̶��.
				if (m_currentslot.GetSlotType() == ItemType.SlotType.INVEN && m_nextslot.GetSlotType() == ItemType.SlotType.USERINFO)
				{
					if(m_currentslot.item ==ItemType.Item.ITEM)
					{
						Debug.Log("��� �������� ���â�� ����� �� �����ϴ�.");
						return;
					}
					else if(m_currentslot.item == ItemType.Item.WEAPON && m_nextslot.item == ItemType.Item.ARMOR)
					{
						Debug.Log("���� �������� �� ���Կ� ���� �� �� �����ϴ�.");
						return;
					}
					else if(m_currentslot.item ==ItemType.Item.ARMOR)
					{

						if(m_nextslot.item == ItemType.Item.WEAPON)
						{
							Debug.Log("�� �������� ���� ���Կ� ���� �� �� �����ϴ�.");
							return;
						}
						else if(m_nextslot.item ==ItemType.Item.ARMOR && m_currentslot.GetImageIndex() != m_nextslot.GetImageIndex())
						{
							Debug.Log("�� ������ ���� ������ ���� �ʽ��ϴ�.");
							return;
						}
					}
				}  
				else if (m_currentslot.GetSlotType() == ItemType.SlotType.USERINFO && m_nextslot.GetSlotType() ==ItemType.SlotType.USERINFO)  //���â���� ���â�� �̵��ϴ� ���̶��.
				{

					Debug.Log("�ش� �������� �ٸ� ��� �������� �ű� �� �����ϴ�.");
					return;


				}


				if (m_nextslot.GetisSlot() )   //�������� �ִٸ�
				{
					if (m_currentslot.item == m_nextslot.item && m_currentslot.GetImageIndex() == m_nextslot.GetImageIndex())      //�ν����� �������� ���ٸ� => ��ġ��
					{
						if(m_currentslot.GetSlotType() == ItemType.SlotType.INVEN && m_nextslot.GetSlotType() == ItemType.SlotType.USERINFO)
						{
							Debug.Log("�ش� �������� �̹� ���Ǿ� �ֽ��ϴ�. ��� ������ �õ��� �ּ���...");
							return;
						}

						ChangeUserStat(m_currentslot, m_nextslot);

						int temp = m_currentslot.GetAmount() + m_nextslot.GetAmount();
						m_nextslot.SetAmount(temp);
						m_currentslot.InfoClear();  //���� ���Կ� �ִ� ������ �ʱ�ȭ.
						

					}
					else            //�� ������ �������� ���� �ʴٸ� => ����.
					{
						
						if(m_currentslot.GetAmount() > 1)
						{
							Debug.Log("������ ���Ƽ� �̵��� �� �����ϴ�. ������� ������ �õ��� �ּ���.");
						}
						else
						{
							ChangeUserStat(m_currentslot, m_nextslot);

							//���� �ڵ�
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
				else                         //������쿣 �������� �ش� �������� �ű��.
				{

					if( m_currentslot.GetSlotType() ==ItemType.SlotType.INVEN &&m_nextslot.GetSlotType() == ItemType.SlotType.USERINFO)  //�κ����� ��� â���� �̵��� ���.
					{

						//���� �ɷ�ġ ��ȭ.
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

		if(current.GetSlotType() == ItemType.SlotType.USERINFO)  //���â���� �������� �����.
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
		else if( !next.GetisSlot() && current.GetSlotType() == ItemType.SlotType.INVEN)        //���â(����)�� �� ����
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
