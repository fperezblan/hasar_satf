using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HasarArgentina;

namespace HASAR_SATF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main : Window
    {
        HasarArgentina.ImpresoraFiscalRG3561 imp = new HasarArgentina.ImpresoraFiscalRG3561();

        public Main()
        {
            InitializeComponent();
        }

        private void btn_Conectar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                imp.Conectar(tb_IP.Text, Convert.ToInt32(tb_Puerto.Text));
                MessageBox.Show("Conexion exitosa");

                HasarArgentina.RespuestaConsultarDatosInicializacion datos = imp.ConsultarDatosInicializacion();
                HasarArgentina.RespuestaConsultarEstado estado = imp.ConsultarEstado();

                
                lbl_numeropuntodeventa.Content = datos.NumeroPos.ToString();
                lbl_razonsocial.Content= datos.RazonSocial.ToString();
                lbl_responsabilidadanteeliva.Content = datos.ResponsabilidadIVA.ToString();

                lbl_ultimocbte.Content = estado.NumeroUltimoComprobante.ToString();
            }
            catch (Exception ex){ 
                MessageBox.Show("Hubo un error: " + ex.Message);
            }
        }

        private void btn_CierreZ_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                imp.CerrarJornadaFiscal(HasarArgentina.TipoReporte.ReporteZ);
                MessageBox.Show("Se envió comando con éxito.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un error: " + ex.Message);
            }
        }

        private void btn_CierreX_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                imp.CerrarJornadaFiscal(HasarArgentina.TipoReporte.ReporteX);
                MessageBox.Show("Se envió comando con éxito.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un error: " + ex.Message);
            }
        }

        private void btn_PruebaFTA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                imp.CargarDatosCliente("Empresa Prueba 1", "20222222223", TiposDeResponsabilidadesCliente.ResponsableInscripto, TiposDeDocumentoCliente.TipoCUIT, "Domicilio Genérico");
                imp.AbrirDocumento(TiposComprobante.FacturaA);
                imp.ImprimirItem("Item prueba",
                    1,
                    0.01,
                    HasarArgentina.CondicionesIVA.Gravado,
                    21.0,
                    HasarArgentina.ModosDeMonto.ModoSumaMonto,
                    HasarArgentina.ModosDeImpuestosInternos.IIFijoKIVA,
                    0.0,
                    HasarArgentina.ModosDeDisplay.DisplayNo,
                    HasarArgentina.ModosDePrecio.ModoPrecioTotal, 1, "123");
                imp.ImprimirPago("Efectivo", 0.01, HasarArgentina.ModosDePago.Pagar);
                imp.CerrarDocumento();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un error: " + ex.Message);
            }
        }

        private void btn_PruebaFTB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                imp.AbrirDocumento(TiposComprobante.FacturaB);
                imp.ImprimirItem("Item prueba",
                    1,
                    0.01,
                    HasarArgentina.CondicionesIVA.Gravado,
                    21.0,
                    HasarArgentina.ModosDeMonto.ModoSumaMonto,
                    HasarArgentina.ModosDeImpuestosInternos.IIFijoKIVA,
                    0.0,
                    HasarArgentina.ModosDeDisplay.DisplayNo,
                    HasarArgentina.ModosDePrecio.ModoPrecioTotal, 1, "123");
                imp.ImprimirPago("Efectivo", 0.01, HasarArgentina.ModosDePago.Pagar);
                imp.CerrarDocumento();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un error: " + ex.Message);
            }
        }

        private void btn_PruebaDNFH_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un error: " + ex.Message);
            }
        }

        private void btn_CancelarDocumentoActual_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                imp.Cancelar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un error: " + ex.Message);
            }
        }
    }
}
