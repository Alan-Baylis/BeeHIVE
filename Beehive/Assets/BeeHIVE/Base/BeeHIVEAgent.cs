using UnityEngine;
using System.Collections;
using BeeHive;

public class BeeHIVEAgent : MonoBehaviour {
	
	public ScriptableTree behaviorTreeObj;
	public float tickRate = 0.06f;
	BH_BehaviorTree tree;

	// Use this for initialization
	protected virtual void Start () {
		InitBeeHive();
	}

	protected void InitBeeHive(){
		tree = BeeHIVELoader.LoadTree(behaviorTreeObj);
		tree.SetSource (this);
		StartCoroutine ("RunBhvTree");
	}

	IEnumerator RunBhvTree(){
		while(Application.isPlaying){			
			yield return new WaitForSeconds (tickRate);
			tree.Tick();
		}
	}
}
