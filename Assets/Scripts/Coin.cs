using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
   private Animator anim;
   private void Awake(){
    anim = GetComponent<Animator>();
   } 
   private void OnTriggerEnter(Collider other){
    if (other.tag == "Player")
    {
      Debug.Log("collided");
    GameManager.Instance.GetCoin();
    anim.SetTrigger("Collected");
    }
   }
   private void OnEnable(){
    anim.SetTrigger("Spawn");
   }
}
