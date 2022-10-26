using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    
    
    //Movement
    private CharacterController controller;
    private const float LANE_DISTANCE = 2.5f;
    private const float TURN_SPEED = 0.05f;

    private bool isRunning = false;
    private float jumpForce = 5.0f;
    private float gravity = 12.0f;
    private Animator anim;
    private float verticalVelocity;
    
    private int desiredLane = 1; //0 is left, 1 is middle, 2 is right

    // speed modifier
    private float originalSpeed = 7.0f;
    private float speed;
    private float speedIncreaseLastTick; 
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f;
    
    private void Start(){
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        speed = originalSpeed;
    }

    private void Update(){
        if (!isRunning)
            return;

        if (Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            GameManager.Instance.UpdateModifier(speed - originalSpeed);
        }
        // Gather input on which lane we should be
        if (Input.GetKeyDown(KeyCode.LeftArrow) || MobileInput.Instance.SwipeLeft)
        {
            moveLane(false);
        }
          if (Input.GetKeyDown(KeyCode.RightArrow) || MobileInput.Instance.SwipeRight)
        {
            moveLane(true);
        }

        //calculate where we should be in future
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if(desiredLane == 0)
            targetPosition += Vector3.left * LANE_DISTANCE;
        else if(desiredLane == 2)    
            targetPosition += Vector3.right * LANE_DISTANCE;

        // let's calculate our move delta
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;   

        //calculate y

        bool isGrounded = IsGrounded();
        anim.SetBool("Grounded",isGrounded);
        if(isGrounded){
            verticalVelocity = -0.1f;
            if (Input.GetKeyDown(KeyCode.Space) || MobileInput.Instance.SwipeUp)
            {
                //Jump
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;

            }
            else if(MobileInput.Instance.SwipeDown){
                // Slide
                StartSliding();
                Invoke("StopSliding",1.0f);
            }
            }else{
                verticalVelocity -= (gravity * Time.deltaTime);

                //fast falling mechaniccs
            if (Input.GetKeyDown(KeyCode.Space) || MobileInput.Instance.SwipeDown)
            {
                verticalVelocity = -jumpForce;
            }
            }
        

        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        //move the pengu
        controller.Move(moveVector * Time.deltaTime); 

        // rotate the player according to the movement
        Vector3 dir = controller.velocity;
        if(dir != Vector3.zero){
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);
        }
    }

    private void StartSliding(){
        anim.SetBool("Sliding",true);
        controller.height /= 3;
        controller.center = new Vector3(controller.center.x, controller.center.y / 3, controller.center.z);
        controller.radius /= 2;

    } 

    private void StopSliding(){
        anim.SetBool("Sliding",false);
        controller.height *= 3;
        controller.center = new Vector3(controller.center.x, controller.center.y * 3, controller.center.z);
        controller.radius *= 2;
    }
    private void moveLane(bool goingRight){
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane,0,2);
    }
    private bool IsGrounded(){
        Ray groundRay = new Ray(new Vector3(controller.bounds.center.x,(controller.bounds.center.y - controller.bounds.extents.y) + 0.2f, controller.bounds.center.z),Vector3.down); 
        Debug.DrawRay(groundRay.origin,groundRay.direction,Color.cyan,1.0f);

        return Physics.Raycast(groundRay,0.2f + 0.1f);
    }

    public void StartRunning(){
        isRunning = true;
        anim.SetTrigger("StartRunning");
    }

    private void Crash(){
        anim.SetTrigger("Death");
        isRunning = false;
        GameManager.Instance.OnDeath();
    }
    private void OnControllerColliderHit(ControllerColliderHit hit){
        switch (hit.gameObject.tag)
        {
            case "Obstacle":
            Crash();
            break;
        }
    }
}
