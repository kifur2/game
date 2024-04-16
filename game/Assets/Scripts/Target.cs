using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;
    public GameObject[] pickups;
    public Transform[] parentObjects;

    public MonsterSpawner spawner;

    void Start()
    {
	    if ((parentObjects == null || parentObjects.Length == 0) && (pickups != null && pickups.Length > 0))
	    {
		    parentObjects = new Transform[pickups.Length];
	    }
    }

    public void TakeDamage(float amount) {
        health -= amount;
        if(health <= 0f) {
            Die();
	    }
    }

    void Die()
    {
	    spawner.spawnedMonsters.Remove(this.gameObject);
        Destroy(gameObject);
        if(pickups.Length > 0) { 
			int index = Random.Range(0, pickups.Length);
			Vector3 pickupPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
			Instantiate(pickups[index], pickupPosition, Quaternion.identity, parentObjects[index]);
	    }
    }
}
