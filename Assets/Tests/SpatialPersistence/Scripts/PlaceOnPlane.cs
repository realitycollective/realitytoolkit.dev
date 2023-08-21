using RealityCollective.ServiceFramework.Services;
using RealityCollective.Utilities;
using RealityCollective.Extensions;
using RealityToolkit.SpatialPersistence.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Listens for touch events and performs an AR raycast from the screen touch point.
/// AR raycasts will only hit detected trackables like feature points and planes.
///
/// If a raycast hits a trackable, the <see cref="PlacedPrefab"/> is instantiated
/// and moved to the hit position.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    private GameObject placedPrefab;

    [SerializeField]
    private GameObject placementIndicatorPrefab;

    public RectTransform[] ScreenUIToIgnore = new RectTransform[0];

    [SerializeField]
    private TMPro.TMP_Text textStatus;

    private ISpatialPersistenceService anchorService;

    private Dictionary<Guid, GameObject> anchors = new Dictionary<Guid, GameObject>();

    private Pose placementPose = new Pose();
    private bool placementPoseIsValid = false;
    private bool placementIndicatorConfigured => placementIndicator != null && placementIndicatorPrefab != null;
    private GameObject placementIndicator;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject PlacedPrefab
    {
        get { return placedPrefab; }
        set { placedPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject SpawnedObject { get; private set; }

    protected void Start()
    {
        StaticLogger.DebugMode = true;
        raycastManager = GetComponent<ARRaycastManager>();
        if (ServiceManager.Instance.TryGetService(out anchorService))
        {
            anchorService.CreateAnchorSucceeded += SpatialPersistenceSystem_CreateAnchorSucceeded;
            anchorService.CreateAnchorFailed += SpatialPersistenceSystem_CreateAnchorFailed;
            anchorService.SpatialPersistenceStatusMessage += SpatialPersistenceSystem_SpatialPersistenceStatusMessage;
            anchorService.AnchorLocated += SpatialPersistenceSystem_AnchorLocated;
            anchorService.AnchorUpdated += SpatialPersistenceSystem_AnchorUpdated;
            anchorService.SpatialPersistenceError += AnchorService_SpatialPersistenceError;
            anchorService.StartSpatialPersistenceService();
            StaticLogger.Log($"Anchor System started with {anchorService.ServiceModules.Count} Modules");
        }
        if (placementIndicatorPrefab != null)
        {
            placementIndicator = Instantiate(placementIndicatorPrefab);
        }
    }

    private void AnchorService_SpatialPersistenceError(string message)
    {
        // Bad things happened, but what?
        UpdateStatusText(message, Color.red);
    }

    private void SpatialPersistenceSystem_AnchorUpdated(Guid anchorID, GameObject gameObject)
    {
        StaticLogger.Log($"Anchor [{anchorID}] updated and positioned at [{gameObject.transform.position}]-[{gameObject.transform.rotation}]");
    }

    private void SpatialPersistenceSystem_AnchorLocated(Guid anchorID, GameObject gameObject)
    {
        StaticLogger.Log($"Anchor found [{anchorID}] and placed at [{gameObject.transform.position}]-[{gameObject.transform.rotation}]");
        //Attach a 3D Object to the Empty Anchor Object
        var locatedAnchor = GameObject.Instantiate(PlacedPrefab, gameObject.transform);
        locatedAnchor.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    private void SpatialPersistenceSystem_SpatialPersistenceStatusMessage(string statusMessage)
    {
        // If more data is required during the anchoring process, the Spatial Persistence system needs to feedback to the user.
        UpdateStatusText(statusMessage, Color.black);
    }

    private void SpatialPersistenceSystem_CreateAnchorFailed()
    {
        StaticLogger.LogError("Anchor Failed to Create");
        UpdateStatusText($"Anchor Failed to Create", Color.red);
    }

    private void SpatialPersistenceSystem_CreateAnchorSucceeded(Guid anchorID, GameObject anchoredObject)
    {
        // Reset initial touched object
        GameObject.Destroy(SpawnedObject);
        SpawnedObject = null;

        // Cache Placed object for future use
        anchors.EnsureDictionaryItem(anchorID, anchoredObject);

        if (anchoredObject.IsNotNull())
        {
            // Place an Object on the new Anchor
            var placedAnchor = GameObject.Instantiate(PlacedPrefab, anchoredObject.transform);
            placedAnchor.GetComponent<MeshRenderer>().material.color = Color.magenta;
        }

        // Update UI that placement was successful
        UpdateStatusText($"Anchor ID [{anchorID}] Saved", Color.green);
    }

    private bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    protected void Update()
    {
        if (placementIndicatorConfigured)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
        }

        // Is there a touch, if not, return
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        // Is the touch NOT on a configured UI area. If it hits UI no touch areas, return
        if (!ValidARTouchLocation(touchPosition))
        {
            return;
        }

        // Use the ARRaycast Manager to ray cast into the scene to hit a ARPlane
        if (raycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;

            if (SpawnedObject == null)
            {
                // If this is a new placement, place a temp object where it was touched
                UpdateStatusText($"Placing Model at [{hitPose}]", Color.black);
                SpawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
                SpawnedObject.GetComponent<MeshRenderer>().material.color = Color.red;

                // Pass the touched position to the Spatial Persistence service to create an Anchor
                anchorService?.TryCreateAnchor(hitPose.position, hitPose.rotation, DateTimeOffset.Now.AddDays(1));
            }
            //else
            //{
            //    StaticLogger.Log($"Moving Model to [{hitPose}]");
            //    // Dumb code to move a touched area, above code should be separated to a "Create Anchor" method.
            //    // Once the Anchor creation process has started, moving the object has no effect on the process.
            //    spawnedObject.transform.position = hitPose.position;
            //}
        }
    }

    private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    private ARRaycastManager raycastManager;

    public void ClearAndFindAnchors()
    {
        List<Guid> anchorIDs = new List<Guid>();
        foreach (KeyValuePair<Guid, GameObject> item in anchors)
        {
            GameObject.Destroy(item.Value);
            anchorIDs.Add(item.Key);
        }

        anchorService.TryClearAnchorCache();

        anchorService.TryFindAnchors(anchorIDs.ToArray());
    }

    public bool ValidARTouchLocation(Vector2 touchPosition)
    {
        StaticLogger.Log("Validating Touch Position");
        // Detects user tap and if not in UI, call OnSelectObjectInteraction above
        foreach (var rt in ScreenUIToIgnore)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, touchPosition))
            {
                StaticLogger.Log("Touch Position Invalid");
                return false;
            }
        }
        StaticLogger.Log("Touch Position valid");
        return true;
    }

    private void UpdateStatusText(string message, Color color)
    {
        if (textStatus)
        {
            textStatus.color = color;
            textStatus.text = message;

            StaticLogger.Log(message);
        }
    }

    #region Placement Indicator
    private void UpdatePlacementPose()
    {
        if (Camera.current != null)
        {
            var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

            if (raycastManager != null && raycastManager.Raycast(screenCenter, s_Hits, TrackableType.FeaturePoint))
            {
                placementPoseIsValid = s_Hits.Count > 0;
                if (placementPoseIsValid)
                {
                    placementPose = s_Hits[0].pose;

                    var cameraForward = Camera.current.transform.forward;
                    var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                    placementPose.rotation = Quaternion.LookRotation(cameraBearing);
                }
            }
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }
    #endregion Placement Indicator
}