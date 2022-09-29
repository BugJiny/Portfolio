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


	public GameObject skillPrefab;         //������.
	public GameObject m_skillPanel;        //��ų�� ��ũ�� �����Ҽ� �ִ� �г�.
	public GameObject m_skillTreePanel;    //��ųƮ�� ������ �ٳ����� �׽�Ʈ �غ��� �ִ� �г�.
	public GameObject m_playerLevelText;
	public GameObject m_playerPointText;




	private GameObject[] m_skillArr;      //��ũ ���ÿ��� ���� �迭.(������ ��ų�� ���� ����)
	private GameObject[] m_skillTreeArr;  //��ųƮ������ ���� �迭( �������� �������� �����ؼ� ������ ��ų ����)

	private SkillData.skillData[] m_skillListToArr;  //����Ʈ�� �迭�� ���� fot���� ����ؼ�.
	private SkillData.skillData[] m_skillTreeListToArr; 

	private bool m_link=false;        //��ų ��ũ�� Ȯ���ϴ� ����.

	private Vector3 m_createPoint = new Vector3(-610, 450, 0);                  //   �ʱ���ġ.

	private int m_firstlink;      //ù��° ������ ��ų�� �ε���
	private int m_secondlink;     //�ι�° ������ ��ų�� �ε���

    // Start is called before the first frame update
    void Start()
    {

		//���� ��ų�� 1���� �ִٸ�.
		if(SkillData.Instance.skillList.Count !=0)
		{
			m_skillArr = new GameObject[SkillData.Instance.skillList.Count];
			m_skillListToArr = SkillData.Instance.skillList.ToArray();  //�迭�� ����

			int row = 0;
			int column = 0;

			for (int i = 0; i < SkillData.Instance.skillList.Count; i++)
			{

				m_skillArr[i] = Instantiate(skillPrefab, m_createPoint, Quaternion.identity, GameObject.Find("Canvas").transform.Find("SkillPanel").  //��ų ������ �迭����
					transform.Find("Scroll Rect").transform.Find("Contents").transform);

				

				if (m_createPoint.x + row * 200 <= 610)  //������ ���� ����
				{
					m_skillArr[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(m_createPoint.x + row * 200, m_createPoint.y - column * 250, m_createPoint.z);  //�����տ����� ��ǥ ����.

					row++;
				}
				else
				{
					column++;
					row = 0;
					m_skillArr[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(m_createPoint.x + row * 200, m_createPoint.y - column * 250, m_createPoint.z);
					row++;
				}


				m_skillArr[i].transform.Find("Image").GetComponent<Image>().sprite = m_skillListToArr[i].icon;  //�����տ� �̹��� ����
				m_skillArr[i].transform.Find("Level").GetComponent<Text>().text = $"LV{m_skillListToArr[i].level}";  //��ų�� ȹ�� ������ ����.

				



				int index = i;
				m_skillArr[i].transform.Find("Link").GetComponent<Button>().onClick.AddListener(() => Link(index));  //�� ��ų���� ��ư �̺�Ʈ �Ҵ�.




			}
		}

		

    }

    // Update is called once per frame
    void Update()
    {
		m_playerLevelText.GetComponent<Text>().text = $"���緹��:{level}";
		m_playerPointText.GetComponent<Text>().text = $"����Ʈ:{skillpoint}";

	}

	public void Link(int index)
	{
		if (m_link)
		{
			m_link = false;
			m_secondlink = index;

			if(m_firstlink !=m_secondlink)  //�ε��� ��ȣ�� ���� == �ڱ��ڽ��� Ŭ���Ѱ�.  �ڱ��ڽ��� Ŭ���Ѱ� �ƴ϶��...
			{
				addLinkSKill(m_firstlink, m_secondlink);  //�ΰ��� ��ų�� ��ũ��Ų��.
			}

			m_skillArr[m_firstlink].transform.Find("Link").transform.Find("Text").GetComponent<Text>().color = Color.green;  //��ũ ó���� �÷��� �������.


		}
		else
		{
			m_link = true;
			m_skillArr[index].transform.Find("Link").transform.Find("Text").GetComponent<Text>().color = Color.red;  //Ŭ���� ��ų�� �̸� �� ����
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

		//�� ��ų ������ ��(�� �������� ��ũ�� �ɸ��� ������.)
		if(m_skillListToArr[first_select].level >m_skillListToArr[second_select].level)                   //ù��°�� ���� ��ų�� �� ����(����) ��ų�ΰ�?
		{
			m_skillListToArr[first_select].LinkSkillAdd(second_select, m_skillListToArr[second_select].name);

		}
		else if(m_skillListToArr[first_select].level < m_skillListToArr[second_select].level)             //�ι�°�� ���� ��ų�� �� ����(����) ��ų�ΰ�?
		{
		
			m_skillListToArr[second_select].LinkSkillAdd(first_select, m_skillListToArr[first_select].name);
		}


		//������.
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
			Debug.Log("�ִ� ����Ʈ �Դϴ�.");
			return;
		}
	   skillpoint += 10;
	}

	public void PlayerLevelUp()
	{
		if(level ==100)
		{
			Debug.Log("�ִ� �����Դϴ�.");
			return;
		}

		level +=10;
	}

	public void Plus(int index)
	{

		//�÷��̾� ������ ��ų�� �ø��� �ִ� �����ΰ�.

		//��ų �ȿ��� ��ũ ��ų�� ���� ������ �Ǿ� �ִ°�.

		PrefabInfo m_info = m_skillTreeArr[index].GetComponent<PrefabInfo>();

		
		if(level < m_skillTreeListToArr[index].level)
		{
			Debug.Log("������ �����մϴ�.");
			return;
		}

		if (skillpoint == 0)
		{
			Debug.Log("��ų����Ʈ�� �����մϴ�.");
			return;
		}


		if (m_skillTreeListToArr[index].linkSkillList.Count != 0)  //����Ʈ�� ���Ұ� �ִٸ�
		{
			foreach (int linkindex in m_skillTreeListToArr[index].linkSkillList)  //�� ������ �ε����� ������ͼ�.
			{
				if(m_skillTreeListToArr[linkindex].currentlevel != m_skillTreeListToArr[linkindex].point)
				{
					Debug.Log($"{m_skillTreeListToArr[linkindex].name},{m_skillTreeListToArr[linkindex].currentlevel}��ũ��ų�� ���� �����͵��� �ʾҽ��ϴ�.");
					return;
				}
			}
		}
		

		if (m_skillTreeListToArr[index].currentlevel == m_skillTreeListToArr[index].point)
		{
			Debug.Log("�ְ����Դϴ�.");
			return;
		}

		if  (m_skillTreeListToArr[index].currentlevel < m_skillTreeListToArr[index].point)
		{
			m_skillTreeArr[index].transform.Find("Image").GetComponent<Image>().color = Color.white;
			m_skillTreeListToArr[index].currentlevel++;
			m_info.info.currentlevel++;
			skillpoint--;

		}


		//��ų����Ʈ ����
	}


	public void Minus(int index)
	{

		PrefabInfo m_info = m_skillTreeArr[index].GetComponent<PrefabInfo>();

		if (m_skillTreeListToArr[index].currentlevel == 0)                  //��ư�� �������� 0�̶��.
		{
			Debug.Log("�� �̻� ������ ����Ʈ�� �����ϴ�.");
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
			m_skillTreeArr[index].transform.Find("Image").GetComponent<Image>().color = Color.gray;  //����Ʈ�� ���ҽ������� 0�̶��.
		}


		//��ų����Ʈ ȸ��.
	}



	public void SkillTreeShow()
	{

		isDrag = false;
		m_skillPanel.SetActive(false);
		m_skillTreePanel.SetActive(true);
		SkillData.Instance.SkillLevelSort();  //������ ������ ��Ŵ.


		m_skillTreeArr = new GameObject[SkillData.Instance.LevelSortSkillList.Count];
		m_skillTreeListToArr = SkillData.Instance.LevelSortSkillList.ToArray(); 

		int row = 0;
		int column = 0;

		for (int i = 0; i < SkillData.Instance.LevelSortSkillList.Count; i++)
		{

			m_skillTreeArr[i] = Instantiate(skillPrefab, m_createPoint, Quaternion.identity, GameObject.Find("Canvas").transform.Find("SkillTreePanel").  //��ų ������ �迭����
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
					m_skillTreeArr[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(m_createPoint.x + row * 200, m_createPoint.y - column * 250, m_createPoint.z);  //�����տ����� ��ǥ ����.
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


			m_skillTreeArr[i].transform.Find("Image").GetComponent<Image>().sprite = m_skillTreeListToArr[i].icon;  //�����տ� �̹��� ����
			m_skillTreeArr[i].transform.Find("Image").GetComponent<Image>().color = Color.gray;



			m_skillTreeArr[i].transform.Find("Level").GetComponent<Text>().text = $"LV{m_skillTreeListToArr[i].level}";  //��ų�� ȹ�� ������ ����.

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
