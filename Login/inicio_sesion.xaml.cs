using System;
using System.Windows;
using System.Data.SQLite;
using Registro.Principal;
using Registro.Login.Database;
using System.Windows.Controls;

namespace Registro.Login
{
    public partial class InicioSesion : Window
    {
        public InicioSesion()
        {
            InitializeComponent();
            CargarUsuariosEnCombo();
        }

        private void BotonLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckBoxMostrarClave.IsChecked == true)
                {
                    TxtClave.Password = TxtClaveVisible.Text;
                }

                string usuario = TxtUsuario.Text;
                string clave = TxtClave.Password;

                if (ValidarUsuario(usuario, clave))
                {
                    FormPrincipal ventanaPrincipal = new FormPrincipal();
                    ventanaPrincipal.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Usuario o clave incorrectos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    TxtClave.Clear();
                    TxtClaveVisible.Clear(); // Limpiamos también el TextBox visible
                    TxtClave.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Excepción", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidarUsuario(string usuario, string clave)
        {
            string sql = "SELECT ID, Nombre FROM Usuarios WHERE Nombre = @usuario AND Clave = @clave";
            
            using (var cmd = new SQLiteCommand(sql, Globales.Conexion))
            {
                cmd.Parameters.AddWithValue("@usuario", usuario);
                cmd.Parameters.AddWithValue("@clave", clave);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Globales.IdUsuario = reader["ID"].ToString();
                        Globales.NombreUsuario = reader["Nombre"].ToString();
                        return true;
                    }
                }
            }
            return false;
        }

        private void CargarUsuariosEnCombo()
        {
            try
            {
                string sql = "SELECT Nombre FROM Usuarios";
                
                using (var cmd = new SQLiteCommand(sql, Globales.Conexion))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ComboBoxUsuarios.Items.Add(reader["Nombre"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}");
            }
        }

        private void ComboBoxUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxUsuarios.SelectedItem != null)
            {
                TxtUsuario.Text = ComboBoxUsuarios.SelectedItem.ToString();
            }
        }
        
        private void CheckBoxMostrarClave_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxMostrarClave.IsChecked == true)
            {

                TxtClaveVisible.Text = TxtClave.Password;
                TxtClaveVisible.Visibility = Visibility.Visible;
                TxtClave.Visibility = Visibility.Collapsed;
            }
            else
            {
                TxtClave.Password = TxtClaveVisible.Text;
                TxtClave.Visibility = Visibility.Visible;
                TxtClaveVisible.Visibility = Visibility.Collapsed;
            }
        }
    }
}