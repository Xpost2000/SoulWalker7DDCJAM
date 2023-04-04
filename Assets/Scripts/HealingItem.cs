using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  This is basically just a data holder.
*/
public class HealingItem : MonoBehaviour
{
    // Start is called before the first frame update
    public int amount;

    void Start() {
        
    }

    void DestroyItem() {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
