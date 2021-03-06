using RoR2;
using System.Linq;
using UnityEngine;

namespace ManipulatorMod.Modules.Components
{
    [RequireComponent(typeof(CharacterBody))]
    [RequireComponent(typeof(InputBankTest))]
    [RequireComponent(typeof(TeamComponent))]
    public class ManipulatorTracker : MonoBehaviour
    {
        public GameObject trackerPrefab;
        public float maxTrackingDistance = 60f;
        public float maxTrackingAngle = 35f;
        public float trackerUpdateFrequency = 10f;

        private HurtBox trackingTarget;
        private TeamComponent teamComponent;
        private InputBankTest inputBank;
        private Indicator indicator;
        private readonly BullseyeSearch search = new BullseyeSearch();
        private float trackerUpdateStopwatch;

        private void Awake()
        {
            this.indicator = new Indicator(base.gameObject, this.trackerPrefab);
        }

        private void Start()
        {
            this.inputBank = base.GetComponent<InputBankTest>();
            this.teamComponent = base.GetComponent<TeamComponent>();
        }

        public HurtBox GetTrackingTarget()
        {
            return this.trackingTarget;
        }

        private void OnEnable()
        {
            this.indicator.active = true;
        }

        private void OnDisable()
        {
            this.indicator.active = false;
        }

        private void OnDestroy()
        {
            this.indicator.active = false;
        }

        private void FixedUpdate()
        {
            this.trackerUpdateStopwatch += Time.fixedDeltaTime;

            if (this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency)
            {
                this.trackerUpdateStopwatch -= 1f / this.trackerUpdateFrequency;

                Ray aimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
                this.SearchForTarget(aimRay);

                this.indicator.targetTransform = (this.trackingTarget ? this.trackingTarget.transform : null);
            }
        }

        private void SearchForTarget(Ray aimRay)
        {
            this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(this.teamComponent.teamIndex);
            this.search.filterByLoS = true;
            this.search.searchOrigin = aimRay.origin;
            this.search.searchDirection = aimRay.direction;
            this.search.sortMode = BullseyeSearch.SortMode.Distance;
            this.search.maxDistanceFilter = this.maxTrackingDistance;
            this.search.maxAngleFilter = this.maxTrackingAngle;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);

            this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
        }
    }
}
