using UnityEngine;

namespace GPF.UI
{
    public class AnimationStateDetector : MonoBehaviour
    {
        Animator anim;

        int flippingNameHash = Animator.StringToHash("flipping");
        int headsNameHash = Animator.StringToHash("heads");
        int tailsNameHash = Animator.StringToHash("tails");

        void Awake()
        {
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (anim != null)
            {
                var currentValue = DataStore.Instance.Get<bool>(CoinDataPaths.ANIMATING_FLIP);
                var animState = anim.GetCurrentAnimatorStateInfo(0);
                var steady = animState.shortNameHash == headsNameHash ||
                    (animState.shortNameHash == tailsNameHash && animState.normalizedTime >= 0.99f);
                var flipping = !steady;
                if (currentValue != flipping)
                    DataStore.Instance.Set(CoinDataPaths.ANIMATING_FLIP, flipping);
            }
        }
    }
}
