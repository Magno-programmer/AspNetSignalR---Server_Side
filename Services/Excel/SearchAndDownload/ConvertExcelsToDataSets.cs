using ClosedXML.Excel;
using System.Data;

namespace AspNetSignalIR.Services.Excel.SearchAnDownload;

public class ConvertExcelsToDataSets
{

    // Conversor de Excel em DataSet
    internal static DataSet ConvertExcelToDataSet(string filePath)
    {
        var dataSet = new DataSet();

        using (var workbook = new XLWorkbook(filePath))
        {
            foreach (var worksheet in workbook.Worksheets)
            {
                var dataTable = new DataTable(worksheet.Name);

                // Adiciona as colunas ao DataTable com base na primeira linha da planilha (cabeçalho)
                bool firstRow = true;
                foreach (var row in worksheet.RowsUsed())
                {
                    if (firstRow)
                    {
                        foreach (var cell in row.Cells())
                        {
                            string columnName = cell.Value.ToString();
                            dataTable.Columns.Add(string.IsNullOrWhiteSpace(columnName) ? $"Column{cell.Address.ColumnNumber}" : columnName);
                        }
                        firstRow = false;
                    }
                    else
                    {
                        var dataRow = dataTable.NewRow();
                        int columnIndex = 0;
                        foreach (var cell in row.Cells(1, dataTable.Columns.Count))
                        {
                            dataRow[columnIndex++] = cell.Value;
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }

                // Adiciona o DataTable ao DataSet
                dataSet.Tables.Add(dataTable);
            }
        }

        return dataSet;
    }

}
