using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Input;
using Registro.Login.Database;

namespace Registro.EditarAlumnos
{
    public partial class EditarAlumnos_Formulario : Window
    {
        private int? _expediente;

        public EditarAlumnos_Formulario(int? expediente = null)
        {
            InitializeComponent();
            _expediente = expediente;

            if (_expediente.HasValue)
            {
                TxtExpediente.IsReadOnly = true;
                CargarDatosAlumno();
            }
        }

        private void CargarDatosAlumno()
        {
            try
            {
                using (var cmd = new SQLiteCommand("SELECT * FROM Alumnos WHERE Expediente = @expediente", Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@expediente", _expediente.Value);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TxtExpediente.Text = reader["Expediente"].ToString();
                            TxtNombre.Text = reader["Nombre"].ToString();
                            TxtApellidos.Text = reader["Apellidos"].ToString();
                            TxtCorreo.Text = reader["Correo"].ToString();
                            TxtTelefono.Text = reader["Telefono"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el alumno: {ex.Message}");
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtExpediente.Text) || string.IsNullOrWhiteSpace(TxtNombre.Text) || string.IsNullOrWhiteSpace(TxtApellidos.Text))
            {
                MessageBox.Show("Expediente, Nombre y Apellidos son campos obligatorios.");
                return;
            }

            if (!int.TryParse(TxtExpediente.Text, out int expediente))
            {
                MessageBox.Show("El expediente debe ser un número válido.");
                return;
            }

            string query;
            if (_expediente.HasValue) // Actualizar
            {
                query = "UPDATE Alumnos SET Nombre = @nombre, Apellidos = @apellidos, Correo = @correo, Telefono = @telefono WHERE Expediente = @expediente";
            }
            else // Insertar
            {
                if (ExpedienteExiste(expediente))
                {
                    MessageBox.Show("El expediente ya existe en la base de datos.");
                    return;
                }
                query = "INSERT INTO Alumnos (Expediente, Nombre, Apellidos, Correo, Telefono) VALUES (@expediente, @nombre, @apellidos, @correo, @telefono)";
            }

            try
            {
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@expediente", expediente);
                    cmd.Parameters.AddWithValue("@nombre", TxtNombre.Text);
                    cmd.Parameters.AddWithValue("@apellidos", TxtApellidos.Text);
                    cmd.Parameters.AddWithValue("@correo", TxtCorreo.Text);
                    cmd.Parameters.AddWithValue("@telefono", TxtTelefono.Text);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Alumno guardado con éxito.");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el alumno: {ex.Message}");
            }
        }

        private bool ExpedienteExiste(int expediente)
        {
            try
            {
                using (var cmd = new SQLiteCommand("SELECT COUNT(1) FROM Alumnos WHERE Expediente = @expediente", Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@expediente", expediente);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al verificar el expediente: {ex.Message}");
                return true; // Asumir que existe para prevenir duplicados en caso de error
            }
        }
    }
}
