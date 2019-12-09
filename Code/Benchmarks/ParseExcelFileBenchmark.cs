using System.IO;
using BenchmarkDotNet.Attributes;
using OfficeOpenXml;

namespace Benchmarks
{
    public class ParseExcelFileBenchmark
    {
        private ExcelPackage _package;
        private ExcelWorksheet _worksheet;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _package = new ExcelPackage(new FileInfo("Values.xlsx"));
            _worksheet = _package.Workbook.Worksheets[0];
        }

        [Benchmark(Baseline = true)]
        public int FindFirstEmptyRow1()
        {
            var currentRow = 0;
            while (++currentRow < _worksheet.Cells.Rows)
            {
                var row = _worksheet.Cells[currentRow, 1, currentRow, 3];
                var hasContent = false;
                foreach (var cell in row)
                {
                    if (cell.Value == null)
                        continue;

                    hasContent = true;
                    break;
                }

                if (!hasContent)
                    return currentRow;
            }

            return _worksheet.Cells.Rows;
        }

        [Benchmark]
        public int FindFirstEmptyRow2()
        {
            var currentRow = 0;
            var worksheetCells = _worksheet.Cells;
            var numberOfWorksheetRows = worksheetCells.Rows;
            while (++currentRow < numberOfWorksheetRows)
            {
                var row = worksheetCells[currentRow, 1, currentRow, 3];
                var hasContent = false;
                foreach (var cell in row)
                {
                    if (cell.Value == null)
                        continue;

                    hasContent = true;
                    break;
                }

                if (!hasContent)
                    return currentRow;
            }

            return numberOfWorksheetRows;
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _package.Dispose();
        }
    }
}