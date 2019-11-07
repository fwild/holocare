using HoloToolkit.Examples.SpatialUnderstandingFeatureOverview;
using HoloToolkit.Unity;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts
{
    public class RoomMappingManager : MonoBehaviour
    {
        [SerializeField]
        Button m_finishMapping, m_proceedToPlaceUI;
        [SerializeField]
        TextMesh m_mainTextMesh;
        [SerializeField]
        AudioSource m_audioSource;
        [SerializeField]
        AudioClip m_sittableFoundClip, m_noSittableFoundClip;
        [SerializeField]
        GameObject m_UIParent, m_UIParentBackPanel, m_assistant;
        void Start()
        {
            m_mainTextMesh.text = "Walk around and scan in your playspace";

            m_finishMapping.OnButtonClicked += M_finishMapping_OnButtonClicked;
            m_proceedToPlaceUI.OnButtonClicked += M_proceedToPlaceUI_OnButtonClicked;
        }

        private void M_proceedToPlaceUI_OnButtonClicked(GameObject obj)
        {
            m_proceedToPlaceUI.gameObject.SetActive(false);
            SpaceVisualizer.Instance.ClearGeometry();

            m_mainTextMesh.text = "Please, click on the menu in order to\nplace it on a wall";
            StartSpatialMapping();
        }

        private void StartSpatialMapping()
        {
            SpatialMappingManager.Instance.DrawVisualMeshes = true;
            SpatialMappingManager.Instance.StartObserver();
            PulseManager.Instance.StartPulse(GazeManager.Instance.HitPosition);

            m_UIParent.GetComponent<BoxCollider>().enabled = true;
            m_UIParentBackPanel.GetComponent<MeshRenderer>().enabled = true;
            m_assistant.SetActive(true);

            TapToPlace tapToPlace = m_UIParent.GetComponent<TapToPlace>();
            tapToPlace.IsBeingPlaced = true;
            tapToPlace.OnPlacementChanged += TapToPlace_OnPlacementChanged;

        }

        private void TapToPlace_OnPlacementChanged(bool obj)
        {
            if (!obj)
            {
                SpatialMappingManager.Instance.DrawVisualMeshes = false;
                SpatialMappingManager.Instance.StopObserver();

                foreach (var item in SpatialMappingManager.Instance.GetSurfaceObjects())
                {
                    item.Collider.enabled = false;
                }

                PulseManager.Instance.StopPulse();
            }
            m_UIParent.GetComponent<BoxCollider>().enabled = false;
            m_UIParent.GetComponent<Billboard>().enabled = false;
            m_UIParent.GetComponent<TapToPlace>().enabled = false;
            m_UIParentBackPanel.GetComponent<MeshRenderer>().enabled = false;

            m_mainTextMesh.text = "";

            UIManager.Instance.ShowMainMenu(true);
            SpatialMappingManager.Instance.gameObject.SetActive(false);
        }

        bool m_shouldCheckScanState, m_scanComplete, m_shouldCheckForSittables;
        bool m_shouldCheckForRemainingSUMesh;
        float m_cumulatedTime = 0;
        private void M_finishMapping_OnButtonClicked(GameObject obj)
        {
            if ((SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning) &&
                !SpatialUnderstanding.Instance.ScanStatsReportStillWorking)
            {
                SpatialUnderstanding.Instance.RequestFinishScan();
                m_shouldCheckScanState = true;
                m_shouldCheckForRemainingSUMesh = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (m_shouldCheckScanState)
            {
                switch (SpatialUnderstanding.Instance.ScanState)
                {
                    case SpatialUnderstanding.ScanStates.Scanning:
                        // Get the scan stats
                        IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
                        if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) == 0)
                        {
                            //m_mainTextMesh.text = "playspace stats query failed";
                            break;
                        }
                        //m_mainTextMesh.text = "Walk around and scan in your playspace";
                        break;
                    case SpatialUnderstanding.ScanStates.Finishing:
                        //m_mainTextMesh.text = "Finalizing scan (please wait)";
                        break;
                    case SpatialUnderstanding.ScanStates.Done:
                        //m_mainTextMesh.text = "Scan complete - Use the menu to run queries";
                        m_scanComplete = true;
                        m_shouldCheckScanState = false;
                        break;
                    default:
                        //m_mainTextMesh.text = "ScanState = " + SpatialUnderstanding.Instance.ScanState.ToString();
                        break;
                }
            }
            if (m_scanComplete && !m_shouldCheckForSittables)
            {
                m_shouldCheckForSittables = true;
                m_finishMapping.gameObject.SetActive(false);
                FindSittablePlaces();
            }
            //if (m_shouldCheckForRemainingSUMesh && m_cumulatedTime < 4)
            //{
            //    Renderer[] spatialUnderstandingRenderers = SpatialUnderstanding.Instance.GetComponentsInChildren<Renderer>();
            //    foreach (var sUMesh in spatialUnderstandingRenderers)
            //    {
            //        sUMesh.enabled = false;
            //        MeshCollider collider = sUMesh.GetComponent<MeshCollider>();
            //        if (collider != null)
            //            collider.enabled = false;

            //    }
            //    m_cumulatedTime += Time.deltaTime;
            //}
            //else
            //{
            //    m_shouldCheckForRemainingSUMesh = false;
            //}
        }

        private void FindSittablePlaces()
        {
            int shapeCount = SpaceVisualizer.Instance.Query_Shape_FindPositionsOnShape("Sittable");
            if (shapeCount <= 0)
            {
                m_audioSource.clip = m_noSittableFoundClip;
                m_mainTextMesh.text = "No sittable places were found,\nyou may want to bring a chair";
            }
            else
            {
                m_audioSource.clip = m_sittableFoundClip;
                m_mainTextMesh.text = "Please, sit on one of those places.\n When you're done, press Ok";
            }

            m_proceedToPlaceUI.gameObject.SetActive(true);
            m_audioSource.Play();

            SpatialUnderstanding.Instance.gameObject.SetActive(false);
            //Renderer[] spatialUnderstandingRenderers = SpatialUnderstanding.Instance.GetComponentsInChildren<Renderer>();
            //foreach (var sUMesh in spatialUnderstandingRenderers)
            //{
            //    sUMesh.enabled = false;
            //    MeshCollider collider = sUMesh.GetComponent<MeshCollider>();
            //    if (collider != null)
            //        collider.enabled = false;

            //}
        }
    }
}
