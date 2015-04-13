using UnityEngine;
using System.Collections;

public class ButtonHandle : MonoBehaviour {
	public GameObject oGate;
	public bool Open;
	public Vector3 newPosition;

	private Vector3 startPos;


	// Use this for initialization
	void Start () { 
		startPos = oGate.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void intract(){
		if(Open){
			Open=false;
			StartCoroutine(MoveFunction());
			}
		else{
			Open=true;
			StartCoroutine(MoveFunction2());
		}
	}

	IEnumerator MoveFunction()
	{
		float timeSinceStarted = 0f;
		while (true)
		{
			timeSinceStarted += Time.deltaTime;
			oGate.transform.position = Vector3.Lerp(oGate.transform.position, newPosition, timeSinceStarted);
			
			// If the object has arrived, stop the coroutine
			if (oGate.transform.position == newPosition)
			{
				yield break;
			}
			
			// Otherwise, continue next frame
			yield return null;
		}
	}

	IEnumerator MoveFunction2()
	{
		float timeSinceStarted = 0f;
		while (true)
		{
			timeSinceStarted += Time.deltaTime;
			oGate.transform.position = Vector3.Lerp(oGate.transform.position, startPos, timeSinceStarted);
			
			// If the object has arrived, stop the coroutine
			if (oGate.transform.position == startPos)
			{
				yield break;
			}
			
			// Otherwise, continue next frame
			yield return null;
		}
	}
}