﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Query;
using Leap.Unity.GraphicalRenderer;

public class ExampleArrayController : MonoBehaviour {

  [SerializeField]
  private AnimationCurve _blendShapeCurve;

  [SerializeField]
  private float _cubeRise = 0.1f;

  [SerializeField]
  private Gradient _gradient;

  private List<LeapGraphic> _graphics = new List<LeapGraphic>();
  private List<Vector3> _originalPositions = new List<Vector3>();
  private List<LeapBlendShapeData> _blendShapeData = new List<LeapBlendShapeData>();
  private List<LeapRuntimeTintData> _tintData = new List<LeapRuntimeTintData>();

  private void Start() {
    _graphics.AddRange(GetComponentsInChildren<LeapGraphic>());
    _graphics.Query().Select(g => g.transform.localPosition).FillList(_originalPositions);
    _graphics.Query().Select(g => g.GetFirstFeatureData<LeapBlendShapeData>()).FillList(_blendShapeData);
    _graphics.Query().Select(g => g.GetFirstFeatureData<LeapRuntimeTintData>()).FillList(_tintData);
  }

  private void Update() {
    Random.InitState(0);

    for (int i = 0; i < _graphics.Count; i++) {
      float d = 10 * noise(_graphics[i].transform.localPosition, 42.0f, 0.8f);
      float s = Mathf.Sin(d);

      float n = noise(_graphics[i].transform.position * 3, 23, 0.35f);
      float bn = _blendShapeCurve.Evaluate(n);

      _blendShapeData[i].amount = bn;
      _graphics[i].transform.localPosition = _originalPositions[i] + Vector3.up * (bn * _cubeRise + d * (bn * 0.03f + 0.01f));
      _tintData[i].color = _gradient.Evaluate(n);
    }
  }

  private float noise(Vector3 offset, float seed, float speed) {

    float x1 = seed * 23.1239879f;
    float y1 = seed * 82.1239812f;
    x1 -= (int)x1;
    y1 -= (int)y1;
    x1 *= 10;
    y1 *= 10;

    float x2 = seed * 23.1239879f;
    float y2 = seed * 82.1239812f;
    x2 -= (int)x2;
    y2 -= (int)y2;
    x2 *= 10;
    y2 *= 10;

    return Mathf.PerlinNoise(offset.x + x1 + Time.time * speed, offset.z + y1 + Time.time * speed) * 0.5f +
           Mathf.PerlinNoise(offset.x + x2 - Time.time * speed, offset.z + y2 - Time.time * speed) * 0.5f;
  }


}
