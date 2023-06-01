using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target;

    /* 카메라의 고정값 설정 */
    private float _x,_y,_z;
    int viewMode;

    private KeyCode[] keyCodes = {
         KeyCode.Keypad1,
         KeyCode.Keypad2,
         KeyCode.Keypad3,
         KeyCode.Keypad4,
         KeyCode.Keypad5,
         KeyCode.Keypad6,
         KeyCode.Keypad7,
         KeyCode.Keypad8,
         KeyCode.Keypad9,
     };
    void Start()
    {
        setValue(2);
    }

    // 모든 업데이트 이후에 호출
    void LateUpdate()
    {

        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                viewMode = i+1;
                setValue(viewMode);
            }
        }

        setCamara();
    }

    void setValue(int num)
    {
        viewMode = num;
        switch (num)
        {
            case 1:
            case 3:
            case 7:
            case 9:
                _z = 1.2f;
                _x = 0.6f;
                _y = 0.3f;
                break;
            case 2:
                _z = 10f;
                _y = 5f;
                break;
            case 4:
            case 6:
                _x = 10f;
                _y = 5f;
                break;
            case 5:
                _z = 4f;
                _y = 20f;
                break;
            case 8:
                _z = 0.55f;
                _y = 0.5f;
                break;
        }
    }
    private void setCamara()
    {
        switch (viewMode)
        {
            case 1:
                viewMode1(false, true);
                break;
            case 3:
                viewMode1(false, false);
                break;
            case 2:
            case 5:
                transform.position = target.position - smoothAngle() * Vector3.forward * _z + Vector3.up * _y;
                transform.LookAt(target);
                break;
            case 4:
                viewMode4(true);
                break;
            case 6:
                viewMode4(false);
                break;
            case 7:
                viewMode1(true, true);
                break;
            case 8:
                viewMode8();
                break;
            case 9:
                viewMode1(true, false);
                break;
        }

    }

    private void viewMode1(bool isForward, bool isLeft)
    {
        transform.position = target.transform.position  - target.transform.up *_y
        + (isForward ? target.transform.forward * _z : -target.transform.forward *_z)
        + (isLeft ? - target.transform.right *_x : target.transform.right *_x);
        
        transform.LookAt(target);
        transform.position += (isLeft ? - target.transform.right *_x : target.transform.right *_x);
    }
    private void viewMode4(bool isLeft)
    {
        transform.position = target.transform.position + target.transform.up *_y 
        + (isLeft ? - target.transform.right * _x : target.transform.right * _x );
        transform.LookAt(target);
    }
    private void viewMode8()
    {
        transform.position = target.transform.position + target.transform.forward * _z + target.transform.up *_y;
        transform.eulerAngles = target.eulerAngles + Vector3.right*15;
    }

    private Quaternion smoothAngle()
    {
        // 현재 카메라의 y축 앵글과 높이
        float currAngleY = transform.eulerAngles.y;
        float currHeight = transform.position.y;

        // 타겟의 앵글과 높이
        float targetAngleY = target.eulerAngles.y;
        float targetHeight = target.position.y + _y;

        // 원하는 앵글을 시간변위에 따라 현재 앵글값 얻음
        currAngleY = Mathf.LerpAngle(currAngleY, targetAngleY, 2 * Time.deltaTime);

        // 높이도 마찬가지
        currHeight = Mathf.Lerp(currHeight, targetHeight, 1.5f * Time.deltaTime);

        // 이동할 앵글을 회전으로 변환
        return Quaternion.Euler(0, currAngleY, 0);
    }
}
