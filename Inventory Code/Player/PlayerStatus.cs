using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
	public class CharacterStatus
	{





		public float Attack { get; set; }
		public float addAtk { get; set; }


		public float Defence { get; set; }
		public float addDef { get; set; }

		public float Critical { get; set; }
		public float addCri { get; set; }
		public float Speed { get; set; }  //이동속도.
		public float addSpd { get; set; }
		//생성자.
		public CharacterStatus()
		{
			Attack = 5.0f;
			Defence = 5.0f;
			Speed = 8.0f;
			Critical = 10.0f;

			addCri = 0.0f;
			addAtk = 0.0f;
			addDef = 0.0f;
			addSpd = 0.0f;

		}


	}	
}