using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class red_soldier : MonoBehaviour
{
    
   
    public Collider2D _cd;
    public Rigidbody2D _rd;
    [Header("属性")]
    public UnitData data;
    public int currentLevel;//现在等级
    public float health;//生命
    [SerializeField] private float moveSpeed;//移动速度
    public float unitVolum;//体积
    public int unitArmor;//装甲
    public float unitAccurracy;//单位精准度
    [SerializeField]private float defultRotation=-90f;//默认角度
    public float rotatSpeed = 90f;//旋转速度


    public float targetAngle;//目标角度
    public bool isAttack= false;//正在攻击
    // Start is called before the first frame update
    void Start()
    {
        _rd = GetComponent<Rigidbody2D>();
        _cd = GetComponent<Collider2D>();
        ApplyLevelStats();
    }

    // Update is called once per frame
    void Update()
    {
        Health();
        Move();
    }
    
    public void Move()
    {
        if (!isAttack){
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            ////rotat
            Quaternion defultAngle = Quaternion.Euler(0, 0, defultRotation);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, defultAngle, rotatSpeed * Time.deltaTime);
        }
        else{   
            Quaternion enemyRotat = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, enemyRotat, rotatSpeed * Time.deltaTime);
        }
    }
    void Health()
    {
        if (health <= 0) {
            Destroy(gameObject);
        }
    }
    public void attackRotat(GameObject target)
    {
        targetAngle = Mathf.Atan2(target.transform.position.y - transform.position.y, 
                                    target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
    }
    public void ApplyLevelStats()
    {
        UnitLevelData.LevelStats levelStats = data.level.levels[currentLevel];
        //等级数据脚本 组 的形参 定义调用的形参  单位数据脚本内的形参 等级数据脚本内的形参 
        health=levelStats.health;
        //moveSpeed = levelStats.speed;
        unitVolum = levelStats.volum;
        unitAccurracy = levelStats.accurracy;
        unitArmor = levelStats.armor;
    }
}
