using UnityEngine;
using DG.Tweening;

namespace Game.WoolSort
{
    public class RopeWaveRenderer : MonoBehaviour
    {
        [Header("Wave Parameters")]
        [SerializeField] private float m_Amplitude = 2f;          // Wave amplitude (height)
        [SerializeField] private float m_Frequency = 1f;          // Wave frequency
        [SerializeField] private float m_WaveSpeed = 5f;          // Wave propagation speed
        [SerializeField] private float m_PhaseOffset = 0f;        // Initial phase

        [Header("Render Settings")]
        [SerializeField] private LineRenderer m_LineRenderer;
        [SerializeField] private Transform m_StartPoint;
        [SerializeField] private Transform m_EndPoint;
        [SerializeField] private int m_Resolution = 50;          // Resolution (number of points)
        [SerializeField] private float m_LineWidth = 0.1f;        // Line thickness

        private Vector3[] wavePoints;
        private float timeElapsed = 0f;
        private MaterialPropertyBlock propertyBlock;

        public LineRenderer lineRenderer => m_LineRenderer;
        public Transform startPoint => m_StartPoint;
        public Transform endPoint => m_EndPoint;

        void Awake()
        {
            SetupLineRenderer();
            InitializeWavePoints();
        }

        void Update()
        {
            timeElapsed += Time.deltaTime;
            UpdateWave();
        }

        void SetupLineRenderer()
        {
            // Configure LineRenderer
            lineRenderer.startWidth = m_LineWidth;
            lineRenderer.endWidth = m_LineWidth;
            lineRenderer.positionCount = m_Resolution;
            lineRenderer.useWorldSpace = true;

            // Disable shadow casting and receiving to optimize performance
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
        }

        void InitializeWavePoints()
        {
            wavePoints = new Vector3[m_Resolution];
        }

        void UpdateWave()
        {
            // Calculate direction and distance between two points
            Vector2 direction = (m_EndPoint.position - m_StartPoint.position).normalized;
            float totalDistance = Vector2.Distance(m_StartPoint.position, m_EndPoint.position);
            float stepSize = totalDistance / (m_Resolution - 1);

            for (int i = 0; i < m_Resolution; i++)
            {
                float t = (float)i / (m_Resolution - 1); // Parameter t from 0 to 1

                // Position along the line between the two points
                Vector2 positionOnLine = Vector2.Lerp(m_StartPoint.position, m_EndPoint.position, t);

                // Distance from the start point in the wave direction
                float distanceAlongWave = t * totalDistance;

                // Compute wave amplitude at this position
                float waveValue = m_Amplitude * Mathf.Sin(2f * Mathf.PI * m_Frequency * (timeElapsed - distanceAlongWave / m_WaveSpeed) + m_PhaseOffset);

                // Reduce amplitude to 0 at the start and end points
                float fadeOut = Mathf.Sin(t * Mathf.PI); // Create curve from 0 to 1 then back to 0
                waveValue *= fadeOut;

                // Vector perpendicular to the wave direction (to create vertical oscillation)
                Vector2 perpendicular = new Vector2(-direction.y, direction.x);

                // Final position = line position + perpendicular oscillation
                Vector2 finalPosition = positionOnLine + perpendicular * waveValue;

                wavePoints[i] = new Vector3(finalPosition.x, finalPosition.y, 0f);
            }

            // Ensure the start and end points remain fixed
            wavePoints[0] = new Vector3(m_StartPoint.position.x, m_StartPoint.position.y, 0f);
            wavePoints[m_Resolution - 1] = new Vector3(m_EndPoint.position.x, m_EndPoint.position.y, 0f);

            // Update LineRenderer positions
            lineRenderer.SetPositions(wavePoints);
        }

        public void SetAmplitude(float newAmplitude)
        {
            m_Amplitude = newAmplitude;
        }

        public void SetFrequency(float newFrequency)
        {
            m_Frequency = newFrequency;
        }

        public void SetWaveSpeed(float newSpeed)
        {
            m_WaveSpeed = newSpeed;
        }

        public void SetWaveRange(Vector2 start, Vector2 end)
        {
            m_StartPoint.position = start;
            m_EndPoint.position = end;
        }

        public void SetStartPoint(Vector2 start)
        {
            m_StartPoint.position = start;
        }

        public void SetEndPoint(Vector2 end)
        {
            m_EndPoint.position = end;
        }

        public void SetPhaseOffset(float offset)
        {
            m_PhaseOffset = offset;
        }

        public void ResetTime()
        {
            timeElapsed = 0f;
        }

        public void PauseWave(bool pause)
        {
            enabled = !pause;
        }

        public void SetColor(Color color)
        {
            lineRenderer.material.color = color;
            lineRenderer.sharedMaterial.DOFade(1, 3 * Time.deltaTime).From(0);
        }
    }
}