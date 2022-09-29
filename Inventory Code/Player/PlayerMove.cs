#define DEBUG
//#undef DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class PlayerMove : MonoBehaviour
{


	//public float speed;      // 캐릭터 움직임 스피드


	private CharacterController m_characterController; // 캐릭터 컨트롤러
	private Vector3 m_movePoint; // 이동 위치 저장
	private Camera m_mainCamera; // 메인 카메라


	public static Player.CharacterStatus m_status;


	void Start()
	{

		m_movePoint = this.gameObject.transform.position;  //게임 시작할때 움직이지 않게.
		m_status = new Player.CharacterStatus();
		m_mainCamera = Camera.main;
		m_characterController = GetComponent<CharacterController>();
	}

	void Update()
	{
		//캐릭터 컨트롤러가 없다면 리턴(예외처리)
		if (m_characterController == null) return;

		PlayerControl();


		// 목적지까지 거리가 0.1f 보다 멀다면
		if (Vector3.Distance(transform.position, m_movePoint) > 0.1f)
		{
			// 이동
			Move();
		}
	}


	void PlayerControl()
	{
		// 좌클릭 이벤트가 들어왔다면 && 화면 UI에 맞지 않았을 경우.
		if (Input.GetMouseButtonUp(1) && !EventSystem.current.IsPointerOverGameObject())
		{
			// 카메라에서 레이저를 쏜다.
			Ray ray = m_mainCamera.ScreenPointToRay(Input.mousePosition);

#if (DEBUG)
			// Scence 에서 카메라에서 나오는 레이저 눈으로 확인하기(Debug용)
			Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 1f);
#endif

			// 레이저가 뭔가에 맞았다면
			if (Physics.Raycast(ray, out RaycastHit raycastHit))
			{
				// 맞은 위치를 목적지로 저장
				m_movePoint = raycastHit.point;
#if (DEBUG)
				//Debug용
				Debug.Log("movePoint : " + m_movePoint.ToString());
				Debug.Log("맞은 객체 : " + raycastHit.transform.name);
#endif
			}
		}
	}


	void Move()
	{
		// thisUpdatePoint 는 이번 업데이트(프레임) 에서 이동할 포인트를 담는 변수다.
		// 이동할 방향(이동할 곳-현재 위치) 곱하기 속도를 해서 이동할 위치값을 계산한다.
		Vector3 thisUpdatePoint = (m_movePoint - transform.position).normalized * (m_status.Speed+m_status.addSpd);


		// characterController 는 캐릭터 이동에 사용하는 컴포넌트다.
		// simpleMove 는 자동으로 중력을 계산해서 이동시켜주는 메소드다.
		// 값으로 이동할 포인트를 전달해주면 된다.
		m_characterController.SimpleMove(thisUpdatePoint);

		
	}
}
