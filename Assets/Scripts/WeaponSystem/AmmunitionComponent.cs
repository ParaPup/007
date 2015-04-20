using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Proxy component for AmmunitionInvItem struct. Acts as a container holding an AmmunitionInvItem object with the specified values.
public class AmmunitionComponent : MonoBehaviour
{
	public Ammunition ammunition;

	[SerializeField] [ReadOnly] private Collider pickupCollisionComp;
    [SerializeField] [ReadOnly] private Rigidbody physicsComp;
    [SerializeField] [ReadOnly] private MeshFilter meshComp;
    [SerializeField] [ReadOnly] private MeshRenderer meshRendComp;
    [SerializeField] [ReadOnly] private Collider modelCollisionComp;
    [SerializeField] [ReadOnly] private Material materialComp;

	public void Awake()
	{
		physicsComp = GetComponent<Rigidbody>();
		pickupCollisionComp = GetComponent<Collider>();
		Transform model = transform.FindChild("Model");
		if (model != null)
		{
			meshComp = model.GetComponent<MeshFilter>();
			meshRendComp = model.GetComponent<MeshRenderer>();
			modelCollisionComp = model.GetComponent<Collider>();
			if (meshRendComp != null) materialComp = meshRendComp.material;
		}
		ValidateImperatives();
	}

	private void ValidateImperatives()
	{
		if (physicsComp == null)
		{
			Debug.LogError(name + " does not contain a Physics Component (RigidBody)");
		}
		if (pickupCollisionComp == null)
		{
			Debug.LogError(name + " does not contain a Pickup Collision Component (Collider)");
		}
		if (transform.FindChild("Model") == null)
		{
			Debug.LogError(name + " must contain a child object named \"Model\"");
		}
		if (meshComp == null)
		{
			Debug.LogError(name + "'s Model does not contain a Mesh Filter Component (MeshFilter)");
		}
		if (meshRendComp == null)
		{
			Debug.LogError(name + "'s Model does not contain a Renderable Mesh Component (MeshRenderer)");
		}
		if (modelCollisionComp == null)
		{
			Debug.LogError(name + "'s Model does not contain a Collision Component (Collider)");
		}
		if (materialComp == null)
		{
			Debug.LogError(name + "'s Model does not contain a Material Component (Material)");
		}
	}

	public void OnPickup(Transform actor)
	{
        DestroyObject(gameObject);
	}

	public void OnDrop()
	{
		// Re-enable Physics properties
		physicsComp.isKinematic = false;
		physicsComp.detectCollisions = true;
		transform.SetParent(null);
		// Re-enable the Pickup components
		StartCoroutine(ReenableCollisionAfterDelay(1.0f));
		// "Throw" weapon forward
		physicsComp.AddForce(transform.forward * 8, ForceMode.Impulse);
	}

	// Handle pickup collision
	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			var clone = other.GetComponent<WeaponManager>();
            if (clone != null) clone.PickUpAmmo(this);
		}
	}

	// Coroutine that delays the reactivation of the pickup physics component by (float) seconds number of seconds
	private IEnumerator ReenableCollisionAfterDelay(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		pickupCollisionComp.enabled = true;
	}
}
