using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemType : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{

	public enum SlotType {INVEN=1,USERINFO,SYNTHESIS, DEFAULT=-1 }  //인벤토리,유저정보창(장비창),합성창
	public enum Item {WEAPON=1,ARMOR,ITEM ,DEFAULT = -1 }  // 무기,방어구,아이템



	public SlotType slotType = SlotType.DEFAULT;  //현재 슬롯의 종류가 무엇인지.
	public Item item = Item.DEFAULT;  //해당 아이템 타입.
	private int amount = 0;   //수량
	private int imgIndex = 0;  //이미지 번호
	private Sprite ItemImg;  //아이템 이미지.
	private bool isSlot = false;  //현재 슬롯에 재료가 있는지.

	



	//[SerializeField]
	//private bool ResultSlot = false;    //  결과창 슬롯인지.


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
