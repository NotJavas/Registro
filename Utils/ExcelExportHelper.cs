using System;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;

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

            string rutaPlantilla = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plantillas", nombrePlantilla);

            if (!File.Exists(rutaPlantilla))
            {
                MessageBox.Show($"No se encontró la plantilla en:\n{rutaPlantilla}\n\nAsegúrate de que el archivo tiene la propiedad 'Copiar en el directorio de salida' activada.", "Error de Plantilla", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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
                workbook = excelApp.Workbooks.Open(rutaPlantilla);
                worksheet = (Excel.Worksheet)workbook.ActiveSheet;

                int startRow = 3; 
                int totalCols = dataView.Table.Columns.Count;
                int totalRows = dataView.Count;

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
                writeRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

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

        public static void ExportarFichaAlumnoAPdf(DataRowView alumno, string rutaPlantilla, string directorioSalida)
        {
            if (!File.Exists(rutaPlantilla))
            {
                throw new FileNotFoundException($"No se encontró la plantilla en:\n{rutaPlantilla}");
            }

            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                excelApp = new Excel.Application { Visible = false };
                workbook = excelApp.Workbooks.Open(rutaPlantilla);
                worksheet = (Excel.Worksheet)workbook.ActiveSheet;

                // Llenar celdas
                worksheet.Cells[4, 3] = alumno["Expediente"];
                worksheet.Cells[6, 3] = alumno["Nombre"];
                worksheet.Cells[8, 3] = alumno["Semestre"];
                worksheet.Cells[10, 3] = alumno["Direccion"];
                worksheet.Cells[13, 3] = alumno["TelefonoAlumno"];
                worksheet.Cells[15, 3] = alumno["TelefonoContacto"];
                worksheet.Cells[18, 3] = alumno["FechaNacimiento"];
                worksheet.Cells[20, 3] = alumno["Edad"];
                worksheet.Cells[22, 3] = alumno["Genero"];
                worksheet.Cells[24, 3] = alumno["Correo"];
                worksheet.Cells[26, 3] = alumno["Alergias"];
                worksheet.Cells[29, 3] = alumno["LugarNacimiento"];
                worksheet.Cells[31, 3] = alumno["PadreTutor"];
                worksheet.Cells[33, 3] = alumno["MadreTutora"];
                worksheet.Cells[35, 3] = alumno["TotalNA"];

                // Generar nombre de archivo y exportar a PDF
                string nombreArchivo = $"Ficha_{alumno["Expediente"]}_{alumno["Nombre"].ToString().Replace(" ", "_")}.pdf";
                string rutaCompletaPdf = Path.Combine(directorioSalida, nombreArchivo);

                worksheet.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, rutaCompletaPdf);
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
