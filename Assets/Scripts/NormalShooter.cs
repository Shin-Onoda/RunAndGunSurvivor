using NUnit.Framework;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;

public class NormalShooter : MonoBehaviour
{
    [Header("Bullet管理スクリプトと連携")]
    public BulletManager bulletManager;

    [Header("生成オブジェクトと位置")]
    public GameObject bulletPrefabs;//生成対象プレハブ
    public GameObject gate; //生成位置

    [Header("弾速")]
    public float shootSpeed = 10.0f; //弾速

    GameObject bullets; //生成した弾をまとめるオブジェクト

    const int maxShootPower = 3;
    int shootPower = 1;

    [Header("ソードのスクリプト")]
    public NormalSword normalSword;
    
    //InputAction(Playerマップ)のAttackアクションがおされたら
    void OnAttack(InputValue value)
    {
        if (normalSword.GetIsSword()) return;

        if(GameManager.gameState == GameState.retry)
        {
            GameManager.RetryScene();
        }
        else if(GameManager.gameState == GameState.result)
        {
            GameManager gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
            gm.NextScene(gm.nextScene);
        }
        else
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletManager.GetBulletRemaining() > 0)
        {
            GameObject obj = Instantiate(bulletPrefabs, gate.transform.position, Quaternion.Euler(90, 0, 0));

            obj.transform.parent = bullets.transform;

            // 弾を消費する
            bulletManager.ConsumeBullet();

            Rigidbody bulletRbody = obj.GetComponent<Rigidbody>();
            bulletRbody.AddForce(new Vector3 (0, 0, shootSpeed), ForceMode.Impulse);
        }
        else
        {
            bulletManager.RecoverBullet();
        }
    }
       
    void Start()
    {
        bullets = GameObject.FindWithTag("Bullets");
    }

    public void ShootPowerUp()
    {
        shootPower++;
        if (shootPower > maxShootPower) shootPower = maxShootPower;
        GameObject canvas = GameObject.FindGameObjectWithTag("UI");
        canvas.GetComponent<UIController>().UpdateGun();
    }

    public void ShootPowerDown()
    {
        shootPower--;
        if (shootPower <= 0) shootPower = 1;
        GameObject canvas = GameObject.FindGameObjectWithTag("UI");
        canvas.GetComponent<UIController>().UpdateGun();
    }

    public int GetShootPower()
    {
        return shootPower;
    }
}
