using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;
	public Camera oCam;
	public float health;
	public AudioClip pain;
	public UnityEngine.UI.Slider healthBar;
	public bool crouching;

	void Update() {
		intract();
		//healthBar.value = health;

		CharacterController controller = GetComponent<CharacterController>();
		if (controller.isGrounded) {
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if (Input.GetButton("Jump"))
				moveDirection.y = jumpSpeed;
		}
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
		crouch();
	}

	void crouch(){
		if(Input.GetButtonDown("Crouch")){
			Debug.Log("crouch");
			if(crouching){
				crouching = false;
				Debug.Log("crouching");
				gameObject.GetComponent<Transform>().transform.localScale = new Vector3(1F, 1F, 1F);
				speed = 6F;
			}
			else if(!crouching){
				crouching = true;
				speed = 3F;
				Debug.Log("!crouching");
				gameObject.GetComponent<Transform>().transform.localScale = new Vector3(1F, 0.5F, 1F);
			}
		}
	}

	RaycastHit rhit;
	void intract(){
		if (Input.GetButtonDown("Intract")){
			Ray ray = oCam.ScreenPointToRay(new Vector3(oCam.pixelWidth/2, oCam.pixelHeight/2, 0));
			if (Physics.Raycast(ray, out rhit, 3)){
				if(rhit.collider.gameObject.tag == "intractable"){
				rhit.collider.gameObject.SendMessage("intract");
				}
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "DeathZone"){
			Application.LoadLevel(Application.loadedLevelName);
		}
	}

	void hit(float damage){
		health -= damage;
		GetComponent<AudioSource>().PlayOneShot(pain);
	}

	IEnumerator Death(){
		yield return new WaitForSeconds(3F);
		Debug.Log ("Do Death Scene");
		Application.LoadLevel(Application.loadedLevelName);
	}
}