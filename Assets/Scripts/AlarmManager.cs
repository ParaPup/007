using UnityEngine;
using System.Collections;

public class AlarmManager : MonoBehaviour {
	public bool bAlarmed;
	public AudioSource aAlarm;
	public GameObject[] AlarmLights;

	// Use this for initialization
	void Start () {
		AlarmLights = GameObject.FindGameObjectsWithTag("alarmLight");
	}
	
	public float duration = 1.0F;
	public Color color0 = Color.red;
	public Color color1 = Color.clear;
	
	// Update is called once per frame
	void Update () {
		float t = Mathf.PingPong(Time.time, duration) / duration;

		if(bAlarmed){
			for(int i = 0; i < AlarmLights.Length; i++){
				AlarmLights[i].GetComponent<Light>().color = Color.Lerp(color0, color1, t);
			}
		}
	}

	void intract(){
		if(bAlarmed){
			Debug.Log ("SHUTTING DOWN");
			bAlarmed=false;
			for(int i = 0; i < AlarmLights.Length; i++){
				AlarmLights[i].GetComponent<Light>().enabled = false;
			}
			aAlarm.Stop();
		}
		else{
			Debug.Log ("ON MO FO");
			bAlarmed=true;
			for(int i = 0; i < AlarmLights.Length; i++){
				AlarmLights[i].GetComponent<Light>().enabled = true;
			}
			aAlarm.Play();
		}
	}
}
