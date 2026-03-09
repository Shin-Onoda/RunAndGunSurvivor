using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRun : MonoBehaviour
{
    // レーン制御
    const int MinLane = -2;
    const int MaxLane = 2;
    const float LaneWidth = 1.0f;
    int targetLane;

    // プレイヤーライフ
    const int DefaultLife = 3;
    int life = DefaultLife;

    // 被ダメージ時の硬直時間
    const float StunDuration = 0.5f;
    float recoverTime = 0.0f;

    CharacterController controller;
    Animator animator;

    // 移動
    Vector3 moveDirection = Vector3.zero;
    float currentMoveInputX;    // InputSystemの入力値
    Coroutine resetIntervalCol; // 入力インターバル用コルーチン

    public float gravity = 20.0f;
    public float speedZ = 5.0f;
    public float speedX = 3.0f;
    public float speedJump = 8.0f;
    public float accelerationZ = 10.0f;

    [Header("ソードのスクリプト")]
    public NormalSword normalSword;

    void OnMove(InputValue value)
    {
        // ソードAction中は検知しない
        if (normalSword.GetIsSword()) return;
        // 入力インターバル中なら検知しない（コルーチン）
        if (resetIntervalCol == null)
        {
            Vector2 inputVector = value.Get<Vector2>();     // 検知した値をinputVectorに格納
            currentMoveInputX = inputVector.x;              // レーン移動用にx値だけを格納する
        }
    }

    void OnJump(InputValue value)
    {
        // ソードAction中は検知しない
        if (normalSword.GetIsSword()) return;
        Jump();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManager.gameState == GameState.stageclear || GameManager.gameState == GameState.result) return;
        {
            
        }
        //InputManagerシステムの場合
        //if (Input.GetKeyDown("left")) MoveToLeft();
        //if (Input.GetKeyDown("right")) MoveToRight();
        //if (Input.GetKeyDown("space")) Jump();

        if (currentMoveInputX < 0) MoveToLeft();
        if (currentMoveInputX > 0) MoveToRight();

        if (IsStun())
        {
            // スタン中はY軸以外の移動値を0にする
            moveDirection.x = 0;
            moveDirection.z = 0;
            // recoverTimeをカウントダウン
            recoverTime -= Time.deltaTime;
        }
        else
        {
            float acceleratedZ = moveDirection.z + (accelerationZ * Time.deltaTime);
            moveDirection.z = Mathf.Clamp(acceleratedZ, 0, speedZ);

            float ratioX = (targetLane * LaneWidth - transform.position.x) / LaneWidth;
            moveDirection.x = ratioX * speedX;
        }

        moveDirection.y -= gravity * Time.deltaTime;

        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        controller.Move(globalDirection * Time.deltaTime);

        if (controller.isGrounded) moveDirection.y = 0;
    }

    public int Life()
    {
        return life;
    }

    public void LifeUp()
    {
        life++;
        if(life > DefaultLife) life = DefaultLife;      // バリデーション
        GameObject canvas = GameObject.FindGameObjectWithTag("UI");
        canvas.GetComponent<UIController>().UpdateLife(Life());
    }

    public void LifeDown()
    {
        life--;
        GameObject canvas = GameObject.FindGameObjectWithTag("UI");
        canvas.GetComponent<UIController>().UpdateLife(Life());
    }

    bool IsStun()
    {
        return recoverTime > 0 || life <= 0;
    }

    public void MoveToLeft()
    {
        // 硬直中なら何もしない
        if (IsStun()) return;
        // 地面にいるかつ、targetが最小でない場合
        if(controller.isGrounded && targetLane > MinLane)
        {
            targetLane--;
            currentMoveInputX = 0;  // 入力値を戻す
            resetIntervalCol = StartCoroutine(ResetIntervalCol()); // 次の入力検知までのインターバルを作る
        }
    }

    public void MoveToRight()
    {
        // 硬直中なら何もしない
        if (IsStun()) return;
        // 地面にいるかつ、targetが最大でない場合
        if (controller.isGrounded && targetLane < MaxLane)
        {
            targetLane++;
            currentMoveInputX = 0;  // 入力値を戻す
            resetIntervalCol = StartCoroutine(ResetIntervalCol()); // 次の入力検知までのインターバルを作る
        }
    }

    public void Jump()
    {
        if(IsStun()) return;
        if (controller.isGrounded)
        {
            moveDirection.y = speedJump;
        }
    }

    IEnumerator ResetIntervalCol()
    {
        yield return new WaitForSeconds(0.1f);
        resetIntervalCol = null;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (IsStun()) return;
        if(hit.gameObject.tag == "Enemy")
        {
            LifeDown();
            GetComponent<NormalShooter>().ShootPowerDown();
            recoverTime = StunDuration;

            if (life <= 0) GameManager.gameState = GameState.gameover;

            // Destroy(hit.gameObject);
            hit.gameObject.GetComponent<Wall>().CreateEffect();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Goal")
        {
            GameManager.gameState = GameState.gameclear;
        }
    }
}
