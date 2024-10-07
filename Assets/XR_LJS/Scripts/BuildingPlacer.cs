using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
// using Newtonsoft.Json;  // Json 파싱을 위해 필요 (Newtonsoft.Json 패키지 필요)

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer instance;  // 싱글톤 패턴
    public LayerMask groundLayerMask;  // 바닥 레이어 마스크
                                       // public GridManager gridManager;  // 그리드 매니저 (그리드 상태 관리)

    protected GameObject _toBuild;  // 현재 배치 중인 오브젝트
    protected Camera _mainCamera;  // 메인 카메라
    protected Ray _ray;  // 레이
    protected RaycastHit _hit;  // 레이캐스트 히트 결과
                                //  protected ObjectData _objectData;  // JSON으로 받은 오브젝트 데이터

    private void Awake()
    {
        instance = this;  // 싱글톤 설정
        _mainCamera = Camera.main;  // 메인 카메라 참조
    }

    private void Update()
    {
        // if (_objectData != null) // JSON으로 받은 오브젝트가 있을 때만 실행
        {
            // 마우스 우클릭으로 배치 취소
            if (Input.GetMouseButtonDown(1))
            {
                CancelBuilding();
                return;
            }

            // 마우스가 UI 위에 있으면 배치 프리뷰 숨기기
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (_toBuild.activeSelf) _toBuild.SetActive(false);
                return;
            }
            else if (!_toBuild.activeSelf) _toBuild.SetActive(true);

            // 마우스 위치에서 레이 발사
           //  _ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
           // if (Physics.Raycast(_ray, out _hit, 1000f, groundLayerMask))
           // {
           // 레이 히트 지점에 그리드 스냅 적용
           //Vector3 snappedPosition = gridManager.GetSnappedPosition(_hit.point, _objectData.size);
           //  _toBuild.transform.position = snappedPosition;

                // 그리드에 오브젝트가 있으면 빨간색으로, 없으면 초록색으로 설정
                // if (gridManager.IsOccupied(snappedPosition, _objectData.size))
                //  {
                //     SetPlacementMode(PlacementMode.Invalid);
                // }
                //    else
                //    {
                //        SetPlacementMode(PlacementMode.Valid);
                //    }

                //    // 마우스 좌클릭으로 배치 확정
                //    if (Input.GetMouseButtonDown(0))
                //    {
                //        if (_toBuild.GetComponent<BuildingManager>().hasValidPlacement)
                //        {
                //            PlaceObject();
                //        }
                //    }
                //}
            else if (_toBuild.activeSelf) _toBuild.SetActive(false);
        }
    }

    // 서버로부터 받은 JSON 데이터 파싱 및 오브젝트 생성 준비
    //public void SetObjectDataFromJson(string jsonData)
    //{
    //    _objectData = JsonConvert.DeserializeObject<ObjectData>(jsonData);
    //    PrepareBuilding();
    //}

    protected virtual void PrepareBuilding()
    {
        if (_toBuild) Destroy(_toBuild);

        // 서버로부터 받은 오브젝트 프리팹을 인스턴스화
        //  _toBuild = Instantiate(Resources.Load<GameObject>(_objectData.prefabPath));
        _toBuild.SetActive(false);

        BuildingManager m = _toBuild.GetComponent<BuildingManager>();
        m.isFixed = false;
        m.SetPlacementMode(PlacementMode.Valid);
    }

    private void CancelBuilding()
    {
        Destroy(_toBuild);
        _toBuild = null;
        //  _objectData = null;
    }

    private void PlaceObject()
    {
        BuildingManager m = _toBuild.GetComponent<BuildingManager>();
        m.SetPlacementMode(PlacementMode.Fixed);

        // 그리드에 오브젝트 고정
        //   gridManager.OccupyGrid(_toBuild.transform.position, _objectData.size);

        _toBuild = null;
        //  _objectData = null;
    }

    private void SetPlacementMode(PlacementMode mode)
    {
        BuildingManager m = _toBuild.GetComponent<BuildingManager>();
        m.SetPlacementMode(mode);
    }
}
