using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SkillData : MonoBehaviour
{

	private static SkillData instance = null;

	public List<skillData> skillList = new List<skillData>();   //��ų�� ������ ������ �ִ� ����Ʈ.
	public List<skillData> LevelSortSkillList = new List<skillData>();  //������ ���ĵ� ����Ʈ.

	public Sprite[] sprites;  //����� ��ų �̹��� ���ҽ�


	enum SkillType
	{ 
	
		Basic=0,
		Link =1
	}


	public struct skillData
	{

		public Sprite icon;      //������
		public string name;      //�̸�
		public int level;        //��ų ���� �ִ� ����
		public int point;        //��ų ����Ʈ(�ִ� ����)
		public int currentlevel; //���� ��ų����.
		public List<int> linkSkillList; //�ڽ����� ��ũ�� �ɷ����ִ� ��ų���� ����Ʈ.
		public List<string> linkSkillNameList;  //�ڽ����� ��ũ�� �ɸ� ��ų���� �̸�.

		//������ ����.
		public skillData(Sprite _icon,string _name,int _level,int _point)
		{
			icon = _icon;
			name = _name;
			level = _level;
			point = _point;
			currentlevel = 0;  //�ʱⰪ 0���� ����
			linkSkillList = new List<int>();
			linkSkillNameList = new List<string>();
		}


		public void LinkSkillAdd(int skillIndex,string skillName)
		{
			if(linkSkillList.Count !=0)
			{

				foreach(int index in linkSkillList)
				{
					if(index ==skillIndex)
					{
						Debug.Log("�ش罺ų�� �̹� ��ũ �Ǿ��ֽ��ϴ�.");
						return;
					}
				}

			}
			linkSkillList.Add(skillIndex);
			linkSkillNameList.Add(skillName);
		}


		//������.
		public void ShowData()
		{
			Debug.Log($"{icon.name},{name},{level},{point}");
		}

		


	};


	public void SkillLevelSort()
	{
		if (skillList.Count == 0)
			return;
		LevelSortSkillList = skillList.OrderBy(x => x.level).ToList();
	}


	public static SkillData Instance
	{
		get
		{
			if(null == instance)
			{
				return null;
			}

			return instance;
		}
	}


	//��ų�� �����͸� �߰��ϴ� �Լ�.
	public void DataSet(Sprite icon, string name, int level, int point)
	{
		skillData sd = new skillData(icon, name, level, point);


		foreach(skillData element in skillList)
		{
			if(element.name == sd.name && element.level == sd.level)
			{
				Debug.Log("�̸��� �����ִ� ������ �Ȱ��� ��ų���ֽ��ϴ�. �ٽ� �������ּ���.");
				return;
			}

		}


		Debug.Log("��ų �����͸� �����߽��ϴ�.");
		Debug.Log("��ų ������ �Ϸ��߽��ϴ�.");
		skillList.Add(sd);

		
	}


	void Awake()
	{

		if (instance == null)
		{
			instance = this;


			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}

		sprites = Resources.LoadAll<Sprite>("SkillIcon");
	}

	

}
