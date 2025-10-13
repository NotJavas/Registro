using System;
using System.Data.SQLite;
using System.Windows;
using Registro.Login.Database;

namespace Registro.EditarMaestros
{
    public partial class EditarMaestros_Formulario : Window
    {
        private int? _maestroId;

        public EditarMaestros_Formulario(int? maestroId = null)
        {
            InitializeComponent();
            _maestroId = maestroId;

            if (_maestroId.HasValue)
            {
                CargarDatosMaestro();
            }
        }

        private void CargarDatosMaestro()
        {
            try
            {
                using (var cmd = new SQLiteCommand("SELECT * FROM Maestros WHERE ID = @id", Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@id", _maestroId.Value);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TxtId.Text = reader["ID"].ToString();
                            TxtNombre.Text = reader["Nombre"].ToString();
                            TxtCorreo.Text = reader["Correo"].ToString();
                            TxtFacultad.Text = reader["Facultad"].ToString();
                            TxtTipo.Text = reader["Tipo"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el maestro: {ex.Message}");
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
            if (_maestroId.HasValue) // Actualizar
            {
                query = "UPDATE Maestros SET Nombre = @nombre, Correo = @correo, Facultad = @facultad, Tipo = @tipo WHERE ID = @id";
            }
            else // Insertar
            {
                query = "INSERT INTO Maestros (Nombre, Correo, Facultad, Tipo) VALUES (@nombre, @correo, @facultad, @tipo)";
            }

            try
            {
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", TxtNombre.Text);
                    cmd.Parameters.AddWithValue("@correo", TxtCorreo.Text);
                    cmd.Parameters.AddWithValue("@facultad", TxtFacultad.Text);
                    cmd.Parameters.AddWithValue("@tipo", TxtTipo.Text);

                    if (_maestroId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@id", _maestroId.Value);
                    }

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Maestro guardado con éxito.");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el maestro: {ex.Message}");
            }
        }
    }
}
