using System;
using System.Data;
using System.IO; // Necesario para Path
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using Registro.Login.Database; // Para acceder a Globales.RutaAplicacion si es necesario

namespace Registro.Utils
{
    public static class ExcelExportHelper
    {
        public static void ExportToExcel(DataView dataView, string nombreArchivoSalida, string nombrePlantilla)
        {
            if (dataView == null || dataView.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 1. Construir la ruta a la plantilla
            // Se asume que la plantilla está en: bin/Debug/Plantillas/nombrePlantilla.xlsx
            string rutaPlantilla = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plantillas", nombrePlantilla);

            if (!File.Exists(rutaPlantilla))
            {
                MessageBox.Show($"No se encontró la plantilla en:\n{rutaPlantilla}\n\nAsegúrate de que el archivo tiene la propiedad 'Copiar en el directorio de salida' activada.", "Error de Plantilla", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Preguntar dónde guardar el resultado
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Archivo de Excel (*.xlsx)|*.xlsx",
                FileName = nombreArchivoSalida
            };

            if (saveFileDialog.ShowDialog() != true) return;

            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                excelApp = new Excel.Application();
                // AQUI ESTA EL CAMBIO: Abrimos la plantilla en lugar de crear uno nuevo
                workbook = excelApp.Workbooks.Open(rutaPlantilla);
                worksheet = (Excel.Worksheet)workbook.ActiveSheet;

                // Suponemos que los datos empiezan en la fila que tú decidas. 
                // Por estándar, digamos que tu plantilla tiene encabezados en la fila 2 y datos en la 3.
                int startRow = 3; 
                int totalCols = dataView.Table.Columns.Count;
                int totalRows = dataView.Count;

                // Escribir datos (Matriz rápida)
                object[,] dataArr = new object[totalRows, totalCols];
                for (int r = 0; r < totalRows; r++)
                {
                    for (int c = 0; c < totalCols; c++)
                    {
                        dataArr[r, c] = dataView[r][c].ToString();
                    }
                }

                Excel.Range startCell = (Excel.Range)worksheet.Cells[startRow, 1];
                Excel.Range endCell = (Excel.Range)worksheet.Cells[startRow + totalRows - 1, totalCols];
                Excel.Range writeRange = worksheet.Range[startCell, endCell];
                
                writeRange.Value2 = dataArr;

                // Opcional: Ajustar bordes a los nuevos datos insertados
                writeRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                // Guardar COMO un nuevo archivo (para no sobreescribir la plantilla)
                workbook.SaveAs(saveFileDialog.FileName);

                MessageBox.Show("Reporte generado con éxito.", "Exportación", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (workbook != null) workbook.Close(false);
                if (excelApp != null) excelApp.Quit();

                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                if (excelApp != null) Marshal.ReleaseComObject(excelApp);

                GC.Collect();
            }
        }
    }
}