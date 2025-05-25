using UnityEngine;
using TMPro;
using R3;
using Model;
using VContainer;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [Inject] public Score scoreModel { get; set; }

    private void Start()
    {
        scoreModel.scoreRP.Subscribe(score => scoreText.text = $"Score: {score}");
    }
}