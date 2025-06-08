using UnityEngine;

public class TimedObjectDestruction : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 1f;

    private float _elapsedTime = 0f;

    void Update()
    {
        if (_elapsedTime >= _lifeTime)
            Destroy(this.gameObject);
        else
            _elapsedTime += Time.deltaTime;
    }

    public void SetLifeTime(float lifeTime)
    {
        _lifeTime = lifeTime;
    }
}

