using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D playerRigid;
    void Start()
    {
        playerRigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }

    public void Move(Vector2 vector)
    {
        playerRigid.linearVelocity = vector;
    }

}
