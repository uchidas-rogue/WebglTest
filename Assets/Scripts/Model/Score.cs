using R3;

namespace Model
{
    public class Score
    {
        // スコアのReactiveProperty（R3版）
        public ReactiveProperty<int> scoreRP = new ReactiveProperty<int>(0);
    }
}