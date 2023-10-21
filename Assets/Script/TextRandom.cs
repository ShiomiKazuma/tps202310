using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextRandom : MonoBehaviour
{
    public GameObject _textPos;

    public float _shakePower = 5.0f;
    Vector3 _textInitPos;
    void Start()
    {
        _textInitPos = _textPos.transform.position;
        _textPos.SetActive(false);
    }

    void Update()
    {
        _textPos.transform.position = _textInitPos + Random.insideUnitSphere * _shakePower;
    }

}
