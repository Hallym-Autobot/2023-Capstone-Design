# 2023-Capstone-Design
* 과제: 유니티를 이용한 자율주행 시뮬레이터 개발
* 팀명: 오토봇
* 팀원: 서무혁, 이제일, 장재현, 최준호
## 개발 환경
* Window
* Python 3.9
* Pytorch 2.0 (CUDA 11.8)
* Unity ML-Agents Toolkit Release 20 
## 디렉터리 구조
```text
├─ config           # 알고리즘의 하이퍼파라미터
├─ envs             # 빌드한 유니티 환경
├─ result           # 학습 로그(Tensorflow), 결과 모델
├─ unity_capstone   # 유니티 프로젝트
└─ environment.yaml # anaconda 가상환경 설정 
```
## 실행 방법
* 터미널에서 anaconda를 이용해 python 가상환경을 설치합니다. 
  * ```conda create -f environment.yaml```
  * ```conda activate mlagents```
* 다운받은 프로젝트의 경로에서 다음을 실행합니다.
  * ```mlagents-learn [학습할 Agent의 config 파일] --env [env 경로]```
* 학습 과정 및 결과는 Tensorboard에서 확인이 가능합니다.
  * ```tensorboard --logdir ./result ```

## 시연 영상
![PoliceAgent_1-min](https://github.com/Hallym-Autobot/2023-Capstone-Design/assets/81278212/9645845f-4fc3-4188-ba98-ce9e94772adc)
