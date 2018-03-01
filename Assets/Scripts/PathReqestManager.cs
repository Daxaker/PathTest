using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathReqestManager : MonoBehaviour {
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    private PathRequest curPathRequest;
    private static PathReqestManager instance;

    private PathFinding pathfinding;
    private bool isProcessingPath;
    void Awake() {
        instance = this;
        pathfinding = GetComponent<PathFinding>();
    }
    public static void RequestPath(Vector3 start, Vector3 target, Action<Vector3[], bool> callback) {
        PathRequest newRequest = new PathRequest(start, target, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    private void TryProcessNext() {
        if (!isProcessingPath && pathRequestQueue.Count > 0) {
            curPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartPathFind(curPathRequest.start, curPathRequest.target);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success) {
        curPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    public struct PathRequest {
        public Vector3 start;
        public Vector3 target;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 startt, Vector3 target, Action<Vector3[], bool> callback) {
            this.start = startt;
            this.target = target;
            this.callback = callback;
        }
    }
}
