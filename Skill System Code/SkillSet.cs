using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using System.Text.RegularExpressions;
using UnityEngine.UI;

public class SkillSet : MonoBehaviour
{

	public static bool isDrag = true;
	public static int level = 1;
	public static int skillpoint = 0;


	public GameObject skillPrefab;         //프리팹.
	public GameObject m_skillPanel;        //스킬의 링크를 셋팅할수 있는 패널.
	public GameObject m_skillTreePanel;    //스킬트리 셋팅을 다끝내고 테스트 해볼수 있는 패널.
	public GameObject m_playerLevelText;
	public GameObject m_playerPointText;




	private GameObject[] m_skillArr;      //링크 셋팅에서 사용될 배열.(유저가 스킬을 만든 순서)
	private GameObject[] m_skillTreeArr;  //스킬트리에서 사용될 배열( 유저한테 레벨별로 정렬해서 보여줄 스킬 순서)

	private SkillData.skillData[] m_skillListToArr;  //리스트를 배열로 접근 fot문을 사용해서.
	private SkillData.skillData[] m_skillTreeListToArr; 

	private bool m_link=false;        //스킬 링크를 확인하는 변수.

	private Vector3 m_createPoint = new Vector3(-610, 450, 0);                  //   초기위치.

	private int m_firstlink;      //첫번째 선택한 스킬의 인덱스
	private int m_secondlink;     //두번째 선택한 스킬의 인덱스

    // Start is called before the first frame update
    void Start()
    {

		//생성 스킬이 1개라도 있다면.
		if(SkillData.Instance.skillList.Count !=0)
		{
			m_skillArr = new GameObject[SkillData.Instance.skillList.Count];
			m_skillListToArr = SkillData.Instance.skillList.ToArray();  //배열로 접근

			int row = 0;
			int column = 0;

			for (int i = 0; i < SkillData.Instance.skillList.Count; i++)
			{

				m_skillArr[i] = Instantiate(skillPrefab, m_createPoint, Quaternion.identity, GameObject.Find("Canvas").transform.Find("SkillPanel").  //스킬 프리팹 배열생성
					transform.Find("Scroll Rect").transform.Find("Contents").transform);

				

				if (m_createPoint.x + row * 200 <= 610)  //행으로 먼저 전개
				{
					m_skillArr[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(m_createPoint.x + row * 200, m_createPoint.y - column * 250, m_createPoint.z);  //프리팹에대한 좌표 설정.

					row++;
				}
				else
				{
					column++;
					row = 0;
					m_skillArr[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(m_createPoint.x + row * 200, m_createPoint.y - column * 250, m_createPoint.z);
					row++;
				}


				m_skillArr[i].transform.Find("Image").GetComponent<Image>().sprite = m_skillListToArr[i].icon;  //프리팹에 이미지 설정
				m_skillArr[i].transform.Find("Level").GetComponent<Text>().text = $"LV{m_skillListToArr[i].level}";  //스킬의 획득 레벨을 설정.

				



				int index = i;
				m_skillArr[i].transform.Find("Link").GetComponent<Button>().onClick.AddListener(() => Link(index));  //각 스킬들의 버튼 이벤트 할당.




			}
		}

		

    }

    // Update is called once per frame
    void Update()
    {
		m_playerLevelText.GetComponent<Text>().text = $"현재레벨:{level}";
		m_playerPointText.GetComponent<Text>().text = $"포인트:{skillpoint}";

	}

	public void Link(int index)
	{
		if (m_link)
		{
			m_link = false;
			m_secondlink = index;

			if(m_firstlink !=m_secondlink)  //인덱스 번호가 같다 == 자기자신을 클릭한것.  자기자신을 클릭한게 아니라면...
			{
				addLinkSKill(m_firstlink, m_secondlink);  //두개의 스킬을 링크시킨다.
			}

			m_skillArr[m_firstlink].transform.Find("Link").transform.Find("Text").GetComponent<Text>().color = Color.green;  //링크 처리후 컬러색 원래대로.


		}
		else
		{
			m_link = true;
			m_skillArr[index].transform.Find("Link").transform.Find("Text").GetComponent<Text>().color = Color.red;  //클리한 스킬의 이름 색 변경
			m_firstlink = index;
		}
		
	}


	public void ExitSkillList()
	{

		m_skillPanel.SetActive(false);
		m_skillTreePanel.SetActive(false);
	}

	private void addLinkSKill(int first_select, int second_select)
	{

		//두 스킬 우위를 비교(더 높은쪽이 링크가 걸리는 쪽으로.)
		if(m_skillListToArr[first_select].level >m_skillListToArr[second_select].level)                   //첫번째로 누른 스킬이 더 상위(레벨) 스킬인가?
		{
			m_skillListToArr[first_select].LinkSkillAdd(second_select, m_skillListToArr[second_select].name);

		}
		else if(m_skillListToArr[first_select].level < m_skillListToArr[second_select].level)             //두번째로 누른 스킬이 더 상위(레벨) 스킬인가?
		{
		
			m_skillListToArr[second_select].LinkSkillAdd(first_select, m_skillListToArr[first_select].name);
		}


		//디버깅용.
		Debug.Log($"first:{m_skillListToArr[first_select].linkSkillList.Count}");
		Debug.Log($"second:{m_skillListToArr[second_select].linkSkillList.Count}");

	}

	public void PrevScene()
	{
		m_skillPanel.SetActive(false);
		m_skillTreePanel.SetActive(false);
		SceneManager.LoadScene("SkillCreate");
	}

	public void LinkSetting()
	{
		isDrag = true;
		m_skillPanel.SetActive(true);
		m_skillTreePanel.SetActive(false);
	}


	public void SkillPointUp()
	{

		if(skillpoint >= 100)
		{
			Debug.Log("최대 포인트 입니다.");
			return;
		}
	   skillpoint += 10;
	}

	public void PlayerLevelUp()
	{
		if(level ==100)
		{
			Debug.Log("최대 레벨입니다.");
			return;
		}

		level +=10;
	}

	public void Plus(int index)
	{

		//플레이어 레벨이 스킬을 올릴수 있는 레벨인가.

		//스킬 안에서 링크 스킬이 전부 마스터 되어 있는가.

		PrefabInfo m_info = m_skillTreeArr[index].GetComponent<PrefabInfo>();

		
		if(level < m_skillTreeListToArr[index].level)
		{
			Debug.Log("레벨이 부족합니다.");
			return;
		}

		if (skillpoint == 0)
		{
			Debug.Log("스킬포인트가 부족합니다.");
			return;
		}


		if (m_skillTreeListToArr[index].linkSkillList.Count != 0)  //리스트의 원소가 있다면
		{
			foreach (int linkindex in m_skillTreeListToArr[index].linkSkillList)  //그 원소의 인덱스를 가지고와서.
			{
				if(m_skillTreeListToArr[linkindex].currentlevel != m_skillTreeListToArr[linkindex].point)
				{
					Debug.Log($"{m_skillTreeListToArr[linkindex].name},{m_skillTreeListToArr[linkindex].currentlevel}링크스킬이 아직 마스터되지 않았습니다.");
					return;
				}
			}
		}
		

		if (m_skillTreeListToArr[index].currentlevel == m_skillTreeListToArr[index].point)
		{
			Debug.Log("최고레벨입니다.");
			return;
		}

		if  (m_skillTreeListToArr[index].currentlevel < m_skillTreeListToArr[index].point)
		{
			m_skillTreeArr[index].transform.Find("Image").GetComponent<Image>().color = Color.white;
			m_skillTreeListToArr[index].currentlevel++;
			m_info.info.currentlevel++;
			skillpoint--;

		}


		//스킬포인트 차감
	}


	public void Minus(int index)
	{

		PrefabInfo m_info = m_skillTreeArr[index].GetComponent<PrefabInfo>();

		if (m_skillTreeListToArr[index].currentlevel == 0)                  //버튼을 눌렀을때 0이라면.
		{
			Debug.Log("더 이상 감소할 포인트가 없습니다.");
			return;
		}


		if (m_skillTreeListToArr[index].currentlevel > 0)         
		{
			m_skillTreeListToArr[index].currentlevel--;
			m_info.info.currentlevel--;
			skillpoint++;
		}


		if (m_skillTreeListToArr[index].currentlevel == 0)
		{
			m_skillTreeArr[index].transform.Find("Image").GetComponent<Image>().color = Color.gray;  //포인트를 감소시켯을떄 0이라면.
		}


		//스킬포인트 회복.
	}



	public void SkillTreeShow()
	{

		isDrag = false;
		m_skillPanel.SetActive(false);
		m_skillTreePanel.SetActive(true);
		SkillData.Instance.SkillLevelSort();  //레벨로 정렬을 시킴.


		m_skillTreeArr = new GameObject[SkillData.Instance.LevelSortSkillList.Count];
		m_skillTreeListToArr = SkillData.Instance.LevelSortSkillList.ToArray(); 

		int row = 0;
		int column = 0;

		for (int i = 0; i < SkillData.Instance.LevelSortSkillList.Count; i++)
		{

			m_skillTreeArr[i] = Instantiate(skillPrefab, m_createPoint, Quaternion.identity, GameObject.Find("Canvas").transform.Find("SkillTreePanel").  //스킬 프리팹 배열생성
				transform.Find("Scroll Rect").transform.Find("Contents").transform);

			if(i > 0 && m_skillTreeListToArr[i-1].level == m_skillTreeListToArr[i].level)
			{
				if (m_createPoint.x + row * 200 >= 610)
				{
					column++;
					row = 0;
					m_skillTreeArr[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(m_createPoint.x + row * 200, m_createPoint.y - column * 250, m_createPoint.z);
					row++;
				}
				else
				{
					m_skillTreeArr[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(m_createPoint.x + row * 200, m_createPoint.y - column * 250, m_createPoint.z);  //프리팹에대한 좌표 설정.
					row++;
				}
			}
			else
			{
				column++;
				row = 0;
				m_skillTreeArr[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(m_createPoint.x + row * 200, m_createPoint.y - column * 250, m_createPoint.z);
				row++;
			}


			m_skillTreeArr[i].transform.Find("Image").GetComponent<Image>().sprite = m_skillTreeListToArr[i].icon;  //프리팹에 이미지 설정
			m_skillTreeArr[i].transform.Find("Image").GetComponent<Image>().color = Color.gray;



			m_skillTreeArr[i].transform.Find("Level").GetComponent<Text>().text = $"LV{m_skillTreeListToArr[i].level}";  //스킬의 획득 레벨을 설정.

			m_skillTreeArr[i].transform.Find("Plus").gameObject.SetActive(true);
			m_skillTreeArr[i].transform.Find("Minus").gameObject.SetActive(true);

			int index = i;
			m_skillTreeArr[i].transform.Find("Plus").GetComponent<Button>().onClick.AddListener(() => Plus(index));
			m_skillTreeArr[i].transform.Find("Minus").GetComponent<Button>().onClick.AddListener(() => Minus(index));
			m_skillTreeArr[i].transform.Find("Link").gameObject.SetActive(false);
		


			PrefabInfo info = m_skillTreeArr[i].GetComponent<PrefabInfo>();
			info.info = m_skillTreeListToArr[i];

		}

	}
}
