using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using Registro.Login.Database;

namespace Registro.EditarMaestros
{
    public partial class EditarMaestros : Window
    {
        public EditarMaestros() { InitializeComponent(); }
        private void Window_Loaded(object sender, RoutedEventArgs e) { try { Globales.Conexion.Open(); CargarDatos(); } catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); } }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) { if (Globales.Conexion.State == ConnectionState.Open) Globales.Conexion.Close(); }

        private void CargarDatos()
        {
            // Consulta corregida
            string query = "SELECT ID, Nombre, Correo, Facultad, Tipo FROM Maestros";
            try {
                using (var adapter = new SQLiteDataAdapter(query, Globales.Conexion))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    MaestrosGrid.ItemsSource = dt.DefaultView;
                }
            } catch (Exception ex) { MessageBox.Show($"Error al cargar: {ex.Message}"); }
        }

        private void MaestrosGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MaestrosGrid.SelectedItem is DataRowView row)
            {
                // Campos corregidos
                TxtId.Text = row["ID"].ToString();
                TxtNombre.Text = row["Nombre"].ToString();
                TxtCorreo.Text = row["Correo"].ToString();
                TxtFacultad.Text = row["Facultad"].ToString();
                TxtTipo.Text = row["Tipo"].ToString();
            }
        }

        private void MenuNuevo_Click(object sender, RoutedEventArgs e)
        {
            TxtId.Clear(); TxtNombre.Clear(); TxtCorreo.Clear(); TxtFacultad.Clear(); TxtTipo.Clear();
            TxtNombre.Focus();
        }

        private void MenuGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNombre.Text)) { MessageBox.Show("El nombre es obligatorio."); return; }

            // Consultas corregidas
            string query = string.IsNullOrEmpty(TxtId.Text)
                ? "INSERT INTO Maestros (Nombre, Correo, Facultad, Tipo) VALUES (@nom, @correo, @fac, @tipo)"
                : "UPDATE Maestros SET Nombre = @nom, Correo = @correo, Facultad = @fac, Tipo = @tipo WHERE ID = @id";
            try
            {
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@nom", TxtNombre.Text);
                    cmd.Parameters.AddWithValue("@correo", TxtCorreo.Text);
                    cmd.Parameters.AddWithValue("@fac", TxtFacultad.Text);
                    cmd.Parameters.AddWithValue("@tipo", Convert.ToInt32(TxtTipo.Text));
                    if (!string.IsNullOrEmpty(TxtId.Text)) cmd.Parameters.AddWithValue("@id", int.Parse(TxtId.Text));
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Maestro guardado."); CargarDatos();
            }
            catch (Exception ex) { MessageBox.Show($"Error al guardar: {ex.Message}"); }
        }

        private void MenuEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtId.Text)) { MessageBox.Show("Selecciona un maestro a eliminar."); return; }
            if (MessageBox.Show("¿Seguro?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            try
            {
                using (var cmd = new SQLiteCommand("DELETE FROM Maestros WHERE ID = @id", Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@id", int.Parse(TxtId.Text));
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Maestro eliminado."); MenuNuevo_Click(null, null); CargarDatos();
            }
            catch (Exception ex) { MessageBox.Show($"Error al eliminar: {ex.Message}"); }
        }
    }
}