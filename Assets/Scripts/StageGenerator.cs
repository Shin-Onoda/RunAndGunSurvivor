using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    const int StageChipSize = 30;   // Zのスケールが30であるステージ

    int currentChipIndex;   // 現在作成済みのステージ番号

    public Transform character; // プレイヤーの位置
    public GameObject[] stageChips; // 生成されるステージのカタログ
    public int startChipIndex;  // 最初のステージ番号
    public int preInstantiate;  // どこまで先のステージを用意しておくか
    public List<GameObject> generatedStageList = new List<GameObject>();    // 今現在ヒエラルキーに存在しているステージ情報を生成順に取得

    void Start()
    {
        // 初期の現在番号を定めている
        currentChipIndex = startChipIndex - 1;
        // 初期の状態からまずはいくつかステージ生成
        UpdateStage(preInstantiate);
    }

    void Update()
    {
        // キャラクターの位置から現在のステージチップのインデックスを計算
        int charaPositionIndex = (int)(character.position.z / StageChipSize);

        // 次のステージチップに入ったらステージの更新処理を行う
        if (charaPositionIndex + preInstantiate > currentChipIndex)
        {
            UpdateStage(charaPositionIndex + preInstantiate);
        }
    }

    // 指定のIndexまでのステージチップを生成して、管理下に置く
    void UpdateStage(int toChipIndex)
    {
        if (toChipIndex <= currentChipIndex) return;

        // 指定のステージチップまでを作成
        for(int i = currentChipIndex + 1; i <= toChipIndex; i++)
        {
            GameObject stageObject = GenerateStage(i);

            // 生成したステージチップを管理リストに追加
            generatedStageList.Add(stageObject);
        }

        // ステージ保持上限内になるまで古いステージを削除
        while (generatedStageList.Count > preInstantiate + 2) DestroyOldestStage();

        currentChipIndex = toChipIndex;
    }

    // 指定のインデックス位置にStageオブジェクトをランダムに生成
    GameObject GenerateStage(int chipIndex)
    {
        int nextStageChip = Random.Range(0, stageChips.Length);

        GameObject stageObject = (GameObject)Instantiate(
            stageChips[nextStageChip],
            new Vector3(0, 0, chipIndex * StageChipSize),
            Quaternion.identity
        );

        return stageObject;
    }

    // 一番古いステージを削除
    void DestroyOldestStage()
    {
        GameObject oldStage = generatedStageList[0];
        generatedStageList.RemoveAt(0);
        Destroy(oldStage);
    }
}
