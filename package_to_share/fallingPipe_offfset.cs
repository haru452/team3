using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; //２行足しただけ、16と27行目

public class FallingPipe : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    [Header("スコア設定")]
    public float maxY; // 落ち始める最高の高さ（100点の位置）
    public float minY; // 落ちきる最低の高さ（0点の位置）

    private bool isCaught = false;
    private bool isFalling = false;
    private int pipeScore = 0;
    public float offsetY = 50; // これと


    public void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        // 物理挙動を最初は止めておく
        rb.isKinematic = true;
        rb.useGravity = false;
        transform.position = new Vector3(transform.position.x, offsetY, transform.position.z);// これたす
    }

    void OnEnable()
    {
        // XR Grab Interactable の「掴まれた時」のイベントにメソッドを登録
        grabInteractable.selectEntered.AddListener(OnCaught);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnCaught);
    }

    // ゲーム管理者（Manager）から呼び出して物理落下を開始させる
    public void Fall()
    {
        isFalling = true;
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    // 掴まれた瞬間に実行されるメソッド
    private void OnCaught(SelectEnterEventArgs args)
    {
        // 既にキャッチされている、またはまだ落ちていない場合は無視
        if (isCaught || !isFalling) return;

        isCaught = true;
        isFalling = false;

        // 1. 掴んだ瞬間のこのオブジェクトのY座標を取得
        float caughtY = transform.position.y;

        // 2. 最低点〜最高点の間で、どの位置にいるかを0.0〜1.0で計算
        // (最高点に近いほど 1.0、最低点に近いほど 0.0 になる)
        float scorePercentage = Mathf.InverseLerp(minY, maxY, caughtY);

        // 3. 割合を0〜100の整数スコアに変換
        pipeScore = Mathf.RoundToInt(scorePercentage * 100);

        // クランプ（念のため0〜100の範囲内に絶対収める）
        pipeScore = Mathf.Clamp(pipeScore, 0, 100);

        Debug.Log($"{gameObject.name} をキャッチ！ 得点: {pipeScore}");

        // ゲーム管理者にこの棒の得点を報告する
        PipeGameManager.Instance.AddScore(pipeScore);
    }
}
