﻿using UnityEngine;
using System.Collections;

namespace P1
{
		public class SliderDragger : MonoBehaviour
		{

				public float moveIncrement;
				private float moveAmountX;
				private float deltaX;
				public float moveLimit = 1;
				private Vector3 origPos;
				private bool isThisHit = false;
				private int prevSliderInt;
				public int sliderInt;

				private	float snappedXint;
				private float snappedXpos;

				public GameObject HandleVisMesh;
				public GameObject HandleVisGRP;
				
				public AudioSource sliderClickSound;
				
				private SliderManager sliderManager;
				private Vector3 pos;
		
				void Start() {

				}
				
				void Awake () {
					sliderManager =  (SliderManager)GetComponentInParent (typeof(SliderManager));
					if (sliderManager == null) {
						Debug.LogWarning ("You are missing a Slider Manager in the scene.");
					}
					rigidbody.position = sliderManager.transform.position;

				}
		
			
	
				void Update ()
				{
						prevSliderInt = sliderInt;
						pos = rigidbody.position;
						pos.x = Mathf.Clamp(pos.x, 0.0f + sliderManager.transform.position.x, (1.0f * sliderManager.transform.localScale.x) + sliderManager.transform.position.x);
						rigidbody.position = pos;

						if (rigidbody.position.x < (1.0f  * sliderManager.transform.localScale.x) + sliderManager.transform.position.x) {
							HandleVisGRP.transform.position = rigidbody.position;
						}

				}
				void FixedUpdate(){
					float sliderValue = sliderManager.MaxLimit * this.transform.localPosition.x;
							sliderInt = (int)(sliderValue +.5f);
							sliderManager.TextSliderValue.text = sliderInt.ToString ();
							if(prevSliderInt != sliderInt){
		//						Debug.Log("Click Sound");
								sliderClickSound.Play();
							}
				}

				
				void OnTriggerEnter(){
//					StopCoroutine("hiLightPause");
					StopAllCoroutines();
					sliderManager.SliderBarHandleMesh.renderer.material = sliderManager.SliderHandleActive;
				}
				
				void OnTriggerExit(){
					StartCoroutine(hiLightPause());
				}
				
				IEnumerator hiLightPause () {
					yield return new WaitForSeconds (.2f);
					sliderManager.SliderBarHandleMesh.renderer.material = sliderManager.SliderHandle;
					StartCoroutine(SnapToInterval());
					
			
		}
		
		
				
		public IEnumerator SnapToInterval () {
					snappedXint = (Mathf.Round (sliderInt / sliderManager.Interval)) * sliderManager.Interval;
					Debug.Log ("snappedXint = " + snappedXint);
					snappedXpos = (1f / sliderManager.MaxLimit) * snappedXint;
					Debug.Log ("snappedXpos = " + snappedXpos);
					
					Vector3 dest  = new Vector3 (snappedXpos, this.transform.localPosition.y, this.transform.localPosition.z);
					this.transform.localPosition =  Vector3.Lerp(transform.localPosition, dest, .25f);
					
					sliderManager.TextSliderValue.text = snappedXint.ToString ();
					yield return null; 
					if(transform.localPosition.x == snappedXpos){
						sliderClickSound.Play();
					}
			
				}
		}
}