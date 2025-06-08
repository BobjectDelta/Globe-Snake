using System.Collections.Generic;
using UnityEngine;

public class SnakeBodyController : MonoBehaviour
{
    [SerializeField] private GameObject _bodySegmentPrefab;
    [SerializeField] private float _segmentLength = 1f; 
    [SerializeField] private int _initialSegments = 2; 

    [SerializeField] private float _minRecordMoveDistance = 0.05f; 
    [SerializeField] private float _minRecordAngleDelta = 2.0f;

    private List<Vector3> pathHistory = new List<Vector3>();
    public List<Transform> segmentTransforms = new List<Transform>();

    private Transform _headTransform;
    private Vector3 _lastRecordPosition; 
    private Vector3 _lastRecordDirection; 

    private void Awake()
    {
        _headTransform = transform;
    }

    void Start()
    {
        pathHistory.Add(_headTransform.position);
        _lastRecordPosition = _headTransform.position;
        _lastRecordDirection = _headTransform.forward; 

        float minInitHistoryDistance = (_initialSegments + 1) * _segmentLength; 
        int initPointsCount = 0;
        int initPointsTries = 100;
        Vector3 initPos = _headTransform.position;
        Vector3 initDir = _headTransform.forward;

        while (minInitHistoryDistance > 0 && initPointsCount < initPointsTries)
        {
            initPos -= initDir * _segmentLength * 0.5f; 
            pathHistory.Add(initPos);
            minInitHistoryDistance -= _segmentLength * 0.5f;
            initPointsCount++;
        }

        for (int i = 0; i < _initialSegments; i++)       
            AddSegment();    
    }

    void FixedUpdate()
    {
        Vector3 curHeadPosition = _headTransform.position;
        Vector3 curHeadDirection = _headTransform.forward;

        float curDistance = Vector3.Distance(curHeadPosition, _lastRecordPosition);
        float curAngleDelta = Vector3.Angle(_lastRecordDirection, curHeadDirection);

        if (curDistance >= _minRecordMoveDistance || curAngleDelta >= _minRecordAngleDelta)
        {
            pathHistory.Insert(0, curHeadPosition);
            _lastRecordPosition = curHeadPosition; 
            _lastRecordDirection = curHeadDirection;
        }
        else if (pathHistory.Count == 0) 
        {
            pathHistory.Add(curHeadPosition);
            _lastRecordPosition = curHeadPosition;
            _lastRecordDirection = curHeadDirection;
        }

        float minHistoryDistance = (segmentTransforms.Count + 0.5f) * _segmentLength; 
        int minPointsBuffer = 5; 
        float curHistoryDistance = 0f;
        int pointsToKeep = 1; 

        for (int i = 0; i < pathHistory.Count - 1; i++)
        {
            float segmentDistance = Vector3.Distance(pathHistory[i], pathHistory[i + 1]);
            curHistoryDistance += segmentDistance;
            pointsToKeep++; 

            if (curHistoryDistance >= minHistoryDistance && pointsToKeep >= minPointsBuffer)
                break; 
        }

        if (pointsToKeep < minPointsBuffer) 
            pointsToKeep = minPointsBuffer;
        if (pathHistory.Count > 0 && pointsToKeep < 1) 
            pointsToKeep = 1; 

        if (pathHistory.Count > pointsToKeep)       
            pathHistory.RemoveRange(pointsToKeep, pathHistory.Count - pointsToKeep);
        

        for (int i = 0; i < segmentTransforms.Count; i++)
        {
            float targetDistance = (i + 1) * _segmentLength;
            Vector3 targetPosition = FindPointOnPath(targetDistance);

            if (targetPosition != Vector3.zero)
            {
                segmentTransforms[i].position = targetPosition;             
                Vector3 lookAheadPoint = FindPointOnPath(Mathf.Max(0.1f, targetDistance - 0.1f)); 

                if (lookAheadPoint != Vector3.zero && lookAheadPoint != segmentTransforms[i].position)
                {
                    Vector3 lookDirection = lookAheadPoint - segmentTransforms[i].position;
                    if (lookDirection != Vector3.zero && lookDirection.sqrMagnitude > 0.0001f)                     
                        segmentTransforms[i].rotation = Quaternion.LookRotation(lookDirection, Vector3.up);                   
                }
            }
            
        }
    }

    private Vector3 FindPointOnPath(float distance)
    {
        if (pathHistory.Count < 2) 
            return Vector3.zero;
        if (distance < 0) 
            distance = 0;

        float distanceCovered = 0f;
        for (int i = 0; i < pathHistory.Count - 1; i++)
        {
            Vector3 segmentStart = pathHistory[i]; 
            Vector3 segmentEnd = pathHistory[i + 1]; 
            float segmentDistance = Vector3.Distance(segmentStart, segmentEnd);

            if (distanceCovered + segmentDistance >= distance)
            {
                float distanceIntoSegment = distance - distanceCovered;
                float t = (segmentDistance > 0.0001f) ? (distanceIntoSegment / segmentDistance) : 0f; 
                return Vector3.Lerp(segmentStart, segmentEnd, t); 
            }
            distanceCovered += segmentDistance;
        }

        return Vector3.zero;
    }


    public void AddSegment()
    {        
        float spawnDistance = (segmentTransforms.Count + 1) * _segmentLength;
        Vector3 spawnPosition = FindPointOnPath(spawnDistance);

        if (spawnPosition == Vector3.zero)
        {
            if (segmentTransforms.Count > 0)
                spawnPosition = segmentTransforms[segmentTransforms.Count - 1].position;
            else
                spawnPosition = _headTransform.position;         
        }

        GameObject newSegment = Instantiate(_bodySegmentPrefab, spawnPosition, Quaternion.identity);
        segmentTransforms.Add(newSegment.transform);
    }
}

