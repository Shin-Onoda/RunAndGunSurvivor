using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("生成プレハブオブジェクト")]
    public GameObject effectPrefab; // 生成プレハブ

    [Header("耐久力")]
    public float life = 5.0f; // 耐久力

    [Header("ダメージ時間・振動対象・振動スピード・振動量")]
    public float damageTime = 0.25f; // ダメージ中時間
    public GameObject damageBody; // 振動対象オブジェクト
    public float speed = 75.0f; // 振動スピード
    public float amplitude = 1.5f;  // 振動量

    Vector3 startPosition; // 振動対象の初期位置
    float x; // 振動による移動座標

    Coroutine currentDamage; //ダメージコルーチン

    [Header("スコア")]
    public int point = 100;

    void Start()
    {
        startPosition = damageBody.transform.localPosition;
    }

    void Update()
    {
        // ダメージコルーチン発動中処理
        if(currentDamage != null)
        {
            x = (amplitude * 0.01f) * Mathf.Sin(Time.time * speed);
            damageBody.transform.localPosition = startPosition + new Vector3(x, 0, 0);
        }
    }

    // 衝突
    void OnTriggerEnter(Collider other)
    {
        if (currentDamage != null) return;

        // 衝突相手がBulletタグを持っていた場合
        if (other.gameObject.tag == "Bullet" || other.gameObject.tag == "Sword")
        {
            string tag = other.gameObject.tag;

            // ダメージコルーチンを発動
            currentDamage = StartCoroutine(DamageCol(tag));
            if(life <= 0)   // lifeが0になったら消滅
            {
                ScoreManager.ScoreUp(point);
                CreateEffect();
            }
        }
    }

    // ダメージコルーチン
    IEnumerator DamageCol(string tag)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (tag == "Bullet")
        {
            life -= player.GetComponent<NormalShooter>().GetShootPower();     // 体力を減少
        }
        else if(tag == "Sword")
        {
            life -= player.GetComponent<NormalSword>().GetSwordPower();     // 体力を減少
        }
        yield return new WaitForSeconds(0.1f);
        // コルーチンを発動情報を削除
        currentDamage = null;
        yield return new WaitForSeconds(0.1f);
        // 振動していたボディを元の位置に戻す
        damageBody.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void CreateEffect()
    {
        if(effectPrefab != null)
        {
            // エフェクトプレハブを生成
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
        }

        // Wall自身は削除
        Destroy(gameObject);
    }
}
