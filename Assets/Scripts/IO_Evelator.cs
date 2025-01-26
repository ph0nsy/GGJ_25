using System.Collections;
using UnityEngine;

public class IO_Evelator : InteractableObject
{
    public float heightMax = 2.0f;
    public override void Action(){
        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.GetComponent<BoxCollider>().size.y*2, LayerMask.GetMask("Player"));
        foreach(Collider hitCollider in colliders){
            if(hitCollider.gameObject.name == "Player") StartCoroutine(Elevator(2.5f, hitCollider.gameObject));
        }
        StartCoroutine(ElevatorSolo(2.5f));
    }

    IEnumerator ElevatorSolo(float time){
        float tempTime = time;
        while(tempTime > 0){
            this.transform.position += new Vector3(0, heightMax*Time.deltaTime*time, 0);
            tempTime -= Time.deltaTime;
        }
        yield return null;
    }

    IEnumerator Elevator(float time, GameObject other){
        float tempTime = 0;
        while(tempTime < time){
            other.transform.position += new Vector3(0, heightMax*Time.deltaTime*time, 0);
            this.transform.position += new Vector3(0, heightMax*Time.deltaTime*time, 0);
            tempTime += Time.deltaTime;
        }

        yield return null;
    }
}
