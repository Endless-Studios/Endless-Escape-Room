namespace Ai
{
    public class AttackComponent : AiComponent
    {
        private void Awake()
        {
            entity.OnStartedAttacking.AddListener(HandleStartedAttacking);
            entity.OnDealtDamage.AddListener(HandleDealDamage);
        }

        private void HandleStartedAttacking()
        {
            references.CameraFocus.Focus();
        }

        private void HandleDealDamage()
        {
            gameplayInfo.Target.DealDamage(attributes.AttackDamage);
            gameplayInfo.Target.StartFadeout(FadeoutCompleteCallback);
        }

        private void FadeoutCompleteCallback()
        {
            entity.Disappear();
            references.CameraFocus.Unfocus();
        }
    }
}