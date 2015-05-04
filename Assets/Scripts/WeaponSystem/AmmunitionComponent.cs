using UnityEngine;

// Proxy component for Ammunition. Acts as a container holding an Ammunition object with the specified values.
public class AmmunitionComponent : MonoBehaviour
{
	public Ammunition ammunition;

    // Destroy world object on pickup
	public void OnPickup(Transform actor)
	{
        DestroyObject(gameObject);
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
}
