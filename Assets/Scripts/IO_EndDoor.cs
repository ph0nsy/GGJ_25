using UnityEngine;
using UnityEngine.SceneManagement;

public class IO_EndDoor : InteractableObject
{
    public int sisterScene_idx = 3;

    public override void Action(){ 
        SceneManager.LoadScene(sisterScene_idx);
     }
}
