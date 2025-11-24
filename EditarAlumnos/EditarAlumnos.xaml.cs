using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Registro.Login.Database;
using Registro.Utils;

namespace Registro.EditarAlumnos
{
    public partial class EditarAlumnos : Window
    {
        private DataTable _alumnosDataTable;

        public EditarAlumnos()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                if (Globales.Conexion.State != ConnectionState.Open)
                {
                    Globales.Conexion.Open();
                }

                const string query = "SELECT Expediente, Nombre, Semestre, Direccion, TelefonoAlumno, TelefonoContacto, FechaNacimiento, Edad, Genero, Correo, Alergias, LugarNacimiento, PadreTutor, MadreTutora, TotalNA FROM Alumnos";
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    var adapter = new SQLiteDataAdapter(cmd);
                    _alumnosDataTable = new DataTable();
                    adapter.Fill(_alumnosDataTable);
                    AlumnosGrid.ItemsSource = _alumnosDataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AlumnosGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlumnosGrid.SelectedItem is DataRowView selectedRow)
            {
                TxtExpediente.Text = selectedRow["Expediente"].ToString();
                TxtNombre.Text = selectedRow["Nombre"].ToString();
                TxtSemestre.Text = selectedRow["Semestre"].ToString();
                TxtDireccion.Text = selectedRow["Direccion"].ToString();
                TxtTelefonoAlumno.Text = selectedRow["TelefonoAlumno"].ToString();
                TxtTelefonoContacto.Text = selectedRow["TelefonoContacto"].ToString();
                if (DateTime.TryParse(selectedRow["FechaNacimiento"].ToString(), out DateTime fechaNac))
                {
                    DpFechaNacimiento.SelectedDate = fechaNac;
                }
                else
                {
                    DpFechaNacimiento.SelectedDate = null;
                }
                TxtEdad.Text = selectedRow["Edad"].ToString();
                TxtGenero.Text = selectedRow["Genero"].ToString();
                TxtCorreo.Text = selectedRow["Correo"].ToString();
                TxtAlergias.Text = selectedRow["Alergias"].ToString();
                TxtLugarNacimiento.Text = selectedRow["LugarNacimiento"].ToString();
                TxtPadreTutor.Text = selectedRow["PadreTutor"].ToString();
                TxtMadreTutora.Text = selectedRow["MadreTutora"].ToString();
                TxtTotalNA.Text = selectedRow["TotalNA"].ToString();
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtExpediente.Text) || string.IsNullOrWhiteSpace(TxtNombre.Text))
            {
                MessageBox.Show("Expediente y Nombre son campos obligatorios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string query;
            bool isNew = AlumnosGrid.SelectedItem == null;

            if (isNew)
            {
                query = "INSERT INTO Alumnos (Expediente, Nombre, Semestre, Direccion, TelefonoAlumno, TelefonoContacto, FechaNacimiento, Edad, Genero, Correo, Alergias, LugarNacimiento, PadreTutor, MadreTutora, TotalNA) VALUES (@expediente, @nombre, @semestre, @direccion, @telefonoAlumno, @telefonoContacto, @fechaNacimiento, @edad, @genero, @correo, @alergias, @lugarNacimiento, @padreTutor, @madreTutora, @totalNA)";
            }
            else
            {
                query = "UPDATE Alumnos SET Nombre = @nombre, Semestre = @semestre, Direccion = @direccion, TelefonoAlumno = @telefonoAlumno, TelefonoContacto = @telefonoContacto, FechaNacimiento = @fechaNacimiento, Edad = @edad, Genero = @genero, Correo = @correo, Alergias = @alergias, LugarNacimiento = @lugarNacimiento, PadreTutor = @padreTutor, MadreTutora = @madreTutora, TotalNA = @totalNA WHERE Expediente = @expediente";
            }

            try
            {
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@expediente", TxtExpediente.Text);
                    cmd.Parameters.AddWithValue("@nombre", TxtNombre.Text);
                    cmd.Parameters.AddWithValue("@semestre", TxtSemestre.Text);
                    cmd.Parameters.AddWithValue("@direccion", TxtDireccion.Text);
                    cmd.Parameters.AddWithValue("@telefonoAlumno", TxtTelefonoAlumno.Text);
                    cmd.Parameters.AddWithValue("@telefonoContacto", TxtTelefonoContacto.Text);
                    cmd.Parameters.AddWithValue("@fechaNacimiento", DpFechaNacimiento.SelectedDate?.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@edad", TxtEdad.Text);
                    cmd.Parameters.AddWithValue("@genero", TxtGenero.Text);
                    cmd.Parameters.AddWithValue("@correo", TxtCorreo.Text);
                    cmd.Parameters.AddWithValue("@alergias", TxtAlergias.Text);
                    cmd.Parameters.AddWithValue("@lugarNacimiento", TxtLugarNacimiento.Text);
                    cmd.Parameters.AddWithValue("@padreTutor", TxtPadreTutor.Text);
                    cmd.Parameters.AddWithValue("@madreTutora", TxtMadreTutora.Text);
                    cmd.Parameters.AddWithValue("@totalNA", TxtTotalNA.Text);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Alumno guardado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarDatos();
                Nuevo_Click(null, null); // Limpiar formulario
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el alumno: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            AlumnosGrid.SelectedItem = null;
            TxtExpediente.Clear();
            TxtNombre.Clear();
            TxtSemestre.Clear();
            TxtDireccion.Clear();
            TxtTelefonoAlumno.Clear();
            TxtTelefonoContacto.Clear();
            DpFechaNacimiento.SelectedDate = null;
            TxtEdad.Clear();
            TxtGenero.Clear();
            TxtCorreo.Clear();
            TxtAlergias.Clear();
            TxtLugarNacimiento.Clear();
            TxtPadreTutor.Clear();
            TxtMadreTutora.Clear();
            TxtTotalNA.Clear();
            TxtExpediente.Focus();
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (AlumnosGrid.SelectedItem is DataRowView selectedRow)
            {
                var result = MessageBox.Show("¿Estás seguro de que quieres eliminar este alumno?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int expediente = Convert.ToInt32(selectedRow["Expediente"]);
                        const string query = "DELETE FROM Alumnos WHERE Expediente = @expediente";
                        using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                        {
                            cmd.Parameters.AddWithValue("@expediente", expediente);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Alumno eliminado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        CargarDatos();
                        Nuevo_Click(null, null); // Limpiar formulario
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar el alumno: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un alumno para eliminar.", "Selección Requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileName = $"Reporte_Alumnos_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                string template = "Plantilla_Alumnos.xlsx";

                ExcelExportHelper.ExportToExcel((DataView)AlumnosGrid.ItemsSource, fileName, template);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportarFichas_Click(object sender, RoutedEventArgs e)
        {
            if (AlumnosGrid.Items.Count == 0)
            {
                MessageBox.Show("No hay alumnos en la lista para exportar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Title = "Selecciona una carpeta para guardar las fichas",
                Filter = "Directorio|*.this.is.a.dummy.extension",
                FileName = "Selecciona una carpeta"
            };

            if (dialog.ShowDialog() == true)
            {
                string path = Path.GetDirectoryName(dialog.FileName);
                string plantillaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plantillas", "Ficha_Alumnos.xlsx");
                int contador = 0;

                try
                {
                    foreach (DataRowView alumno in AlumnosGrid.Items)
                    {
                        ExcelExportHelper.ExportarFichaAlumnoAPdf(alumno, plantillaPath, path);
                        contador++;
                    }
                    MessageBox.Show($"{contador} fichas de alumnos han sido exportadas a PDF con éxito.", "Exportación Completa", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error durante la exportación:\n{ex.Message}", "Error de Exportación", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Filtro_TextChanged(object sender, TextChangedEventArgs e)
        {
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            if (_alumnosDataTable == null) return;

            var filterExpression = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(FilterExpediente.Text))
                filterExpression.Append($"CONVERT(Expediente, 'System.String') LIKE '%{FilterExpediente.Text}%' AND ");
            if (!string.IsNullOrWhiteSpace(FilterNombre.Text))
                filterExpression.Append($"Nombre LIKE '%{FilterNombre.Text}%' AND ");
            if (!string.IsNullOrWhiteSpace(FilterSemestre.Text))
                filterExpression.Append($"CONVERT(Semestre, 'System.String') LIKE '%{FilterSemestre.Text}%' AND ");
            if (!string.IsNullOrWhiteSpace(FilterCorreo.Text))
                filterExpression.Append($"Correo LIKE '%{FilterCorreo.Text}%' AND ");
            if (!string.IsNullOrWhiteSpace(FilterEdad.Text))
                filterExpression.Append($"CONVERT(Edad, 'System.String') LIKE '%{FilterEdad.Text}%' AND ");
            if (!string.IsNullOrWhiteSpace(FilterGenero.Text))
                filterExpression.Append($"Genero LIKE '%{FilterGenero.Text}%' AND ");
            if (!string.IsNullOrWhiteSpace(FilterDireccion.Text))
                filterExpression.Append($"Direccion LIKE '%{FilterDireccion.Text}%' AND ");

            if (filterExpression.Length > 0)
                filterExpression.Length -= 5;

            _alumnosDataTable.DefaultView.RowFilter = filterExpression.ToString();
        }
    }
}
