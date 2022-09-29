using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SkillManager : MonoBehaviour
{
	public GameObject m_SkillSelect;  //스킬아이콘을 select하면 보여줄 Image변수.
	public GameObject m_SkillPanel;  //스킬 목록을 보여주는 패널
	public GameObject m_nextBtn;     //next scene button
	public GameObject m_CreateBtn;  //스킬 생성버튼


	public InputField m_ifSkillName;  //스킬 이름
	public InputField m_ifSkillLevel; //스킬을 배울수 있는 레벨
	public InputField m_ifSkillPoint; //스킬 포인트(스킬 최대레벨 설정)



	private GameObject[,] m_skillIconArr = new GameObject[5,13];    //x축 좌표는 같고 y축좌표가 다른 버튼 5개
	//private Sprite[] m_sprites;  //스킬 목록

	

	private int SetSkillImage(int k)   
	{
		int index = 0;
		switch (k / 8)
		{
			case 0:
				index = (k % 8) + 21;
				break;
			case 1:
				index = (k % 8) + 31;
				break;
			case 2:
				index = (k % 8) + 41;
				break;
			case 3:
				index = (k % 8) + 51;
				break;
			case 4:
				index = (k % 8) + 61;
				break;
			case 5:
				index = (k % 8) + 71;
				break;
			case 6:
				index = (k % 8) + 81;
				break;
			case 7:
				index = (k % 8) + 91;
				break;
			default:
				break;
		}
		return index;
	}


	// Start is called before the first frame update
	void Start()
    {

		//m_sprites = Resources.LoadAll<Sprite>("SkillIcon");

		int k = 0;
		for(int i=0; i<5;i++)
		{
			for (int j=0;j<13;j++)
			{
				
				if(k==64) //0~63까지 64개 요소만 생성
				{
					break;
				}
				m_skillIconArr[i, j] = m_SkillPanel.transform.GetChild(k).gameObject;  //스킬 패널에 있는 자식 오브젝트를 할당.
				m_skillIconArr[i, j].GetComponent<RectTransform>().anchoredPosition = new Vector3(-737 + (j * 120), 145.0f - (i * 120.0f), 0.0f); //포지션 셋팅.
				m_skillIconArr[i, j].GetComponent<Image>().sprite = SkillData.Instance.sprites[SetSkillImage(k)]; //스킬 이미지 셋팅

				//for문에서 addListener 람다식은 주의해야한다 순환참조 문제가 발생할 우려가있음.

				//순환참조(javascript = 클로저)문제 해결방안.
				int kindex = k;

				//각 이미지 버튼에 이벤트 추가 및 함수 매개변수로 구분해서 할당.
				//
				m_skillIconArr[i, j].GetComponent<Button>().onClick.AddListener(() => SetSkillIcon(SetSkillImage(kindex))); 


				k++; //0~64까지
			}
		}

		m_SkillSelect.GetComponent<Image>().sprite = SkillData.Instance.sprites[21];
		m_SkillPanel.SetActive(false);

	}

    // Update is called once per frame
    void Update()
	{

	}



	public void ShowSkillList()
	{
		//스킬 패널 활성화.
		m_SkillPanel.SetActive(true);

		//넥스트 버튼 비활성화.
		m_nextBtn.SetActive(false);

		//스킬 생성 버튼 비활성화
		m_CreateBtn.SetActive(false);

		//이름 입력필드 비활성화
		m_ifSkillName.gameObject.SetActive(false);

		//레벨 입력필드 비활성화
		m_ifSkillLevel.gameObject.SetActive(false);

		//스킬포인트 입력필드 비활성화
		m_ifSkillPoint.gameObject.SetActive(false);

	
	}

	public void ExitSkillList()
	{
		//스킬 패널 비활성화.
		m_SkillPanel.SetActive(false);

		//넥스트 버튼 활성화.
		m_nextBtn.SetActive(true);

		//스킬 생성 버튼 활성화
		m_CreateBtn.SetActive(true);

		//이름 입력필드 활성화
		m_ifSkillName.gameObject.SetActive(true);

		//레벨 입력필드 활성화
		m_ifSkillLevel.gameObject.SetActive(true);

		//스킬포인트 입력필드 활성화
		m_ifSkillPoint.gameObject.SetActive(true);

		
	}

	public void SetSkillIcon(int kIndex)
	{
		m_SkillSelect.GetComponent<Image>().sprite = SkillData.Instance.sprites[kIndex];

	}

	public void NextScene()
	{
		SceneManager.LoadScene("SkillSet");
	}


	private bool CheckInputField(InputField name,InputField level, InputField point)
	{

		//맞는지 체크.
		int i = 0;

		if(!int.TryParse(name.text, out i) && int.TryParse(level.text, out i) && int.TryParse(point.text, out i))
		{
			//데이터 저장.
			SkillData.Instance.DataSet(m_SkillSelect.GetComponent<Image>().sprite,name.text,int.Parse(level.text),int.Parse(point.text));
			return true;
		}

			

		return false;

	}

	public void CreateSkill()
	{

		//하나라도 비어있다면 생성 불가능 하게.
		if (m_ifSkillName.text.Length == 0 || m_ifSkillLevel.text.Length == 0 || m_ifSkillPoint.text.Length == 0)
		{
			Debug.Log("스킬의 이름,레벨,포인트를 모두 입력해주시기 바랍니다.");
			return;
		}


		if (!CheckInputField(m_ifSkillName, m_ifSkillLevel, m_ifSkillPoint))
		{
			Debug.Log("필드에 이상한 값이 들어가있습니다. 레벨과 포인트는 숫자를 입력해주세요...");
			return;
		}

		//입력 필드 초기화.
		m_ifSkillName.placeholder.GetComponent<Text>().text = "스킬 이름을 입력해주세요...";
		m_ifSkillName.text = "";

		m_ifSkillLevel.placeholder.GetComponent<Text>().text = "스킬을 배우실 레벨을 입력해주세요...";
		m_ifSkillLevel.text = "";

		m_ifSkillPoint.placeholder.GetComponent<Text>().text = "스킬 포인트를 입력해주세요...";
		m_ifSkillPoint.text = "";
	}

}
