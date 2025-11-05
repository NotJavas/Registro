using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using Registro.Login.Database;

namespace Registro.EditarUsuario
{
    public partial class EditarUsuario : Window
    {
        private int? _userId;

        public EditarUsuario()
        {
            InitializeComponent();
            CargarDatos();
        }

        public EditarUsuario(int userId)
        {
            InitializeComponent();
            _userId = userId;
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                if (Globales.Conexion.State != ConnectionState.Open)
                    Globales.Conexion.Open();

                string query = "SELECT ID, Nombre, Correo, Clave, Tipo FROM Usuarios";
                if (_userId.HasValue)
                {
                    query += " WHERE ID = " + _userId.Value;
                }

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

        private void MenuNuevo_Click(object sender, RoutedEventArgs e)
        {
            var form = new EditarUsuario();
            form.Owner = this;
            if (form.ShowDialog() == true)
            {
                CargarDatos();
            }
        }

        private void MenuEditar_Click(object sender, RoutedEventArgs e)
        {
            if (UserGrid.SelectedItem is DataRowView selectedRow)
            {
                var form = new EditarUsuario(Convert.ToInt32(selectedRow["ID"]));
                form.Owner = this;
                if (form.ShowDialog() == true)
                {
                    CargarDatos();
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un usuario para editar.");
            }
        }

        private void MenuEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (UserGrid.SelectedItem is DataRowView selectedRow)
            {
                if (MessageBox.Show("¿Estás seguro de que quieres eliminar este usuario?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var cmd = new SQLiteCommand("DELETE FROM Usuarios WHERE ID = @id", Globales.Conexion))
                        {
                            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(selectedRow["ID"]));
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Usuario eliminado con éxito.");
                        CargarDatos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar el usuario: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un usuario para eliminar.");
            }
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
