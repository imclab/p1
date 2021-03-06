﻿using UnityEngine;
using System.Collections;

using System.IO;
using System.Text;

using SimpleJSON;

using ButtonMonkey;

namespace P1
{
	
	public class SliderTrialTrigger : MonoBehaviour {
	
		MonkeyTalker monkeySee;
		SliderMonkey monkeyDo;
		public GameObject pinPrompt;
		private SliderManager sliderManager;
		private SliderDragger sliderDragger;
		private int prevHandCount = 0;
		
		SliderTrialTrigger() {
			monkeySee = MonkeyTalker.instance;
			monkeySee.Log ("\nTesting SliderMonkey... for Science!");
			monkeyDo = new SliderMonkey();
			monkeyDo.TrialEvent += TrialUpdate;
		}

    public void Trigger()
    {
      StartCoroutine(StepThroughTrial());
    }
		
		public void Start() {
			DoStart();
			sliderManager = (SliderManager)FindObjectOfType (typeof(SliderManager));
			if (sliderManager == null) {
				Debug.LogWarning ("You are missing a Slider Manager in the scene.");
			}
			sliderDragger =  (SliderDragger)FindObjectOfType (typeof(SliderDragger));
			if (sliderDragger == null) {
				Debug.LogWarning ("You are missing a Slider Dragger in the scene.");
			}
		}
		
		public void DoStart ()
		{
			monkeyDo.ConfigureTest ("slider");
			monkeyDo.TrialEvent += TrialUpdate;
			
			monkeyDo.Start ();
			monkeySee.Log ("Monkey, type: " + monkeyDo.GetTrialKeysString ());
			pinPrompt.GetComponent<PINPrompt> ().UpdatePIN (monkeyDo.GetTrialKeysString ());
		}
		
		// Called once for each key pushed
		void  TrialUpdate (MonkeyTester trial)
		{
			if (monkeyDo.StageComplete ()) {
				// Show final correct result
				pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);
				monkeySee.Log ("Autopsy report for monkey:\n" + monkeyDo.ToString ());
				
				if (CameraManager.instance) {
					CameraManager.instance.NextScene();
				}
			} else {
				if (monkeyDo.TrialComplete ()) {
					// Show final correct result
					pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);
					
					monkeyDo.Start ();
					monkeySee.Log ("Monkey, type: " + monkeyDo.GetTrialKeysString ());
					pinPrompt.GetComponent<PINPrompt> ().UpdatePIN (monkeyDo.GetTrialKeysString ());
				} else {
					if (monkeyDo.WasCorrect ()) {
						monkeySee.Log ("Good monkey! Next, type: " + monkeyDo.GetTrialKeysString () [monkeyDo.GetTrialStep ()]);
						pinPrompt.GetComponent<PINPrompt> ().TogglePIN (true);
					} else {
						monkeySee.Log ("Bad monkey! You were told to type: " + monkeyDo.GetTrialKeysString () [monkeyDo.GetTrialStep ()]);
						pinPrompt.GetComponent<PINPrompt> ().TogglePIN (false);
					}
				}
			}
		}
//		bool isHandInTestTrigger = false;
//		int colliderCount = 0;
//		bool isHandNearSlider = false;
//		void OnTriggerEnter (){
////			monkeySee.Log ("OnTriggerEnter isHandNearSlider = " + isHandNearSlider);
//			
//			isHandNearSlider = true;
//			colliderCount++;
//			// if its a hand
//			//start or advance test
//		}
//		
//		void OnTriggerStay () {
//		}
//		
//		void OnTriggerExit () {
//			colliderCount--;
//			if(colliderCount == 0 && isHandNearSlider == true){
//				monkeySee.Log ("OnTriggerExit isHandNearSlider = " + isHandNearSlider);
//				isHandNearSlider = false;
//				StepThroughTrial();
//
//			}
//		}
		
		IEnumerator StepThroughTrial (){
			char x = sliderDragger.sliderInt.ToString()[0];
			monkeySee.Log ("char x = " + x);
			monkeySee.Log ("Slider Selected: " + sliderDragger.sliderInt);
			monkeyDo.WhenPushed (true, sliderDragger.sliderInt);
			yield return new WaitForSeconds(1.0f);
//			sliderDragger.sliderInt = 0;
//			sliderManager.SliderHandleGRP.transform.localPosition = new Vector3 (0.0f, sliderManager.SliderHandleGRP.transform.localPosition.y, sliderManager.SliderHandleGRP.transform.localPosition.z);
		}
		public HandController theHands;
		
		void Update (){
			//@Frame (Current frame)
			if (prevHandCount > 0 && theHands.GetFrame().Hands.Count == 0) {
				// Trigger the function to check if slider is at right spot
				monkeySee.Log ("Triggering StepThroughTrial");
			}
			
			//@Frame - 1 (Last frame)
			prevHandCount = theHands.GetFrame().Hands.Count;
//			monkeySee.Log ("prevHandCount = " + prevHandCount);
//			if(Input.GetKeyUp(KeyCode.T)){
//				monkeySee.Log ("sliderInt = " + sliderDragger.sliderInt);
//				char x = sliderDragger.sliderInt.ToString()[0];
//				monkeySee.Log ("char x = " + x);
//				monkeyDo.WhenPushed (true, sliderDragger.sliderInt.ToString()[0]);
//			}

		}
		
	}
}
