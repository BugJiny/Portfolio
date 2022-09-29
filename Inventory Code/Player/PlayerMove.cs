#define DEBUG
//#undef DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class PlayerMove : MonoBehaviour
{


	//public float speed;      // ĳ���� ������ ���ǵ�


	private CharacterController m_characterController; // ĳ���� ��Ʈ�ѷ�
	private Vector3 m_movePoint; // �̵� ��ġ ����
	private Camera m_mainCamera; // ���� ī�޶�


	public static Player.CharacterStatus m_status;


	void Start()
	{

		m_movePoint = this.gameObject.transform.position;  //���� �����Ҷ� �������� �ʰ�.
		m_status = new Player.CharacterStatus();
		m_mainCamera = Camera.main;
		m_characterController = GetComponent<CharacterController>();
	}

	void Update()
	{
		//ĳ���� ��Ʈ�ѷ��� ���ٸ� ����(����ó��)
		if (m_characterController == null) return;

		PlayerControl();


		// ���������� �Ÿ��� 0.1f ���� �ִٸ�
		if (Vector3.Distance(transform.position, m_movePoint) > 0.1f)
		{
			// �̵�
			Move();
		}
	}


	void PlayerControl()
	{
		// ��Ŭ�� �̺�Ʈ�� ���Դٸ� && ȭ�� UI�� ���� �ʾ��� ���.
		if (Input.GetMouseButtonUp(1) && !EventSystem.current.IsPointerOverGameObject())
		{
			// ī�޶󿡼� �������� ���.
			Ray ray = m_mainCamera.ScreenPointToRay(Input.mousePosition);

#if (DEBUG)
			// Scence ���� ī�޶󿡼� ������ ������ ������ Ȯ���ϱ�(Debug��)
			Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 1f);
#endif

			// �������� ������ �¾Ҵٸ�
			if (Physics.Raycast(ray, out RaycastHit raycastHit))
			{
				// ���� ��ġ�� �������� ����
				m_movePoint = raycastHit.point;
#if (DEBUG)
				//Debug��
				Debug.Log("movePoint : " + m_movePoint.ToString());
				Debug.Log("���� ��ü : " + raycastHit.transform.name);
#endif
			}
		}
	}


	void Move()
	{
		// thisUpdatePoint �� �̹� ������Ʈ(������) ���� �̵��� ����Ʈ�� ��� ������.
		// �̵��� ����(�̵��� ��-���� ��ġ) ���ϱ� �ӵ��� �ؼ� �̵��� ��ġ���� ����Ѵ�.
		Vector3 thisUpdatePoint = (m_movePoint - transform.position).normalized * (m_status.Speed+m_status.addSpd);


		// characterController �� ĳ���� �̵��� ����ϴ� ������Ʈ��.
		// simpleMove �� �ڵ����� �߷��� ����ؼ� �̵������ִ� �޼ҵ��.
		// ������ �̵��� ����Ʈ�� �������ָ� �ȴ�.
		m_characterController.SimpleMove(thisUpdatePoint);

		
	}
}
