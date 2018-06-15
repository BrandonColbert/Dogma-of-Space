using UnityEngine;

[RequireComponent(typeof(BreakableObject))]
public class BreakObject : MonoBehaviour {
    ///<summary>Used to force a fracture or shatter in the editor</summary>
    public bool forceBreak = false, forceShatter = false;

    void Update() {
        if(forceBreak) {
            forceBreak = forceShatter = false;
            GetComponent<BreakableObject>().Break();
        } else if(forceShatter) {
            forceBreak = forceShatter = false;
            GetComponent<BreakableObject>().Shatter();
        }
    }
}