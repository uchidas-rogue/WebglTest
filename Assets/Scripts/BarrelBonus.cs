using UnityEngine;
using TMPro;

public class BarrelBonus : MonoBehaviour
{
    public enum BonusType { BulletCountUp, BulletDamageUp }
    [SerializeField] public BonusType bonusType = BonusType.BulletCountUp;
    [SerializeField] private int bonusValue = 1; // 増加量
    [HideInInspector] public BulletSpawner bulletSpawner; // 外部からセット
    [HideInInspector] public AudioClip barrelBurstSound; // タルが破壊されたときの音（外部からセット）
    [HideInInspector] public AudioClip levelupSound; // レベルアップの音（外部からセット）

    public int hp = 3; // 外部からセット可能に（public）
    [SerializeField] private TextMeshPro tmpText; // InspectorでBarrelと一緒に配置されるTextMeshProをセット

    private AudioSource barrelSoundSource; // Bulletの音（Inspectorでセット）

    private bool isBonusGiven = false; // ボーナス重複防止用

    private void Start()
    {
        // 親オブジェクトからAudioSourceを取得
        barrelSoundSource = GetComponentInParent<AudioSource>();
    }

    public void UpdateText()
    {
        if (tmpText != null)
        {
            string bonusStr = "";
            switch (bonusType)
            {
                case BonusType.BulletCountUp:
                    bonusStr = "Bullet Count UP";
                    break;
                case BonusType.BulletDamageUp:
                    bonusStr = "Bullet Damage UP";
                    break;
            }
            tmpText.text = $"{bonusStr}\nHP:{hp}";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && !isBonusGiven)
        {
            // Bulletのdamageを取得
            int damage = 1;
            Bullet bulletScript = other.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                damage = bulletScript.damage;
            }

            hp -= damage;
            UpdateText();

            if (hp <= 0)
            {
                barrelSoundSource.PlayOneShot(barrelBurstSound); // タルが破壊されたときの音を再生
                barrelSoundSource.PlayOneShot(levelupSound); // レベルアップの音を再生
                
                isBonusGiven = true; // ボーナス発動済みフラグ
                if (bulletSpawner != null)
                {
                    if (bonusType == BonusType.BulletCountUp)
                    {
                        bulletSpawner.IncreaseBulletCount(bonusValue);
                    }
                    else if (bonusType == BonusType.BulletDamageUp)
                    {
                        bulletSpawner.IncreaseBulletDamage(bonusValue);
                    }
                }
                Destroy(gameObject);
            }
        }
    }
}