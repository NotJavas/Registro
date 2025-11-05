
using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;

namespace Registro.Utils
{
    public static class ExcelExportHelper
    {
        public static void ExportToExcel(DataView dataView, string title, string defaultFileName)
        {
            if (dataView == null || dataView.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Archivo de Excel (*.xlsx)|*.xlsx",
                Title = title,
                FileName = defaultFileName
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                excelApp = new Excel.Application();
                workbook = excelApp.Workbooks.Add();
                worksheet = (Excel.Worksheet)workbook.ActiveSheet;
                worksheet.Name = title;

                // Escribir encabezados
                for (int i = 0; i < dataView.Table.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1] = dataView.Table.Columns[i].ColumnName;
                    ((Excel.Range)worksheet.Cells[1, i + 1]).Font.Bold = true;
                }

                // Escribir datos
                for (int i = 0; i < dataView.Count; i++)
                {
                    for (int j = 0; j < dataView.Table.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = dataView[i][j].ToString();
                    }
                }

                worksheet.Columns.AutoFit();
                workbook.SaveAs(saveFileDialog.FileName);

                MessageBox.Show($"Reporte guardado con éxito en:\n{saveFileDialog.FileName}", "Exportación Completa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar a Excel: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Liberación de recursos COM
                if (workbook != null) workbook.Close(false);
                if (excelApp != null) excelApp.Quit();

                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                if (excelApp != null) Marshal.ReleaseComObject(excelApp);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
