using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

	public Transform target;

	public float speed = 1;

	private Vector3[] path;

	private int targetIndex;
	// Use this for initialization
	void Start () {
		PathReqestManager.RequestPath(transform.position, target.position, OnPathRecievd);
	}

	private void OnPathRecievd(Vector3[] path, bool success)
	{
		if (success)
		{
			this.path = path;
			StopCoroutine(FollowPath());
			StartCoroutine(FollowPath());
		}
	}

	IEnumerator FollowPath()
	{
		Vector3 currentWayPoint = path[0];
		while (true)
		{
			currentWayPoint.y = transform.position.y;
			if (transform.position == currentWayPoint)
			{
				targetIndex++;
				if (targetIndex >= path.Length)
				{
					yield break;
				}

				currentWayPoint = path[targetIndex];
			}
				transform.position = Vector3.MoveTowards(transform.position, 
					new Vector3(currentWayPoint.x, transform.position.y, currentWayPoint.z), speed * Time.deltaTime);
				yield return null; 
		}
	}

	private void OnDrawGizmos()
	{
		if (path != null)
		{
			for (var i = targetIndex; i < path.Length; i++)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], Vector3.one * 0.1f);
				if (i == targetIndex)
				{
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else
				{
					Gizmos.DrawLine(path[i - 1], path[i]);
				}
			}
		}
	}
}
