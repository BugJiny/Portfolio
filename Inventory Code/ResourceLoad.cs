using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoad : MonoBehaviour
{

	public static Sprite[] weapon_img = new Sprite[4];         //����,���̽�,������,��
	public static Sprite[] armor_img = new Sprite[6];          //�Ź�,����,�尩,�Ӹ�,����,���(�Ϲݵ��)
	public static Sprite[] R_armor_img = new Sprite[6];        //�Ź�,����,�尩,�Ӹ�,����,���(������)
	public static Sprite[] item_img = new Sprite[3];           //��,ö,��

	// Start is called before the first frame update
	void Start()
    {
		weapon_img = Resources.LoadAll<Sprite>("Weapon");
		armor_img = Resources.LoadAll<Sprite>("Armor");
		R_armor_img = Resources.LoadAll<Sprite>("RareArmor");
		item_img = Resources.LoadAll<Sprite>("Item");

	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
    }
}
