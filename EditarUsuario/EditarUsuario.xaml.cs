using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using Registro.Login.Database;
using Registro.Utils;

namespace Registro.EditarUsuario
{
    public partial class EditarUsuario : Window
    {
        public EditarUsuario()
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

                const string query = "SELECT ID, Nombre, Correo, Clave, Tipo FROM Usuarios";
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    var adapter = new SQLiteDataAdapter(cmd);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    UserGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UserGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserGrid.SelectedItem is DataRowView selectedRow)
            {
                TxtNombre.Text = selectedRow["Nombre"].ToString();
                TxtCorreo.Text = selectedRow["Correo"].ToString();
                TxtClave.Text = selectedRow["Clave"].ToString();
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
            bool isNew = UserGrid.SelectedItem == null;

            if (isNew)
            {
                query = "INSERT INTO Usuarios (Nombre, Correo, Clave, Tipo) VALUES (@nombre, @correo, @clave, @tipo)";
            }
            else
            {
                query = "UPDATE Usuarios SET Nombre = @nombre, Correo = @correo, Clave = @clave, Tipo = @tipo WHERE ID = @id";
            }

            try
            {
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", TxtNombre.Text);
                    cmd.Parameters.AddWithValue("@correo", TxtCorreo.Text);
                    cmd.Parameters.AddWithValue("@clave", TxtClave.Text);
                    cmd.Parameters.AddWithValue("@tipo", TxtTipo.Text);

                    if (!isNew)
                    {
                        var selectedRow = (DataRowView)UserGrid.SelectedItem;
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(selectedRow["ID"]));
                    }

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Usuario guardado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarDatos();
                Nuevo_Click(null, null); // Limpiar formulario
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            UserGrid.SelectedItem = null;
            TxtNombre.Clear();
            TxtCorreo.Clear();
            TxtClave.Clear();
            TxtTipo.Clear();
            TxtNombre.Focus();
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (UserGrid.SelectedItem is DataRowView selectedRow)
            {
                var result = MessageBox.Show("¿Estás seguro de que quieres eliminar este usuario?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int userId = Convert.ToInt32(selectedRow["ID"]);
                        const string query = "DELETE FROM Usuarios WHERE ID = @id";
                        using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                        {
                            cmd.Parameters.AddWithValue("@id", userId);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Usuario eliminado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        CargarDatos();
                        Nuevo_Click(null, null); // Limpiar formulario
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar el usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un usuario para eliminar.", "Selección Requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            string fileName = $"Reporte_Usuarios_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            ExcelExportHelper.ExportToExcel((DataView)UserGrid.ItemsSource, "Reporte de Usuarios", fileName);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Globales.Conexion.State == ConnectionState.Open)
            {
                Globales.Conexion.Close();
            }
        }
    }
}
