using System.Collections.Generic;
 
namespace Simplex
{
    public class SimplexMethod
    {
        //source - simplex table without basis variables
        private double[,] _table; // simplex table
 
        private int _m;
        private int _n;

        private List<int> _basis; // list of basis variables
 
        public SimplexMethod(double[,] source)
        {
            _m = source.GetLength(0);
            _n = source.GetLength(1);
            _table = new double[_m, _n + _m - 1];
            _basis = new List<int>();
 
            for (int i = 0; i < _m; i++)
            {
                for (int j = 0; j < _table.GetLength(1); j++)
                {
                    if (j < _n)
                        _table[i, j] = source[i, j];
                    else
                        _table[i, j] = 0;
                }
                // set 1 before basis variable
                if (_n + i >= _table.GetLength(1)) continue;
                _table[i, _n + i] = 1;
                _basis.Add(_n + i);
            }
 
            _n = _table.GetLength(1);
        }
        public double[,] Calculate(double[] result)
        {
            while (!IsItEnd())
            {
                var mainCol = FindMainCol();
                var mainRow = FindMainRow(mainCol);
                _basis[mainRow] = mainCol;
 
                double[,] newTable = new double[_m, _n];
 
                for (int j = 0; j < _n; j++)
                    newTable[mainRow, j] = _table[mainRow, j] / _table[mainRow, mainCol];
 
                for (int i = 0; i < _m; i++)
                {
                    if (i == mainRow)
                        continue;
 
                    for (int j = 0; j < _n; j++)
                        newTable[i, j] = _table[i, j] - _table[i, mainCol] * newTable[mainRow, j];
                }
                _table = newTable;
            }
            
            // put values in result
            for (int i = 0; i < result.Length; i++)
            {
                int k = _basis.IndexOf(i + 1);
                if (k != -1)
                    result[i] = _table[k, 0];
                else
                    result[i] = 0;
            }
 
            return _table;
        }
 
        private bool IsItEnd()
        {
            bool flag = true;
 
            for (int j = 1; j < _n; j++)
            {
                if (_table[_m - 1, j] < 0)
                {
                    flag = false;
                    break;
                }
            }
 
            return flag;
        }
 
        private int FindMainCol()
        {
            var mainCol = 1;
 
            for (int j = 2; j < _n; j++)
                if (_table[_m - 1, j] < _table[_m - 1, mainCol])
                    mainCol = j;
 
            return mainCol;
        }
 
        private int FindMainRow(int mainCol)
        {
            int mainRow = 0;
 
            for (int i = 0; i < _m - 1; i++)
                if (_table[i, mainCol] > 0)
                {
                    mainRow = i;
                    break;
                }
 
            for (int i = mainRow + 1; i < _m - 1; i++)
                if (_table[i, mainCol] > 0 && _table[i, 0] / _table[i, mainCol] 
                    < _table[mainRow, 0] / _table[mainRow, mainCol])
                    mainRow = i;
 
            return mainRow;
        }
    }
}