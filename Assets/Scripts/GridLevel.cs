using System;
using UnityEngine;
using UnityEngine.UI;

public enum eGridType { None = 0, Tree, Grass }

public class GridLevel : MonoBehaviour
{
    // we are considering the grid size is 6x8 
    private readonly int _column = 6;
    private readonly int _row = 8;
    private Grid[,] _puzzleGrid;

    public ScrollRect ScrollRect;  
    public GameObject GridPrefab;

    public Sprite TreeSprite;
    public Sprite GrassSprite;

    private int EachGridTypeCountInColumn => _column / 2;
    private int EachGridTypeCountInRow => _row / 2;

    private void Start()
    {
        _puzzleGrid = new Grid[_column, _row];
        CreateGridArea();
        //GenerateSolution();
    }

    private void CreateGridArea()
    {
        for(int rowIndex = 0; rowIndex < _row; rowIndex++)
        {
            for(int columnIndex = 0; columnIndex < _column; columnIndex++)
            {
                GameObject gridObject = Instantiate(GridPrefab, ScrollRect.content);
                Grid grid = gridObject.AddComponent<Grid>();
                _puzzleGrid[columnIndex, rowIndex] = grid;
                grid.Init(rowIndex, columnIndex);
            }
        }
    }

    private void GenerateSolution()
    {
        for(int rowIndex = 0; rowIndex < _row; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < _column; columnIndex++)
            {
                Grid currentGrid = _puzzleGrid[columnIndex, rowIndex];

                // left grid
                eGridType leftGridType = GetLeftGridType(columnIndex, rowIndex);
                bool isCurrentGridSet = SetCurrentGrid(currentGrid, leftGridType);
                if (isCurrentGridSet) continue;

                // bottom
                eGridType bottomGridType = GetBottomGridType(columnIndex, rowIndex);
                isCurrentGridSet = SetCurrentGrid(currentGrid, bottomGridType);
                if (isCurrentGridSet) continue;

                // check number of each grid type on left side
                eGridType leftSideFilledGridType = GetLeftSidesFilledGridType(columnIndex, rowIndex);
                isCurrentGridSet= SetCurrentGrid(currentGrid, leftSideFilledGridType);
                if(isCurrentGridSet) continue;


                // check number of each grid type on right side

                int random = UnityEngine.Random.Range(1, Enum.GetNames(typeof(eGridType)).Length);
                eGridType gridType = (eGridType)random;
                currentGrid.Set(gridType, GetSprite(gridType));
            }
        }
    }

    private bool SetCurrentGrid(Grid currentGrid, eGridType lastGridType)
    {
        switch (lastGridType)
        {
            case eGridType.None:
                return false;
            case eGridType.Tree:
                currentGrid.Set(eGridType.Grass, GetSprite(eGridType.Grass));
                return true;
            case eGridType.Grass:
                currentGrid.Set(eGridType.Tree, GetSprite(eGridType.Tree));
                return true;
        }

        return false;
    }

    private eGridType GetLeftGridType(int columnIndex, int rowIndex)
    {
        int leftGridIndex = rowIndex;
        int treeGridCount = 0;
        int grassGridCount = 0;
        for(int i=0; i<2; i++)
        {
            leftGridIndex -= 1;
            if (leftGridIndex < 0)
                return eGridType.None;

            Grid leftGrid = _puzzleGrid[columnIndex, leftGridIndex];
            if(leftGrid.GridType == eGridType.None)
                return eGridType.None;

            if(leftGrid.GridType == eGridType.Tree)
                treeGridCount++;
            else if(leftGrid.GridType == eGridType.Grass)
                grassGridCount++;
        }

        return treeGridCount >= 2 ? eGridType.Tree : grassGridCount >= 2 ? eGridType.Grass : eGridType.None;
    }

    private eGridType GetBottomGridType(int columnIndex, int rowIndex)
    {
        int bottomGridIndex = columnIndex;
        int treeGridCount = 0;
        int grassGridCount = 0;

        for (int i = 0; i < 2; i++)
        {
            bottomGridIndex -= 1;
            if (bottomGridIndex < 0)
                return eGridType.None;

            Grid bottomGrid = _puzzleGrid[bottomGridIndex, rowIndex];
            if (bottomGrid.GridType == eGridType.None)
                return eGridType.None;

            if (bottomGrid.GridType == eGridType.Tree)
                treeGridCount++;
            else if (bottomGrid.GridType == eGridType.Grass)
                grassGridCount++;
        }

        return treeGridCount >= 2 ? eGridType.Tree : grassGridCount >= 2 ? eGridType.Grass : eGridType.None;
    }

    private eGridType GetLeftSidesFilledGridType(int rowIndex, int columnIndex)
    {
        // left side
        int leftSideTreeCount = 0;
        int leftSideGrassCount = 0;
        for(int i=0; i<columnIndex; i++)
        {
            Grid grid = _puzzleGrid[rowIndex, i];
            if (grid.GridType == eGridType.None)
                return eGridType.None;

            if (grid.GridType == eGridType.Tree)
                leftSideTreeCount++;
            else if (grid.GridType == eGridType.Grass)
                leftSideGrassCount++;
        }

        return leftSideTreeCount >= EachGridTypeCountInColumn ? eGridType.Tree : leftSideGrassCount >= EachGridTypeCountInColumn ? eGridType.Grass : eGridType.None;
    }

    private eGridType GetBottomSidesFilledGridType(int rowIndex, int columnIndex)
    {
        // bottom side
        int bottomSideTreeCount = 0;
        int bottomSideGrassCount = 0;
        for (int i = 0; i < rowIndex; i++)
        {
            Grid grid = _puzzleGrid[i, columnIndex];
            if (grid.GridType == eGridType.None)
                return eGridType.None;

            if (grid.GridType == eGridType.Tree)
                bottomSideTreeCount++;
            else if (grid.GridType == eGridType.Grass)
                bottomSideGrassCount++;
        }

        return bottomSideTreeCount >= EachGridTypeCountInColumn ? eGridType.Tree : bottomSideGrassCount >= EachGridTypeCountInColumn ? eGridType.Grass : eGridType.None;
    }

    private Sprite GetSprite(eGridType gridType)
    {
        switch (gridType)
        {
            case eGridType.Tree:
                return TreeSprite;
            case eGridType.Grass:
                return GrassSprite;
        }

        return null;
    }

    public void OnClickRandomButton()
    {
        ResetGrid();
        GenerateSolution();
    }

    private void ResetGrid()
    {
        for (int columnIndex = 0; columnIndex < _row; columnIndex++)
        {
            for (int rowIndex = 0; rowIndex < _column; rowIndex++)
            {
                _puzzleGrid[rowIndex, columnIndex].GridType = eGridType.None;
            }
        }
    }
}