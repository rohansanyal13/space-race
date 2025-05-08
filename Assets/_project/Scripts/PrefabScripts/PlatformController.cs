using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public float raisedHeight = 3f;
    public float moveSpeed = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isRaised = false;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * raisedHeight;
    }

    void Update()
    {
        Vector3 desiredPosition = isRaised ? targetPos : startPos;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, moveSpeed * Time.deltaTime);
    }

    public void SetRaised(bool raise)
    {
        isRaised = raise;
    }
}
