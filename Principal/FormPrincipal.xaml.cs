using System.Windows;

namespace Registro.Principal
{
    public partial class FormPrincipal : Window
    {
        public FormPrincipal()
        {
            InitializeComponent();
        }
        
        private void BotonUsuarios_Click(object sender, RoutedEventArgs e)
        {
            var ventanaUsuarios = new EditarUsuario.EditarUsuario();
            ventanaUsuarios.ShowDialog();
        }
        
        private void BotonAlumnos_Click(object sender, RoutedEventArgs e)
        {
            var ventanaAlumnos = new EditarAlumnos.EditarAlumnos();
            ventanaAlumnos.ShowDialog();
        }
        
        private void BotonMaestros_Click(object sender, RoutedEventArgs e)
        {
            var ventanaMaestros = new EditarMaestros.EditarMaestros();
            ventanaMaestros.ShowDialog();
        }
        
        private void BotonMaterias_Click(object sender, RoutedEventArgs e)
        {
            var ventanaMaterias = new EditarMaterias.EditarMaterias();
            ventanaMaterias.ShowDialog();
        }
    }
}
