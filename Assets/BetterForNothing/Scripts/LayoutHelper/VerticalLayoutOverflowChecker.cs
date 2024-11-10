using UnityEngine;
using UnityEngine.UI;

namespace BetterForNothing.Scripts.LayoutHelper
{
    public class VerticalLayoutOverflowChecker : MonoBehaviour
    {
        private GameObject _simulationContainer;
        private Canvas _parentCanvas;

        private void Awake()
        {
            // 부모 캔버스 찾기
            _parentCanvas = GetComponentInParent<Canvas>();
            if (_parentCanvas == null)
            {
                Debug.LogError("VerticalLayoutOverflowChecker must be placed under a Canvas!");
                return;
            }

            // 시뮬레이션 컨테이너 초기화
            InitializeSimulationContainer();
        }

        private void InitializeSimulationContainer()
        {
            _simulationContainer = new GameObject("LayoutSimulationContainer", typeof(RectTransform));
            _simulationContainer.transform.SetParent(_parentCanvas.transform, false);
        
            var rectTransform = _simulationContainer.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        
            _simulationContainer.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_simulationContainer != null)
            {
                Destroy(_simulationContainer);
            }
        }

        public bool WillOverflow(RectTransform originalLayout, RectTransform newElement)
        {
            if (_parentCanvas == null || _simulationContainer == null) return false;

            // 1. 시뮬레이션 레이아웃 생성
            GameObject simLayout = CreateSimulatedLayout(originalLayout);
        
            try
            {
                // 2. 현재 모든 자식 요소들의 복사본 생성
                CopyExistingElements(originalLayout, simLayout.transform);
            
                // 3. 새로운 요소의 복사본 추가
                GameObject newElementCopy = GameObject.Instantiate(newElement.gameObject, simLayout.transform);
                newElementCopy.SetActive(true);
            
                // 4. 레이아웃 리빌드 강제 실행
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(simLayout.GetComponent<RectTransform>());
            
                // 5. 실제 필요한 높이 계산
                float requiredHeight = CalculateRequiredHeight(simLayout.GetComponent<RectTransform>());
                float availableHeight = originalLayout.rect.height;

                return requiredHeight > availableHeight;
            }
            finally
            {
                // 6. 시뮬레이션 객체들 정리
                if (simLayout != null)
                {
                    Destroy(simLayout);
                }
            }
        }

        private GameObject CreateSimulatedLayout(RectTransform originalLayout)
        {
            var simLayout = new GameObject("SimulatedLayout", typeof(RectTransform), typeof(VerticalLayoutGroup));
            simLayout.transform.SetParent(_simulationContainer.transform, false);
        
            var simRect = simLayout.GetComponent<RectTransform>();
            var simGroup = simLayout.GetComponent<VerticalLayoutGroup>();
            var originalGroup = originalLayout.GetComponent<VerticalLayoutGroup>();

            // RectTransform 설정 복사
            simRect.anchorMin = originalLayout.anchorMin;
            simRect.anchorMax = originalLayout.anchorMax;
            simRect.anchoredPosition = originalLayout.anchoredPosition;
            simRect.sizeDelta = originalLayout.sizeDelta;
            simRect.pivot = originalLayout.pivot;
            simRect.localScale = originalLayout.localScale;

            // 캔버스의 스케일 고려
            if (_parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                simRect.position = originalLayout.position;
            }
        
            // VerticalLayoutGroup 설정 복사
            simGroup.padding = originalGroup.padding;
            simGroup.spacing = originalGroup.spacing;
            simGroup.childAlignment = originalGroup.childAlignment;
            simGroup.childControlHeight = originalGroup.childControlHeight;
            simGroup.childControlWidth = originalGroup.childControlWidth;
            simGroup.childForceExpandHeight = originalGroup.childForceExpandHeight;
            simGroup.childForceExpandWidth = originalGroup.childForceExpandWidth;
        
            return simLayout;
        }

        private void CopyExistingElements(RectTransform originalLayout, Transform simulatedParent)
        {
            for (int i = 0; i < originalLayout.childCount; i++)
            {
                Transform originalChild = originalLayout.GetChild(i);
                if (!originalChild.gameObject.activeSelf) continue;

                GameObject childCopy = GameObject.Instantiate(originalChild.gameObject, simulatedParent);
                childCopy.SetActive(true);

                // 레이아웃 엘리먼트 설정 복사
                LayoutElement originalElement = originalChild.GetComponent<LayoutElement>();
                if (originalElement != null)
                {
                    LayoutElement copyElement = childCopy.GetComponent<LayoutElement>();
                    if (copyElement == null) copyElement = childCopy.AddComponent<LayoutElement>();
                
                    copyElement.preferredWidth = originalElement.preferredWidth;
                    copyElement.preferredHeight = originalElement.preferredHeight;
                    copyElement.flexibleWidth = originalElement.flexibleWidth;
                    copyElement.flexibleHeight = originalElement.flexibleHeight;
                    copyElement.minWidth = originalElement.minWidth;
                    copyElement.minHeight = originalElement.minHeight;
                }
            }
        }

        private float CalculateRequiredHeight(RectTransform simulatedRect)
        {
            float totalHeight = 0f;
        
            VerticalLayoutGroup simGroup = simulatedRect.GetComponent<VerticalLayoutGroup>();
            totalHeight += simGroup.padding.top + simGroup.padding.bottom;
        
            for (int i = 0; i < simulatedRect.childCount; i++)
            {
                RectTransform child = simulatedRect.GetChild(i) as RectTransform;
                if (child != null)
                {
                    totalHeight += LayoutUtility.GetPreferredHeight(child);
                
                    if (i < simulatedRect.childCount - 1)
                        totalHeight += simGroup.spacing;
                }
            }

            // 캔버스 스케일러 고려
            var canvasScaler = _parentCanvas.GetComponent<CanvasScaler>();
            if (canvasScaler != null && canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize)
            {
                float scaleFactor = _parentCanvas.scaleFactor;
                totalHeight *= scaleFactor;
            }
        
            return totalHeight;
        }
    }
}