using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlendShapeMgr : MonoBehaviour
{
    // 메쉬의 BlendShape옵션을 슬라이더에 연결하여 슬라이더를 통해 BlendShape 키 갑을 조정한다.

    // 슬라이더 가져오기
    public Slider slider1;
    public Slider slider2;

    public FlexibleColorPicker fcp;
    
    // 커스텀할 오브젝트와 해당 오브젝트의 SkinnedMeshRenderer컴포넌트 불러오기
    [SerializeField]
    GameObject customObj;
    Mesh mesh;
    SkinnedMeshRenderer smr;
    Material material;

    void Start()
    {
        customObj = GameObject.FindWithTag("Player"); // 나중에는 서버에서 명령을 받아서 해당 prefab을 불러오는 것으로 변경
        mesh = customObj.GetComponent<Mesh>();
        smr = customObj.GetComponent<SkinnedMeshRenderer>();
        material = smr.material;
    }

    void Update()
    {
        //SkinnedMeshRenderer.SetBlendShapeWeight(조절할 BlendShape의 인덱스 번호, 해당하는 슬라이더의 value);
        smr.SetBlendShapeWeight(0, slider1.value);
        smr.SetBlendShapeWeight(1, slider2.value);
        material.color = fcp.color;
    }
}
