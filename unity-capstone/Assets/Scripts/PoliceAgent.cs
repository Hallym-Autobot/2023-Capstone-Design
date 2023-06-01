using UnityEngine;
using UnityEngine.AI;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PoliceAgent : Agent
{
    public Transform[] targets;
    public Transform[] spawns;
    
    private Rigidbody rBody;
    private CarController ctrl;
    private NavMeshPath path;
    private Transform target;
    private EnvironmentParameters resetParams;
    
    private Vector3 tPos;
    private Vector3 direction;
    private float pathDist;
    private float preDist;
    private int idx;

    public override void Initialize()
    {
        path = new NavMeshPath();
        ctrl = GetComponentInChildren<CarController>();
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // 출발지, 목적지 설정
        int sIdx = 0;
        int tIdx = 0;
        var dst = Random.Range(0, 2);
        
        // 짝수가 나오면 상행, 홀수가 나오면 하행선으로 학습
        if (dst < 1)
        {
            sIdx = Random.Range(0, spawns.Length / 2) * 2;
            tIdx = Random.Range(0, targets.Length / 2) * 2;
        }
        else
        {
            sIdx = Random.Range(0, spawns.Length / 2) * 2 + 1;
            tIdx = Random.Range(0, targets.Length / 2) * 2 + 1;
        }
        
        transform.position = spawns[sIdx].position;
        transform.rotation = spawns[sIdx].rotation;
        target = targets[tIdx];
        
        // agent 초기화
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
        ctrl.Reset();
        
        // 경로 설정
        MakeNav();
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 carInfo = new Vector3(ctrl.wheelColliders[0].rpm, ctrl.wheelColliders[1].rpm, rBody.velocity.magnitude); // 차량의 RPM 정보
        direction = (path.corners[idx] - transform.position).normalized; // 현재 위치에서 체크포인트를 바라보는 방향
        direction.y = 0f;
        Quaternion heading = Quaternion.LookRotation(direction);
        
        sensor.AddObservation(rBody.velocity);
        sensor.AddObservation(rBody.angularVelocity);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(heading);
        sensor.AddObservation(carInfo);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var continuousAction = actions.ContinuousActions;
        ctrl.throttle = continuousAction[0];
        ctrl.steer = continuousAction[1];
        ctrl.brake = Mathf.Clamp(continuousAction[2], 0f, 1f);
        CalcReward(); // 리워드 계산
        
#if UNITY_EDITOR
        DrawPath(); // 유니티 에디터에서 학습하는 경우 화면에 경로를 표시 
#endif
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionOut = actionsOut.ContinuousActions;
        continuousActionOut[0] = Input.GetAxis("Vertical");
        continuousActionOut[1] = Input.GetAxis("Horizontal");
        continuousActionOut[2] = Input.GetAxis("Jump");
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("wall"))
        {
            AddReward(-10f);
            EndEpisode();   
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        
        if (other.gameObject.CompareTag("car") || other.gameObject.CompareTag("agent"))
        {
            AddReward(-10f);
            EndEpisode();   
        }
    }
    
    private void MakeNav()
    {
        // 목적지까지의 경로 계산
        int layerMask = 1 << LayerMask.NameToLayer("Default");
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 5f, layerMask);
        bool isPath = NavMesh.CalculatePath(hit.point, target.position, NavMesh.AllAreas, path); 
        idx = 0;
        
        // 경로가 제대로 설정되지 않았다면 에피소드 중단
        if (!isPath)
            EpisodeInterrupted();
        else
        {
            // 경로의 총 길이 계산: (현재 위치에서 다음 체크포인트까지의 거리) + (나머지 경로 길이)
            tPos = path.corners[idx];
            
            if (path.corners.Length != 1)
                for (int i = 1; i < path.corners.Length - 1; i++)
                    pathDist += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            
            preDist = pathDist + Vector3.Distance(transform.position, tPos);
        }
    }
    
    private void CalcReward()
    {
        // 총 이동 거리 계산 (이전 상태의 남은 거리에서 더 많이 줄어들수록 보상 증가)
        float mDist = Vector3.Distance(transform.position, tPos);
        float curDist = pathDist + mDist;
        float reward = Mathf.Clamp(preDist - curDist, -1f, 1f);
        
        if (mDist > 1.5f)
        {
            AddReward(reward);
            preDist = curDist;
        }
        else if (idx + 1 < path.corners.Length)
        {
            // 체크포인트에 도달한 경우
            AddReward(10f);
            idx += 1;
            tPos = path.corners[idx];
            
            pathDist = 0f;
            for (int i = idx; i < path.corners.Length - 1; i++)
                pathDist += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            preDist = pathDist + Vector3.Distance(transform.position, tPos);
        }
        else
        {
            // 목적지에 도착한 경우
            AddReward(100f);
            EndEpisode();
        }
    }
    
#if UNITY_EDITOR
    private void DrawPath()
    {
        for (int i = idx; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red, 0.1f, true);
        Debug.DrawLine(transform.position, path.corners[idx], color: Color.blue, 0.1f, true);
    }
#endif
}
