using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemType : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{

	public enum SlotType {INVEN=1,USERINFO,SYNTHESIS, DEFAULT=-1 }  //�κ��丮,��������â(���â),�ռ�â
	public enum Item {WEAPON=1,ARMOR,ITEM ,DEFAULT = -1 }  // ����,��,������



	public SlotType slotType = SlotType.DEFAULT;  //���� ������ ������ ��������.
	public Item item = Item.DEFAULT;  //�ش� ������ Ÿ��.
	private int amount = 0;   //����
	private int imgIndex = 0;  //�̹��� ��ȣ
	private Sprite ItemImg;  //������ �̹���.
	private bool isSlot = false;  //���� ���Կ� ��ᰡ �ִ���.

	



	//[SerializeField]
	//private bool ResultSlot = false;    //  ���â ��������.


	//public bool GetResultSlot() { return ResultSlot; }
	//public void SetResultSlot(bool m) { ResultSlot = m; }



	public SlotType GetSlotType() { return slotType; }
	public void SetSlotType(SlotType s) { slotType = s; }

	public bool GetisSlot() { return isSlot; }
	public void SetisSlot(bool slot) { isSlot = slot; }

	public int GetImageIndex() { return imgIndex; }
	public void SetImageIndex(int index) { imgIndex = index; }

	public Sprite GetItemImage() { return ItemImg; }
	public void SetItemImage(Sprite img) { ItemImg = img; }

	public int GetAmount() { return amount; }
	public void SetAmount(int m) { amount = m; }


	public void InfoClear()
	{
		amount = 0;
		imgIndex = 0;
		ItemImg = null;
		isSlot = false;
		item = Item.DEFAULT;
	}

	// Start is called before the first frame update
	void Start()
    {
		if (isSlot)
		{

			switch (item)
			{
				case Item.WEAPON:
					transform.GetChild(1).GetComponent<Image>().sprite = ResourceLoad.weapon_img[imgIndex];
					ItemImg = ResourceLoad.weapon_img[imgIndex];
					break;
				case Item.ARMOR:
					transform.GetChild(1).GetComponent<Image>().sprite = ResourceLoad.armor_img[imgIndex];
					ItemImg = ResourceLoad.armor_img[imgIndex];
					break;
				case Item.ITEM:
					transform.GetChild(1).GetComponent<Image>().sprite = ResourceLoad.item_img[imgIndex];
					ItemImg = ResourceLoad.item_img[imgIndex];
					break;
				case Item.DEFAULT:
					transform.GetChild(1).GetComponent<Image>().sprite = null;
					ItemImg = null;
					break;
			}

			transform.GetChild(2).GetComponent<Text>().text = amount.ToString();
		}
	}

    // Update is called once per frame
    void Update()
    {

		if (isSlot)
		{
			switch (item)
			{
				case Item.WEAPON:
					transform.GetChild(1).GetComponent<Image>().sprite = ResourceLoad.weapon_img[imgIndex];
					ItemImg = ResourceLoad.weapon_img[imgIndex];
					break;
				case Item.ARMOR:
					transform.GetChild(1).GetComponent<Image>().sprite = ResourceLoad.armor_img[imgIndex];
					ItemImg = ResourceLoad.armor_img[imgIndex];
					break;
				case Item.ITEM:
					transform.GetChild(1).GetComponent<Image>().sprite = ResourceLoad.item_img[imgIndex];
					ItemImg = ResourceLoad.item_img[imgIndex];
					break;
				case Item.DEFAULT:
					transform.GetChild(1).GetComponent<Image>().sprite = null;
					ItemImg = null;
					break;
			}

			transform.GetChild(2).GetComponent<Text>().text = amount.ToString();


		}
		else
		{
			transform.GetChild(1).GetComponent<Image>().sprite = null;
			transform.GetChild(2).GetComponent<Text>().text = "";
		}


	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		
		if(isSlot)
		{
			InvenSystem.Instance.ShowTooltip(this.transform.localPosition,item, GetImageIndex());
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		
		InvenSystem.Instance.HideTooltip();
	}
}
