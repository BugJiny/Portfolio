#define DEBUG
//#undef DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Interaction : MonoBehaviour
{
	
	public GameObject interaction_img;  //상호작용 이미지


	public GameObject Inventory;      //인벤토리 오브젝트


	private Collider[] colls;
	private float range;  //감지범위

    // Start is called before the first frame update
    void Start()
    {
		//Inventory = transform.Find("Inventory").gameObject;
		//interaction_img = transform.Find("Interaction").gameObject;


		range = 2f;
		interaction_img.SetActive(false);

	}

    // Update is called once per frame
    void Update()
    {
		//범위 내의 상호작용 가능한 오브젝트를 찾고
		colls = Physics.OverlapSphere(transform.position, range, 1 << 6);   //Layer6번 => 상호작용 오브젝트

		//오브젝트가 있다면
		if (colls.Length != 0)
		{
			//해당 오브젝트의 좌표보다 조금 높은 위치에 설정후
			Vector3 pos = Camera.main.WorldToScreenPoint(colls[0].gameObject.transform.position);
			pos.y += 100.0f;


			//상호작용 UI 이미지의 좌표를 설정 및 활성화
			interaction_img.transform.position = pos;
			interaction_img.SetActive(true);

			if(Input.GetKeyDown(KeyCode.F))
			{
				bool isFull = true;
				int index=0;
				int type=0;

				var sloatArr = Inventory.GetComponent<InvenSystem>().m_AllSlot.ToArray();

				for (int i = 0; i < sloatArr.Length; i++)
				{
					if (!sloatArr[i].GetComponent<ItemType>().GetisSlot())  //아이템 슬롯이 비어있다면
					{
						isFull = false;
						index = i;  //해당 인덱스를 저장.
						int rand = Random.Range(0, 10);  //얻어지는 아이템 종류를 결정.

						if (rand >= 0 && rand < 5)            //50%확률로 아이템
						{
							type = 3;
						}
						else if (rand >= 5 && rand < 8)         //30% 확률 방어구
						{
							type = 2;
						}
						else if (rand >= 8 && rand < 10)       //20% 확률  무기
						{
							type = 1;
						}
						break;

					}
				}

				//인벤토리가 꽉 차있지 않다면.
				if (!isFull)
				{
					int randomItem = 0;
					int amount = 0;
					int equalIndex = 0;
					bool isItem = false;   //해당종류와 같은아이템이 이미 있는지.


					switch ((ItemType.Item)type)
					{
						case ItemType.Item.WEAPON:
							randomItem = Random.Range(0, 4);
							amount = 1;
							break;
						case ItemType.Item.ARMOR:
							randomItem = Random.Range(0, 6);
							amount = 1;
							break;
						case ItemType.Item.ITEM:
							randomItem = Random.Range(0, 3);
							amount = 5;
							break;
					}


					//중복아이템 찾는 로직
					if (index != 0)
					{
						for (int i = 0; i < index; i++)
						{

							if (sloatArr[i].GetComponent<ItemType>().item == (ItemType.Item)type &&          //아이템 타입과 아이템 인덱스 번호가 같다면 같은 아이템이기때문에.
								sloatArr[i].GetComponent<ItemType>().GetImageIndex() == randomItem)
							{
								equalIndex = i;
								isItem = true;
								break;
							}

						}
					}


					if (isItem)  //중복아이템이 있다면
					{
						int temp = sloatArr[equalIndex].GetComponent<ItemType>().GetAmount();
						amount += temp;
						//해당 갯수만큼 더해서 넣어줌.
						sloatArr[equalIndex].GetComponent<ItemType>().SetAmount(amount);
					}
					else
					{
						sloatArr[index].GetComponent<ItemType>().SetAmount(amount);
						sloatArr[index].GetComponent<ItemType>().SetisSlot(true);
						sloatArr[index].GetComponent<ItemType>().SetImageIndex(randomItem);
						sloatArr[index].GetComponent<ItemType>().item = (ItemType.Item)type;
					}
				}
				else
				{
					Debug.Log("인벤토리가 꽉 차있습니다.");
				}

			}

#if(DEBUG)
			//Debug.Log(colls[0].gameObject.name);
#endif

		}
		else  //오브젝트가 없다면 상호작용 UI 이미지 비활성화
		{
			interaction_img.SetActive(false);
		}

    }


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, range);
	}


}
