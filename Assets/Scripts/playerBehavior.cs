using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBehavior : MonoBehaviour
{
    public float speed; //speed mult
    public float jumpPower; //power mult
    public float jumpLength;//how "long" the boost from the jump lasts
    private float jumpJuice;//how long of the boost remains; working variable

    public float wJumpPower; //power mult (wall jump)
    public float wJumpLength;//how "long" the boost from the jump lasts
    private float wJumpJuice;//how long of the boost remains; working variable

    public float airDragMul; //less air control but higher speeds possible while bhopping
    public float airSpeedMul; //hard to make this work this way; definitely something i need to look into

    public float floorDetDist; //how long is our floor detection ray
    public float wallDetDist; //how long is our wall detection ray (this should just be the local height of the player but hey y'never know)

    public float gravityMultiplier;
    public float dragX;

    float animTimer = 0;
    public float animSpeed = 0.25f;
    int walkframes;

    Rigidbody2D myBody;
    BoxCollider2D myCollider;
    SpriteRenderer myRenderer;

    public Sprite idleSprite;
    public Sprite jumpSprite;
    public Sprite fallSprite;
    public Sprite walkSprite1;
    public Sprite walkSprite2;
    public Sprite wallSprite;
    

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
        if(gameObject.transform.position.y < -10){
            gameObject.transform.position = new Vector2(0, 0);
        }

        handleFloorWall();
        checkKeys();
        handleMotion();
        jumpPhysics();
        handleSprites();
    }

    void checkKeys()
    {
        if(Input.GetKey(KeyCode.RightArrow)){
            moveDir = 1;
            faceRight = false;
        } else if(Input.GetKey(KeyCode.LeftArrow)){
            moveDir = -1;
            faceRight = true;
        } else{
            moveDir = 0;
        }
        if(Input.GetKey(KeyCode.Space) && onFloor){
            jumpJuice = jumpLength;
            onFloor = false;
        } else if(Input.GetKey(KeyCode.Space) && isWallSliding){    //handling wall jump
            wJumpJuice = wJumpLength;
            isWallSliding = false;
        }
        if(Input.GetKey(KeyCode.Space) && jumpJuice > 0){
            myBody.velocity += new Vector2(0, jumpPower * jumpJuice/jumpLength);
            jumpJuice--;
        } else if(!Input.GetKey(KeyCode.Space) && jumpJuice > jumpJuice/0.6){
            myBody.velocity += new Vector2(0, jumpPower * jumpJuice/jumpLength);
            jumpJuice--;
        } else if(!Input.GetKey(KeyCode.Space) && jumpJuice > 0){
            jumpJuice = 0;
        }
        if(Input.GetKey(KeyCode.Space) && wJumpJuice > 0 && jumpJuice == 0){   //handling wall jump
            Vector2 wallJumpDir = new Vector2(-moveDir, 1).normalized * wJumpPower;
            myBody.velocity += wallJumpDir;
            if(myBody.velocity.y >= 4f * wJumpPower){
                myBody.velocity = new Vector2(myBody.velocity.x, 4f * wJumpPower);
            }
            wJumpJuice--;
        } else if(!Input.GetKey(KeyCode.Space) && wJumpJuice > 0){
            wJumpJuice = 0;
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
        if(onFloor){
            myBody.velocity = new Vector2(myBody.velocity.x * dragX, myBody.velocity.y);
            myBody.velocity += new Vector2(moveDir * speed, 0);
        } else{
            myBody.velocity = new Vector2(myBody.velocity.x * dragX * airDragMul, myBody.velocity.y);
            myBody.velocity += new Vector2(moveDir * speed * airSpeedMul, 0);
        }
    }

    void handleSprites(){
        myRenderer.flipX = !faceRight;
        
        
        if(onFloor && moveDir != 0){
            animTimer += Time.deltaTime;
            if(animTimer > animSpeed){
                if(walkframes == 0){
                    myRenderer.sprite = walkSprite1;
                    walkframes++;
                } else if(walkframes == 1){
                    myRenderer.sprite = idleSprite;
                    walkframes++;
                } else if(walkframes == 2){
                    myRenderer.sprite = walkSprite2;
                    walkframes++;
                } else if(walkframes == 3){
                    myRenderer.sprite = idleSprite;
                    walkframes = 0;
                }
                animTimer = 0;
            }

        }else if(!onFloor && !isWallSliding){
            if(myBody.velocity.y > 0){
                myRenderer.sprite = jumpSprite;
            } else if(myBody.velocity.y < 0){
                myRenderer.sprite = fallSprite;
            }
        } else if(!onFloor && isWallSliding){
            myRenderer.sprite = wallSprite;
        } else{
            myRenderer.sprite = idleSprite;
        }
    }

    void handleFloorWall(){
        //floor first, then wall; we want walljump bools to generate properly if we're smashing against the wall AND in midair
        //in other words, we can't just be next to a wall; we have to be moving towards it to trigger the wallslide

        if(faceRight){
            wallRayHeading = headingMul;
        }else wallRayHeading = -headingMul;

        Vector2 Xoffset = new Vector2(transform.position.x + (myCollider.size.x/2), transform.position.y - floorDetDist);  //this tells us what offset we use
        Vector2 Xoffsetwall = new Vector2 (transform.position.x - (myCollider.size.x * wallRayHeading), transform.position.y + (myCollider.size.y/2));
        //remember, wallRayHeading is dependent on headingmul!
        RaycastHit2D floorDet = Physics2D.Raycast(Xoffset, new Vector2(-1, 0), myCollider.size.x);
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

        //debug rays
        Debug.DrawRay(Xoffset, new Vector2(-myCollider.size.x, 0), Color.green);
        Debug.DrawRay(Xoffsetwall, new Vector2(0, -wallDetDist), Color.green);

        //after this is wall det code. active only when inputting left or right and not on ground
        RaycastHit2D wallDet = Physics2D.Raycast(Xoffsetwall, new Vector2(0, -1), wallDetDist);
        if(moveDir != 0 && !onFloor){
            if(wallDet.collider != null){
                Debug.Log(wallDet.collider.tag);
                if(wallDet.collider.tag == "Floor" || wallDet.collider.tag == "MovingPlatform"){
                    isWallSliding = true;
                } else{
                    isWallSliding = false;
                }
            } else{
                    isWallSliding = false;  //so many catches but all of them are necessary pain
            }
        } else isWallSliding = false;
        if(onFloor){
            isWallSliding = false;
        }

        if(isWallSliding && myBody.velocity.y <= -5){
            myBody.velocity = new Vector2(myBody.velocity.x, -5);
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