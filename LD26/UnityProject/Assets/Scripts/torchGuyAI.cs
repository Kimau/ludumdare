using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class torchGuyAI : MonoBehaviour 
{
	public PatrolPath patrol;
	public int patrolNode = 0;
	
	NavMeshAgent navAgent;
	TorchMeshMaker torch;
	
	float lookRot = 0.0f;
	bool stopped = false;
	Vector3 lastPatrolPoint = Vector3.zero;
	bool investigating = false;
	
	void Awake()
	{
		lookRot = Random.value;
		torch = GetComponentInChildren<TorchMeshMaker>();
		navAgent = GetComponent<NavMeshAgent>();
		
		navAgent.updateRotation = true;
		navAgent.updatePosition = true;
	}
	
	void Start()
	{
		stopped = false;
		navAgent.updateRotation = true;
		navAgent.updatePosition = true;
	}
	
	void Update()
	{
		if(stopped)
			return;
		
		if(!navAgent.pathPending && navAgent.remainingDistance < 1.0f)
		{
			if(investigating)
			{
				navAgent.SetDestination(lastPatrolPoint);
				investigating = false;
			}
			else
			{
				navAgent.SetDestination(patrol[patrolNode]);				
				patrolNode = (patrolNode+1) % patrol.PathLength;
			}
		}
		
		if(torch)
			torch.transform.localRotation = Quaternion.AngleAxis(Mathf.Sin(lookRot+Time.realtimeSinceStartup)*10.0f, transform.up);
	}
	
	public void FoundPlayer(Vector3 playerPos)
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		
		player.SendMessage("Caught");
	}
	
	public void StopLevel()
	{
		stopped = true;
		navAgent.updateRotation = false;
		navAgent.updatePosition = false;
	}
	
	public void Noise(Vector3 position)
	{
		lastPatrolPoint = navAgent.destination;
		investigating = true;
		navAgent.SetDestination(position);
	}
}
