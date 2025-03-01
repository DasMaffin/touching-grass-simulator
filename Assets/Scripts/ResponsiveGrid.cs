using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveGrid : MonoBehaviour
{
    public int columns = 10; // Set your desired number of columns.
    public int rows = 4;    // Set your desired number of rows.

    private GridLayoutGroup gridLayout;

    void Start()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
        if(gridLayout.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            columns = gridLayout.constraintCount;
        }
        else if(gridLayout.constraint == GridLayoutGroup.Constraint.FixedRowCount)
        {
            rows = gridLayout.constraintCount;
        }
        AdjustCellSize();
        Settings.Instance.onChangeResolution += AdjustCellSize;
    }

    void AdjustCellSize()
    {
        RectTransform rt = GetComponent<RectTransform>();
        float width = rt.rect.width;
        float height = rt.rect.height;

        // Calculate cell width and height by subtracting padding and spacing.
        float cellWidth = (width - gridLayout.padding.left - gridLayout.padding.right - (columns - 1) * gridLayout.spacing.x) / columns;
        float cellHeight = (height - gridLayout.padding.top - gridLayout.padding.bottom - (rows - 1) * gridLayout.spacing.y) / rows;

        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
    }
}
