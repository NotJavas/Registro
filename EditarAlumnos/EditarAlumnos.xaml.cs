using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using Registro.Login.Database;

namespace Registro.EditarAlumnos
{
    public partial class EditarAlumnos : Window
    {
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

                const string query = "SELECT Expediente, Nombre, Semestre, Correo, Edad, Genero FROM Alumnos";
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    var adapter = new SQLiteDataAdapter(cmd);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    AlumnosGrid.ItemsSource = dataTable.DefaultView;
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
                TxtCorreo.Text = selectedRow["Correo"].ToString();
                TxtEdad.Text = selectedRow["Edad"].ToString();
                TxtGenero.Text = selectedRow["Genero"].ToString();
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
                query = "INSERT INTO Alumnos (Expediente, Nombre, Semestre, Correo, Edad, Genero) VALUES (@expediente, @nombre, @semestre, @correo, @edad, @genero)";
            }
            else
            {
                query = "UPDATE Alumnos SET Nombre = @nombre, Semestre = @semestre, Correo = @correo, Edad = @edad, Genero = @genero WHERE Expediente = @expediente";
            }

            try
            {
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@expediente", TxtExpediente.Text);
                    cmd.Parameters.AddWithValue("@nombre", TxtNombre.Text);
                    cmd.Parameters.AddWithValue("@semestre", TxtSemestre.Text);
                    cmd.Parameters.AddWithValue("@correo", TxtCorreo.Text);
                    cmd.Parameters.AddWithValue("@edad", TxtEdad.Text);
                    cmd.Parameters.AddWithValue("@genero", TxtGenero.Text);

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
            TxtCorreo.Clear();
            TxtEdad.Clear();
            TxtGenero.Clear();
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
    }
}
