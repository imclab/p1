﻿using UnityEngine;
using System.Collections;
using UnityTest;

public class PushThumb : MonoBehaviour
{
		public Vector3 thumbPush = new Vector3 (0, -300, 0);
		public GameObject target = null;

		// Use this for initialization
		void Start ()
		{
				if (target == null)
						target = gameObject;
				target.rigidbody.AddForce (thumbPush);
		}
	
		// Update is called once per frame
		void Update ()
		{

		}
}
