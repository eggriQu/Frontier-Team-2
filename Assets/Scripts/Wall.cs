using System.Collections;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private BoxCollider box;

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
}
