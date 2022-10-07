/*
 *
 *   GridLevel.cs
 *   Responsible for creating an Grid based on size ( in this case 6x8 ) and generate the solution.
 *   ========================================================================================================================
 *   CreateGridArea() method based on the rule where
 *   The width and height of the grid must be an even number size e.g. 6x8, 10x10
 *   ========================================================================================================================
 *   GenerateSolution() method based on the rules where
 *   In each row and column there must not be more than a run of 2 of the same object. Diagonals are not a concern.
 *   The amount of each type must be equal in each row and in each column.
 *   =========================================================================================================================
 *   we are considering the grid area from upper left corner and draw the grid based on x and y
 *   
 *                       ^
 *                       | 
 *                       |
 *                   -----------------------------------------------------
 *           <-----  |         |        |        |       |       |        |
 *                   |         |        |        |       |       |        |
 *                   |  0x0    |  1x0   |  2x0   |  3x0  |  4x0  |  5x0   |
 *                   -----------------------------------------------------
 *                   |         |        |        |       |       |        |
 *                   |         |        |        |       |       |        |
 *                   |  0x1    |  1x1   |  2x1   |  3x1  |  4x1  |  5x1   |
 *                   -----------------------------------------------------
 *   NOTE : 
 *   We are using here ScrollRect for quick way to positing the grid using Grid Layout Group attached to ScrollRect.Content
 *   If we change the _width and _height, we can easily write one method which can change the ScrollRect Rect Transform width and height...
 *   The alternative approach if we dont want to use ScrollRect is to set the Grid position when we insantiate GridPrefab.
 *   In our case the size of GridPrefab is 100x100. 
 *   we can use the following code
 *   public Vector2 GridSize = new Vector2(100, 100);
 *   GameObject gridObject = Instantiate(GridPrefab, new Vector3(x * GridSize.x, y*GridSize.y), Quaternion.identity, ScrollRect.content);
*/

using System;
using UnityEngine;
using UnityEngine.UI;

public enum eGridType { None = 0, Tree, Grass }

public class GridLevel : MonoBehaviour
{
    // we are considering the grid size is 6x8 
    [SerializeField] private int _width = 10;
    [SerializeField] private int _height = 12;

    private Grid[,] _puzzleGrid;

    public ScrollRect ScrollRect;
    public Text AttemptText;
    public Text StatusText;
    public GameObject GridPrefab;

    public Sprite TreeSprite;
    public Sprite GrassSprite;

    private Vector2 _gridSize = new(100, 100);
    private float _gridOffset = 20f;
    private int EachGridTypeCountInWidth => _width / 2;
    private int EachGridTypeCountInHeight => _height / 2;

    private static int AttemptCount = 0;

    private void Start()
    {
        _puzzleGrid = new Grid[_width, _height];
        ScrollRect.GetComponent<RectTransform>().sizeDelta = 
            new Vector2((_width * _gridSize.x) + _gridOffset, (_height * _gridSize.y) + _gridOffset);
        
        CreateGridArea();

        GenerateSolutionOnly();
    }

    private void GenerateSolutionOnly()
    {
        AttemptCount = 0;

        bool isSolutionValid = GenerateSolution();
        AttemptText.text = $"Attempt : {AttemptCount}";
        StatusText.text = isSolutionValid ? $"Status : Success" : $"Status : Failed";
    }


    /// <summary>
    /// Generate solution recursively until find the solution which meet the requirement
    /// </summary>
    private void GenerateSolutionRecursively()
    {
        bool isSolutionValid = GenerateSolution();

        if (!isSolutionValid)
        {
            GenerateSolutionRecursively(); 
        }
        else
        {
            AttemptText.text = $"Attempt : {AttemptCount}";
            StatusText.text = $"Status : Success";
            AttemptCount = 0;
        }
    }


    /// <summary>
    /// Just creating an Grid Area, and filling the puzzleGrid 2d Array. 
    /// </summary>
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

    /// <summary>
    /// Generating a solution based on rules where each row and column there must not be more than a run of 2 of the same object. Diagonals are not a concern.
    /// and amount of each type must be equal in each row and in each column.
    /// here, we are starting from the grid 0x0, and continue from here to next grid 1x0... 2x0... and so on
    /// In that case, we are considering for left and top grids to get the gridType as bottom and right grids are not supposed to be set yet and give GridType.None only.
    /// </summary>
    private bool GenerateSolution()
    {
        AttemptCount++;

        for (int y = 0; y < _height; y++)
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

                // check number of each gridType on left side
                (eGridType filledGridType, eGridType preferedGridType) leftSideFilledGridType = GetLeftSidesFilledGridType(x, y);
                isCurrentGridSet = SetCurrentGrid(currentGrid, leftSideFilledGridType.filledGridType);
                if (isCurrentGridSet) continue;

                // check number of each gridType on top side
                (eGridType filledGridType, eGridType preferedGridType) topSideFilledGridType = GetTopSidesFilledGridType(x, y);
                isCurrentGridSet = SetCurrentGrid(currentGrid, topSideFilledGridType.filledGridType);
                if (isCurrentGridSet) continue;

                // check random grid...if there is no prefered grid
                if(leftSideFilledGridType.preferedGridType == eGridType.None && topSideFilledGridType.preferedGridType == eGridType.None)
                {
                    int random = UnityEngine.Random.Range(1, Enum.GetNames(typeof(eGridType)).Length);
                    eGridType gridType = (eGridType)random;
                    currentGrid.Set(gridType, GetSprite(gridType));
                }
                else if (leftSideFilledGridType.preferedGridType == topSideFilledGridType.preferedGridType)
                {
                    currentGrid.Set(leftSideFilledGridType.preferedGridType, GetSprite(leftSideFilledGridType.preferedGridType));
                }
                else
                {
                    if (leftSideFilledGridType.preferedGridType == eGridType.None && topSideFilledGridType.preferedGridType != eGridType.None)
                        currentGrid.Set(topSideFilledGridType.preferedGridType, GetSprite(topSideFilledGridType.preferedGridType));
                    else if(topSideFilledGridType.preferedGridType == eGridType.None && leftSideFilledGridType.preferedGridType != eGridType.None)
                        currentGrid.Set(leftSideFilledGridType.preferedGridType, GetSprite(leftSideFilledGridType.preferedGridType));
                    else
                    {
                        int random = UnityEngine.Random.Range(1, Enum.GetNames(typeof(eGridType)).Length);
                        eGridType gridType = (eGridType)random;
                        currentGrid.Set(gridType, GetSprite(gridType));
                    }
                }
            }
        }

        return IsSolutionValid();
    }

    /// <summary>
    /// check and return true or false if solution match equal numbers of each gridTypes or not
    private bool IsSolutionValid()
    {
        // check if all width sides have equal number of gridTypes
        int treeInWidthCount = 0;
        int grassInWidthCount = 0;

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Grid grid = _puzzleGrid[x, y];
                switch (grid.GridType)
                {
                    case eGridType.Tree:
                        treeInWidthCount++; break;
                    case eGridType.Grass:
                        grassInWidthCount++; break;
                }
            }

            if(treeInWidthCount != EachGridTypeCountInWidth || grassInWidthCount != EachGridTypeCountInWidth)
                return false;

            treeInWidthCount = 0;
            grassInWidthCount = 0;
        }

        // check if all height sides have equal number of gridTyes
        int treeInHeightCount = 0;
        int grassInHeightCount = 0;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Grid grid = _puzzleGrid[x, y];
                switch (grid.GridType)
                {
                    case eGridType.Tree:
                        treeInHeightCount++; break;
                    case eGridType.Grass:
                        grassInHeightCount++; break;
                }
            }

            if (treeInHeightCount != EachGridTypeCountInHeight || grassInHeightCount != EachGridTypeCountInHeight)
                return false;

            treeInHeightCount = 0;
            grassInHeightCount = 0;
        }

        return true;
    }

    /// <summary>
    /// Set the current Grid, based on the result the last gridType we had on our search,
    /// so, if we can find that 2 tiles on the left or top are tree, the current grid would be Grass or vice versa.
    /// </summary>
    /// <param name="currentGrid"></param>
    /// <param name="lastGridType"></param>
    /// <returns></returns>
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


    /// <summary>
    /// Get the gridType on the left side of current grid (x, y)
    /// x and y are the current grid indexes from _puzzleGrid.
    /// It will return the gridType based on last 2 grid type we have on left side of current grid.
    /// </summary>
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


    /// <summary>
    /// Get the gridType on the top side of current grid (x, y)
    /// x and y are the current grid indexes from _puzzleGrid.
    /// It will return the gridType based on last 2 grid type we have on top side of current grid.
    /// </summary>
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


    /// <summary>
    /// Get the gridType on left side of current grid which count is already equal to half of the width.
    /// In our case the width = 6, so return GridType.Tree if treeCount >= 3 or GridType.Grass if grassCount >= 3
    /// </summary>
    private (eGridType filledGridType, eGridType preferedGridType) GetLeftSidesFilledGridType(int x, int y)
    {
        // left side
        int leftSideTreeCount = 0;
        int leftSideGrassCount = 0;
        for(int i = 0; i < x; i++)
        {
            Grid grid = _puzzleGrid[i, y];
            if (grid.GridType == eGridType.None)
                return (eGridType.None, eGridType.None);

            if (grid.GridType == eGridType.Tree)
                leftSideTreeCount++;
            else if (grid.GridType == eGridType.Grass)
                leftSideGrassCount++;
        }

        return (leftSideTreeCount >= EachGridTypeCountInWidth ? eGridType.Tree : 
                leftSideGrassCount >= EachGridTypeCountInWidth ? eGridType.Grass : 
                eGridType.None, GetPreferedGridType(leftSideTreeCount, leftSideGrassCount));
    }


    /// <summary>
    /// Get the gridType on top side of current grid which count is already equal to half of the height.
    /// In our case the height = 8, so return GridType.Tree if treeCount >= 4 or GridType.Grass if grassCount >= 4 
    /// </summary>
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


    // return the sprite based on grid type
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

    /// <summary>
    /// random solution button..
    /// It will generate solution again
    /// set the gridType to None for each grid before genrating the solution.
    /// </summary>
    public void OnClickRandomButton()
    {
        ResetGrid();
        GenerateSolutionOnly();    
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