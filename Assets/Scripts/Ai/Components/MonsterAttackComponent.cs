namespace Ai
{
    /// <summary>
    /// This class handles the attack behavior for the "Monster" Ai type.
    /// </summary>
    public class MonsterAttackComponent : AiComponent
    {
        private void Awake()
        {
            entity.OnStartedAttacking.AddListener(HandleStartedAttacking);
            entity.OnDealtDamage.AddListener(HandleDealDamage);
        }

        private void HandleStartedAttacking()
        {
            PlayerCore.LocalPlayer.PlayerTarget.HandleAttacked();
            references.CameraFocus.AttemptFocus();
            references.CameraFocus.AllowUnfocusKey = false;
        }

        private void HandleDealDamage()
        {
            if (gameplayInfo.Target is null && gameplayInfo.RecentTarget is not null)
            {
                gameplayInfo.Target = gameplayInfo.RecentTarget;
            }
            
            PlayerCore.LocalPlayer.PlayerTarget.DealDamage(attributes.AttackDamage);
            PlayerCore.LocalPlayer.PlayerTarget.StartFadeout(FadeoutCompleteCallback);
        }

        private void FadeoutCompleteCallback()
        {
            if(gameplayInfo.TargetHideout)
                PlayerCore.LocalPlayer.PlayerTarget.SnapToPosition(gameplayInfo.TargetHideout.InteractionPoint);
            entity.Disappear();
            references.CameraFocus.AllowUnfocusKey = true;
            references.CameraFocus.Unfocus();
        }
    }
}