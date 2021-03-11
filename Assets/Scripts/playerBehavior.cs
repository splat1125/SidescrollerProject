using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBehavior : MonoBehaviour
{
    public float speed; //speed mult
    public float jumpPower; //power mult
    public float jumpLength;//how "long" the boost from the jump lasts
    private float jumpJuice;//how long of the boost remains; working variable

    public float floorDetDist; //how long is our floor detection ray
    public float wallDetDist; //how long is our wall detection ray (this should just be the local height of the player but hey y'never know)

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
    float wallRayHeading; // left or right detached from motioncode for walljump logic
    public float headingMul;    //how far out the ray should be cast

    public bool onFloor;
    public bool isWallSliding;  //public for debug purposes; will private these later lol

    public static bool faceRight = true;    //sprite heading

    void Start()
    {
        myBody = gameObject.GetComponent<Rigidbody2D>();
        myCollider = gameObject.GetComponent<BoxCollider2D>();
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {  
        //if(onFloor && Mathf.Abs(myBody.velocity.y) > 0.01){
        //    onFloor = false;
        //}
        if(faceRight){
            wallRayHeading = headingMul;
        }else wallRayHeading = -headingMul;

        handleFloorWall();
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
        } 
        if(Input.GetKey(KeyCode.W) && jumpJuice > 0){
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

    void handleFloorWall(){
        //floor first, then wall; we want walljump bools to generate properly if we're smashing against the wall AND in midair
        //in other words, we can't just be next to a wall; we have to be moving towards it to trigger the wallslide
        
        Vector2 Xoffset = new Vector2(transform.position.x + (transform.localScale.x/2), transform.position.y - floorDetDist);  //this tells us what offset we use
        RaycastHit2D floorDet = Physics2D.Raycast(Xoffset, new Vector2(-1, 0), transform.localScale.x);
        if(floorDet.collider != null){
            Debug.Log(floorDet.collider.tag);
            if(floorDet.collider.tag == "Floor" || floorDet.collider.tag == "MovingPlatform"){
                onFloor = true;
            } else{
                onFloor = false;
            }
        } else{
            onFloor = false;
        }

        //floor debug ray
        Debug.DrawRay(Xoffset, new Vector2(-transform.localScale.x, 0), Color.green);

        Xoffset = new Vector2 (transform.position.x - (transform.localScale.x * wallRayHeading), transform.position.y + (transform.localScale.y/2));
        RaycastHit2D wallDet = Physics2D.Raycast(Xoffset, new Vector2(0, -1), wallDetDist);
        Debug.DrawRay(Xoffset, new Vector2(0, -wallDetDist), Color.green);
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
