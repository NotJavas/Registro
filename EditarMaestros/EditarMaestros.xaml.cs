using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using Registro.Login.Database;

namespace Registro.EditarMaestros
{
    public partial class EditarMaestros : Window
    {
        private int? _maestroId;

        public EditarMaestros()
        {
            InitializeComponent();
            CargarDatos();
        }

        public EditarMaestros(int maestroId)
        {
            InitializeComponent();
            _maestroId = maestroId;
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                if (Globales.Conexion.State != ConnectionState.Open)
                    Globales.Conexion.Open();

                string query = "SELECT ID, Nombre, Correo, Facultad, Tipo FROM Maestros";
                if (_maestroId.HasValue)
                {
                    query += " WHERE ID = " + _maestroId.Value;
                }

                using (SQLiteCommand cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    MaestrosGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuNuevo_Click(object sender, RoutedEventArgs e)
        {
            var form = new EditarMaestros();
            form.Owner = this;
            if (form.ShowDialog() == true)
            {
                CargarDatos();
            }
        }

        private void MenuEditar_Click(object sender, RoutedEventArgs e)
        {
            if (MaestrosGrid.SelectedItem is DataRowView selectedRow)
            {
                var form = new EditarMaestros(Convert.ToInt32(selectedRow["ID"]));
                form.Owner = this;
                if (form.ShowDialog() == true)
                {
                    CargarDatos();
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un maestro para editar.");
            }
        }

        private void MenuEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (MaestrosGrid.SelectedItem is DataRowView selectedRow)
            {
                if (MessageBox.Show("¿Estás seguro de que quieres eliminar este maestro?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var cmd = new SQLiteCommand("DELETE FROM Maestros WHERE ID = @id", Globales.Conexion))
                        {
                            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(selectedRow["ID"]));
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Maestro eliminado con éxito.");
                        CargarDatos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar el maestro: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un maestro para eliminar.");
            }
        }
    }
}
