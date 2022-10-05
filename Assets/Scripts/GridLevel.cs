using System;
using UnityEngine;
using UnityEngine.UI;

public enum eGridType { None = 0, Tree, Grass }

public class GridLevel : MonoBehaviour
{
    // we are considering the grid size is 6x8 
    private readonly int _width = 6;
    private readonly int _height = 8;
    private Grid[,] _puzzleGrid;

    public ScrollRect ScrollRect;  
    public GameObject GridPrefab;

    public Sprite TreeSprite;
    public Sprite GrassSprite;

    private int EachGridTypeCountInWidth => _width / 2;
    private int EachGridTypeCountInHeight => _height / 2;

    private void Start()
    {
        _puzzleGrid = new Grid[_width, _height];
        CreateGridArea();
        GenerateSolution();
    }

    private void CreateGridArea()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                GameObject gridObject = Instantiate(GridPrefab, ScrollRect.content);
                Grid grid = gridObject.AddComponent<Grid>();
                _puzzleGrid[x, y] = grid;
                grid.Init(x, y);
            }
        }
    }

    private void GenerateSolution()
    {
        for(int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Grid currentGrid = _puzzleGrid[x, y];

                // left grid
                eGridType leftGridType = GetLeftGridType(x, y);
                bool isCurrentGridSet = SetCurrentGrid(currentGrid, leftGridType);
                if (isCurrentGridSet) continue;

                // top grid
                eGridType topGridType = GetTopGridType(x, y);
                isCurrentGridSet = SetCurrentGrid(currentGrid, topGridType);
                if (isCurrentGridSet) continue;

                // check number of each grid type on left side
                eGridType leftSideFilledGridType = GetLeftSidesFilledGridType(x, y);
                isCurrentGridSet = SetCurrentGrid(currentGrid, leftSideFilledGridType);
                if (isCurrentGridSet) continue;

                // check number of each grid type on top side
                (eGridType filledGridType, eGridType preferedGridType) topSideFilledGridType = GetTopSidesFilledGridType(x, y);
                isCurrentGridSet = SetCurrentGrid(currentGrid, topSideFilledGridType.filledGridType);
                if (isCurrentGridSet) continue;

                // check random grid...if there is no prefered grid
                if(topSideFilledGridType.preferedGridType != eGridType.None)
                {
                    currentGrid.Set(topSideFilledGridType.preferedGridType, GetSprite(topSideFilledGridType.preferedGridType));
                }
                else
                {
                    int random = UnityEngine.Random.Range(1, Enum.GetNames(typeof(eGridType)).Length);
                    eGridType gridType = (eGridType)random;
                    currentGrid.Set(gridType, GetSprite(gridType));
                }
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

    private eGridType GetLeftGridType(int x, int y)
    {
        int leftGridIndex = x;
        int treeGridCount = 0;
        int grassGridCount = 0;

        for(int i=0; i<2; i++)
        {
            leftGridIndex -= 1;
            if (leftGridIndex < 0)
                return eGridType.None;

            Grid leftGrid = _puzzleGrid[leftGridIndex, y];
            if(leftGrid.GridType == eGridType.None)
                return eGridType.None;

            if(leftGrid.GridType == eGridType.Tree)
                treeGridCount++;
            else if(leftGrid.GridType == eGridType.Grass)
                grassGridCount++;
        }

        return treeGridCount >= 2 ? eGridType.Tree : grassGridCount >= 2 ? eGridType.Grass : eGridType.None;
    }

    private eGridType GetTopGridType(int x, int y)
    {
        int topGridIndex = y;
        int treeGridCount = 0;
        int grassGridCount = 0;

        for (int i = 0; i < 2; i++)
        {
            topGridIndex -= 1;
            if (topGridIndex < 0)
                return eGridType.None;

            Grid topGrid = _puzzleGrid[x, topGridIndex];
            if (topGrid.GridType == eGridType.None)
                return eGridType.None;

            if (topGrid.GridType == eGridType.Tree)
                treeGridCount++;
            else if (topGrid.GridType == eGridType.Grass)
                grassGridCount++;
        }

        return treeGridCount >= 2 ? eGridType.Tree : grassGridCount >= 2 ? eGridType.Grass : eGridType.None;
    }

    private eGridType GetLeftSidesFilledGridType(int x, int y)
    {
        // left side
        int leftSideTreeCount = 0;
        int leftSideGrassCount = 0;
        for(int i = 0; i < x; i++)
        {
            Grid grid = _puzzleGrid[i, y];
            if (grid.GridType == eGridType.None)
                return eGridType.None;

            if (grid.GridType == eGridType.Tree)
                leftSideTreeCount++;
            else if (grid.GridType == eGridType.Grass)
                leftSideGrassCount++;
        }

        return leftSideTreeCount >= EachGridTypeCountInWidth ? eGridType.Tree : leftSideGrassCount >= EachGridTypeCountInWidth ? eGridType.Grass : eGridType.None;
    }

    private (eGridType filledGridType, eGridType preferedGridType)  GetTopSidesFilledGridType(int x, int y)
    {
        // top side
        int topSideTreeCount = 0;
        int topSideGrassCount = 0;
        for (int i = 0; i < y; i++)
        {
            Grid grid = _puzzleGrid[x, i];
            if (grid.GridType == eGridType.None)
                return (eGridType.None, eGridType.None);

            if (grid.GridType == eGridType.Tree)
                topSideTreeCount++;
            else if (grid.GridType == eGridType.Grass)
                topSideGrassCount++;
        }

        return (topSideTreeCount >= EachGridTypeCountInHeight ? eGridType.Tree : 
                topSideGrassCount >= EachGridTypeCountInHeight ? eGridType.Grass : 
                eGridType.None, GetPreferedGridType(topSideTreeCount, topSideGrassCount));
    }

    private eGridType GetPreferedGridType(int treeCount, int grassCount)
    {
        return treeCount > grassCount ? eGridType.Grass : grassCount > treeCount ? eGridType.Tree : eGridType.None;
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
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _puzzleGrid[x, y].Set(eGridType.None, null);
            }
        }
    }
}