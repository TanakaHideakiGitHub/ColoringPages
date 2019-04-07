using System.Collections;
using System.Collections.Generic;
using Tanaka;
using UnityEngine;

public class ObjectToucher : MonoBehaviour
{

	void Start ()
    {

    }
	
	void Update ()
    {
        var hits = UtilTouch.GetTouchObjects<PictureBookAnimation>();
        if(hits != null)
        {
            foreach (var hit in hits)
            {
                if(hit != null)
                    hit.OnTouched();
            }
        }
    }
}
