using Sound;
using UnityEngine;

namespace Ai
{
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
            
            for (int i = 0; i < numHits; i++)
            {
                RaycastHit hit = hits[i];
                SoundBlockingModifier modifier = hit.transform.GetComponent<SoundBlockingModifier>();
                float soundBlockingValue = SoundManager.Instance.DefaultSoundBlockingValue;
                float distanceToBarrier = Vector3.Distance(currentSoundOrigin, hit.point);
                
                if (modifier)
                {
                    soundBlockingValue = modifier.SoundBlockingValue;
                }

                float dBAtBarrier = CalculateDBAfterDistanceFalloff(currentDB, distanceToBarrier);
                currentDB = dBAtBarrier - soundBlockingValue;
                
                if (currentDB <= minPerceivedDB)
                    return currentDB;

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