/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Meta.XR.Samples;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Meta.XR.MRUtilityKitSamples.BouncingBall
{
    [MetaCodeSample("MRUKSample-BouncingBall")]
    public class BouncingcBallMgr : MonoBehaviour
    {
        [SerializeField] private Transform trackingSpace;
        [SerializeField] private Transform rightControllerPivot;
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private DogMovement dog;
        [SerializeField] private Transform player;

        private BouncingBallLogic currentBall;
        private bool ballGrabbed;
        private Queue<BouncingBallLogic> ballQueue = new();
        private Queue<BouncingBallLogic> ballFetchedQueue = new();
        private bool dogIsFetching = false;
        private bool fetchInProgress = false;
        private const int MAX_BALL = 5;
        private const int MIN_BALL = 1;
        private LineRenderer lineRenderer;
        private bool dogGoingToTarget = false;
        public Vector3 currentLaserTarget { get; private set; }

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();

                lineRenderer.positionCount = 2;
                lineRenderer.startWidth = 0.01f;
                lineRenderer.endWidth = 0.01f;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.red;
                lineRenderer.enabled = true; // Enable if you want it visible from start
            }
            Debug.Log($"Nam11 lineRenderer = {lineRenderer}");
        }

        private void Update()
        {
            const OVRInput.RawButton grabButton = OVRInput.RawButton.RHandTrigger;
            bool isButtonDown = OVRInput.GetDown(grabButton);

            // Ying you can use the ball ballGrabbed or any button to pet the dog.
            /* if (!ballGrabbed && isButtonDown)
            {
                currentBall = Instantiate(ballPrefab).GetComponent<BouncingBallLogic>();
                ballGrabbed = true;
                Debug.Log($"Nam11 BouncingBallMgr Update Trigger pulled!  currentBall = {currentBall.transform.position}");
            }

            if (ballGrabbed)
            {
                currentBall.Rigidbody.position = rightControllerPivot.position;
                //Debug.Log($"Nam11 BouncingBallMgr Update ballGrabbed pulled! currentBall.Rigidbody.position = {currentBall.Rigidbody.position}");
                if (OVRInput.GetUp(grabButton))
                {
                    var localVel = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
                    var vel = trackingSpace.TransformVector(localVel);
                    var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
                    currentBall.Release(rightControllerPivot.position, vel, angVel);
                    ballGrabbed = false;
                    Debug.Log($"Nam11 BouncingBallMgr Update ball release dogIsFetching = {dogIsFetching} ballQueue.Count = {ballQueue.Count} fetchInProgress = {fetchInProgress}");
                }
            } */

            if (lineRenderer != null)
            {
                Ray ray = new Ray(rightControllerPivot.position, rightControllerPivot.forward);
                RaycastHit hitInfo;

                if (Physics.Raycast(ray, out hitInfo, 50f))
                {
                    Vector3 hitPoint = hitInfo.point;

                    // Update laser to hit point
                    lineRenderer.SetPosition(0, rightControllerPivot.position);
                    lineRenderer.SetPosition(1, hitPoint);

                    // Optionally store this point for later voice command
                    currentLaserTarget = hitPoint;
                }
                else
                {
                    // Laser to maximum length
                    Vector3 farPoint = rightControllerPivot.position + rightControllerPivot.forward * 50f;
                    lineRenderer.SetPosition(0, rightControllerPivot.position);
                    lineRenderer.SetPosition(1, farPoint);

                    currentLaserTarget = farPoint; // still store the fallback point
                }
            }
            else
            {
                Debug.Log($"Nam11 Update lineRenderer = {lineRenderer}");
            }

            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            {
                if (ballQueue.Count + ballFetchedQueue.Count < MAX_BALL)
                {
                    const float speed = 10f;
                    var newBall = Instantiate(ballPrefab).GetComponent<BouncingBallLogic>();
                    const float shiftToPreventCollisionWithGrabbedBall = 0.1f;
                    var pos = rightControllerPivot.position +
                            rightControllerPivot.forward * shiftToPreventCollisionWithGrabbedBall;
                    newBall.Release(pos, rightControllerPivot.forward * speed, Vector3.zero);
                    ballQueue.Enqueue(newBall);
                    Debug.Log($"Nam11 BouncingBallMgr Update RIndexTrigger pulled! pos = {pos}" +
                    $"rightControllerPivot.position = {rightControllerPivot.position} dir = {rightControllerPivot.forward}");
                }
                StartCoroutine(DestroyBall());
            }
            if (!dogIsFetching && ballQueue.Count > 0 && !fetchInProgress)
            {
                Debug.Log("Nam11 BouncingBallMgr Dispatch TryFetchNextBall");
                fetchInProgress = true;
                StartCoroutine(TryFetchNextBall());
            }
        }
        private void ThrowBall(Vector3 position, Vector3 velocity, Vector3 angularVelocity)
        {
            var ball = Instantiate(ballPrefab).GetComponent<BouncingBallLogic>();
            ball.Release(position, velocity, angularVelocity);

            ballQueue.Enqueue(ball);
            //StartCoroutine(TryFetchNextBall());
        }

        private IEnumerator TryFetchNextBall()
        {
            Debug.Log($"Nam11 BouncingBallMgr TryFetchNextBall start ballQueue.Count = {ballQueue.Count}");
            if (ballQueue.Count > 0)
            {
                var nextBall = ballQueue.Dequeue();
                yield return StartCoroutine(WaitForBallToSettleAndFetch(nextBall));
                Debug.Log("Nam11 BouncingBallMgr TryFetchNextBall ball stop. Dog about to fetch the ball.");
                dogIsFetching = true;
                dog.MakeDogFetchBall(nextBall.transform, () =>
                {
                    dogIsFetching = false;
                    Rigidbody rb = nextBall.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.WakeUp();
                });

                // Wait until the fetch is done
                while (dogIsFetching)
                {
                    yield return null;
                }
                ballFetchedQueue.Enqueue(nextBall);

            }
            fetchInProgress = false;
            Debug.Log($"Nam11 BouncingBallMgr TryFetchNextBall end ballQueue.Count = {ballQueue.Count} ballFetchedQueue.Count = {ballFetchedQueue.Count}");
        }

        private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(obj);
        }

        private IEnumerator WaitForBallToSettleAndFetch(BouncingBallLogic ball)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogWarning("Ball has no Rigidbody.");
                yield break;
            }

            // Wait until velocity and angular velocity are low
            while (rb.velocity.magnitude > 0.2f)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f); // Optional delay for realism
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Debug.Log($"Nam11 WaitForBallToSettleAndFetch end ball.position = {ball.transform.position}");
        }

        private IEnumerator DestroyBall()
        {
            if (ballFetchedQueue.Count > MIN_BALL)
            {
                Debug.Log($"Nam11 BouncingBallMgr TryFetchNextBall Destroy ball");
                BouncingBallLogic destroyBall = ballFetchedQueue.Dequeue();
                yield return StartCoroutine(destroyBall.DestroyBall(1.0f));
            }
        }
    }
}
