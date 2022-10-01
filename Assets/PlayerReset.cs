using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReset : MonoBehaviour
{
    Vector3 resetPos;
    void Start() {
        resetPos = transform.position;
    }
    public void SetRestartPos(Vector3 pos) {
        resetPos = pos;
    }
    public Vector3 GetRestartPos() {
        return resetPos;
    }
}
