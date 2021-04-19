using System.Collections;
using System.Collections.Generic;
using RDG.PuckGame.Scripts;
using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public static class DrawGameArea {

  static DrawGameArea() {
    SceneView.duringSceneGui += Draw;
  }
  private static void Draw(SceneView _) {
    DrawSpawnPoint(Object.FindObjectOfType<GameSpawnPointBeh>());
    DrawTargetPoint(Object.FindObjectOfType<GameTargetPointBeh>());
    
    var area = Object.FindObjectOfType<GameAreaBeh>();
    if (area == null || !area.isActiveAndEnabled) {
      return;
    }
    
    var center = area.transform.position;
    DrawBox(center, area.Size, Color.magenta);
    
    var crossPoint = Object.FindObjectOfType<GameCrossPointBeh>();
    if (crossPoint == null || !crossPoint.isActiveAndEnabled) {
      return;
    }
    
    var crossPosition = crossPoint.transform.position;
    DrawHorizontalLine(new Vector2(center.x, crossPosition.y), area.Size.x, Color.blue);
  }

  private static void DrawTargetPoint(GameTargetPointBeh targetPoint) {
    if (targetPoint == null) {
      return;
    }
    
    DrawBox(targetPoint.transform.position, Vector2.one * Mathf.Max(targetPoint.Radius*2.0f, 0.01f), Color.red);
  }

  private static void DrawSpawnPoint(GameSpawnPointBeh spawnPoint) {
    if (spawnPoint == null) {
      return;
    }
    DrawBox(spawnPoint.transform.position, Vector2.one * Mathf.Max(spawnPoint.Size*2.0f, 0.01f), Color.green);
  }

  private static void DrawHorizontalLine(Vector2 center, float size, Color color) {
    var halfSize = size / 2.0f;
    Debug.DrawLine(
      new Vector3(center.x - halfSize, center.y, 0.0f),
      new Vector3(center.x + halfSize, center.y, 0.0f),
      color
    );
  }

  private static void DrawBox(Vector2 center, Vector2 size, Color color) {
    var halfSize = size / 2.0f;
    var pts = new[]{
      new Vector3(center.x - halfSize.x, center.y - halfSize.y, 0.0f),
      new Vector3(center.x + halfSize.x, center.y - halfSize.y, 0.0f),
      new Vector3(center.x + halfSize.x, center.y + halfSize.y, 0.0f),
      new Vector3(center.x - halfSize.x, center.y + halfSize.y, 0.0f),
    } ;
    for (var i = 0; i < pts.Length; i++) {
      var first = pts[i];
      var second = i+1 == pts.Length ? pts[0] : pts[i + 1];
      Debug.DrawLine(first, second, color);
    }
  }
}
