using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TransparencyManager : MonoBehaviour
{
    
    [SerializeField] private float fadeDuration = 0.5f; // the duration of the fade in seconds
    private int worldObjectLayer = 12; // the layer that the objects to be made transparent belong to
    [SerializeField] private float minAlpha = 0f;
    [SerializeField] private Transform player;
    [SerializeField] private float radius = 3f;

    private Color _fadedColor;
    private Color _unFadedColor = Color.white; 
    private LayerMask _worldObjectLayerMask;
    
    private List<GameObject> _curObjects = new List<GameObject>();
    private List<SpriteRenderer> _curObjectsRenderers = new List<SpriteRenderer>();

    private List<GameObject> _prevObjects = new List<GameObject>();
    private List<SpriteRenderer> _prevSpriteRenderers = new List<SpriteRenderer>();

    private void Start()
    {
        _worldObjectLayerMask =  1 << worldObjectLayer;
        _fadedColor = new Color(1, 1, 1, minAlpha);
    }

    void UpdateObjectsInRadius()
    {
        Collider[] hits = Physics.OverlapSphere(player.position, radius, _worldObjectLayerMask);
        var size = hits.Length;
        
        if (size == 0)
            print("size 0");
        
        _curObjects.Clear();
        _curObjectsRenderers.Clear();

        for (int i = 0; i < size; i++)
        {   
            // object is in sphere and closer than player so add it to objects.
            //if (ObjectCloserThanPlayer(hits[i].transform))
            //{   
                _curObjects.Add(hits[i].gameObject);
                _curObjectsRenderers.Add(_curObjects[i].GetComponent<SpriteRenderer>());
            //}
        }
        
      

    }

    private void SetUpPrevObjects()
    {
        if (_curObjects.Count == _prevObjects.Count)
        {
            _prevObjects.Clear();
            return;
        }

        Debug.Log("prev size before: " + _prevObjects.Count);

        for (int i = 0; i < _prevObjects.Count; ++i)
        {
            if (_curObjects.Contains(_prevObjects[i]))
            {
                _prevObjects.Remove(_prevObjects[i]);
                _prevSpriteRenderers.Remove(_prevSpriteRenderers[i]);
            }
        }
        Debug.Log("prev size after: " + _prevObjects.Count);

    }

    private void SetObjectsSprite()
    {   
        
        
        for (int i = 0; i < _curObjectsRenderers.Count; i++)
        {
            StartCoroutine(FadeToTransparent(_curObjectsRenderers[i]));
        }

        for (int i = 0; i < _prevSpriteRenderers.Count; i++)
        {
            StartCoroutine(FadeToOpaque(_prevSpriteRenderers[i]));
        }

        _prevObjects = _curObjects;
        _prevSpriteRenderers = _curObjectsRenderers;
        
      
    }
    

    private bool ObjectCloserThanPlayer(Transform curObject)
    {
        Vector2 cameraPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 playerPos = new Vector2(player.position.x, player.position.z);
        Vector2 objPos = new Vector2(curObject.position.x, curObject.position.z);
        
        // Get the distance from the camera to the object
        float objectDistance = Vector2.Distance(cameraPos, objPos);
        // Get the distance from the camera to the player
        float playerDistance = Vector2.Distance(cameraPos, playerPos);

        // Compare the distances and return the result
        return objectDistance <= playerDistance;
    }


    private void FixedUpdate()
    {
        UpdateObjectsInRadius();
        SetUpPrevObjects();
        SetObjectsSprite();
       
    }


    IEnumerator FadeToTransparent(SpriteRenderer spriteRenderer)
    {
        // Fade the object to transparent over the specified duration
        for (float t = minAlpha; t < 1.0f; t += Time.deltaTime / fadeDuration)
        {
            spriteRenderer.color = new Color(1,1,1,1-t);
            yield return null;
        }

        spriteRenderer.color = _fadedColor;

    }

    IEnumerator FadeToOpaque(SpriteRenderer spriteRenderer)
    {
       
        // Fade the object to transparent over the specified duration
        for (float t = minAlpha; t < 1.0f; t += Time.deltaTime / fadeDuration)
        {
            spriteRenderer.color = new Color(1,1,1,t);
            yield return null;
        }

        spriteRenderer.color = _unFadedColor;

    }
    
    private void OnDrawGizmosSelected()
    {
        Color color = new Color(0.0f, 1f, 1f, 0.2f);
        Gizmos.color = color;
        Gizmos.DrawSphere(player.position, radius);
    }
    
    

}
