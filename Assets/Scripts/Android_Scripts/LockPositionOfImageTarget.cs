using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class LockPositionOfImageTarget : MonoBehaviour
{
    private Manager localManager;
    [SerializeField]private GameObject activeTarget;
    private GameObject previousActiveTarget = null;
    [SerializeField]private Vector3 lockedPositions;

    private void Start()
    {
        localManager = this.gameObject.GetComponent<Manager>();
        lockedPositions = new Vector3(0,0,0);
    }

    private void Update()
    {
        //activeTarget = localManager.GetActiveImageTarget();
        //if (previousActiveTarget != null)
        //{
        //    if (previousActiveTarget != activeTarget)
        //    {
        //        lockedPositions.Set(activeTarget.transform.position.x, activeTarget.transform.position.y, activeTarget.transform.position.z);
        //        previousActiveTarget = activeTarget;
        //    }
        //    //here we mantain the target initial transform position locked
        //    else
        //    {
        //        activeTarget.transform.position = lockedPositions;
        //    }
        //}
        //else
        //{
        //    if(activeTarget != null)
        //    {
        //        lockedPositions.Set(activeTarget.transform.position.x, activeTarget.transform.position.y, activeTarget.transform.position.z);
        //    }
            
        //    previousActiveTarget = activeTarget;
        //}
    }
}
