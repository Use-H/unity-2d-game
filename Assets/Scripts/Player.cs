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
    //0403���� Accelerater,brake
    public void Accelerate(float force)
    {
        playerRigid.AddForce(transform.right * force, ForceMode2D.Force);
    }
    public void Brake(float intensity)
    {
        playerRigid.linearVelocity *= intensity;
    }


    public void Move(Vector2 vector)
    {
        playerRigid.linearVelocity = vector;
    }

}
