using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    public Grid Grid;
    public GameObject PObject;
    Vector3Int lastCellPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 마우스 포인터 위치에 따라 그리드 셀 좌표 계산
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        Vector3Int cellPosition = Grid.WorldToCell(mouseWorldPosition);

        // 오브젝트가 새로운 셀에 들어갔을 때만 스냅
        if (cellPosition != lastCellPosition)
        {
            lastCellPosition = cellPosition;

            // 그리드 셀 좌표를 월드 좌표로 변환하여 3D 오브젝트 위치 업데이트
            Vector3 snappedPosition = Grid.CellToWorld(cellPosition);
            PObject.transform.position = snappedPosition;

            // 배치 가능한지 여부에 따라 색상 변경
            if (IsPlacementValid(cellPosition))
            {
                PObject.GetComponent<Renderer>().material.color = Color.green; // 배치 가능: 초록색
            }
            else
            {
                PObject.GetComponent<Renderer>().material.color = Color.red; // 배치 불가: 빨간색
            }
        }

        // 90도 단위로 회전
        if (Input.GetKeyDown(KeyCode.R))
        {
            PObject.transform.Rotate(0, 90, 0);
        }

        // 배치 확정 (마우스 클릭)
        if (Input.GetMouseButtonDown(0) && IsPlacementValid(cellPosition))
        {
            PlaceObject(cellPosition);
        }
    }

    // 마우스 위치를 월드 좌표로 변환
    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    // 배치 가능한지 확인 (충돌 검사)
    bool IsPlacementValid(Vector3Int cellPosition)
    {
        Collider[] colliders = Physics.OverlapBox(Grid.CellToWorld(cellPosition), PObject.transform.localScale / 2);
        return colliders.Length == 0; // 충돌이 없으면 배치 가능
    }

    // 오브젝트 배치 확정
    void PlaceObject(Vector3Int cellPosition)
    {
        GameObject placedObject = Instantiate(PObject);
        placedObject.transform.position = Grid.CellToWorld(cellPosition);
        placedObject.GetComponent<Renderer>().material.color = Color.white; // 배치된 오브젝트는 흰색으로 표시
    }
}

