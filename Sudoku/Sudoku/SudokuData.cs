using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
{
    public class SudokuData
    {
        /// <summary>
        /// Next possible value is nValue + 1 at field arData[x,y]
        /// </summary>
        public int[,] arData = new int[9, 9];
        public int x;
        public int y;
        public int nValue;

        public SudokuData()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    arData[i, j] = 0;
                }
            }
        }

        public SudokuData(SudokuData other)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    arData[i, j] = other.arData[i, j];
                }
            }

            x = other.x;
            y = other.y;
            nValue = other.nValue;
        }
    }
}
