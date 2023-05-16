using Sound;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This component is analogous to the human ear. It listens for sounds to be emitted and checks if the sound reaches
    /// the sensor with enough volume to be heard.
    /// </summary>
    public class HearingSensor : MonoBehaviour
    {
        [SerializeField] private float minPerceivedDB;
        
        private readonly RaycastHit[] hits = new RaycastHit[10];
        
        private void Awake()
        {
            SoundManager.OnSoundEmitted += HandleOnSoundEmitted;
        }

        private void OnDestroy()
        {
            SoundManager.OnSoundEmitted -= HandleOnSoundEmitted;
        }

        private void HandleOnSoundEmitted(EmittedSoundData soundData)
        {
            if (soundData.SoundKind == SoundEnum.AiGenerated)
                return;

            float perceivedDB = CalculatePerceivedDB(soundData);
            if (perceivedDB > minPerceivedDB)
            {
                
            }
        }

        /// <summary>
        /// Calculate the final dB of a sound when it reaches the HearingSensor. This is not physically accurate but
        /// estimates the dB based on distance falloff and the SoundBlockingValue of any obstacles between the source and
        /// the sensor
        /// </summary>
        /// <param name="soundData">Information about the emitted sound</param>
        /// <returns></returns>
        private float CalculatePerceivedDB(EmittedSoundData soundData)
        {
            Vector3 position = transform.position;
            
            //Get the distance from the source of the sound to the sensor
            float distanceToSoundSource = Vector3.Distance(position, soundData.Position);

            // Raycast from the origin of the sound against all the layers that could possibly block sound
            int numHits = Physics.RaycastNonAlloc(soundData.Position, (position - soundData.Position).normalized, hits, distanceToSoundSource, SoundManager.Instance.SoundBlockerMask);

            // If we hit nothing return the db of the sound after calculating its falloff over the distance
            if (numHits == 0)
            {
                float dbAfterFalloff = CalculateDBAfterDistanceFalloff(soundData.DBAtSource, distanceToSoundSource);
                return dbAfterFalloff;
            }

            float currentDB = soundData.DBAtSource;
            Vector3 currentSoundOrigin = soundData.Position;
            
            //Step through each hit collider
            for (int i = 0; i < numHits; i++)
            {
                //Get the hit collider and check if it has a SoundBlockingModifier. If it does use that value otherwise use the default
                RaycastHit hit = hits[i];
                SoundBlockingModifier modifier = hit.transform.GetComponent<SoundBlockingModifier>();
                float soundBlockingValue = SoundManager.Instance.DefaultSoundBlockingValue;
                float distanceToBarrier = Vector3.Distance(currentSoundOrigin, hit.point);
                
                if (modifier)
                {
                    soundBlockingValue = modifier.SoundBlockingValue;
                }

                //Calculate the volume of the sound when it reaches the hit collider 
                float dBAtBarrier = CalculateDBAfterDistanceFalloff(currentDB, distanceToBarrier);
                currentDB = dBAtBarrier - soundBlockingValue;
                
                //If the current volume of the sound is under our minPerceivedDB  
                if (currentDB <= minPerceivedDB)
                    return currentDB;

                //If there are still more hits to evaluate we need to calculate where the sound will come out of the
                //collider so we can calculate falloff again
                if (i + 1 < numHits)
                {
                    RaycastHit nextHit = hits[i + 1];
                    Vector3 toVector = hit.point - nextHit.point;
                    Ray ray = new Ray(nextHit.point, toVector.normalized);
                    if (hit.collider.Raycast(ray, out RaycastHit backSideHit, toVector.magnitude))
                    {
                        currentSoundOrigin = backSideHit.point;
                    }
                }
                //Otherwise we need to calculate the final falloff to the target
                else
                {
                    Vector3 toVector = hit.point - position;
                    Ray ray = new Ray(position, toVector.normalized);
                    
                    if (hit.collider.Raycast(ray, out RaycastHit backsideHit, toVector.magnitude))
                    {
                        currentSoundOrigin = backsideHit.point;
                        return CalculateDBAfterDistanceFalloff(currentDB,
                            Vector3.Distance(currentSoundOrigin, position));
                    }
                }
            }
            
            return currentDB;
        }
        
        /// <summary>
        /// Calculates the volume of the sound over distance. Initial DB is presumed to be recorded at 1m to simplify
        /// calculation
        /// </summary>
        /// <param name="initialDB">Volume of the sound in dB presumed to be 1 meter from the source when recorded</param>
        /// <param name="distance">Distance from the source of the sound and the object receiving the sound</param>
        /// <returns>The dB of the sound after traveling distance</returns>
        private static float CalculateDBAfterDistanceFalloff(float initialDB, float distance)
        {
            // Convert dB to intensity (power) using the formula: Intensity = 10^(dB/10)
            float sourceIntensity = Mathf.Pow(10, initialDB / 10);

            // Calculate the intensity at the target distance using the inverse square law: I2 = I1 * (d1^2 / d2^2)
            float intensity = sourceIntensity * (Mathf.Pow(1, 2) / Mathf.Pow(distance, 2));

            // Convert the target intensity back to dB using the formula: dB = 10 * log10(Intensity)
            float dBAfterFalloff = 10 * Mathf.Log10(intensity);

            return dBAfterFalloff;
        }
    }
}