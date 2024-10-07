using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlacementMode
{
    Fixed,  // 배치가 확정된 상태
    Valid,  // 배치가 유효한 상태
    Invalid // 배치가 유효하지 않은 상태
}

public class BuildingManager : MonoBehaviour
{
    public Material validPlacementMaterial;  // 유효한 배치 시 적용될 재질
    public Material invalidPlacementMaterial;  // 유효하지 않은 배치 시 적용될 재질

    public MeshRenderer[] meshComponents;  // 건물의 각 MeshRenderer 컴포넌트들
    private Dictionary<MeshRenderer, List<Material>> initialMaterials;  // 각 MeshRenderer에 원래 적용된 재질들을 저장할 딕셔너리

    [HideInInspector] public bool hasValidPlacement;  // 현재 배치가 유효한지 여부를 저장
    [HideInInspector] public bool isFixed;  // 배치가 고정되었는지 여부를 저장

    private int _nObstacles;  // 충돌하는 장애물의 개수


    private void Awake()
    {
        hasValidPlacement = true;  // 배치가 처음엔 유효하다고 설정
        isFixed = true;  // 처음엔 배치가 고정되었다고 설정
        _nObstacles = 0;  // 처음엔 장애물 개수가 0으로 시작

        _InitializeMaterials();  // 초기 재질 설정
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFixed) return;  // 배치가 고정된 상태라면 충돌을 무시

        // 바닥 오브젝트는 무시
        if (_IsGround(other.gameObject)) return;

        _nObstacles++;  // 충돌한 장애물 개수 증가
        SetPlacementMode(PlacementMode.Invalid);  // 장애물이 존재하면 유효하지 않은 배치로 설정
    }

    private void OnTriggerExit(Collider other)
    {
        if (isFixed) return;  // 배치가 고정된 상태라면 충돌을 무시

        // 바닥 오브젝트는 무시
        if (_IsGround(other.gameObject)) return;

        _nObstacles--;  // 충돌한 장애물 개수 감소
        if (_nObstacles == 0)  // 장애물이 없을 경우 유효한 배치로 설정
            SetPlacementMode(PlacementMode.Valid);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _InitializeMaterials();  // 에디터에서 스크립트를 수정할 때마다 초기 재질 설정을 다시 호출
    }
#endif

    public void SetPlacementMode(PlacementMode mode)
    {
        if (mode == PlacementMode.Fixed)
        {
            isFixed = true;  // 배치가 고정됨
            hasValidPlacement = true;  // 고정된 배치는 항상 유효
        }
        else if (mode == PlacementMode.Valid)
        {
            hasValidPlacement = true;  // 배치가 유효
        }
        else
        {
            hasValidPlacement = false;  // 배치가 유효하지 않음
        }
        SetMaterial(mode);  // 모드에 맞는 재질 설정
    }

    public void SetMaterial(PlacementMode mode)
    {
        if (mode == PlacementMode.Fixed)
        {
            // 배치가 고정된 경우 원래 재질로 복원
            foreach (MeshRenderer r in meshComponents)
                r.sharedMaterials = initialMaterials[r].ToArray();
        }
        else
        {
            // 유효/유효하지 않은 배치에 맞는 재질 적용
            Material matToApply = mode == PlacementMode.Valid
                ? validPlacementMaterial : invalidPlacementMaterial;

            Material[] m;
            int nMaterials;
            foreach (MeshRenderer r in meshComponents)
            {
                nMaterials = initialMaterials[r].Count;  // 원래 적용된 재질의 개수 확인
                m = new Material[nMaterials];
                for (int i = 0; i < nMaterials; i++)
                    m[i] = matToApply;  // 모든 재질을 해당 모드에 맞는 재질로 설정
                r.sharedMaterials = m;
            }
        }
    }

    private void _InitializeMaterials()
    {
        if (initialMaterials == null)
            initialMaterials = new Dictionary<MeshRenderer, List<Material>>();  // 초기화할 딕셔너리 생성
        if (initialMaterials.Count > 0)
        {
            foreach (var l in initialMaterials) l.Value.Clear();  // 기존 재질 리스트 초기화
            initialMaterials.Clear();  // 딕셔너리 비우기
        }

        foreach (MeshRenderer r in meshComponents)
        {
            initialMaterials[r] = new List<Material>(r.sharedMaterials);  // 각 MeshRenderer의 초기 재질을 저장
        }
    }

    private bool _IsGround(GameObject o)
    {
        // 오브젝트가 바닥인지 확인하는 함수
        return ((1 << o.layer) & BuildingPlacer.instance.groundLayerMask.value) != 0;
    }
}
