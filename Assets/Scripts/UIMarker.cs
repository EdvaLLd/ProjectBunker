using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMarker : MonoBehaviour
{
    GameObject toFollow;
    float margin;
    float lifeTime = 10;
    bool shouldFadeAfterTime = true;

    public void Init(GameObject toFollow, float margin, bool shouldFadeAfterTime, float lifeTime = 1)
    {
        this.toFollow = toFollow;
        this.margin = margin;
        this.lifeTime = lifeTime;
        this.shouldFadeAfterTime = shouldFadeAfterTime;
    }

    public void Init(UIMarker marker)
    {
        this.toFollow = marker.toFollow;
        this.margin = marker.margin;
        this.lifeTime = marker.lifeTime;
        this.shouldFadeAfterTime = marker.shouldFadeAfterTime;
    }

    public bool IsConsistent()
    {
        return shouldFadeAfterTime;
    }

    public void SetDuration(float duration)
    {
        lifeTime = duration;
    }
    public void SetShouldFade(bool value)
    {
        shouldFadeAfterTime = value;
    }

    void SetPosition()
    {
        Vector3 pos = toFollow.transform.position;
        pos.y += margin;
        transform.position = Camera.main.WorldToScreenPoint(pos);
    }
    private void Start()
    {
        SetPosition();
    }

    private void Update()
    {
        SetPosition();
        if (shouldFadeAfterTime)
        {
            lifeTime -= Time.deltaTime;
            if(lifeTime < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
