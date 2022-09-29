using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SkillManager : MonoBehaviour
{
	public GameObject m_SkillSelect;  //��ų�������� select�ϸ� ������ Image����.
	public GameObject m_SkillPanel;  //��ų ����� �����ִ� �г�
	public GameObject m_nextBtn;     //next scene button
	public GameObject m_CreateBtn;  //��ų ������ư


	public InputField m_ifSkillName;  //��ų �̸�
	public InputField m_ifSkillLevel; //��ų�� ���� �ִ� ����
	public InputField m_ifSkillPoint; //��ų ����Ʈ(��ų �ִ뷹�� ����)



	private GameObject[,] m_skillIconArr = new GameObject[5,13];    //x�� ��ǥ�� ���� y����ǥ�� �ٸ� ��ư 5��
	//private Sprite[] m_sprites;  //��ų ���

	

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
				
				if(k==64) //0~63���� 64�� ��Ҹ� ����
				{
					break;
				}
				m_skillIconArr[i, j] = m_SkillPanel.transform.GetChild(k).gameObject;  //��ų �гο� �ִ� �ڽ� ������Ʈ�� �Ҵ�.
				m_skillIconArr[i, j].GetComponent<RectTransform>().anchoredPosition = new Vector3(-737 + (j * 120), 145.0f - (i * 120.0f), 0.0f); //������ ����.
				m_skillIconArr[i, j].GetComponent<Image>().sprite = SkillData.Instance.sprites[SetSkillImage(k)]; //��ų �̹��� ����

				//for������ addListener ���ٽ��� �����ؾ��Ѵ� ��ȯ���� ������ �߻��� ���������.

				//��ȯ����(javascript = Ŭ����)���� �ذ���.
				int kindex = k;

				//�� �̹��� ��ư�� �̺�Ʈ �߰� �� �Լ� �Ű������� �����ؼ� �Ҵ�.
				//
				m_skillIconArr[i, j].GetComponent<Button>().onClick.AddListener(() => SetSkillIcon(SetSkillImage(kindex))); 


				k++; //0~64����
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
		//��ų �г� Ȱ��ȭ.
		m_SkillPanel.SetActive(true);

		//�ؽ�Ʈ ��ư ��Ȱ��ȭ.
		m_nextBtn.SetActive(false);

		//��ų ���� ��ư ��Ȱ��ȭ
		m_CreateBtn.SetActive(false);

		//�̸� �Է��ʵ� ��Ȱ��ȭ
		m_ifSkillName.gameObject.SetActive(false);

		//���� �Է��ʵ� ��Ȱ��ȭ
		m_ifSkillLevel.gameObject.SetActive(false);

		//��ų����Ʈ �Է��ʵ� ��Ȱ��ȭ
		m_ifSkillPoint.gameObject.SetActive(false);

	
	}

	public void ExitSkillList()
	{
		//��ų �г� ��Ȱ��ȭ.
		m_SkillPanel.SetActive(false);

		//�ؽ�Ʈ ��ư Ȱ��ȭ.
		m_nextBtn.SetActive(true);

		//��ų ���� ��ư Ȱ��ȭ
		m_CreateBtn.SetActive(true);

		//�̸� �Է��ʵ� Ȱ��ȭ
		m_ifSkillName.gameObject.SetActive(true);

		//���� �Է��ʵ� Ȱ��ȭ
		m_ifSkillLevel.gameObject.SetActive(true);

		//��ų����Ʈ �Է��ʵ� Ȱ��ȭ
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

		//�´��� üũ.
		int i = 0;

		if(!int.TryParse(name.text, out i) && int.TryParse(level.text, out i) && int.TryParse(point.text, out i))
		{
			//������ ����.
			SkillData.Instance.DataSet(m_SkillSelect.GetComponent<Image>().sprite,name.text,int.Parse(level.text),int.Parse(point.text));
			return true;
		}

			

		return false;

	}

	public void CreateSkill()
	{

		//�ϳ��� ����ִٸ� ���� �Ұ��� �ϰ�.
		if (m_ifSkillName.text.Length == 0 || m_ifSkillLevel.text.Length == 0 || m_ifSkillPoint.text.Length == 0)
		{
			Debug.Log("��ų�� �̸�,����,����Ʈ�� ��� �Է����ֽñ� �ٶ��ϴ�.");
			return;
		}


		if (!CheckInputField(m_ifSkillName, m_ifSkillLevel, m_ifSkillPoint))
		{
			Debug.Log("�ʵ忡 �̻��� ���� ���ֽ��ϴ�. ������ ����Ʈ�� ���ڸ� �Է����ּ���...");
			return;
		}

		//�Է� �ʵ� �ʱ�ȭ.
		m_ifSkillName.placeholder.GetComponent<Text>().text = "��ų �̸��� �Է����ּ���...";
		m_ifSkillName.text = "";

		m_ifSkillLevel.placeholder.GetComponent<Text>().text = "��ų�� ���� ������ �Է����ּ���...";
		m_ifSkillLevel.text = "";

		m_ifSkillPoint.placeholder.GetComponent<Text>().text = "��ų ����Ʈ�� �Է����ּ���...";
		m_ifSkillPoint.text = "";
	}

}
