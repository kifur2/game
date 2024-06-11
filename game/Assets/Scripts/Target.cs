using UnityEngine;


public class Target : MonoBehaviour
{
    private bool _alive = true;

    public float health = 50f;
    public GameObject[] pickups;
    public Transform parentObject;
    private Animation _animation;
    public HealthBar healthBar;

    private const string HitEffectClipName = "HitEffect";

    public MonsterSpawner spawner;
    public ParticleSystem impactParticleSystem;

    void Start()
    {
        _animation = GetComponent<Animation>();
        if (healthBar != null)
            healthBar.SetMaxHealth(System.Convert.ToInt32(health));
    }

    public void TakeDamage(float amount)
    {
        if (!_alive) return;
        if (healthBar != null)
            healthBar.TakeDamage(System.Convert.ToInt32(amount));
        health -= amount;
        
        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySfxAtPosition(HitEffectClipName, transform.position, 1f);
        }
        
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
    }

    private void OnDestroy()
    {
        spawner.spawnedMonsters.Remove(gameObject);
        if (pickups.Length > 0)
        {
            var index = Random.Range(0, pickups.Length);
            var pickupPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
            Instantiate(pickups[index], pickupPosition, Quaternion.identity, parentObject);
        }
    }
}