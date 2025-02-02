using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using Ekkam;

public class Crystal : MonoBehaviour
{
    public bool crumbleTiles = false;

    [SerializeField] GameObject tileUndamaged;
    [SerializeField] GameObject tileSlightlyDamaged;

    [SerializeField] GameObject tileVeryDamaged;
    [SerializeField] GameObject[] tileVeryDamagedPieces;
    
    public bool isBroken = false;

    MeshCollider meshCollider;
    
    Signalable onBreakSignal;

    void Start()
    {
        tileVeryDamaged.SetActive(false);
        tileSlightlyDamaged.SetActive(false);
        tileUndamaged.SetActive(true);

        // Rotate hexagon tile randomly to make the cracks look more natural
        transform.Rotate(0, 0, Random.Range(0, 6) * 60);

        foreach (MeshCollider collider in GetComponentsInChildren<MeshCollider>())
        {
            if (collider.name == "Collider")
            {
                meshCollider = collider;
            }
        }
        
        // DamageTile();
        // DamageTile();
        // DamageTile();
    }

    void Update()
    {
        
    }

    async public void DamageTile()
    {
        if (tileUndamaged.activeSelf)
        {
            tileUndamaged.SetActive(false);
            tileSlightlyDamaged.SetActive(true);
            SoundManager.Instance.PlaySound("crystal-hit");
        }
        else if (tileSlightlyDamaged.activeSelf)
        {
            tileSlightlyDamaged.SetActive(false);
            tileVeryDamaged.SetActive(true);
            SoundManager.Instance.PlaySound("crystal-hit");
        }
        else if (tileVeryDamaged.activeSelf)
        {
            SoundManager.Instance.PlaySound("crystal-hit");
            // jitter the tile
            for (int i = 0; i < 30; i++)
            {
                Vector3 jitter = new Vector3(Random.Range(-0.1f, 0.1f), 0f, Random.Range(-0.1f, 0.1f));
                tileVeryDamaged.transform.position += jitter;
                await Task.Delay(20);
                tileVeryDamaged.transform.position -= jitter;
            }

            meshCollider.enabled = false;
            GetComponent<MeshCollider>().enabled = false;
            SoundManager.Instance.PlaySound("crystal-break");
            foreach (GameObject piece in tileVeryDamagedPieces)
            {
                // Add rigidbody to piece
                Rigidbody rb = piece.AddComponent<Rigidbody>();
                rb.AddExplosionForce(100f, transform.position, 10f);
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            }
            
            isBroken = true;
            
            Invoke("HideTile", 10f);
        }
    }

    void HideTile()
    {
        gameObject.SetActive(false);
    }
}
