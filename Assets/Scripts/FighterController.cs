using UnityEngine;
using System.Collections;

public class FighterController : ShipController {
    void FixedUpdate() {
    	base.FixedUpdate();

    	if(ship is Fighter) {
	        if(Input.GetKey(KeyCode.Space)) {
	            (ship as Fighter).FireShell(true);
	        }
	    }
    }
}