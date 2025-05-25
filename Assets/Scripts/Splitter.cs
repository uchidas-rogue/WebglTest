using UnityEngine;
using DG.Tweening;
using TMPro;

public class Splitter : MonoBehaviour
{
    [Header("Split Settings")]
    [SerializeField] private GameObject splitPrefab;
    [SerializeField] private int splitCount = 3;
    [SerializeField] private float positionOffset = 0.1f;

    [Header("UI")]
    [SerializeField] private TextMeshPro tmpText; // Inspectorからセット

    private void Start()
    {
        if (tmpText != null)
        {
            // %sをsplitCountで置換して表示
            tmpText.text = tmpText.text.Replace("%s", splitCount.ToString());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bulletScript = collision.gameObject.GetComponent<Bullet>();
            if (bulletScript != null && !bulletScript.isSplitGenerated)
            {
                float speed = bulletScript.GetBulletSpeed();
                float maxDist = bulletScript.GetMaxDistance();
                Vector3 dir = bulletScript.GetDirection();

                for (int i = 0; i < splitCount; i++)
                {
                    Vector3 offset = Vector3.zero;
                    if (splitCount > 1)
                    {
                        float totalWidth = positionOffset * (splitCount - 1);
                        offset = transform.right * (-totalWidth / 2f + positionOffset * i);
                    }
                    Vector3 spawnPos = collision.contacts[0].point + offset;

                    GameObject clone = Instantiate(splitPrefab, spawnPos, collision.transform.rotation);

                    // Bullet.csのsetterでパラメータをセット
                    Bullet cloneBullet = clone.GetComponent<Bullet>();
                    if (cloneBullet != null)
                    {
                        cloneBullet.SetBulletParams(speed, maxDist, dir);
                        cloneBullet.isSplitGenerated = true;
                    }
                }
                Destroy(collision.gameObject);
            }
        }
    }
}
