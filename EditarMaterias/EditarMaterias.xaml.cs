using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using Registro.Login.Database;

namespace Registro.EditarMaterias
{
    public partial class EditarMaterias : Window
    {
        public EditarMaterias()
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

                const string query = "SELECT ID, Nombre, Semestre FROM Materias";
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    var adapter = new SQLiteDataAdapter(cmd);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    MateriasGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MateriasGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MateriasGrid.SelectedItem is DataRowView selectedRow)
            {
                TxtNombre.Text = selectedRow["Nombre"].ToString();
                TxtSemestre.Text = selectedRow["Semestre"].ToString();
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNombre.Text) || string.IsNullOrWhiteSpace(TxtSemestre.Text))
            {
                MessageBox.Show("Nombre y Semestre son campos obligatorios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string query;
            bool isNew = MateriasGrid.SelectedItem == null;

            if (isNew)
            {
                query = "INSERT INTO Materias (Nombre, Semestre) VALUES (@nombre, @semestre)";
            }
            else
            {
                query = "UPDATE Materias SET Nombre = @nombre, Semestre = @semestre WHERE ID = @id";
            }

            try
            {
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", TxtNombre.Text);
                    cmd.Parameters.AddWithValue("@semestre", TxtSemestre.Text);

                    if (!isNew)
                    {
                        var selectedRow = (DataRowView)MateriasGrid.SelectedItem;
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(selectedRow["ID"]));
                    }

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Materia guardada con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarDatos();
                Nuevo_Click(null, null); // Limpiar formulario
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la materia: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            MateriasGrid.SelectedItem = null;
            TxtNombre.Clear();
            TxtSemestre.Clear();
            TxtNombre.Focus();
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (MateriasGrid.SelectedItem is DataRowView selectedRow)
            {
                var result = MessageBox.Show("¿Estás seguro de que quieres eliminar esta materia?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int materiaId = Convert.ToInt32(selectedRow["ID"]);
                        const string query = "DELETE FROM Materias WHERE ID = @id";
                        using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                        {
                            cmd.Parameters.AddWithValue("@id", materiaId);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Materia eliminada con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        CargarDatos();
                        Nuevo_Click(null, null); // Limpiar formulario
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar la materia: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una materia para eliminar.", "Selección Requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
