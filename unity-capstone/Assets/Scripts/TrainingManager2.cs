using Unity.AI.Navigation;
using Unity.MLAgents;
using UnityEngine;

[DefaultExecutionOrder(-5)]
public class TrainingManager2 : MonoBehaviour
{
    public GameObject[] TrainingArea;
    public int numAreas = 256;
    public float separation = 500;
    
    private NavMeshSurface area;
    
    private void Awake()
    {
        InitializeEnvironments();
    }
    
    public void InitializeEnvironments()
    {
        if (Academy.Instance.NumAreas > 0)
            numAreas = Academy.Instance.NumAreas;

        float rootNum = Mathf.Sqrt(numAreas);
        int maxNum = Mathf.CeilToInt(rootNum);
        
        for (int x = 0; x < maxNum; x++)
        {
            for (int z = 0; z < 4; z++)
            {
                int trackNum = Random.Range(0, 4);
                var env = Instantiate(TrainingArea[trackNum], new Vector3(x * separation, transform.position.y, z * separation),
                    Quaternion.identity);
                env.name = TrainingArea[trackNum].name;
                area = env.GetComponent<NavMeshSurface>();
                area.RemoveData();
                area.BuildNavMesh();
            }
            
            for (int z = 4; z < maxNum; z++)
            {
                int trackNum = Random.Range(4, TrainingArea.Length);
                var env = Instantiate(TrainingArea[trackNum], new Vector3(x * separation, transform.position.y, z * separation),
                    Quaternion.identity);
                env.name = TrainingArea[trackNum].name;
                area = env.GetComponent<NavMeshSurface>();
                area.RemoveData();
                area.BuildNavMesh();
            }
        }
    }
}
