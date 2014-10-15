﻿using UnityEngine;
using System.Collections;

namespace P1
{
		public class Radical : MonoBehaviour
		{
				public static Radical instance;
				public GameObject TwitterList;
				[HideInInspector]
				public GameObject
						activeTwitter = null;

				private TwitterStatusButton getTwitterStatusButton (Collider other)
				{
						if (other.transform.parent && other.transform.parent.GetComponent<TwitterStatusButton> ()) {
								return other.transform.parent.GetComponent<TwitterStatusButton> ();
						}
						return null;
				}

				// Use this for initialization
				void Start ()
				{
						instance = this;
						this.transform.position = new Vector3 (0.0f, 0.0f, TwitterList.transform.position.z);
				}

				// Update is called once per frame
				void Update ()
				{

				}

				void OnTriggerEnter (Collider other)
				{
						if (Time.time < 1.0f)
								return;

						TwitterStatusButton button = getTwitterStatusButton (other);
						if (button) {
								activeTwitter = button.gameObject;
								button.SetColor (Color.cyan);
						}
				}

				void OnTriggerExit (Collider other)
				{
						if (Time.time < 1.0f)
								return;

						TwitterStatusButton button = getTwitterStatusButton (other);
						if (button) {
								button.ResetColor ();
						}
      
				}
		}
}
