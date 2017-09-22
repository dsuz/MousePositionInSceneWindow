using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// シーンビューでマウスの位置にコライダーを含むオブジェクトがあった場合はその座標をシーンビュー上に表示してその位置にマークを表示する。
/// MenuItem によりメニューからオンオフを切り替えることができる。
/// </summary>
public class MousePointerPositionInSceneView
{
    /// <summary>機能のアクティブ化フラグ</summary>
    private static bool m_enabled = false;
    
    /// <summary>マウス位置をカメラからどれくらい離すかを定義する。大きすぎると近くに見えるオブジェクトを検出できなくなる。</summary>
    private static float m_zAxisOffset = .3f;
    
    /// <summary>レイキャストを飛ばす距離。大きくすると遠いオブジェクトも検出するが重くなる。</summary>
    private static float m_raycastDistance = 100f;

    /// <summary>
    /// 機能のオンオフを切り替える。
    /// </summary>
    [MenuItem("Window/Show mouse pointer position in Scene")]
    public static void SwitchFunction()
    {
        m_enabled = !m_enabled;

        if (m_enabled)
        {
            SceneView.onSceneGUIDelegate -= OnScene;
            SceneView.onSceneGUIDelegate += OnScene;
            Debug.Log("MousePointerPositionInSceneView enabled.");
        }
        else
        {
            SceneView.onSceneGUIDelegate -= OnScene;
            Debug.Log("MousePointerPositionInSceneView disabled.");
        }
    }

    /// <summary>
    /// 機能が有効の時に随時実行される処理。
    /// シーンビューのカメラからマウス位置の方向にレイキャストして、オブジェクトにぶつかったらぶつかった位置にマークを表示し、そのの座標をシーンビューに表示する。
    /// </summary>
    /// <param name="sceneview"></param>
    private static void OnScene(SceneView sceneview)
    {
        // マウスポインタの位置を取得する。
        Vector3 mousePosition = Event.current.mousePosition;

        // マウスポインタの位置を空間座標に変換する。
        mousePosition.z = m_zAxisOffset;    // カメラから少し離す
        mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
        mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);

        Vector3 cameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;

        // カメラ位置からマウスポインタの方向にレイキャストする。
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(cameraPosition, m_raycastDistance * (mousePosition - cameraPosition).normalized, out hit))
        {
            // レイキャストがヒットしたら、ヒットした位置にマーク（球）を表示する。
            Handles.SphereHandleCap(0, hit.point, Quaternion.identity, .3f, EventType.Repaint);

            // ヒットした位置の座標をシーンビュー上の GUI に表示する。
            Handles.BeginGUI();
            string message = "Hit Position (3D): " + hit.point;
            GUILayout.Label(message);
            Handles.EndGUI();
        }
    }
}
