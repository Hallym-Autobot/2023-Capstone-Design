using UnityEngine;

public class CarController : MonoBehaviour
{
    public float throttle = 0f;
    public float steer = 0f;
    public float brake = 0f;
    public float downForce = 1f;
    public float maxAccel = 500f;
    public float maxRot = 45f;
    public float maxBrake = 500f;
    public float maxRPM = 2000f;
    
    public Transform[] wheels;
    public WheelCollider[] wheelColliders;
    
    private Rigidbody _rBody;
    
    private void Start()
    {
        _rBody = GetComponent<Rigidbody>();
        _rBody.centerOfMass += Vector3.down * 0.3f; // 차량 무게중심 변경
    }

    void FixedUpdate()
    {
        Move();
        WheelRotate();
    }
    
    // 차량 움직임 구현 (전륜 차량)
    void Move()
    {
        for (int i = 0; i < 2; i++)
        {
            wheelColliders[i].motorTorque = throttle * maxAccel;
            wheelColliders[i].steerAngle = steer * maxRot;
        }
        
        for (int i = 0; i < 4; i++)
            wheelColliders[i].brakeTorque = brake * maxBrake;

        _rBody.AddForce(-transform.up * downForce * _rBody.velocity.magnitude * 100f); // 속도가 빠를수록 다운포스 증가
        
        // RPM을 제한해 최고 속도 제한
        float cRPM = (wheelColliders[0].rpm + wheelColliders[1].rpm) / 2;
        if (cRPM > maxRPM || cRPM < -maxRPM)
            for (int i = 0; i < 2; i++)
                wheelColliders[i].motorTorque *= -maxRPM / wheelColliders[i].rpm;
    }
    
    // collider의 움직임을 object에 반영
    void WheelRotate()
    {
        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].GetWorldPose(out var pos, out var quat);
            wheels[i].transform.position = pos;
            wheels[i].transform.rotation = quat;
        }
    }
    
    public void Reset()
    {
        foreach (var wheel in wheelColliders)
        {
            wheel.motorTorque = 0f;
            wheel.steerAngle = 0f;
            wheel.brakeTorque = 0f;
        }
    }
}