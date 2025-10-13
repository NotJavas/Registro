using System;
using System.Data.SQLite;
using System.Windows;
using Registro.Login.Database;

namespace Registro.EditarUsuarios
{
    public partial class EditarUsuario_Formulario : Window
    {
        private int? _userId;

        public EditarUsuario_Formulario(int? userId = null)
        {
            InitializeComponent();
            _userId = userId;

            if (_userId.HasValue)
            {
                CargarDatosUsuario();
            }
        }

        private void CargarDatosUsuario()
        {
            try
            {
                using (var cmd = new SQLiteCommand("SELECT * FROM Usuarios WHERE ID = @id", Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@id", _userId.Value);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TxtId.Text = reader["ID"].ToString();
                            TxtNombre.Text = reader["Nombre"].ToString();
                            TxtCorreo.Text = reader["Correo"].ToString();
                            TxtClave.Text = reader["Clave"].ToString();
                            TxtTipo.Text = reader["Tipo"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el usuario: {ex.Message}");
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNombre.Text) || string.IsNullOrWhiteSpace(TxtCorreo.Text))
            {
                MessageBox.Show("Nombre y Correo son campos obligatorios.");
                return;
            }

            string query;
            if (_userId.HasValue)
            {
                query = "UPDATE Usuarios SET Nombre = @nombre, Correo = @correo, Clave = @clave, Tipo = @tipo WHERE ID = @id";
            }
            else
            {
                query = "INSERT INTO Usuarios (Nombre, Correo, Clave, Tipo) VALUES (@nombre, @correo, @clave, @tipo)";
            }

            try
            {
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", TxtNombre.Text);
                    cmd.Parameters.AddWithValue("@correo", TxtCorreo.Text);
                    cmd.Parameters.AddWithValue("@clave", TxtClave.Text);
                    cmd.Parameters.AddWithValue("@tipo", TxtTipo.Text);

                    if (_userId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@id", _userId.Value);
                    }

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Usuario guardado con éxito.");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el usuario: {ex.Message}");
            }
        }
    }
}
