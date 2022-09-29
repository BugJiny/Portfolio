using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
	[SerializeField]
	private GameObject Target;



	private Vector3 Offset;


    // Start is called before the first frame update
    void Start()
    {
		Target = GameObject.Find("Player");
		Offset = new Vector3(0, 15, -10);

	}

    // Update is called once per frame
    void Update()
    {
		transform.position = Target.transform.position + Offset;
    }
}
