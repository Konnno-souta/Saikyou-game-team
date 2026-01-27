using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダブルQCF（↓, ??, →, ↓, ??, → + 攻撃）でFever発動
/// 入力はWASD限定（矢印/スティックは不使用）
/// </summary>
public class CommandInput : MonoBehaviour
{
    [Header("References")]
    public MonoBehaviour feverManager; // FeverSequence() を持つ任意のコンポーネント
    private System.Func<IEnumerator> feverSequenceInvoker;

    [Header("受付タイミング")]
    [Tooltip("コマンド全体の有効時間（秒）")]
    public float commandTimeout = 1.2f;

    [Tooltip("各ステップ間の最大遅延（秒）。超えるとリセット")]
    public float stepGapMax = 0.50f;

    [Tooltip("同じ方向を保持したまま誤進行しないための最小間隔（秒）")]
    public float minHoldAdvance = 0.035f;

    [Tooltip("最後の方向→ボタンのほぼ同時押しの猶予(秒)")]
    public float simultaneousGrace = 0.10f;

    [Header("発動ボタン（攻撃）")]
    public KeyCode attackKey = KeyCode.J;

    // 方向の8方位（ニュートラル含め9）
    public enum Dir { N, U, D, L, R, UL, UR, DL, DR }

    [Serializable]
    public class Command
    {
        public string name;
        public List<Dir> sequence = new List<Dir>();
        public KeyCode finishButton = KeyCode.J;
    }

    [Header("コマンド定義（ダブルQCF + 攻撃）")]
    public Command doubleQcf = new Command
    {
        name = "DoubleQCF",
        sequence = new List<Dir> { Dir.D, Dir.DR, Dir.R, Dir.D, Dir.DR, Dir.R },
        finishButton = KeyCode.J
    };

    // 内部状態
    private int stepIndex = 0;
    private float commandStartTime = -999f;
    private float lastStepTime = -999f;
    private Dir lastDir = Dir.N;
    private bool waitingForButton = false;

    void Awake()
    {
        // FeverSequence() を反射で引き出す（型を固定しない）
        if (feverManager != null)
        {
            var mi = feverManager.GetType().GetMethod("FeverSequence");
            if (mi != null)
            {
                feverSequenceInvoker = () => (IEnumerator)mi.Invoke(feverManager, null);
            }
        }
        // attackKey と finishButton を同期
        doubleQcf.finishButton = attackKey;
    }

    void Update()
    {
        var dir = ReadDirectionWASD();

        // 受付タイムアウト
        if (stepIndex > 0 && (Time.time - commandStartTime) > commandTimeout)
        {
            ResetCommand("timeout");
        }

        // 方向列の進行
        if (!waitingForButton)
        {
            var targetDir = doubleQcf.sequence[stepIndex];

            if (IsDirMatch(dir, targetDir))
            {
                // 押しっぱなしで即進行しないよう少しの間隔を要求
                if (dir != lastDir || (Time.time - lastStepTime) >= minHoldAdvance)
                {
                    AdvanceStep();
                }
            }
            else
            {
                if (stepIndex > 0 && (Time.time - lastStepTime) > stepGapMax)
                {
                    ResetCommand("step gap exceeded");
                }
            }

            if (stepIndex >= doubleQcf.sequence.Count)
            {
                waitingForButton = true;
                lastStepTime = Time.time; // 最終方向時刻更新（同時押し判定の基点）
            }
        }

        // フィニッシュ（攻撃ボタン）
        if (waitingForButton)
        {
            bool withinSimul = (Time.time - lastStepTime) <= simultaneousGrace;

            if (Input.GetKeyDown(doubleQcf.finishButton) || (withinSimul && Input.GetKey(doubleQcf.finishButton)))
            {
                TriggerFever();
                ResetCommand("success");
            }
            else
            {
                // 同時押し猶予を過ぎたら通常のタイムアウト監視
                if (!withinSimul && (Time.time - lastStepTime) > stepGapMax)
                {
                    ResetCommand("finish timeout");
                }
            }
        }

        lastDir = dir;

    }

    private void AdvanceStep()
    {
        if (stepIndex == 0)
        {
            commandStartTime = Time.time;
        }
        lastStepTime = Time.time;
        stepIndex++;
        // Debug.Log($"Step {stepIndex}/{doubleQcf.sequence.Count}");
    }

    private void ResetCommand(string reason)
    {
        // Debug.Log($"Command reset: {reason}");
        stepIndex = 0;
        waitingForButton = false;
        commandStartTime = -999f;
        lastStepTime = -999f;
        lastDir = Dir.N;
    }

    private void TriggerFever()
    {
        if (feverSequenceInvoker != null)
        {
            StartCoroutine(feverSequenceInvoker());
            Debug.Log($"フィーバー発動：{doubleQcf.name}");
        }
        else
        {
            Debug.LogError("feverManager または FeverSequence() が見つかりません。");
        }
    }

    private void ForceFever()
    {
        if (feverSequenceInvoker != null)
        {
            StartCoroutine(feverSequenceInvoker());
            Debug.Log("フィーバー強制発動（デバッグ）");
        }
        else
        {
            Debug.LogError("feverManager が見つかりません！");
        }
    }

    /// <summary>
    /// WASDのみで8方向判定。W=↑, A=←, S=↓, D=→
    /// 同時押しで斜めを作る（例：S+D = ??）
    /// </summary>
    private Dir ReadDirectionWASD()
    {
        int x = 0;
        int y = 0;

        if (Input.GetKey(KeyCode.D)) x += 1;
        if (Input.GetKey(KeyCode.A)) x -= 1;

        if (Input.GetKey(KeyCode.W)) y += 1;
        if (Input.GetKey(KeyCode.S)) y -= 1;

        if (x == 0 && y == 0) return Dir.N;
        if (x == 0 && y > 0) return Dir.U;
        if (x == 0 && y < 0) return Dir.D;
        if (x > 0 && y == 0) return Dir.R;
        if (x < 0 && y == 0) return Dir.L;
        if (x > 0 && y > 0) return Dir.UR;
        if (x < 0 && y > 0) return Dir.UL;
        if (x > 0 && y < 0) return Dir.DR;
        return Dir.DL;
    }

    /// <summary>
    /// 方向一致判定：斜め中の軸方向許容（??中の→/↓をOKに）
    /// </summary>
    private bool IsDirMatch(Dir current, Dir target)
    {
        if (current == target) return true;

        switch (target)
        {
            case Dir.R: return current == Dir.DR || current == Dir.UR;
            case Dir.L: return current == Dir.DL || current == Dir.UL;
            case Dir.U: return current == Dir.UR || current == Dir.UL;
            case Dir.D: return current == Dir.DR || current == Dir.DL;
            // 斜め指定（??/??/??/??）は厳しめ（近傍軸はNG）
            default: return false;
        }
    }
}
