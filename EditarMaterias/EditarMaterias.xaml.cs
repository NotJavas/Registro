using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using Registro.Login.Database;

namespace Registro.EditarMaterias
{
    public partial class EditarMaterias : Window
    {
        public EditarMaterias() { InitializeComponent(); }
        private void Window_Loaded(object sender, RoutedEventArgs e) { try { Globales.Conexion.Open(); CargarDatos(); } catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); } }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) { if (Globales.Conexion.State == ConnectionState.Open) Globales.Conexion.Close(); }

        private void CargarDatos()
        {
            // Consulta corregida
            string query = "SELECT ID, Nombre, Semestre FROM Materias";
            try {
                using (var adapter = new SQLiteDataAdapter(query, Globales.Conexion))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    MateriasGrid.ItemsSource = dt.DefaultView;
                }
            } catch (Exception ex) { MessageBox.Show($"Error al cargar: {ex.Message}"); }
        }

        private void MateriasGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MateriasGrid.SelectedItem is DataRowView row)
            {
                // Campos corregidos
                TxtId.Text = row["ID"].ToString();
                TxtNombre.Text = row["Nombre"].ToString();
                TxtSemestre.Text = row["Semestre"].ToString();
            }
        }

        private void MenuNuevo_Click(object sender, RoutedEventArgs e)
        {
            TxtId.Clear(); TxtNombre.Clear(); TxtSemestre.Clear();
            TxtNombre.Focus();
        }

        private void MenuGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNombre.Text)) { MessageBox.Show("El nombre es obligatorio."); return; }

            // Consultas corregidas
            string query = string.IsNullOrEmpty(TxtId.Text)
                ? "INSERT INTO Materias (Nombre, Semestre) VALUES (@nom, @sem)"
                : "UPDATE Materias SET Nombre = @nom, Semestre = @sem WHERE ID = @id";
            try
            {
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@nom", TxtNombre.Text);
                    cmd.Parameters.AddWithValue("@sem", TxtSemestre.Text);
                    if (!string.IsNullOrEmpty(TxtId.Text)) cmd.Parameters.AddWithValue("@id", int.Parse(TxtId.Text));
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Materia guardada."); CargarDatos();
            }
            catch (Exception ex) { MessageBox.Show($"Error al guardar: {ex.Message}"); }
        }

        private void MenuEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtId.Text)) { MessageBox.Show("Selecciona una materia a eliminar."); return; }
            if (MessageBox.Show("¿Seguro?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            try
            {
                using (var cmd = new SQLiteCommand("DELETE FROM Materias WHERE ID = @id", Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@id", int.Parse(TxtId.Text));
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Materia eliminada."); MenuNuevo_Click(null, null); CargarDatos();
            }
            catch (Exception ex) { MessageBox.Show($"Error al eliminar: {ex.Message}"); }
        }
    }
}