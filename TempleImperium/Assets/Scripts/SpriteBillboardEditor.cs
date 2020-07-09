using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//written by Ase


[ExecuteAlways]
public class SpriteBillboardEditor : MonoBehaviour
{
    //for editor logic entity sprites
    //always faces editor camera, destroys self at runtime. put me on a sprite!

    void Start()
    {
        if (Application.isPlaying)  //if ingame
        {
            Destroy(transform.gameObject);  //kill sprite
        }
    }

    void Update()
    {
        //Debug.Log("editor tick");
        if (SceneView.lastActiveSceneView != null)  //make sure there's a scene camera
        {
            //Debug.Log("aligning sprite");
            transform.LookAt(SceneView.lastActiveSceneView.camera.transform.position, Vector3.up);  //face scene camera
        }
    }


    //https://forum.unity.com/threads/solved-how-to-force-update-in-edit-mode.561436/
    void OnDrawGizmos()
    {

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
#endif
    }
}
