using System.Collections;
using UnityEngine;

public class AxeWeapon : MonoBehaviour
{
    public float spiralSpeed = 5f;
    public float spiralGrowth = 0.5f;
    public float radiusIncrement = 1f;
    public float MeleeCoolDown = 1f;
    public float damage = 10f;
    public int penetration = 1;
    public int count = 10;
    public int ExtraCount = 0;
    public int prefabId;

    private Transform[] bullets;
    private LineRenderer lineRenderer;
    private float[] initialRadii;
    private Coroutine batchCoroutine;
    private float speed;
    private float calculatedSpeed;

    private void Start()
    {
        InitializeWeapon();
    }

    private void InitializeWeapon()
    {
        // Initialize LineRenderer for the spiral path
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.1f;

        bullets = new Transform[count];
        initialRadii = new float[count];

        speed = 150 * Character.WeaponSpeed;
        MeleeCoolDown = MeleeCoolDown * Character.WeaponRate;
        calculatedSpeed = MeleeCoolDown;

        // Start the toggle batch coroutine
        batchCoroutine = StartCoroutine(ToggleBatchCoroutine());
    }

    public void Batch()
    {
        count += ExtraCount;
        lineRenderer.positionCount = count;

        for (int i = 0; i < count; i++)
        {
            Transform bullet;

            if (i < transform.childCount)
            {
                bullet = transform.GetChild(i);
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }

            bullet.gameObject.SetActive(true);
            bullet.position = transform.position;
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            bullets[i] = bullet;
            initialRadii[i] = 0f; // Reset initial radii
            bullet.GetComponent<Bullet>().Init(damage, penetration, Vector3.zero, count);
        }

        count -= ExtraCount;
    }

    private IEnumerator ToggleBatchCoroutine()
    {
        while (true)
        {
            Batch(); // Initialize bullets in batch
            yield return new WaitForSeconds(calculatedSpeed);
            StartCoroutine(SpiralMovementCoroutine()); // Begin spiral movement
        }
    }

    private IEnumerator SpiralMovementCoroutine()
    {
        float[] radii = new float[bullets.Length]; // Track radius for each bullet

        while (true)
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                if (bullets[i] != null)
                {
                    // Increase radius over time to create outward movement
                    radii[i] += radiusIncrement * Time.deltaTime;

                    // Calculate spiral position
                    float angle = (spiralSpeed * i) + (Time.time * spiralSpeed);
                    Vector3 offset = new Vector3(
                        Mathf.Cos(angle) * (radii[i] + spiralGrowth * i),
                        Mathf.Sin(angle) * (radii[i] + spiralGrowth * i),
                        0f
                    );

                    bullets[i].localPosition = offset;

                    // Update LineRenderer position for the spiral path
                    lineRenderer.SetPosition(i, bullets[i].position);
                }
            }
            yield return null; // Wait for the next frame
        }
    }

    private void OnDisable()
    {
        // Clean up when the weapon is disabled
        if (batchCoroutine != null)
            StopCoroutine(batchCoroutine);
    }
}
