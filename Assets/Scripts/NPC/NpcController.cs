using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NpcController : StateController
{
    public float speed = 3f;
    public int panicMultiplier = 1;

    private void Awake()
    {
        //FindInterestingNodes();
    }

    void Start()
    {
        ChangeState(_startState);
    }

    // Update is called once per frame
    void Update()
    {
        CreatePath();
    }

    public void CreatePath()
    {
        if (path.Count > 0)
        {
            int x = 0;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[x].transform.position.x, path[x].transform.position.y, path[x].transform.position.z),
                                (speed * panicMultiplier) * Time.deltaTime);

            if (Vector3.Distance(transform.position, path[x].transform.position) < 0.1f)
            {
                currentNode = path[x];
                currentNode.hasBeenVisited = true;
                path.RemoveAt(x);
                if (path.Count == 0)
                {
                    currentState.Execute(this);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 && path.Count < 1)
        {
            path.Add(other.GetComponent<Node>());
        }
    }
}
