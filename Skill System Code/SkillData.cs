using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SkillData : MonoBehaviour
{

	private static SkillData instance = null;

	public List<skillData> skillList = new List<skillData>();   //스킬의 정보를 가지고 있는 리스트.
	public List<skillData> LevelSortSkillList = new List<skillData>();  //레벨로 정렬된 리스트.

	public Sprite[] sprites;  //사용할 스킬 이미지 리소스


	enum SkillType
	{ 
	
		Basic=0,
		Link =1
	}


	public struct skillData
	{

		public Sprite icon;      //아이콘
		public string name;      //이름
		public int level;        //스킬 배울수 있는 레벨
		public int point;        //스킬 포인트(최대 레벨)
		public int currentlevel; //현재 스킬레벨.
		public List<int> linkSkillList; //자신한테 링크가 걸려져있는 스킬들의 리스트.
		public List<string> linkSkillNameList;  //자신한테 링크가 걸린 스킬들의 이름.

		//생성자 생성.
		public skillData(Sprite _icon,string _name,int _level,int _point)
		{
			icon = _icon;
			name = _name;
			level = _level;
			point = _point;
			currentlevel = 0;  //초기값 0으로 셋팅
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
						Debug.Log("해당스킬은 이미 링크 되어있습니다.");
						return;
					}
				}

			}
			linkSkillList.Add(skillIndex);
			linkSkillNameList.Add(skillName);
		}


		//디버깅용.
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


	//스킬의 데이터를 추가하는 함수.
	public void DataSet(Sprite icon, string name, int level, int point)
	{
		skillData sd = new skillData(icon, name, level, point);


		foreach(skillData element in skillList)
		{
			if(element.name == sd.name && element.level == sd.level)
			{
				Debug.Log("이름과 배울수있는 레벨이 똑같은 스킬이있습니다. 다시 생성해주세요.");
				return;
			}

		}


		Debug.Log("스킬 데이터를 저장했습니다.");
		Debug.Log("스킬 생성을 완료했습니다.");
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
