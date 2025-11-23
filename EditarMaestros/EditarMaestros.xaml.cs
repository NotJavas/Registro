using System;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Registro.Login.Database;
using Registro.Utils;

namespace Registro.EditarMaestros
{
    public partial class EditarMaestros : Window
    {
        private DataTable _maestrosDataTable;

        public EditarMaestros()
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

                const string query = "SELECT ID, Nombre, Correo, Facultad, Tipo FROM Maestros";
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    var adapter = new SQLiteDataAdapter(cmd);
                    _maestrosDataTable = new DataTable();
                    adapter.Fill(_maestrosDataTable);
                    MaestrosGrid.ItemsSource = _maestrosDataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MaestrosGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MaestrosGrid.SelectedItem is DataRowView selectedRow)
            {
                TxtNombre.Text = selectedRow["Nombre"].ToString();
                TxtCorreo.Text = selectedRow["Correo"].ToString();
                TxtFacultad.Text = selectedRow["Facultad"].ToString();
                TxtTipo.Text = selectedRow["Tipo"].ToString();
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNombre.Text) || string.IsNullOrWhiteSpace(TxtCorreo.Text))
            {
                MessageBox.Show("Nombre y Correo son campos obligatorios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string query;
            bool isNew = MaestrosGrid.SelectedItem == null;

            if (isNew)
            {
                query = "INSERT INTO Maestros (Nombre, Correo, Facultad, Tipo) VALUES (@nombre, @correo, @facultad, @tipo)";
            }
            else
            {
                query = "UPDATE Maestros SET Nombre = @nombre, Correo = @correo, Facultad = @facultad, Tipo = @tipo WHERE ID = @id";
            }

            try
            {
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", TxtNombre.Text);
                    cmd.Parameters.AddWithValue("@correo", TxtCorreo.Text);
                    cmd.Parameters.AddWithValue("@facultad", TxtFacultad.Text);
                    cmd.Parameters.AddWithValue("@tipo", TxtTipo.Text);

                    if (!isNew)
                    {
                        var selectedRow = (DataRowView)MaestrosGrid.SelectedItem;
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(selectedRow["ID"]));
                    }

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Maestro guardado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarDatos();
                Nuevo_Click(null, null); // Limpiar formulario
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el maestro: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            MaestrosGrid.SelectedItem = null;
            TxtNombre.Clear();
            TxtCorreo.Clear();
            TxtFacultad.Clear();
            TxtTipo.Clear();
            TxtNombre.Focus();
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (MaestrosGrid.SelectedItem is DataRowView selectedRow)
            {
                var result = MessageBox.Show("¿Estás seguro de que quieres eliminar este maestro?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int maestroId = Convert.ToInt32(selectedRow["ID"]);
                        const string query = "DELETE FROM Maestros WHERE ID = @id";
                        using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                        {
                            cmd.Parameters.AddWithValue("@id", maestroId);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Maestro eliminado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        CargarDatos();
                        Nuevo_Click(null, null); // Limpiar formulario
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar el maestro: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un maestro para eliminar.", "Selección Requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        
        private void ExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileName = $"Reporte_Maestros_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                string template = "Plantilla_Maestros.xlsx"; // Debe coincidir con tu archivo en la carpeta Plantillas

                ExcelExportHelper.ExportToExcel((DataView)MaestrosGrid.ItemsSource, fileName, template);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Filtro_TextChanged(object sender, TextChangedEventArgs e)
        {
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            if (_maestrosDataTable == null) return;

            var filterExpression = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(FilterID.Text))
                filterExpression.Append($"CONVERT(ID, 'System.String') LIKE '%{FilterID.Text}%' AND ");
            if (!string.IsNullOrWhiteSpace(FilterNombre.Text))
                filterExpression.Append($"Nombre LIKE '%{FilterNombre.Text}%' AND ");
            if (!string.IsNullOrWhiteSpace(FilterCorreo.Text))
                filterExpression.Append($"Correo LIKE '%{FilterCorreo.Text}%' AND ");
            if (!string.IsNullOrWhiteSpace(FilterFacultad.Text))
                filterExpression.Append($"Facultad LIKE '%{FilterFacultad.Text}%' AND ");
            if (!string.IsNullOrWhiteSpace(FilterTipo.Text))
                filterExpression.Append($"Tipo LIKE '%{FilterTipo.Text}%' AND ");

            if (filterExpression.Length > 0)
                filterExpression.Length -= 5; // Elimina el último " AND "

            _maestrosDataTable.DefaultView.RowFilter = filterExpression.ToString();
        }
    }
}
