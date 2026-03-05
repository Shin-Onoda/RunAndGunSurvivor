using System.Collections;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    const int maxRemaining = 10; // 充填数の上限

    [Header("弾数・保有マガジン数")]
    public int bulletRemaining = maxRemaining; // 残弾数
    public int magazine = 1; // マガジン数 ※充填時に消費

    [Header("充填時間")]
    public float recoveryTime = 3.0f; // マガジン補充時間
    float counter; // 充填までの残時間

    Coroutine bulletRecover; // 発生中のコルーチン情報の参照用

    //弾の消費
    public void ConsumeBullet()
    {
        if (bulletRemaining > 0)        // 残弾 > 0 ならば
        {
            bulletRemaining--;          // 弾を1消費
        }
    }

    // 残数の取得
    public int GetBulletRemaining()
    {
        return bulletRemaining;
    }

    // 弾の充填
    public void AddBullet(int num)
    {
        bulletRemaining = num;
    }

    // 充填メソッド
    public void RecoverBullet()
    {
        if (bulletRecover == null)
        {
            if(magazine > 0)
            {
                magazine--;                             // マガジンを1消費
                
                bulletRecover = StartCoroutine(RecoverBulletCol());     // コルーチンの発動とコルーチン情報を変数に格納
            }
        }
    }

    // 充填コルーチン
    IEnumerator RecoverBulletCol()
    {
        //counterセット
        counter = recoveryTime;

        while (counter > 0)
        {
            yield return new WaitForSeconds(1.0f); // ウェイト処理
            counter--;
        }
        AddBullet(maxRemaining);
        bulletRecover = null;
    }

    // 画面上に簡易GUI表示
    void OnGUI()
    {
        // 残弾数表示UI
        GUI.color = Color.black;
        string label = "bullet:" + bulletRemaining;
        GUI.Label(new Rect(50, 50, 100, 30), label);

        // 残マガジン表示UI
        label = "magazine:" + magazine;
        GUI.Label(new Rect(50, 75, 100, 30), label);

        // 充填開始～充填完了まで
        // 赤い文字で点滅表示
        if(bulletRecover != null)
        {
            float val = Mathf.Sin(Time.time * 20);
            if (val > 0)
            {
                label = "bulletRecover" + counter;
            }
            else
            {
                label = "";
            }
            GUI.color = Color.red;
            GUI.Label(new Rect(50, 25, 100, 30), label);
        }
    }
}
