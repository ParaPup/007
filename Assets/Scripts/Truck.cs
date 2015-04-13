using UnityEngine;
using System.Collections;

public class Truck : MonoBehaviour {

	public GameObject[] waypoints;
	public GameObject closestObject;
	private NavMeshAgent NMA;

	void Awake(){
		//closestObjectNearMe();
		closestObject = GameObject.Find("Waypoint");
	}

	// Use this for initialization
	void Start () {
		NMA = GetComponent<NavMeshAgent>();
		StartCoroutine(drive(closestObject));
	}
	
	// Update is called once per frame
	void Update () {
		//closestObjectNearMe();
	}

//	void closestObjectNearMe(){
//		waypoints = GameObject.FindGameObjectsWithTag("waypoint");
//		for(int i = 0; i < waypoints.Length; i++)
//		{
//			if(!closestObject){
//				closestObject = waypoints[i];
//			}
//			if(Vector3.Distance(transform.position, waypoints[i].GetComponent<Transform>().position) <= Vector3.Distance(transform.position, closestObject.transform.position)){
//				closestObject = waypoints[i];
//			}
//		}
//	}

	IEnumerator drive(GameObject closestObject){
		yield return new WaitForSeconds(0.5F);
		switch (closestObject.name){
		case "Waypoint":

			while(Vector3.Distance(transform.position, closestObject.transform.position) > 0.5f)
			{
				NMA.SetDestination(closestObject.transform.position);
				yield return null;
			}
			
			yield return new WaitForSeconds(0.5f);
			closestObject = GameObject.Find("Waypoint 1");

			StartCoroutine(drive (closestObject));
			break;
		case "Waypoint 1":
			//First Corner
			while(Vector3.Distance(transform.position, closestObject.transform.position) > 0.5f)
			{
				NMA.SetDestination(closestObject.transform.position);
				yield return null;
			}
			
			yield return new WaitForSeconds(0.5f);
			closestObject = GameObject.Find("Waypoint 2");
			
			StartCoroutine(drive (closestObject));
			break;
		case "Waypoint 2":
			while(Vector3.Distance(transform.position, closestObject.transform.position) > 0.5f)
			{
				NMA.SetDestination(closestObject.transform.position);
				yield return null;
			}
			
			yield return new WaitForSeconds(0.5f);
			closestObject = GameObject.Find("Waypoint 3");
			
			StartCoroutine(drive (closestObject));
			break;
		case "Waypoint 3":
			//Second Corner
			while(Vector3.Distance(transform.position, closestObject.transform.position) > 0.5f)
			{
				NMA.SetDestination(closestObject.transform.position);
				yield return null;
			}
			
			yield return new WaitForSeconds(0.5f);
			closestObject = GameObject.Find("Waypoint 4");
			
			StartCoroutine(drive (closestObject));
			break;
		case "Waypoint 4":
			while(Vector3.Distance(transform.position, closestObject.transform.position) > 0.5f)
			{
				NMA.SetDestination(closestObject.transform.position);
				yield return null;
			}
			
			yield return new WaitForSeconds(0.5f);
			closestObject = GameObject.Find("Waypoint 5");
			
			StartCoroutine(drive (closestObject));
			break;
		case "Waypoint 5":
			//Stop At Gate
			while(Vector3.Distance(transform.position, closestObject.transform.position) > 0.5f)
			{
				NMA.SetDestination(closestObject.transform.position);
				yield return null;
			}
			
			yield return new WaitForSeconds(0.5f);
			closestObject = GameObject.Find("Waypoint 6");
			
			StartCoroutine(drive (closestObject));
			break;
		case "Waypoint 6":
			//Stop for second gate
			while(Vector3.Distance(transform.position, closestObject.transform.position) > 0.5f)
			{
				NMA.SetDestination(closestObject.transform.position);
				yield return null;
			}
			
			yield return new WaitForSeconds(0.5f);
			closestObject = GameObject.Find("Waypoint 7");
			
			StartCoroutine(drive (closestObject));
			break;
		case "Waypoint 7":
			//Through gate
			while(Vector3.Distance(transform.position, closestObject.transform.position) > 0.5f)
			{
				NMA.SetDestination(closestObject.transform.position);
				yield return null;
			}
			
			yield return new WaitForSeconds(0.5f);
			closestObject = GameObject.Find("Waypoint 8");
			
			StartCoroutine(drive (closestObject));
			break;
		case "Waypoint 8":
			while(Vector3.Distance(transform.position, closestObject.transform.position) > 0.5f)
			{
				NMA.SetDestination(closestObject.transform.position);
				yield return null;
			}
			
			yield return new WaitForSeconds(0.5f);
			closestObject = GameObject.Find("Waypoint 9");
			
			StartCoroutine(drive (closestObject));
			break;
		case "Waypoint 9":
			//Park
			while(Vector3.Distance(transform.position, closestObject.transform.position) > 0.5f)
			{
				NMA.SetDestination(closestObject.transform.position);
				yield return null;
			}
			
//			yield return new WaitForSeconds(0.5f);
//			closestObject = GameObject.Find("Waypoint1");
//			
//			StartCoroutine(drive (closestObject));
			break;
		}
	}
}