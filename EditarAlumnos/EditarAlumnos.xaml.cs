using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using Registro.Login.Database;

namespace Registro.EditarAlumnos
{
    public partial class EditarAlumnos : Window
    {
        private bool isNewRecord = false; // Flag para controlar si es un registro nuevo

        public EditarAlumnos()
        {
            InitializeComponent();
        }

        // Carga y abre la conexión
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Globales.Conexion.Open();
                CargarDatos();
            }
            catch (Exception ex) { MessageBox.Show($"Error al conectar: {ex.Message}"); }
        }

        // Cierra la conexión
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Globales.Conexion.State == ConnectionState.Open) Globales.Conexion.Close();
        }

        private void CargarDatos()
        {
            // Consulta corregida con las columnas reales
            string query = "SELECT Expediente, Nombre, Semestre, Correo, Edad, Genero FROM Alumnos";
            try
            {
                using (var adapter = new SQLiteDataAdapter(query, Globales.Conexion))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    AlumnosGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex) { MessageBox.Show($"Error al cargar alumnos: {ex.Message}"); }
        }
        
        private void AlumnosGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlumnosGrid.SelectedItem is DataRowView row)
            {
                // Campos corregidos
                TxtExpediente.Text = row["Expediente"].ToString();
                TxtNombre.Text = row["Nombre"].ToString();
                TxtSemestre.Text = row["Semestre"].ToString();
                TxtCorreo.Text = row["Correo"].ToString();
                TxtEdad.Text = row["Edad"].ToString();
                TxtGenero.Text = row["Genero"].ToString();
                
                TxtExpediente.IsReadOnly = true; // El expediente no debe cambiarse
                isNewRecord = false;
            }
        }

        private void MenuNuevo_Click(object sender, RoutedEventArgs e)
        {
            // Limpia todos los campos
            TxtExpediente.Clear(); TxtNombre.Clear(); TxtSemestre.Clear();
            TxtCorreo.Clear(); TxtEdad.Clear(); TxtGenero.Clear();
            
            TxtExpediente.IsReadOnly = false; // Permite escribir un nuevo expediente
            TxtExpediente.Focus();
            isNewRecord = true;
        }

        private void MenuGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtExpediente.Text) || string.IsNullOrWhiteSpace(TxtNombre.Text))
            {
                MessageBox.Show("El expediente y el nombre son obligatorios.", "Validación");
                return;
            }

            string query;
            if (isNewRecord)
                // INSERT corregido con las columnas y parámetros reales
                query = "INSERT INTO Alumnos (Expediente, Nombre, Semestre, Correo, Edad, Genero) VALUES (@exp, @nom, @sem, @correo, @edad, @gen)";
            else
                // UPDATE corregido
                query = "UPDATE Alumnos SET Nombre = @nom, Semestre = @sem, Correo = @correo, Edad = @edad, Genero = @gen WHERE Expediente = @exp";
            
            try
            {
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@exp", Convert.ToInt32(TxtExpediente.Text));
                    cmd.Parameters.AddWithValue("@nom", TxtNombre.Text);
                    cmd.Parameters.AddWithValue("@sem", TxtSemestre.Text);
                    cmd.Parameters.AddWithValue("@correo", TxtCorreo.Text);
                    cmd.Parameters.AddWithValue("@edad", Convert.ToInt32(TxtEdad.Text));
                    cmd.Parameters.AddWithValue("@gen", TxtGenero.Text);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Alumno guardado.", "Éxito");
                CargarDatos();
                MenuNuevo_Click(null, null); // Limpia para el siguiente
            }
            catch (Exception ex) { MessageBox.Show($"Error al guardar: {ex.Message}"); }
        }
        
        private void MenuEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (AlumnosGrid.SelectedItem == null) { MessageBox.Show("Selecciona un alumno a eliminar."); return; }
            if (MessageBox.Show("¿Seguro?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            
            try
            {
                // DELETE corregido
                using (var cmd = new SQLiteCommand("DELETE FROM Alumnos WHERE Expediente = @exp", Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@exp", Convert.ToInt32(TxtExpediente.Text));
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Alumno eliminado.", "Éxito");
                MenuNuevo_Click(null, null);
                CargarDatos();
            }
            catch (Exception ex) { MessageBox.Show($"Error al eliminar: {ex.Message}"); }
        }
    }
}