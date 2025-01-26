using System.Collections;
using UnityEngine;

public class IO_Evelator : InteractableObject
{
    public float heightMax = 2.0f;
    public GameObject platform;
    public AudioSource sound;
    //public Animator anim; //Arrastrar plataforma
    public override void Action(){
        sound.Play();
        /*Collider[] colliders = Physics.OverlapSphere(platform.transform.position, platform.transform.GetComponent<BoxCollider>().size.y*2, LayerMask.GetMask("Player"));
        foreach(Collider hitCollider in colliders){
            if(hitCollider.gameObject.name == "Player") StartCoroutine(Elevator(2.5f, hitCollider.gameObject));
        }
        //StartCoroutine(ElevatorSolo(2.5f));*/
        //anim.SetBool("name", true);
    }

    IEnumerator ElevatorSolo(float time){
        this.GetComponent<Animator>().enabled = true;
        float tempTime = time;
        while(tempTime > 0){
            platform.transform.position += new Vector3(0, heightMax*Time.deltaTime/time, 0);
            tempTime -= Time.deltaTime;
        }
        yield return null;
    }

    IEnumerator Elevator(float time, GameObject other){
        this.GetComponent<Animator>().enabled = true;
        float tempTime = 0;
        while(tempTime < time){
            other.transform.position += new Vector3(0, heightMax*Time.deltaTime/time, 0);
            platform.transform.position += new Vector3(0, heightMax*Time.deltaTime/time, 0);
            tempTime += Time.deltaTime;
        }

        yield return null;
    }
}
