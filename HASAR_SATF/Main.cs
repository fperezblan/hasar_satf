using System;
using System.Collections.Generic;
using System.IO;
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
using HASAR_SATF.Services;
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
            FillComboBoxes();
        }

        private void btn_Conectar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                imp.Conectar(tb_IP.Text, Convert.ToInt32(tb_Puerto.Text));
                ObtenerEstado();
                btn_Actualizar.IsEnabled = true;
                btn_Conectar.Content = "Conectar nuevamente";
            }
            catch (Exception ex){ 
                MessageBox.Show("Hubo un error: " + ex.Message);
                btn_Conectar.Content = "Conectar";
                btn_Actualizar.IsEnabled = false;

                lbl_numeropuntodeventa.Content = $"Número de Punto de Venta: ";
                lbl_razonsocial.Content = $"Razón social: ";
                lbl_responsabilidadanteeliva.Content = $"Responsabilidad ante el IVA: ";
                lbl_Cuit.Content = $"CUIT: ";
                lbl_IngresosBrutos.Content = $"Ingresos Brutos: ";
                lbl_InicioActividades.Content = $"Fecha de inicio actividades: ";

                lbl_ultimocbte.Content = $"Último comprobante emitido: ";
                lbl_estadoFiscal.Content = $"Estado fiscal: ";
            }
        }
        private void ObtenerEstado()
        {
            try
            {
                HasarArgentina.RespuestaConsultarDatosInicializacion datos = imp.ConsultarDatosInicializacion();
                HasarArgentina.RespuestaConsultarEstado estado = imp.ConsultarEstado();

                lbl_numeropuntodeventa.Content = $"Número de Punto de Venta: {datos.NumeroPos.ToString()}";
                lbl_razonsocial.Content = $"Razón social: {datos.RazonSocial.ToString()}";
                lbl_responsabilidadanteeliva.Content = $"Responsabilidad ante el IVA: {datos.ResponsabilidadIVA.ToString()}";
                lbl_Cuit.Content = $"CUIT: {datos.Cuit.ToString()}";
                lbl_IngresosBrutos.Content = $"Ingresos Brutos: {datos.IngBrutos.ToString()}";
                lbl_InicioActividades.Content = $"Fecha de inicio actividades: {datos.FechaInicioActividades.ToString()}";

                lbl_ultimocbte.Content = $"Último comprobante emitido: {estado.CodigoComprobante.ToString()} - {estado.NumeroUltimoComprobante.ToString()}";
                lbl_estadoFiscal.Content = $"Estado fiscal: {estado.EstadoInterno.ToString()}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un error: " + ex.Message);
            }
        }

        private void FillComboBoxes()
        {
            cbTipoDocumento.ItemsSource = Enum.GetValues(typeof(TiposComprobante));
        }

        private void DescargarDocumento() 
        {
            if(tbComprobanteDesde.Text.Equals(string.Empty) || tbComprobanteHasta.Text.Equals(string.Empty))
            {
                MessageBox.Show("Se debe ingresar comprobante desde y comprobante hasta. En caso de necesitar un solo comprobante colocar el mismo en ambos campos.");
                return;
            }
            if (cbTipoDocumento.SelectedIndex == -1)
            {
                MessageBox.Show("Se debe seleccionar tipo de comprobante.");
                return;
            }
            //var response = imp.ObtenerRangoFechasPorZetas(66, 66);
            var cbteDesde = Convert.ToInt32(tbComprobanteDesde.Text);
            var cbteHasta = Convert.ToInt32(tbComprobanteHasta.Text);

            var tipoDeComprobante = cbTipoDocumento.SelectedItem as TiposComprobante?;

            StringBuilder documentoCompleto = new StringBuilder();
            try
            {
                documentoCompleto.Append(imp.ObtenerPrimerBloqueDocumento(cbteDesde, cbteHasta, (TiposComprobante)tipoDeComprobante, CompresionDeInformacion.NoComprime, XMLsPorBajada.XMLUnico).Informacion);
                while (true)
                {
                    documentoCompleto.Append(imp.ObtenerSiguienteBloqueDocumento().Informacion);
                }
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2147221380)
                {
                    Export.ToFile(".xml", "XML file (.xml)|*.xml", $"Comprobante {(TiposComprobante)tipoDeComprobante} - {cbteDesde.ToString()} - {cbteHasta.ToString()}", documentoCompleto);
                    return;
                }
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
                return;
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
                return;
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
                return;
            }
            MessageBox.Show("Se emitió comprobante A de prueba correctamente.");
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
                return;
            }
            MessageBox.Show("Se emitió comprobante B de prueba correctamente.");
        }

        private void btn_PruebaDNFH_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un error: " + ex.Message);
                return;
            }
            MessageBox.Show("Se emitió comprobante genérico de prueba correctamente.");
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
                return;
            }
            MessageBox.Show("Se se canceló comprobante en curso.");
        }

        private void btn_Actualizar_Click(object sender, RoutedEventArgs e)
        {
            ObtenerEstado();
        }

        private void btnObtenerDocumento_Click(object sender, RoutedEventArgs e)
        {
            DescargarDocumento();
        }
    }
}
