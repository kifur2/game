using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;
    public GameObject[] pickups;
    public Transform parentObject;

    public MonsterSpawner spawner;

    void Start()
    {
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        spawner.spawnedMonsters.Remove(this.gameObject);
        Destroy(gameObject);
        if (pickups.Length > 0)
        {
            int index = Random.Range(0, pickups.Length);
            Vector3 pickupPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
            Instantiate(pickups[index], pickupPosition, Quaternion.identity, parentObject);
        }
    }
}