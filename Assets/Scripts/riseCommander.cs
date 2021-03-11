using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class riseCommander : MonoBehaviour
{
    public bool active;
    public int dirY, dirX;    //-1 is down/left, 1 is up/right. this prevents weird confusions, even though those shouldn't happen
    public float originX, originY;  //where we start; this is set in Start() so these have no reason to be public aside from debug purposes
    public float targetX, targetY;  //where we want to be

    public Vector2 tgtVector;

    public float speed; //how fast we do it

    public bool invertXlogic = false, invertYlogic = false;

    void Start(){
        originX = gameObject.transform.position.x;
        originY = gameObject.transform.position.y;
        //find the normalized, 1-unit vector between us and the target
        tgtVector = (new Vector2(originX, originY) - new Vector2(targetX, targetY)).normalized; //this outputs normally a quadrant I vector
        tgtVector = new Vector2(Mathf.Abs(tgtVector.x), Mathf.Abs(tgtVector.y));    //but in exceptional cases, we convert it to a quadrant I vector

        if(targetX > originX){  //should we invert our logic to account for weirdly placed platforms
            invertXlogic = true;
            dirX = 1;
        } else dirX = -1;
        if(targetY > originY){
            invertYlogic = true;
            dirY = 1;
        } else dirY = -1;
    }

    void FixedUpdate()
    {
        if(active){ //only when we're active...
            if(invertXlogic){   //figuring out if we're at the right or left when starting
                if(gameObject.transform.position.x < originX){
                    dirX = 1;
                } else if(gameObject.transform.position.x > targetX){
                    dirX = -1;
                }
            } else if(!invertXlogic){
                if(gameObject.transform.position.x < targetX){
                    dirX = 1;
                } else if(gameObject.transform.position.x > originX){
                    dirX = -1;
                }
            }

            if(invertYlogic){   //figuring out if we're at the top or bottom when starting
                if(gameObject.transform.position.y < originY){
                    dirY = 1;
                } else if(gameObject.transform.position.y > targetY){
                    dirY = -1;
                }
            } else if(!invertYlogic){
                if(gameObject.transform.position.y < targetY){
                    dirY = 1;
                } else if(gameObject.transform.position.y > originY){
                    dirY = -1;
                }
            }
            
            //after finding what way we're going, we move
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + (dirX * tgtVector.x * speed * Time.deltaTime), gameObject.transform.position.y + (dirY * tgtVector.y * speed * Time.deltaTime), gameObject.transform.position.z);
            //i hate how long this line is ^ why do transform positions need to be so long
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player" && active){
            Debug.Log("I am trying to move the player now!");
            //other.gameObject.transform.position = new Vector3(other.gameObject.transform.position.x + (dirX * tgtVector.x * speed * Time.deltaTime), other.gameObject.transform.position.y + (dirY * tgtVector.y * speed * Time.deltaTime), other.gameObject.transform.position.z);
            other.transform.parent = transform;
        }
    }
    void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player" && active){
            Debug.Log("I am trying to move the player now!");
            //other.gameObject.transform.position = new Vector3(other.gameObject.transform.position.x + (dirX * tgtVector.x * speed * Time.deltaTime), other.gameObject.transform.position.y + (dirY * tgtVector.y * speed * Time.deltaTime), other.gameObject.transform.position.z);
            other.transform.parent = null;
        }
    }
    /*void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player" && active){
            Debug.Log("I am trying to move the player now!");
            other.gameObject.transform.position = new Vector3(other.gameObject.transform.position.x + (dirX * tgtVector.x * speed * Time.deltaTime), other.gameObject.transform.position.y + (dirY * tgtVector.y * speed * Time.deltaTime), other.gameObject.transform.position.z);
            //problem: how do we do this before gravity kicks in so the player stays "parented" to the platform
            //should i just parent the player to the platform? that doesn't really make any sense at all since it wouldn't really translate them right
        }
    }*/
}