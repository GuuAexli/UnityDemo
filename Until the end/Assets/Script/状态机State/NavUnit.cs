using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavUnit : MonoBehaviour
{
    public NavMeshAgent agent;          // 角色身上的 NavMeshAgent
    public float moveSpeed = 3.5f;      // 移动速度（与 Agent 的 Speed 一致）

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.updateRotation = false;    // 我们自己控制旋转
        // 可根据需要调整 agent.radius 和 agent.height
    }

    private void Update()
    {
        // 鼠标左键点击设置目标点
        if (Input.GetMouseButtonDown(0))
        {
            // 将鼠标点击位置转换为世界坐标（2D 坐标）
            Vector2 target2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // 转换为 3D 坐标：x 不变，y 固定为 0，z 对应 2D 的 y
            Vector3 target3D = new Vector3(target2D.x, 0, target2D.y);
            agent.SetDestination(target3D);
        }

        // 只有当 Agent 有路径时才更新位置和旋转
        if (agent.hasPath)
        {
            // 将 Agent 的 3D 位置映射到 2D 位置：agent.x → 2D.x, agent.z → 2D.y
            Vector3 agentPos = agent.transform.position;
            Vector2 newPosition2D = new Vector2(agentPos.x, agentPos.z);

            // 更新角色的 Transform（保持 z = 0）
            transform.position = new Vector3(newPosition2D.x, newPosition2D.y, 0);

            // 将 Agent 的绕 Y 轴旋转映射为绕 Z 轴旋转
            float angle = agent.transform.eulerAngles.y;  // 绕 Y 轴的角度
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
