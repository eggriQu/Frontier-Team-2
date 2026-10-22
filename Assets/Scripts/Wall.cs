using System.Collections;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private BoxCollider box;
    [SerializeField] private GameObject nodePrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        box = GetComponent<BoxCollider>();
        StartCoroutine(WaitToSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator WaitToSpawn()
    {    
        yield return new WaitForSeconds(0.1f);
        box.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damage"))
        {
            Vector3 spawnVector = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Instantiate(nodePrefab, spawnVector, Quaternion.identity, GameObject.Find("NodeParent").transform);
            Destroy(gameObject);
        }
    }
}
