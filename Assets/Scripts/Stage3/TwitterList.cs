using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using ButtonMonkey;

namespace P1
{
		public class TwitterList : MonoBehaviour
		{
				TwitterMonkey monkeyDo;
				TwitterReader tr;
				public GameObject items;
				public List<TwitterStatusButton> statusButtons = new List<TwitterStatusButton> ();
				static int MAX_TWEETS = 0;
				bool targetSet = false;
				public string tweetSource;
				const string TWITTER_LIST_TARGET_STATE = "listTriggerState";
				bool inEditor = false;
				static float MOVE_SCALE_COUNTER = 100f;
				static float MOVE_SCALE = 50;
				static float FRICTION = 0.9f;
				private float lastTouched = 0.0f;

		#region TouchState
				const string TWITTER_LIST_STATE_NAME = "Touched state name";
				const string TWITTER_LIST_UNTOUCHED = "Untouched twitter";
				const string TWITTER_LIST_TOUCHED = "Touched twitter";
				State touchedState;
				public float stopDelay = 0.25f;
				public Radical selector;
		
				void InitTouchState ()
				{
						if (!StateList.HasList (TWITTER_LIST_STATE_NAME))
								StateList.Create (TWITTER_LIST_STATE_NAME,
				                  TWITTER_LIST_UNTOUCHED,
				                  TWITTER_LIST_TOUCHED);
						touchedState = new State (TWITTER_LIST_STATE_NAME,
			                         TWITTER_LIST_UNTOUCHED);
						touchedState.StateChangedEvent += OnStateChanged;
				}

				void OnStateChanged (StateChange sc)
				{
						if (sc.fromState.name == TWITTER_LIST_UNTOUCHED &&
								sc.toState.name == TWITTER_LIST_TOUCHED) {
								UnityEngine.Debug.Log ("You touched Bieber!");
						}
			
						if (sc.fromState.name == TWITTER_LIST_TOUCHED &&
								sc.toState.name == TWITTER_LIST_UNTOUCHED) {
								UnityEngine.Debug.Log ("You jilted Bieber!");
								rigidbody.velocity = Vector3.zero;
								if (Radical.instance) {
										if (Radical.instance.activeTwitter) {
												int pick = Radical.instance.activeTwitter.index;
												UnityEngine.Debug.Log ("Monkey picked: " + pick);
										}
								}
						}
				}
		
				public void Touched ()
				{
						lastTouched = Time.time;
						if (touchedState.state == TWITTER_LIST_UNTOUCHED)
								touchedState.Change (TWITTER_LIST_TOUCHED);
				}
		
				void UpdateTouched ()
				{
						if (touchedState.state == TWITTER_LIST_TOUCHED &&
								Utils.Elapsed (lastTouched, stopDelay)) {
								touchedState.Change (TWITTER_LIST_UNTOUCHED);
								if (selector.activeTwitter) {
										UnityEngine.Debug.Log ("GOOD MONKEY!");
										//PROBLEM: Argument is char not int
										//monkeyDo.WhenPushed (true, selector.activeTwitter.index);
								} else {
										UnityEngine.Debug.Log ("BAD MONKEY!");
								}

						}
				}

			#endregion

#region loop
		
				// Use this for initialization
				void Start ()
				{
						LoadConfigs ();
						InitState ();
						if (tweetSource != "")
								ReadTweets (tweetSource);
						InitTouchState ();
						monkeyDo = new TwitterMonkey ();
						monkeyDo.ConfigureTest ("twitter");
						monkeyDo.TrialEvent += TrialUpdate;
						monkeyDo.Start ();
				}
		
				// Update is called once per frame
				void Update ()
				{
						if (!targetSet) {
								SetRandomTarget ();
						}
						rigidbody.velocity *= FRICTION;

						UpdateTouched ();
				}

				public void TrialUpdate (MonkeyTester trial)
				{
				}

#endregion

				public void LoadConfigs ()
				{
						LoadConfigs ("twitter_config.json");
				}

				public void LoadConfigs (string s)
				{
						JSONNode n = Utils.FileToJSON (s);
						MOVE_SCALE = n ["move_scale"].AsFloat;
						MOVE_SCALE_COUNTER = n ["move_scale_counter"].AsFloat;
						FRICTION = n ["friction"].AsFloat;
						MAX_TWEETS = n ["max_tweets"].AsInt;
				}

				public void InitState ()
				{
						if (!StateList.HasList (TWITTER_LIST_TARGET_STATE))
								InitListTriggerState ();
				}

				public void SetRandomTarget ()
				{
						monkeyDo.statusButtonsCount = statusButtons.Count - 1;
						monkeyDo.Start ();
						int target = monkeyDo.GetTrialKeys () [0];
						if (statusButtons.Count > 0) {
								statusButtons [target].targetState.Change ("target");
								targetSet = true;
						} else {
								targetSet = true;
						}
				}

				public void ReadTweets (string source)
				{
						tr = new TwitterReader ("justin_tweets.json");
						if (tr != null) {
								foreach (Tweet s in tr.statuses) {
										AddStatus (s);
								}
						} 
				}

				void InitListTriggerState ()
				{
						new StateList (TWITTER_LIST_TARGET_STATE, "base", "scrolling");
				}

				void AddStatus (Tweet s)
				{
						if (statusButtons.Count >= MAX_TWEETS)
								return;
						GameObject go = (GameObject)Instantiate (Resources.Load ("TwitterListStatus"));
						go.transform.parent = items.transform;
						go.transform.rotation = transform.rotation;
						go.transform.localScale = Vector3.one;
						TwitterStatusButton status = go.GetComponent<TwitterStatusButton> ();
						status.list = this;
						status.status = s;
						status.index = statusButtons.Count;
						statusButtons.Add (status);
				}

				public TwitterStatusButton PrevStatus (TwitterStatusButton s)
				{
						if (s.index <= 0) {
								return null;
						}
						return statusButtons [s.index - 1];
				}

#region aggregate state changes
/**
 * these states can only be true for a single button;clears sets for other buttons
 */
		
				public void TargetSet (TwitterStatusButton status)
				{
						foreach (TwitterStatusButton s in statusButtons) {
								if (s.index != status.index)
										s.targetState.Change ("base");
						}
				}

				public void ResetAllColors ()
				{
						foreach (TwitterStatusButton sb in statusButtons) {
								sb.ResetColor ();
						}
				}

#endregion

				public void MoveList (Vector3 movement)
				{
						float s;
						if ((movement.y > 0) != (rigidbody.velocity.y > 0))
								s = MOVE_SCALE_COUNTER;
						else 
								s = MOVE_SCALE;
						rigidbody.AddForce (movement * s);
				}
		}
}