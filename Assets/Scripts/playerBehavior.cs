using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBehavior : MonoBehaviour
{
    public float speed;
    public float jumpPower;
    public float jumpLength;
    public float jumpJuice;

    public float gravityMultiplier;
    public float dragX;

    Rigidbody2D myBody;
    BoxCollider2D myCollider;
    SpriteRenderer myRenderer;

    public Sprite jumpSprite;
    public Sprite fallSprite;
    public Sprite walkSprite;
    public Sprite idleSprite;

    float moveDir = 1; // -1 left, 0 none, 1 right
    public bool onFloor;
    public static bool faceRight = true;

    void Start()
    {
        myBody = gameObject.GetComponent<Rigidbody2D>();
        myCollider = gameObject.GetComponent<BoxCollider2D>();
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if(onFloor && Mathf.Abs(myBody.velocity.y) > 0.01){     //this introduces a really weird mechanical quirk that says you can walljump
            onFloor = false;
        }

        checkKeys();
        handleMotion();
        jumpPhysics();
        handleSprites();
    }

    void checkKeys()
    {
        if(Input.GetKey(KeyCode.D)){
            moveDir = 1;
            faceRight = false;
        } else if(Input.GetKey(KeyCode.A)){
            moveDir = -1;
            faceRight = true;
        } else{
            moveDir = 0;
        }
        if(Input.GetKey(KeyCode.W) && onFloor){
            jumpJuice = jumpLength;
            onFloor = false;
        } else if(Input.GetKey(KeyCode.W) && jumpJuice > 0){
            myBody.velocity += new Vector2(0, jumpPower * jumpJuice/jumpLength);
            jumpJuice--;
        } else if(!Input.GetKey(KeyCode.W) && jumpJuice > jumpJuice/0.6){
            myBody.velocity += new Vector2(0, jumpPower * jumpJuice/jumpLength);
            jumpJuice--;
        } else if(!Input.GetKey(KeyCode.W) && jumpJuice > 0){
            jumpJuice = 0;
        }
        /*if(Input.GetKey(KeyCode.W) && onFloor){
            myBody.velocity = new Vector2(myBody.velocity.x, jumpPower);
        }*/
    }

    void jumpPhysics(){
        if(!onFloor){
            myBody.velocity  += Vector2.up * Physics.gravity.y * (gravityMultiplier - 1f) * Time.deltaTime;
        }
    }


    void handleMotion()
    {  
        //horizontal drag
        myBody.velocity = new Vector2(myBody.velocity.x * dragX, myBody.velocity.y);

        //move left and right
        myBody.velocity += new Vector2(moveDir * speed, 0);
    }

    void handleSprites(){
        myRenderer.flipX = faceRight;
        if(!onFloor){
            if(myBody.velocity.y > 0){
                myRenderer.sprite = jumpSprite;
            } else if(myBody.velocity.y < 0){
                myRenderer.sprite = fallSprite;
            }
        } else{
            myRenderer.sprite = idleSprite;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Floor"){
            onFloor = true;
        } else if(other.gameObject.tag == "MovingPlatform"){
            onFloor = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy"){
            Destroy(gameObject);
        }
    }
}
