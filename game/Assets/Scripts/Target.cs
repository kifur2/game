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
        if (!_alive) return;
        Debug.Log("DamageDone: " + amount);
        health -= amount;
        audioSource.PlayDelayed(0.4f);
        audioSource.PlayOneShot(hitEffect);
        if (!(health <= 0f)) return;
        _alive = false;
        Die();
    }

    private void Die()
    {
        _animation.Play("Death");
        var cl = GetComponent<Collider>();
        if (cl != null)
        {
            Destroy(cl);
        }

        Destroy(gameObject, _animation["Death"].length);
        spawner.spawnedMonsters.Remove(gameObject);
        if (pickups.Length <= 0) return;
        var index = Random.Range(0, pickups.Length);
        var pickupPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        Instantiate(pickups[index], pickupPosition, Quaternion.identity, parentObject);
    }
}