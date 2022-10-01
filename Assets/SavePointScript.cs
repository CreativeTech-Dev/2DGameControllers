using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        other.GetComponent<PlayerReset>().SetRestartPos(transform.position);
        Destroy(this.gameObject);
        
    }
}
