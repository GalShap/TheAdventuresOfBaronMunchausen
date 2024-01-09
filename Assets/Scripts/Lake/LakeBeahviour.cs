using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LakeBeahviour : MonoBehaviour
{
    [SerializeField] private List<Collider> preDuckColliders;

    [SerializeField] private List<GameObject> postDuckPlatform;

    [SerializeField] private GameObject lakeBottomCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
    private void SetCollidersState(List<Collider> colliders, bool isActive)
    {
        foreach (var col in colliders)
            col.enabled = isActive;
    }
    
    /// <summary>
    /// will be called by game manager once all ducks are set in lake. moves the colliders in
    /// to position of the ducks. there must be colliders at the same amounts as ducks in scene(since they are
    /// platforms)
    /// </summary>
    /// <param name="colPos">
    /// a list of the positions of the ducks in the lake. 
    /// </param>
    public void EnablePostDuckColliders(List<Vector3> colPos)
    {
        if (colPos.Count != postDuckPlatform.Count)
        {
            Debug.Log("Error: positions and colliders amount isn't the same. Check amount of ducks in game " +
                      "manager or amount of post duck colliders in lake");
            return;
        }

        for (int i = 0; i < postDuckPlatform.Count; i++)
        {
            postDuckPlatform[i].transform.position = colPos[i];
            postDuckPlatform[i].SetActive(true);
        }
        
        lakeBottomCollider.SetActive(true);
    }
    
    /// <summary>
    /// will be called by game manager when player reached lake.
    /// deactivates all lake colliders of that were before the ducks. 
    /// </summary>
    public void DisablePreDucksColliders()
    {
        SetCollidersState(preDuckColliders, false);
    }
    
    

    
}
