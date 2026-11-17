using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EditorVoxelPlacerTool : EditorTool
{
    [Shortcut("Activate Voxel Placement Tool", typeof(SceneView))]

    void OnEnable()
    {
        // Allocate unmanaged resources or perform one-time set up functions here
    }

    void OnDisable()
    {
        // Free unmanaged resources, state teardown.
    }

    static void VoxelPlacerToolShortcut()
    {
        if (Selection.GetFiltered<GameObject>(SelectionMode.TopLevel).Length > 0)
        {
            ToolManager.SetActiveTool<EditorVoxelPlacerTool>();

        }
        else
        {
            Debug.Log("No object selected");
        }
    }

    private void Update()
    {
        Debug.Log("update");
    }

    public override void OnActivated()
    {
        SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Entering Voxel Placement Tool"), .1f);
    }

    public override void OnWillBeDeactivated()
    {
        SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Exiting Voxel Placement Tool"), .1f);
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if (!(window is SceneView sceneView))
        {
            return;
        }

        Handles.BeginGUI();
        using (new GUILayout.HorizontalScope())
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {

            }

            GUILayout.FlexibleSpace();
        }
        Handles.EndGUI();
    }
}
