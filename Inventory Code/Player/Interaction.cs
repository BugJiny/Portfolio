#define DEBUG
//#undef DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Interaction : MonoBehaviour
{
	
	public GameObject interaction_img;  //��ȣ�ۿ� �̹���


	public GameObject Inventory;      //�κ��丮 ������Ʈ


	private Collider[] colls;
	private float range;  //��������

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
		//���� ���� ��ȣ�ۿ� ������ ������Ʈ�� ã��
		colls = Physics.OverlapSphere(transform.position, range, 1 << 6);   //Layer6�� => ��ȣ�ۿ� ������Ʈ

		//������Ʈ�� �ִٸ�
		if (colls.Length != 0)
		{
			//�ش� ������Ʈ�� ��ǥ���� ���� ���� ��ġ�� ������
			Vector3 pos = Camera.main.WorldToScreenPoint(colls[0].gameObject.transform.position);
			pos.y += 100.0f;


			//��ȣ�ۿ� UI �̹����� ��ǥ�� ���� �� Ȱ��ȭ
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
					if (!sloatArr[i].GetComponent<ItemType>().GetisSlot())  //������ ������ ����ִٸ�
					{
						isFull = false;
						index = i;  //�ش� �ε����� ����.
						int rand = Random.Range(0, 10);  //������� ������ ������ ����.

						if (rand >= 0 && rand < 5)            //50%Ȯ���� ������
						{
							type = 3;
						}
						else if (rand >= 5 && rand < 8)         //30% Ȯ�� ��
						{
							type = 2;
						}
						else if (rand >= 8 && rand < 10)       //20% Ȯ��  ����
						{
							type = 1;
						}
						break;

					}
				}

				//�κ��丮�� �� ������ �ʴٸ�.
				if (!isFull)
				{
					int randomItem = 0;
					int amount = 0;
					int equalIndex = 0;
					bool isItem = false;   //�ش������� ������������ �̹� �ִ���.


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


					//�ߺ������� ã�� ����
					if (index != 0)
					{
						for (int i = 0; i < index; i++)
						{

							if (sloatArr[i].GetComponent<ItemType>().item == (ItemType.Item)type &&          //������ Ÿ�԰� ������ �ε��� ��ȣ�� ���ٸ� ���� �������̱⶧����.
								sloatArr[i].GetComponent<ItemType>().GetImageIndex() == randomItem)
							{
								equalIndex = i;
								isItem = true;
								break;
							}

						}
					}


					if (isItem)  //�ߺ��������� �ִٸ�
					{
						int temp = sloatArr[equalIndex].GetComponent<ItemType>().GetAmount();
						amount += temp;
						//�ش� ������ŭ ���ؼ� �־���.
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
					Debug.Log("�κ��丮�� �� ���ֽ��ϴ�.");
				}

			}

#if(DEBUG)
			//Debug.Log(colls[0].gameObject.name);
#endif

		}
		else  //������Ʈ�� ���ٸ� ��ȣ�ۿ� UI �̹��� ��Ȱ��ȭ
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
