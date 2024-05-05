using UnityEngine;


public class Target : MonoBehaviour
{
    private bool _alive = true;

    public float health = 50f;
    public GameObject[] pickups;
    public Transform parentObject;
    private Animation _animation;

    public AudioSource audioSource;
    public AudioClip hitEffect;

    public MonsterSpawner spawner;

    void Start()
    {
        _animation = GetComponent<Animation>();
    }

    public void TakeDamage(float amount)
    {
        if(_alive) { 
			health -= amount;
            audioSource.PlayDelayed(0.4f);
            audioSource.PlayOneShot(hitEffect);
			if (health <= 0f)
			{
			    _alive = false;
			    Die();
			}
	    }
    }

    void Die()
    {

        _animation.Play("Death");
        Destroy(gameObject, _animation["Death"].length);
        spawner.spawnedMonsters.Remove(this.gameObject);
        if (pickups.Length > 0)
        {
            int index = Random.Range(0, pickups.Length);
            Vector3 pickupPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
            Instantiate(pickups[index], pickupPosition, Quaternion.identity, parentObject);
        }
    }
}