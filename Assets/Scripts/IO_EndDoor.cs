using UnityEngine;
using UnityEngine.SceneManagement;

public class IO_EndDoor : InteractableObject
{
    public int sisterScene_idx = 3;

    public override void Action(){ 
        if(GameObject.Find("WrappedObjects").transform.childCount <= 1) SceneManager.LoadScene(sisterScene_idx);
        else Debug.Log("No está todo desbloqueado");
    }
}
