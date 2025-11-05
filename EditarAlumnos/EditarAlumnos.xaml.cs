using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using Registro.Login.Database;

namespace Registro.EditarAlumnos
{
    public partial class EditarAlumnos : Window
    {
        private int? expediente;

        public EditarAlumnos()
        {
            InitializeComponent();
            CargarDatos();
        }

        public EditarAlumnos(int expediente)
        {
            InitializeComponent();
            this.expediente = expediente;
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                if (Globales.Conexion.State != ConnectionState.Open)
                    Globales.Conexion.Open();

                string query = "SELECT Expediente, Nombre, Apellidos, Correo, Telefono FROM Alumnos";
                using (SQLiteCommand cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    AlumnosGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuNuevo_Click(object sender, RoutedEventArgs e)
        {
            var form = new EditarAlumnos();
            form.Owner = this;
            if (form.ShowDialog() == true)
            {
                CargarDatos();
            }
        }

        private void MenuEditar_Click(object sender, RoutedEventArgs e)
        {
            if (AlumnosGrid.SelectedItem is DataRowView selectedRow)
            {
                int expediente = Convert.ToInt32(selectedRow["Expediente"]);
                var form = new EditarAlumnos(expediente);
                form.Owner = this;
                if (form.ShowDialog() == true)
                {
                    CargarDatos();
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un alumno para editar.");
            }
        }

        private void MenuEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (AlumnosGrid.SelectedItem is DataRowView selectedRow)
            {
                if (MessageBox.Show("¿Estás seguro de que quieres eliminar este alumno?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        int expediente = Convert.ToInt32(selectedRow["Expediente"]);
                        using (var cmd = new SQLiteCommand("DELETE FROM Alumnos WHERE Expediente = @expediente", Globales.Conexion))
                        {
                            cmd.Parameters.AddWithValue("@expediente", expediente);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Alumno eliminado con éxito.");
                        CargarDatos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar el alumno: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un alumno para eliminar.");
            }
        }
    }
}
