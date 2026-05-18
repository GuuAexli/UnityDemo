using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class red_Combat : MonoBehaviour
{
    [Header("属性")]
    public red_soldier redSoldier;
    [SerializeField] private Transform fierPos;
    [SerializeField] private GameObject bullet;
    public GameObject weaponInstantiate;
    public GameObject currenterWeapon;
    private Weapon weaponData;
    public float attackRange;
    public float fierRate = 3f;

    public List<GameObject> enemyList = new List<GameObject>();
    [SerializeField] private LayerMask enemyLayer;
    // Start is called before the first frame update
    void Start()
    {
        redSoldier= transform.GetComponent<red_soldier>();
        weaponInstantiate = redSoldier.data.weapon[Random.Range(0, redSoldier.data.weapon.Length)].prefab;
        //现在武器预制体  = 单位数据内存储的武器数据 随机一份  选择随机到的武器数据内存储的武器预制体
        currenterWeapon = Instantiate(weaponInstantiate, transform);
        GetWeapon(currenterWeapon);
    }

    // Update is called once per frame
    void Update()
    {
        getEnemy();
        Attack();
    }
    void getEnemy()
    {
        enemyList.Clear();
        Collider2D[] hitCollider = Physics2D.OverlapCircleAll(
                                            transform.position, 
                                            attackRange, 
                                            enemyLayer);
        foreach(Collider2D cod in hitCollider) {
            if (cod.CompareTag("blue_tag"))
                enemyList.Add(cod.gameObject);
        }
    }
    public GameObject FindClosestObject()
    {
        GameObject closestObject = null;
        float minEnemyDistance = Mathf.Infinity;
        foreach(GameObject Obj in enemyList) {
            Vector3 distance = transform.position - Obj.transform.position;
            float sipDistance = distance.sqrMagnitude;

            if (sipDistance < minEnemyDistance) { 
                closestObject = Obj;
                redSoldier.attackRotat(Obj);
                minEnemyDistance = sipDistance;
            }
        }
        return closestObject;
    }
    void Attack()
    {
        if(FindClosestObject()!=null) { 
            redSoldier.isAttack = true;
        }
        else { 
            redSoldier.isAttack=false;
        }
    }
    public void GetWeapon(GameObject newWeapon)
    {
        if (weaponData.weaponName != newWeapon.GetComponent<Weapon>().weaponName || weaponData == null)
        {
            UpdateWeapon(newWeapon);
        }
    }
    public void UpdateWeapon(GameObject newWeapon)
    {
        Destroy(currenterWeapon);
        weaponInstantiate = newWeapon;
        currenterWeapon=Instantiate(weaponInstantiate);
        currenterWeapon.transform.SetParent(transform);
        currenterWeapon.GetComponent<SpriteRenderer>().enabled = false;
        weaponData = null;
        weaponData = currenterWeapon.GetComponent<Weapon>();

        weaponData._fierPos = fierPos;
        attackRange = weaponData.attackRange;

    }
}
