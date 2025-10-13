using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using Registro.Login.Database;

namespace Registro.EditarUsuarios
{
    public partial class EditarUsuario : Window
    {
        public EditarUsuario()
        {
            InitializeComponent();
            CargarDatos();
            ContarUsuarios();
        }

        // Carga o recarga todos los usuarios en el DataGrid
        private void CargarDatos()
        {
            try
            {
                if (Globales.Conexion.State != ConnectionState.Open)
                    Globales.Conexion.Open();

                string query = "SELECT ID, Nombre, Correo, Clave, Tipo FROM Usuarios";
                using (SQLiteCommand cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    UserGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Cuando se selecciona un usuario en el Grid, llena los campos de texto
        private void UserGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserGrid.SelectedItem is DataRowView selectedRow)
            {
                TxtId.Text = selectedRow["ID"].ToString();
                TxtNombre.Text = selectedRow["Nombre"].ToString();
                TxtCorreo.Text = selectedRow["Correo"].ToString();
                TxtClave.Text = selectedRow["Clave"].ToString();
                TxtTipo.Text = selectedRow["Tipo"].ToString();
            }
        }

        // Limpia los campos para crear un nuevo usuario
        private void BotonNuevo_Click(object sender, RoutedEventArgs e)
        {
            TxtId.Clear();
            TxtNombre.Clear();
            TxtCorreo.Clear();
            TxtClave.Clear();
            TxtTipo.Clear();
            TxtNombre.Focus();
        }

        // Guarda un usuario nuevo o actualiza uno existente
        private void BotonGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNombre.Text) || string.IsNullOrWhiteSpace(TxtCorreo.Text))
            {
                MessageBox.Show("El nombre y el correo son obligatorios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string query;
            if (string.IsNullOrEmpty(TxtId.Text)) // Si no hay ID, es un usuario nuevo (INSERT)
            {
                query = "INSERT INTO Usuarios (Nombre, Clave, Correo, Tipo) VALUES (@nombre, @clave, @correo, @tipo)";
            }
            else // Si hay ID, es una actualización (UPDATE)
            {
                query = "UPDATE Usuarios SET Nombre = @nombre, Clave = @clave, Correo = @correo, Tipo = @tipo WHERE ID = @id";
            }

            try
            {
                if (Globales.Conexion.State != ConnectionState.Open)
                    Globales.Conexion.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", TxtNombre.Text);
                    cmd.Parameters.AddWithValue("@clave", TxtClave.Text);
                    cmd.Parameters.AddWithValue("@correo", TxtCorreo.Text);
                    cmd.Parameters.AddWithValue("@tipo", TxtTipo.Text);
                    
                    if (!string.IsNullOrEmpty(TxtId.Text))
                    {
                        cmd.Parameters.AddWithValue("@id", int.Parse(TxtId.Text));
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Usuario guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CargarDatos(); // Recarga el grid para mostrar los cambios
            }
        }

        // Elimina el usuario seleccionado
        private void BotonEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtId.Text))
            {
                MessageBox.Show("Por favor, selecciona un usuario para eliminar.", "Sin selección", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            // Confirmación antes de borrar
            if (MessageBox.Show("¿Estás seguro de que quieres eliminar este usuario?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                if (Globales.Conexion.State != ConnectionState.Open)
                    Globales.Conexion.Open();
                
                string query = "DELETE FROM Usuarios WHERE ID = @id";
                using (SQLiteCommand cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@id", int.Parse(TxtId.Text));
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Usuario eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BotonNuevo_Click(null, null); // Limpia los campos
                CargarDatos(); // Recarga el grid
            }
        }

        // Asegura que la conexión se cierre al cerrar la ventana
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Globales.Conexion.State == ConnectionState.Open)
            {
                Globales.Conexion.Close();
            }
        }
        
        private void ContarUsuarios()
        {
            try
            {
                if (Globales.Conexion.State != ConnectionState.Open)
                    Globales.Conexion.Open();

                string query = "SELECT COUNT(*) FROM Usuarios";
                using (SQLiteCommand cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    int total = Convert.ToInt32(cmd.ExecuteScalar());
                    UsuariosTotales.Content= total;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}