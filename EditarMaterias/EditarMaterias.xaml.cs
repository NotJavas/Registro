using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using Registro.Login.Database;

namespace Registro.EditarMaterias
{
    public partial class EditarMaterias : Window
    {
        private int? _materiaId;

        public EditarMaterias()
        {
            InitializeComponent();
            CargarDatos();
        }

        public EditarMaterias(int materiaId)
        {
            InitializeComponent();
            _materiaId = materiaId;
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                if (Globales.Conexion.State != ConnectionState.Open)
                    Globales.Conexion.Open();

                string query = "SELECT ID, Nombre, Semestre FROM Materias";
                if (_materiaId.HasValue)
                {
                    query += " WHERE ID = " + _materiaId.Value;
                }

                using (SQLiteCommand cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    MateriasGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuNuevo_Click(object sender, RoutedEventArgs e)
        {
            var form = new EditarMaterias();
            form.Owner = this;
            if (form.ShowDialog() == true)
            {
                CargarDatos();
            }
        }

        private void MenuEditar_Click(object sender, RoutedEventArgs e)
        {
            if (MateriasGrid.SelectedItem is DataRowView selectedRow)
            {
                var form = new EditarMaterias(Convert.ToInt32(selectedRow["ID"]));
                form.Owner = this;
                if (form.ShowDialog() == true)
                {
                    CargarDatos();
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una materia para editar.");
            }
        }

        private void MenuEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (MateriasGrid.SelectedItem is DataRowView selectedRow)
            {
                if (MessageBox.Show("¿Estás seguro de que quieres eliminar esta materia?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var cmd = new SQLiteCommand("DELETE FROM Materias WHERE ID = @id", Globales.Conexion))
                        {
                            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(selectedRow["ID"]));
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Materia eliminada con éxito.");
                        CargarDatos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar la materia: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una materia para eliminar.");
            }
        }
    }
}
