using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
{
    public delegate void SolutionEventHandler(object sender, SolutionEventArgs args);

    /// <summary>
    /// At level <para>step</para> set value <para>value</para>
    /// at position x/y
    /// If final = true then this is part of the solution
    /// </summary>
    public class SolutionEventArgs : EventArgs
    {
        public int step;
        public int value;
        public int x;
        public int y;
        public bool final;
        public bool bDeadEnd;

        public SolutionEventArgs(int nStep, bool final, int nValue, int nX, int nY)
        {
            this.x = nX;
            this.y = nY;
            this.value = nValue;
            this.step = nStep;
            this.final = final;
        }
        public SolutionEventArgs(bool bDeadEnd)
        {
            this.bDeadEnd = bDeadEnd;
        }
    }

    public class Sudoku
    {
        private List<SudokuData> list = new List<SudokuData>();
        private SudokuData current;

        public event SolutionEventHandler Solution;

        public Sudoku()
        {
            list.Add(current = new SudokuData());
        }

        public int getValue(int i, int j)
        {
            return current.arData[i, j];
        }

        public int setValue(int nValue, int i, int j)
        {
            int nTemp = list[list.Count - 1].arData[i, j];
            list[list.Count - 1].arData[i, j] = nValue;

            return nTemp;
        }

        /// <summary>
        /// In a 9x9 square get the left upper corner of the 3x3 square
        /// in which (i,j) lies.
        /// <example>
        /// (0,0)..(2,2) -> (0,0)
        /// (6,3), (6,4), (6, 5)... (8,3), (8,4),(8,5) -> (6, 3)
        /// </example>
        /// </summary>
        /// <param name="i">x Coord</param>
        /// <param name="j">y Coord</param>
        /// <returns></returns>
        private Position GetLeftUpperPosition(int i, int j)
        {
            Position pos = new Position();

            pos.x = (int) ((i / 3) * 3);
            pos.y = (int) ((j / 3) * 3);

            return pos;
        }

        /// <summary>
        /// Returns the list of possible values at field (x,y)
        /// </summary>
        /// <param name="x">x-Coord</param>
        /// <param name="y">y-Coord</param>
        /// <returns></returns>
        public List<int> PossibleValues(int x, int y)
        {
            int i;
            // arValues[i] = true means that i + 1 is a valid value at position (x,y)
            bool[] arValues = new bool[9];

            // All values are initially valid. Remove the ones that turn out to be bad at this position.
            for (i = 0; i < 9; i++)
            {
                arValues[i] = true;
            }

            // check row
            for (i = 0; i < 9; i++)
            {
                if (i != x)
                {
                    if (getValue(i, y) != 0)
                    {
                        // If a field in this row other than at column x contains a number,
                        // than that number cannot appear again in this row
                        arValues[getValue(i, y) - 1] = false;
                    }
                }
            }

            // check column
            for (i = 0; i < 9; i++)
            {
                if (i != y)
                {
                    if (getValue(x, i) != 0)
                    {
                        // If a field in this column other than at row y contains a number,
                        // than that number cannot appear again in this column
                        arValues[getValue(x, i) - 1] = false;
                    }
                }
            }

            // check 3x3 grid
            Position pos = GetLeftUpperPosition(x, y);
            for (i = pos.x; i < pos.x + 3; i++)
            {
                for (int j = pos.y; j < pos.y + 3; j++)
                {
                    if (i != x && j != y)
                    {
                        if (getValue(i, j) != 0)
                        {
                            // A number cannot be in one of the 3x3 grids more than once
                            arValues[getValue(i, j) - 1] = false;
                        }
                    }
                }
            }

            // Now transform the array into a list of numbers that are valid
            List<int> intList = new List<int>();

            for (i = 0; i < 9; i++)
            {
                if (arValues[i] == true)
                {
                    intList.Add((int)(i + 1));
                }
            }

            return intList;
        }

        /// <summary>
        /// Return whether all fields contain a number. 
        /// Doesn't check whether those numbers are really valid.
        /// </summary>
        /// <returns>true when you are done.</returns>
        private bool Done()
        {
            bool bDone = true;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (getValue(i, j) == 0)
                    {
                        bDone = false;
                        goto Exit;
                    }
                }
            }
        Exit:
            return bDone;
        }

        /// <summary>
        /// Find the next free position starting in the current grid
        /// at position (x,y)
        /// </summary>
        /// <param name="x">x-Coord</param>
        /// <param name="y">y-Coord</param>
        /// <returns></returns>
        private bool FindNextFreeField(ref int x, ref int y)
        {
            bool fFound = false;

            for (int i = x; i < 9; i++)
            {
                for (int j = y; j < 9; j++)
                {
                    if (getValue(i, j) == 0)
                    {
                        x = i;
                        y = j;
                        fFound = true;
                        goto Exit;
                    }
                }
            }
        
            Exit:
            return fFound;
        }

        /// <summary>
        /// Callback for a valid entry or a guess. 
        /// Here, we draw the number into our little puzzle.
        /// </summary>
        /// <param name="final">Is this a guess or a valid move?</param>
        /// <param name="value">The number</param>
        /// <param name="x">x-Coord</param>
        /// <param name="y">y-Coord</param>
        private void OnSolution(bool final, int value, int x, int y)
        {
            if (this.Solution != null)
            {
                Solution(this, new SolutionEventArgs(list.Count, final, value, x, y));
            }
            setValue(value, x, y);
        }
        private void OnDeadEnd()
        {
            if (this.Solution != null)
            {
                Solution(this, new SolutionEventArgs(true));
            }
        }

        /// <summary>
        /// In the current grid, at position (current.x, current.y), find the first
        /// valid of the numbers 1..9 at this position, starting with current.nValue + 1,
        /// Given position (x,y) and a value n, the next guess is n+1 at position (x,y).
        /// </summary>
        /// <returns></returns>
        private bool GenerateGuess()
        {
            bool fFound = false;

            for (int t = current.nValue + 1; t < 10; t++)
            {
                if (IsValidForField(t, current.x, current.y))
                {
                    /// We try number t at position (current.x, current.y)
                    /// and continue until we either come to a dead end or solve all.
                    OnSolution(false, t, current.x, current.y);
                    current.nValue = t;
                    fFound = true;
                    break;
                }
            }

            return fFound;
        }

        /// <summary>
        /// There was a field, that cannot contain any number without violation a rule.
        /// So we pop this grid until we come back to one in which there is a next possible move.
        /// </summary>
        /// <returns></returns>
        private bool DeadEnd()
        {
            bool fSuccess = true;

            do
            {
                if (list.Count > 1)
                {
                    list.Remove(current);
                    current = list[list.Count - 1];
                    OnDeadEnd();
                }
                else
                {
                    fSuccess = false;
                    break;
                }
            } while (!GenerateGuess());

            return fSuccess;
        }

        public void Solve()
        {
            // Solve the entire puzzle by trying until there is no possible move.
            // This means failure or success.
            while (NextTry())
            {
            }
        }

        /// <summary>
        /// Find the next valid move. If there is none, generate a valid guess.
        /// This guess iterates through all possible numbers at all free position in a 
        /// reproducable way.
        /// We than push the entire grid with this guess and continue with this new grid.
        /// If we come to a dead end and after popping this grid is the current once more, then 
        /// the next guess on this same grid will continue trying with the next higher number up to 9 
        /// at the same position. If all numbers on this field fail, we find the next free field 
        /// and start trying all numbers on that field...
        /// </summary>
        /// <returns></returns>
        public bool NextTry()
        {
            bool fSuccess = false;
            int x = 0;
            int y = 0;

            if (OneStep())
            {
                // There was one move that is final or no one number that can be in only one square
                fSuccess = true;
            }
            else
            {
                if (!Done())
                {
                    // still more to do
                    if (FindNextFreeField(ref x, ref y))
                    {
                        // this is the next free position
                        current.x = x;
                        current.y = y;
                        current.nValue = 0;

                        if (GenerateGuess())
                        {
                            // Try the first possible number at this position,
                            // then push this whole state.
                            // The next move will continue working with this new state
                            list.Add(current = new SudokuData(current));
                            fSuccess = true;
                        }
                        else
                        {
                            // There is no possible move at this position,
                            // so this state is bad.
                            // Pop this state and toss it, and continue with the previous 
                            // one, and try the next higher number at that previous position.
                            fSuccess = DeadEnd();
                        }
                    }
                    else
                    {
                        fSuccess = DeadEnd();
                    }
                }
            }

            return fSuccess;
        }

        /// <summary>
        /// Find one final entry
        /// </summary>
        /// <returns>true if there was a number that must be in certain position</returns>
        private bool OneStep()
        {
            int nValue = 0;
            int x = 0;
            int y = 0;

            bool fFound = false;

            // Check every single position whether there may be only one number at that position
            for (x = 0; x < 9; x++)
            {
                for (y = 0; y < 9; y++)
                {
                    if (getValue(x, y) == 0)
                    {
                        List<int> arValues = this.PossibleValues(x, y);

                        if (arValues.Count == 1)
                        {
                            // If only one number can be at a given position,
                            // then that is part of the solution
                            nValue = arValues[0];
                            fFound = true;
                            goto Exit;
                        }
                    }
                }
            }

            // For every row: Is there a number that can be at only one position in this row?
            if (this.CheckRows(ref nValue, ref x, ref y))
            {
                fFound = true;
            }
            // For every column: Is there a number that can be at only one position in this column?
            else if (this.CheckColumns(ref nValue, ref x, ref y))
            {
                fFound = true;
            }
            // For every 3x3 square: Is there a number that can be at only one position in this 3x3 square?
            else if (this.Check3x3Squares(ref nValue, ref x, ref y))
            {
                fFound = true;
            }

        Exit:
            if (fFound)
            {
                // Notify whoever wants to know that there is another valid number.
                OnSolution(true, nValue, x, y);
            }
            return fFound;
        }

        /// <summary>
        /// Is nValue a valid value at position (x, y)?
        /// nValue may appear only once in every column, row and 3x3 square
        /// </summary>
        /// <param name="nValue">The number</param>
        /// <param name="x">x-coord</param>
        /// <param name="y">y-coord</param>
        /// <returns>true if this is so, false otherwise</returns>
        public bool IsValidForField(int nValue, int x, int y)
        {
            int i;
            bool bOk = true;

            // check row
            for (i = 0; i < 9; i++)
            {
                if (i != x)
                {
                    if (getValue(i, y) == nValue)
                    {
                        bOk = false;
                        goto Exit;
                    }
                }
            }

            // check column
            for (i = 0; i < 9; i++)
            {
                if (i != y)
                {
                    if (getValue(x, i) == nValue)
                    {
                        bOk = false;
                        goto Exit;
                    }
                }
            }

            // check 3x3 grid
            Position pos = GetLeftUpperPosition(x, y);
            for (i = pos.x; i < pos.x + 3; i++)
            {
                for (int j = pos.y; j < pos.y + 3; j++)
                {
                    if (i != x && j != y)
                    {
                        if (getValue(i, j) == nValue)
                        {
                            bOk = false;
                            goto Exit;
                        }
                    }
                }
            }
            Exit:
            return bOk;
        }

        /// <summary>
        /// Counts the number of valid positions within a 3x3 square for number nValue.
        /// If the returned count is exactly 1, then x2 and y2
        /// are the position where value nValue must be at.
        /// </summary>
        public int CountValidPositionsIn3x3Square(int nValue, int x, int y, ref int x2, ref int y2)
        {
            int nCount = 0;
            int i;
            int j;

            Position pos = GetLeftUpperPosition(x, y);

            for (i = pos.x; i < pos.x + 3; i++)
            {
                for (j = pos.y; j < pos.y + 3; j++)
                {
                    if (getValue(i, j) == 0)
                    {
                        if (this.IsValidForField(nValue, i, j))
                        {
                            x2 = i;
                            y2 = j;
                            nCount++;
                        }
                    }
                }
            }

            return nCount;
        }

        /// <summary>
        /// Counts the number of valid positions within row y for number nValue.
        /// If the returned count is exactly 1, then x2 is the column within this row
        /// is the position where value nValue must be at.
        /// </summary>
        public int CountValidPositionsInRow(int nValue, int y, ref int x2)
        {
            int nCount = 0;
            int i;

            for (i = 0; i < 9; i++)
            {
                if (getValue(i, y) == 0)
                {
                    if (this.IsValidForField(nValue, i, y))
                    {
                        x2 = i;
                        nCount++;
                    }
                }
            }

            return nCount;
        }

        /// <summary>
        /// Counts the number of valid positions within column x for number nValue.
        /// If the returned count is exactly 1, then y2 is the row within this column
        /// is the position where value nValue must be at.
        /// </summary>
        public int CountValidPositionsInColumn(int nValue, int x, ref int y2)
        {
            int nCount = 0;
            int i;

            for (i = 0; i < 9; i++)
            {
                if (getValue(x, i) == 0)
                {
                    if (this.IsValidForField(nValue, x, i))
                    {
                        y2 = i;
                        nCount++;
                    }
                }
            }

            return nCount;
        }

        /// <summary>
        /// Return a list of all those numbers that do NOT occur in row y
        /// </summary>
        /// <param name="y">The row in question</param>
        /// <returns></returns>
        public List<int> GetRemainingNumbersForRow(int y)
        {
            int i;
            List<int> intList = new List<int>();

            // arValues[i] = true means i is missing in row y
            // Array must be initialized to false
            bool[] arValues = new bool[10];

            // 1 <= getValue() <= 9
            for (i = 0; i < 9; i++)
            {
                // we use the number at position (i,y) as an index into array arValues[]
                // to indicate that this number occurs in row y.
                // We therefore use arValues[1] to arValues[9] and arValues[0] is not used.
                arValues[getValue(i, y)] = true;
            }

            // Transform the array into a list
            for (i = 1; i < 10; i++)
            {
                if (arValues[i] == false)
                {
                    intList.Add(i);
                }
            }

            return intList;
        }

        /// <summary>
        /// See <see cref="GetRemainingNumbersForRow"/>
        /// </summary>
        /// <param name="x">The Column to query</param>
        /// <returns></returns>
        public List<int> GetRemainingNumbersForColumn(int x)
        {
            int i;
            List<int> intList = new List<int>();

            // arValues[i] = true means i occurs in column x
            // Array must be initialized to false
            bool[] arValues = new bool[10];

            for (i = 0; i < 9; i++)
            {
                arValues[getValue(x, i)] = true;
            }

            for (i = 1; i < 10; i++)
            {
                if (arValues[i] == false)
                {
                    intList.Add(i);
                }
            }

            return intList;
        }

        /// <summary>
        /// Return a list of numbers that do NOT occur in the 3x3 Square that contains field (x,y)
        /// </summary>
        /// <param name="x">Any x-coord</param>
        /// <param name="y">Any y-coord</param>
        /// <returns></returns>
        public List<int> GetRemainingNumbersFor3x3Square(int x, int y)
        {
            int i;
            int j;

            List<int> intList = new List<int>();

            // arValues[i] = 0 means i is missing in row y
            bool[] arValues = new bool[10];

            // Find the left upper corner of the 3x3 square that (x, y) lies in.
            Position pos = GetLeftUpperPosition(x, y);

            for (i = pos.x; i < pos.x + 3; i++)
            {
                for (j = pos.y; j < pos.y + 3; j++)
                {
                    arValues[getValue(i, j)] = true;
                }
            }
            for (i = 1; i < 10; i++)
            {
                if (arValues[i] == false)
                {
                    intList.Add(i);
                }
            }

            return intList;
        }
        /// <summary>
        /// Check every column whether there is a number that can be in only one position within that column
        /// </summary>
        /// <param name="nValue"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool CheckColumns(ref int nValue, ref int x, ref int y)
        {
            bool bFound = false;

            for (x = 0; x < 9; x++)
            {
                if (CheckColumn(x, ref nValue, ref y))
                {
                    bFound = true;
                    break;
                }
            }
            return bFound;
        }

        /// <summary>
        /// Check every row whether there is a number that can be in only one position within that row
        /// </summary>
        /// <param name="nValue"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool CheckRows(ref int nValue, ref int x, ref int y)
        {
            bool bFound = false;

            for (y = 0; y < 9; y++)
            {
                if (CheckRow(y, ref nValue, ref x))
                {
                    bFound = true;
                    break;
                }
            }
            return bFound;
        }

        /// <summary>
        /// Check every 3x3 square for a number that can be in only one position in that square
        /// </summary>
        /// <param name="nValue"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Check3x3Squares(ref int nValue, ref int x, ref int y)
        {
            bool bFound = false;
            int i;
            int j;

            for (i = 0; i < 9; i += 3)
            {
                for (j = 0; j < 9; j += 3)
                {
                    if (Check3x3Square(i, j, ref nValue, ref x, ref y))
                    {
                        bFound = true;
                        goto Exit;
                    }
                }
            }
            
            Exit:
            return bFound;
        }

        /// <summary>
        /// Find out whether there is a number that can be in only one position in row x.
        /// If there is one, then this number is nValue and it is at postion (x2, y).
        /// </summary>
        public bool CheckRow(int y, ref int nValue, ref int x2)
        {
            bool bFound = false;

            List<int> intList = this.GetRemainingNumbersForRow(y);

            foreach (int n in intList)
            {
                if (1 == this.CountValidPositionsInRow(n, y, ref x2))
                {
                    nValue = n;
                    bFound = true;
                    break;
                }
            }

            return bFound;
        }

        /// <summary>
        /// Find out whether there is a number that can be in only one position in column x.
        /// If there is one, then this number is nValue and it is at postion (x, y2).
        /// </summary>
        /// <param name="x">The column to query</param>
        /// <param name="nValue"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public bool CheckColumn(int x, ref int nValue, ref int y2)
        {
            bool bFound = false;

            // Get all numbers that do not occur in column x
            List<int> intList = this.GetRemainingNumbersForColumn(x);

            foreach (int n in intList)
            {
                if (1 == this.CountValidPositionsInColumn(n, x, ref y2))
                {
                    // There is only one valid position for number n in column x,
                    // and this at row y2.
                    nValue = n;
                    bFound = true;
                    break;
                }
            }

            return bFound;
        }

        /// <summary>
        /// Find out whether there is a number that can be in only one position in its 3x3 square
        /// If there is one, then this number is nValue and it is at postion (x2, y2).
        /// </summary>
        public bool Check3x3Square(int x, int y, ref int nValue, ref int x2, ref int y2)
        {
            bool bFound = false;

            List<int> intList = this.GetRemainingNumbersFor3x3Square(x, y);

            foreach (int n in intList)
            {
                if (1 == this.CountValidPositionsIn3x3Square(n,x, y, ref x2, ref y2))
                {
                    nValue = n;
                    bFound = true;
                    break;
                }
            }

            return bFound;
        }
    }
}
