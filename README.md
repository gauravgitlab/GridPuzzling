# GridPuzzling
GridPuzzle Rules

Generate a completed grid of two values/colours/or objects.
In our example these are represented by grass cells and tree cells.
The width and height of the grid must be an even number size e.g. 6x8, 10x10
In each row and column there must not be more than a run of 2 of the same object. Diagonals are not a concern.
The amount of each type must be equal in each row and in each column.
Thus if the grid is 6 wide, then three trees and three grass must exist in every one.
If the height is different from the width, that column will have a different amount compared to the width, but it still must have an equal number of grass and trees.

An example of a solution...

<img width="288" alt="image001" src="https://user-images.githubusercontent.com/289480/194292174-95db3bdc-2d29-41de-9df5-7c1668d0539e.png">


# Solution
we are considering the grid area from upper left corner and draw the grid based on x and y
    
                         ^
                         | 
                         |
                     -----------------------------------------------------
             <-----  |         |        |        |       |       |        |
                     |         |        |        |       |       |        |
                     |  0x0    |  1x0   |  2x0   |  3x0  |  4x0  |  5x0   |
                     -----------------------------------------------------
                     |         |        |        |       |       |        |
                     |         |        |        |       |       |        |
                     |  0x1    |  1x1   |  2x1   |  3x1  |  4x1  |  5x1   |
                     -----------------------------------------------------
                     
here, we are starting from the grid 0x0, and continue from here to next grid 1x0... 2x0... and so on
In that case, we are considering for left and top grids to get the gridType as bottom and right grids are not supposed to be set yet and give GridType.None only.

![solution window](https://user-images.githubusercontent.com/289480/194321744-39ad28e0-ab70-4662-bbb1-c07cab02b9bf.png)
