using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickObject : MonoBehaviour, IInteractable
{
    PickObject po;
    public Transform holdParent;
    private GameObject heldObj;
   
    
    public float moveForce = 250; // The force which this object will be moved with, if this is a light object this can be held high else it can be held low.
    public void Interact()
    {
        PickUp();
        po.Update();
        
    }

    private void LateUpdate()
    {
        if (holdParent == null)
        {
            HoldParent_Tag[] _holdParent = FindObjectsOfType<HoldParent_Tag>();
            holdParent = _holdParent[0].gameObject.transform;
        }
        
    }
    void Start()
    {
        po = GetComponent<PickObject>();
        
        
    }
    void PickUp()
    {
        if (heldObj == null)
        {
            Rigidbody objRig = this.GetComponent<Rigidbody>();
            objRig.useGravity = false;
            objRig.drag = 10;
            objRig.transform.parent = holdParent;
            heldObj = this.gameObject;
            
        }
        

    }
    void Update()
    {
        MoveObject();
        DropObject();
    }
    void MoveObject()
    {
        if (heldObj == null) return;
        if (Vector3.Distance(heldObj.transform.position, holdParent.position) > 0.1f)
        {
            Vector3 moveDirection = holdParent.position - heldObj.transform.position;
            heldObj.GetComponent<Rigidbody>().AddForce(moveDirection * moveForce);
            
        }
        
    }
    void DropObject()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (heldObj != null)
            {
                Rigidbody heldRig = heldObj.GetComponent<Rigidbody>();
                heldRig.useGravity = true;
                heldRig.drag = 1;

                heldObj.transform.parent = null;
                heldObj = null;
                
            }

        }
        
    }
}
