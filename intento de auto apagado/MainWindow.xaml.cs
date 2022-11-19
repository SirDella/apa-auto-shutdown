using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.Diagnostics;
//using System.Threading;
using System.Windows.Threading;
//using System.Text.RegularExpressions;
using System.Globalization;
//using System.Xml;
//using System.Net;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.Win32;
using forms = System.Windows.Forms;
using System.Threading;
using System.IO;

namespace intento_de_auto_apagado
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        int TIEMPO_ESPERA = 15;
        DispatcherTimer cpuapagar;
        DispatcherTimer netapagar;
        DispatcherTimer avisointernet;
        DispatcherTimer timer15;
        DispatcherTimer timer;
        DispatcherTimer timerParaBoludear;
        DispatcherTimer timerDelayInicio;
        DispatcherTimer timerAnimacionInicio;
        DispatcherTimer timerMain;
        int cpusocontador;
        int netusocontador;

        float netuso;
        int netusoint;
        float netuso2;
        int netusoint2; //variables que estaban adentro de la función

        float cpuso;
        int cpusoint;

        int timer15contador;
        PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        //PerformanceCounter netCounter = new PerformanceCounter("Adaptador de red", "Bytes recibidos/s", "Intel[R] Dual Band Wireless-AC 3165");
        //PerformanceCounter netCounter2 = new PerformanceCounter("Adaptador de red", "Bytes enviados/s", "Intel[R] Dual Band Wireless-AC 3165");
        //int umbral = 100;
        //bool cambiarumbral = false;
        bool apagarseguncpueinternet = false;
        bool salidaapagadocpu = false;
        bool salidaapagadointernet = false;
        int rbint = 0;
        bool iniciado = false;
        bool iniciado2 = false;
        int umbralRed;
        int umbralCpu;
        int minutosTimer;
        int segundosTimer;
        int umbralTiempoInt;

        bool apagarnohibernar = true;
        bool comandoEnviado = false;

        Brush brushVerde = new SolidColorBrush(Color.FromArgb(255, 173, 255, 176));
        Brush brushAmarilla = new SolidColorBrush(Color.FromArgb(255, 252, 255, 176));
        Brush brushRoja = new SolidColorBrush(Color.FromArgb(255, 255, 112, 112));

        SoundPlayer tada = new SoundPlayer(Properties.Resources.tada);
        SoundPlayer balloon = new SoundPlayer(Properties.Resources.Windows_Balloon);
        SoundPlayer ding = new SoundPlayer(Properties.Resources.Windows_Ding);

        //Variables del nuevo metodo de uso de red:
        long lectura;
        long subidaPrev = 0, subida = 0, subidadif = 0;
        long bajadaPrev = 0, bajada = 0, bajadadif = 0;
        NetworkInterface[] interfaces
            = NetworkInterface.GetAllNetworkInterfaces();

        //Variables del promedio: 
        int[] redVector = new int[30];
        int[] redVector2 = new int[30];
        int[] cpuVector = new int[30];
        bool usarPromedio = false;
        int ipromRed = 0;
        int ipromCpu = 0;
        int promedioRed = 0;
        int promedioRed2 = 0;
        int promedioCpu = 0;

        //Variables del aviso de consumo:
        int redAvisoUmbral = 0;
        int cpuAvisoUmbral = 0;
        bool redAvisoOn = false;
        bool cpuAvisoOn = false;
        int dings;

        //Variables del boludeo:
        int R = 255;
        int G = 250;
        int B = 250;
        int step = 2;
        double angle = 45;
        int minRGB = 0;

        //Variables de la animacion:
        float brillo1=26, brillo2=26;
        int step2 = 0;
        int deley = 0;
        double angle2 = 45;

        //temporizador 2.0
        int rbTemporizador = 1;
        String horaTempo = "";
        bool tempoPorHora = false;
        string time = "";

        //Vínculo de apagados:
        bool vincularapagados = false;
        bool salidaApagadoTimer = false;
        private bool isDataDirty;

        //Funcion apagar:
        int modulo;

        forms.NotifyIcon notifyIcon = new forms.NotifyIcon();


        public MainWindow()
        {
            //Tengo que poner algunas cosas acá porque sino se queja de que se usa como tipo en vez de metodo o algo por el estilo
            InitializeComponent();
            //label1.Content = "hola mundo de mierda, saquenme de latinoamérica";
            //si, este es mi hola mundo en C#
            cpuapagar = new DispatcherTimer();
            netapagar = new DispatcherTimer();
            avisointernet = new DispatcherTimer();
            timer15 = new DispatcherTimer();
            timer = new DispatcherTimer();
            timerParaBoludear = new DispatcherTimer();
            timerDelayInicio = new DispatcherTimer();
            timerAnimacionInicio = new DispatcherTimer();
            timerMain = new DispatcherTimer();

            cpuapagar.Interval = TimeSpan.FromMilliseconds(1000);
            netapagar.Interval = TimeSpan.FromMilliseconds(1000);
            avisointernet.Interval = TimeSpan.FromMilliseconds(1000);
            timer15.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Interval = TimeSpan.FromMilliseconds(59999);
            timerParaBoludear.Interval = TimeSpan.FromMilliseconds(16);
            timerDelayInicio.Interval = TimeSpan.FromMilliseconds(2000);
            timerAnimacionInicio.Interval = TimeSpan.FromMilliseconds(10);
            timerMain.Interval = TimeSpan.FromMilliseconds(1000);

            cpuapagar.Tick += new EventHandler(cpuReposoApagar);
            netapagar.Tick += new EventHandler(internetReposoApagar);
            //avisointernet.Tick += new EventHandler(avisoInternet);
            timer15.Tick += new EventHandler(funcionapagar);
            timer.Tick += new EventHandler(funciontemporizador);
            timerParaBoludear.Tick += new EventHandler(funcTimerParaBoludear);
            timerDelayInicio.Tick += new EventHandler(funcTimerDelayInicio);
            timerAnimacionInicio.Tick += new EventHandler(funcTimerAnimacionInicio);
            timerMain.Tick += new EventHandler(main);

            button.Background = brushVerde;
            buttonTempo.Background = brushVerde;
            
            //timerMain.Start();
            rectangulo.Visibility = Visibility.Visible;
            timerAnimacionInicio.Start();
            cargarSettings();
            

            char barrita = (char)92;
            String preRuta = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString();
            String ruta = "";
            int startIndex = 0;
            int i = 0;
            
            for (i = preRuta.Length-1; i > 0; i--)
            {
                if (preRuta[i] == barrita)
                {
                    startIndex = i;
                    i = 0;
                }
            }
            for(i=0; i<startIndex; i++)
            {
                ruta += preRuta[i];
            }
            

            //ruta.Remove(startIndex, (ruta.Length - startIndex - 1));
            ruta = ruta + barrita + "Resources" + barrita + "Shutdown.ico";
            if (File.Exists(ruta)) Properties.Settings.Default.iconLocationSet = ruta;
            Properties.Settings.Default.Save();

            /*
            String mensaje = Properties.Settings.Default.iconLocationSet;
            MessageBoxButton buttons = MessageBoxButton.OK;
            MessageBox.Show(mensaje, "XD", buttons);
            */


            //timerDelayInicio.Start();

            //List<String> values = new List<String>();
            //foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            //{
            //    values.Add(nic.Name);
            //}

            //Thread.Sleep(1000);
        }

        private void main(object sender, EventArgs e)
        {
            visibilidadBotones();
            if (iniciado2)
            {
                if ((bool)rbTempo1.IsChecked) //temporizador
                {
                    int horas = segundosTimer / 60 / 60;
                    int minutos = (segundosTimer / 60) - horas * 60;
                    int segundos = segundosTimer % 60;
                    labelTempo.Content = horas.ToString() + "h " + minutos.ToString() + "m " + segundos.ToString() + "s remaining";

                    if (segundosTimer == 0) //apagar
                    {
                        temporizadorApagar();
                    }

                    if (segundosTimer > 0) segundosTimer = segundosTimer - 1;
                }
                else //segun hora
                {
                    string time = DateTime.Now.ToString("HH:mm");
                    if (time[0] == '0' && time.Length == 5) time = time.Remove(0, 1);
                    textApagadoTemporizado.Content = "Timed shutdown (" + time + ")";
                    if (apagarnohibernar) labelTempo.Content = "Shutting down at " + horaTempo;
                    if (!apagarnohibernar) labelTempo.Content = "Sleeping at " + horaTempo;

                    if (time  ==  horaTempo)
                    {
                        temporizadorApagar();
                    }
                }
            }

        }

        private void temporizadorApagar()
        {
            if (!vincularapagados)
            {
                iniciado2 = false;
                buttonTempo.Background = brushRoja;
                apagar(2);
            }
            else
            {
                salidaApagadoTimer = true;
                buttonTempo.Background= brushRoja;
                labelTempo.Content = "Waiting for the other module...";
            }
        }

        private void apagar(int mod)
        {
            timer15contador = 0;
            timer15.Start();
            modulo = mod;
        }

        private void visibilidadBotones()
        {
            //minutosTimer = stringAInt(editTextTempo.Text);
            //horaTempo = editTextTempo_Copy.Text;

            if (iniciado && iniciado2 || (bool)cbEnlazar.IsChecked)
            {
                if (cbEnlazar.Opacity < 1) cbEnlazar.Opacity = 1.0;
            }
            else if (cbEnlazar.Opacity == 1) cbEnlazar.Opacity = 0.5;

            if (iniciado || iniciado2 || (bool)cbGuardarEstado.IsChecked)
            {
                if (cbGuardarEstado.Opacity < 1) cbGuardarEstado.Opacity = 1;
            }
            else if (cbGuardarEstado.Opacity == 1) cbGuardarEstado.Opacity = 0.5;
            if (((iniciado && !iniciado2) || (!iniciado && iniciado2)) && (bool)cbEnlazar.IsChecked)
            {
                cbEnlazar.Foreground = brushRoja;
            }
            else
            {
                cbEnlazar.Foreground = new SolidColorBrush(Color.FromArgb(255, 185, 185, 185));
            }

        }

        private void funcTimerAnimacionInicio(object sender, EventArgs e)
        {
            if (deley == 0)
            {
                label1.Opacity = 0;
                labelm.Opacity = 0;
                rbRed.Opacity = 0;
                rbCpu.Opacity = 0;
                rbAmbos.Opacity = 0;
                rbHibernar.Opacity = 0;
                rbApagar.Opacity = 0;
                tbCpu.Opacity = 0;
                tbRed.Opacity = 0;
                editTextTiempoUmbral.Opacity = 0;
                porcent.Opacity = 0;
                kbs.Opacity = 0;
                menora1.Opacity = 0;
                menora2.Opacity = 0;
                holdfor.Opacity = 0;
                segundosUmbral.Opacity = 0;
                button.Opacity = 0;
                textApagadoTemporizado.Opacity = 0;
                rbTempo1.Opacity = 0;
                editTextTempo.Opacity = 0;
                textboxtempo.Opacity = 0;
                labelStatus.Opacity = 0;
                labelStatus_Copy.Opacity = 0;
                tbTempo2.Opacity = 0;
                editTextTempo_Copy.Opacity = 0;
                textboxtempo_Copy.Opacity = 0;
                labelTempo.Opacity = 0;
                buttonTempo.Opacity = 0;
                label1_Copy.Opacity = 0;
                cbUsarPromedio.Opacity = 0;
                label1_Copy1.Opacity = 0;
                cbStartOnBoot.Opacity = 0;
                cbEnlazar.Opacity = 0;
                cbGuardarEstado.Opacity = 0;
                creds.Opacity = 0;
                ver.Opacity = 0;
                donate.Opacity = 0;
                barra.Opacity = 0;
                barra2.Opacity = 0;
                barra3.Opacity = 0;
            }
            if (5 < deley && label1.Opacity < 1) label1.Opacity += 0.04;
            if (6 < deley && labelm.Opacity < 1) labelm.Opacity += 0.04;
            if (7 < deley && rbRed.Opacity < 1) { rbRed.Opacity += 0.04; menora1.Opacity += 0.04; tbRed.Opacity += 0.04; kbs.Opacity += 0.04; }
            if (8 < deley && rbCpu.Opacity < 1) { rbCpu.Opacity += 0.04; menora2.Opacity += 0.04; porcent.Opacity += 0.04; tbCpu.Opacity += 0.04; }
            if (9 < deley && rbAmbos.Opacity < 1) rbAmbos.Opacity += 0.04;
            if (10 < deley && holdfor.Opacity < 1) { holdfor.Opacity += 0.04; editTextTiempoUmbral.Opacity += 0.04; segundosUmbral.Opacity += 0.04; }
            if (11 < deley && button.Opacity < 1) button.Opacity += 0.04;
            if (12 < deley && labelStatus.Opacity < 1) labelStatus.Opacity += 0.04;
            if (13 < deley && labelStatus_Copy.Opacity < 1) labelStatus_Copy.Opacity += 0.04;
            if (14 < deley && textApagadoTemporizado.Opacity < 1) textApagadoTemporizado.Opacity += 0.04;
            if (15 < deley && rbTempo1.Opacity < 1) { rbTempo1.Opacity += 0.04; editTextTempo.Opacity += 0.04; textboxtempo.Opacity += 0.04; }
            if (16 < deley && tbTempo2.Opacity < 1) { tbTempo2.Opacity += 0.04; editTextTempo_Copy.Opacity += 0.04; textboxtempo_Copy.Opacity += 0.04; buttonTempo.Opacity += 0.04; }
            if (17 < deley && labelTempo.Opacity < 1) labelTempo.Opacity += 0.04;
            if (18 < deley && label1_Copy.Opacity < 1) label1_Copy.Opacity += 0.04;
            if (19 < deley && rbApagar.Opacity < 1) rbApagar.Opacity += 0.04;
            if (20 < deley && rbHibernar.Opacity < 1) rbHibernar.Opacity += 0.04;
            if (21 < deley && cbUsarPromedio.Opacity < 1) cbUsarPromedio.Opacity += 0.04;
            if (22 < deley && label1_Copy1.Opacity < 1) label1_Copy1.Opacity += 0.04;
            if (23 < deley && cbStartOnBoot.Opacity < 1) cbStartOnBoot.Opacity += 0.04;
            if (24 < deley && cbGuardarEstado.Opacity < 0.5) cbGuardarEstado.Opacity += 0.04;
            if (25 < deley && cbEnlazar.Opacity < 0.5) cbEnlazar.Opacity += 0.04;
            if (26 < deley && creds.Opacity < 1) creds.Opacity += 0.04;
            if (27 < deley && ver.Opacity < 1) ver.Opacity += 0.04;
            if (28 < deley && donate.Opacity < 1) donate.Opacity += 0.04;
            if (28 < deley && barra.Opacity < 0.5) barra.Opacity += 0.04;
            if (28 < deley && barra2.Opacity < 0.5) barra2.Opacity += 0.04;
            if (28 < deley && barra3.Opacity < 0.5) barra3.Opacity += 0.04;



            if (5 < deley && deley < 15 && brillo1 <= 50)
            {
                brillo1 += (float)2.4;
            }
            if (5 < deley && deley < 15 && brillo1<=50)
            {
                brillo1 += (float)2.4;
            }
            if (15 < deley && deley < 65 && brillo1 >= 26)
            {
                brillo1 -= (float)0.48;
            }

            if (12 < deley && deley < 22 && brillo2 <= 50)
            {
                brillo2 += (float)2.4;
            }
            if (22 < deley && deley < 72 && brillo2 >= 26)
            {
                brillo2 -= (float)0.48;
            }

            deley++;
            //label1.Content = deley.ToString();
            if (deley == 72)
            {
                timerAnimacionInicio.Stop();
                deley = 0;
                timerMain.Start();
            }
            //window.Background = new LinearGradientBrush(Color.FromArgb(255, (byte)brillo1, (byte)brillo1, (byte)brillo1), Color.FromArgb(255, (byte)brillo2, (byte)brillo2, (byte)brillo2), angle2);
            rectangulo.Fill = new LinearGradientBrush(Color.FromArgb((byte)((brillo1-26)*1.5), 255, 255, 255), Color.FromArgb((byte)((brillo2-26)*1.5), 255, 255, 255), angle);
        }
    

        private void messageBoxFunc(String mensaje)
        {
            MessageBoxButton buttons = MessageBoxButton.OK;
            MessageBox.Show(mensaje, "XD", buttons);
        }

        private void funcTimerDelayInicio(object sender, EventArgs e)
        {

        }

        private void funcTimerParaBoludear(object sender, EventArgs e)
        {

            if (step == 2)
            {
                G += 5;
                if (G == 255) step++;
            }
            if (step == 3)
            {
                R -= 5;
                if (R == minRGB) step++;
            }
            if (step == 4)
            {
                B += 5;
                if (B == 255) step++;
            }
            if (step == 5)
            {
                G -= 5;
                if (G == minRGB) step++;
            }
            if (step == 6)
            {
                R += 5;
                if (R == 255) step++;
            }
            if (step == 7)
            {
                B -= 5;
                if (B == minRGB)
                {
                    step = 2;
                    minRGB += 5;
                }
            }

            //window.Background = new SolidColorBrush(Color.FromArgb(255, (byte)R, (byte)G, (byte)B));

            window.Background = new LinearGradientBrush(Color.FromArgb(255, (byte)B, (byte)R, (byte)G), Color.FromArgb(255, (byte)R, (byte)G, (byte)B), angle);
            if (minRGB == 255)
            {
                timerParaBoludear.Stop();
                minRGB = 0;
                window.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
        }

        private void funciontemporizador(object sender, EventArgs e)
        {
            funcionTemporizador();
        }

        private void funcionTemporizador() 
        {
            /*
            string time = DateTime.Now.ToString("HH:mm");
            if (time[0] == '0') time.Remove(0);
            textApagadoTemporizado.Content = "Timed Shutdown " + time;

            if (!tempoPorHora) minutosTimer -= 1;
            if (minutosTimer == 0 && !tempoPorHora)
            {
                timer.Stop();
                iniciado2 = false;
                if (!vincularapagados)
                {
                    buttonTempo.Background = brushRoja;
                    timer15contador = 0;
                    timer15.Start();
                }
                else
                {
                    salidaApagadoTimer = true;
                }
            }
            if (!tempoPorHora)
            {
                if (minutosTimer != 1) labelTempo.Content = minutosTimer + " minutes remaining";
                else labelTempo.Content = minutosTimer + " minute remaining";
            }

            if (tempoPorHora)
            {
                if (apagarnohibernar) labelTempo.Content = "Shutting down at " + horaTempo;
                if (!apagarnohibernar) labelTempo.Content = "Sleeping at " + horaTempo;
                if (time.Equals(horaTempo))
                {
                    if (!vincularapagados)
                    {
                        buttonTempo.Background = brushRoja;
                        timer15contador = 0;
                        timer15.Start();
                    }
                    else
                    {
                        salidaApagadoTimer = true;
                    }
                }
            }
            */
        }

        private void funcionapagar(object sender, EventArgs e)
        {
            if (timer15contador == 0) funcionMaximizar();
            cpuapagar.Stop();
            netapagar.Stop();
            apagarseguncpueinternet = false;
            if (timer15contador == 0)
            {
                Process.Start("shutdown", "/s /t 60");
                comandoEnviado = true;
            }
            if (timer15contador == 1)
            {
                Process.Start("shutdown", "/a");
                comandoEnviado = false;
            }
            if (timer15contador == TIEMPO_ESPERA + 1)
            {
                timer15.Stop();
                labelStatus.Content = "Ended";
            }



            if (apagarnohibernar)
            {
                if (timer15contador == TIEMPO_ESPERA + 1)
                {
                    Process.Start("shutdown", "/s /t 1");
                    System.Windows.Application.Current.Shutdown();
                }
                else
                {
                    if (iniciado) labelStatus.Content = "Shutting down in " + (TIEMPO_ESPERA - timer15contador);
                    if (modulo == 2 || modulo == 3) labelTempo.Content = "Shutting down in " + (TIEMPO_ESPERA - timer15contador);
                }
            }
            else
            {
                if (timer15contador == TIEMPO_ESPERA + 1)
                {
                    //Process.Start("shutdown", "/h");
                    System.Windows.Forms.Application.SetSuspendState(System.Windows.Forms.PowerState.Suspend, false, false);
                    System.Windows.Application.Current.Shutdown();
                }
                else
                {
                    if (iniciado) labelStatus.Content = "Sleeping in " + (TIEMPO_ESPERA - timer15contador);
                    if (modulo == 2 || modulo == 3) labelTempo.Content = "Sleeping in " + (TIEMPO_ESPERA - timer15contador);
                }

            }
            timer15contador += 1;
            balloon.Play();
        }
        /*
        private void avisoInternet(object sender, EventArgs e)
        {
            bool playding=false;
            if (cpuAvisoOn == false && redAvisoOn == false)
            {
                avisointernet.Stop();
                if (iniciado == false)
                {
                    cpuapagar.Stop();
                    netapagar.Stop();
                    labelStatus.Content = "Avisos apagados";
                    labelStatus_Copy.Visibility = Visibility.Hidden;
                }
            }
            
           
            if (redAvisoOn)
            {
                if (!netapagar.IsEnabled) netapagar.Start();

                if (limpiarString(tbavisored.Text) != "")
                {
                    redAvisoUmbral = int.Parse(limpiarString(tbavisored.Text), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
                }
                else
                {
                    redAvisoUmbral = 0;
                }

                if (netusoint > redAvisoUmbral || netusoint2 > redAvisoUmbral)
                {
                    playding = true;
                }
            }
            */

        /*
        if (cpuAvisoOn)
        {
            if (!cpuapagar.IsEnabled) cpuapagar.Start();

            if (limpiarString(tbAvisoCpu.Text) != "")
            {
                cpuAvisoUmbral= int.Parse(limpiarString(tbAvisoCpu.Text), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
            }
            else
            {
                cpuAvisoUmbral = 0;
            }

            if (cpusoint > cpuAvisoUmbral)
            {
                playding = true;
            }
        }

       if (playding && dings<10)
       {
           ding.Play();
           dings++;
       }
       if (dings==10)
       {
           balloon.Play();
           dings++;
       }
       if (dings == 11 && playding == false)
       {
           dings = 0;
           balloon.Play();
       }
       if (!playding)
       {
           dings = 0;
       }

    }
    */

        private void internetReposoApagar(object sender, EventArgs e)
        {
            //float netuso;
            //int netusoint;
            //float netuso2;
            //int netusoint2;

            //netuso = netCounter.NextValue();
            //netusoint = (int)netuso / 1024;
            //netuso2 = netCounter2.NextValue();
            //netusoint2 = (int)netuso2 / 1024;


            usoDeRed();
            netusoint = (int)bajadadif;
            netusoint2 = (int)subidadif;

            if (usarPromedio)
            {
                calcPromedioRed(netusoint, netusoint2);
                netusoint = promedioRed;
                netusoint2 = promedioRed2;
            }

            if (netusoint < umbralRed && netusoint2 < umbralRed && iniciado && (rbint == 1 || rbint == 3))
            {
                if (netusocontador < umbralTiempoInt) netusocontador++;
                // decir(cpusocontador.ToString(), true);
            }
            else
            {
                netusocontador = 0;
                salidaapagadointernet = false;
            }

            if (!cpuapagar.IsEnabled)
            {
                if (labelStatus_Copy.IsVisible) labelStatus_Copy.Visibility = Visibility.Hidden;
                labelStatus.Content = "⬇" + netusoint.ToString() + "kB/s  ⬆" + netusoint2.ToString() + " kB/s  " + netusocontador.ToString() + "/" + umbralTiempoInt;
            }
            else
                if (!labelStatus_Copy.IsVisible) labelStatus_Copy.Visibility = Visibility.Visible;
            labelStatus_Copy.Content = "⬇" + netusoint.ToString() + "kB/s  ⬆" + netusoint2.ToString() + " kB/s  " + netusocontador.ToString() + "/" + umbralTiempoInt;



            //labelReconocimiento.Content = cpusocontador.ToString() + "/60";
            if (netusocontador == umbralTiempoInt && iniciado && (rbint == 1 || rbint == 3))
            {
                if (apagarseguncpueinternet == false)
                {
                    //shuttingDown = true;
                    button.Background = brushRoja;
                    if (!vincularapagados)
                    {
                        netapagar.Stop();
                        timer15contador = 0;
                        timer15.Start();
                        netusocontador = 0;
                    }
                    else
                    {
                        if (salidaApagadoTimer)
                        {
                            timer15contador = 0;
                            timer15.Start();
                            netusocontador = 0;
                        }
                    }


                }

                salidaapagadointernet = true;
            }

        }

        private void cpuReposoApagar(object sender, EventArgs e)
        {
            //float cpuso;
            //int cpusoint;

            cpuso = cpuCounter.NextValue();
            cpusoint = (int)cpuso;

            if (usarPromedio)
            {
                calcPromedioCpu(cpusoint);
                cpusoint = promedioCpu;
            }


            if (cpusoint < umbralCpu && iniciado && (rbint == 3 || rbint == 2))
            {

                if (cpusocontador < umbralTiempoInt) cpusocontador++;
                // decir(cpusocontador.ToString(), true);
            }
            else
            {
                cpusocontador = 0;
                salidaapagadocpu = false;
            }

            labelStatus.Content = "CPU: " + cpusoint.ToString() + "%  " + cpusocontador.ToString() + "/" + umbralTiempoInt;

            //labelReconocimiento.Content = cpusocontador.ToString() + "/60";
            if (cpusocontador == umbralTiempoInt && iniciado && (rbint == 3 || rbint == 2))
            {
                if (apagarseguncpueinternet == false)
                {
                    if (!vincularapagados)
                    {
                        cpuapagar.Stop();
                        cpusocontador = 0;

                        button.Background = brushRoja;
                        timer15contador = 0;
                        timer15.Start();
                    }
                    else
                    {
                        button.Background = brushRoja;
                        if (salidaApagadoTimer)
                        {
                            timer15contador = 0;
                            timer15.Start();
                        }
                    }
                }
                salidaapagadocpu = true;
            }



            if (salidaapagadocpu == true && salidaapagadointernet == true && apagarseguncpueinternet == true && iniciado && (rbint == 3))
            {

                if (!vincularapagados)
                {
                    cpuapagar.Stop();
                    cpusocontador = 0;
                    button.Background = brushRoja;
                    timer15contador = 0;
                    timer15.Start();
                    salidaapagadocpu = false;
                    salidaapagadointernet = false;
                }
                else
                {
                    button.Background = brushRoja;
                    if (salidaApagadoTimer)
                    {
                        timer15contador = 0;
                        timer15.Start();
                    }
                }

            }


        }

        private void rbRed_Checked(object sender, RoutedEventArgs e)
        {
            rbint = 1;
        }

        private void rbCpu_Checked(object sender, RoutedEventArgs e)
        {
            rbint = 2;
        }

        private void rbAmbos_Checked(object sender, RoutedEventArgs e)
        {
            rbint = 3;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            usoDeRed();
            if (!iniciado)
            {
                String umbral = tbCpu.Text;
                if (limpiarString(umbral) != "")
                {
                    umbralCpu = int.Parse(limpiarString(umbral), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
                }
                else
                {
                    umbralCpu = 0;
                }

                umbral = tbRed.Text;
                if (limpiarString(umbral) != "")
                {
                    umbralRed = int.Parse(limpiarString(umbral), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
                }
                else
                {
                    umbralRed = 0;
                }

                String umbralTiempo = editTextTiempoUmbral.Text;
                if (limpiarString(umbralTiempo) != "")
                {
                    umbralTiempoInt = int.Parse(limpiarString(umbralTiempo), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
                }
                else
                {
                    umbralTiempoInt = 0;
                }

                if (umbralRed == 420 && umbralCpu == 69) timerParaBoludear.Start();
                //if (umbralTiempoInt == 1) segundosUmbral.Text = "second";
                //if (umbralTiempoInt != 1) segundosUmbral.Text = "segundos";

                if (rbint == 1)
                {
                    netapagar.Start();
                }
                if (rbint == 2)
                {
                    cpuapagar.Start();
                }
                if (rbint == 3)
                {
                    netapagar.Start();
                    cpuapagar.Start();
                    apagarseguncpueinternet = true;
                    labelStatus_Copy.Visibility = Visibility.Visible;
                    labelStatus_Copy.Content = "";
                }
                netusocontador = 0;
                cpusocontador = 0;
                labelStatus.Content = "Starting...";
                button.Content = "Stop";
                iniciado = true;
                button.Background = brushAmarilla;
            }
            else
            {
                if (comandoEnviado) Process.Start("shutdown", "/a");
                timer15.Stop();
                netapagar.Stop();
                cpuapagar.Stop();
                apagarseguncpueinternet = false;
                labelStatus.Content = "Stopped";
                button.Content = "Start";
                if (timer15contador > 14)
                {
                    labelStatus.Content = "Stopped, whew";
                    tada.Play();
                    timer15contador = 0;
                }
                button.Background = brushVerde;
                iniciado = false;
                labelStatus_Copy.Visibility = Visibility.Hidden;

            }

        }

        private void rbApagar_Checked(object sender, RoutedEventArgs e)
        {
            apagarnohibernar = true;
            if (iniciado2 && tempoPorHora)
            {
                labelTempo.Content = "Shutting down at " + horaTempo;
            }
        }

        private void rbHibernar_Checked(object sender, RoutedEventArgs e)
        {
            apagarnohibernar = false;
            if (iniciado2 && tempoPorHora)
            {
                labelTempo.Content = "Sleeping at " + horaTempo;
            }
        }

        private void buttonTempo_Click(object sender, RoutedEventArgs e)
        {
            if (!iniciado2 && !timer15.IsEnabled)
            {
                buttonTempo.Content = "Stop";
                buttonTempo.Background = brushAmarilla;
                iniciado2 = true;
                labelTempo.Content = "Starting...";
                leerCasillasTimer();
            }
            else
            {
                salidaApagadoTimer = false;
                labelTempo.Content = "Stopped";
                buttonTempo.Content = "Start";
                buttonTempo.Background = brushVerde;
                iniciado2 = false;
            }

            if (timer15.IsEnabled)
            {
                if (comandoEnviado) Process.Start("shutdown", "/a");
                timer15.Stop();
                if (timer15contador > 14)
                {
                    tada.Play();
                    timer15contador = 0;
                }
            }
            /*
            if (!iniciado2)
            {
                String tiempo = editTextTempo.Text;
                if (limpiarString(tiempo) != "")
                {
                    minutosTimer = int.Parse(limpiarString(tiempo), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
                }
                else
                {
                    minutosTimer = 0;
                }
                horaTempo = editTextTempo_Copy.Text;
                //if (horaTempo[0] == '0') horaTempo = horaTempo.Remove('0');
               // editTextTempo_Copy.Text = horaTempo;

                if (rbTemporizador == 1)
                {
                    tempoPorHora = false;
                }
                if (rbTemporizador == 2) tempoPorHora = true;

                buttonTempo.Content = "Stop";
                buttonTempo.Background = brushAmarilla;

                timer.Start();
                if (minutosTimer != 1)
                {
                    labelTempo.Content = minutosTimer + " minutes remaining";
                    textboxtempo.Text = "minutes";
                }
                else
                {
                    labelTempo.Content = minutosTimer + " minutes remaining";
                    textboxtempo.Text = "minute";
                }

                if (tempoPorHora)
                {
                    if (apagarnohibernar) labelTempo.Content = "Shutting down at " + horaTempo;
                    if (!apagarnohibernar) labelTempo.Content = "Sleeping at " + horaTempo;
                }

                iniciado2 = true;

                if (minutosTimer <= 0 && !tempoPorHora)
                {
                    buttonTempo.Background = brushRoja;
                    if (!vincularapagados)
                    {
                        timer15contador = 0;
                        timer15.Start();
                        minutosTimer = 0;
                    }
                    else
                    {
                        salidaApagadoTimer = true;
                    }
                }
            }
            else
            {
                salidaApagadoTimer = false;
                if (comandoEnviado) Process.Start("shutdown", "/a");
                labelTempo.Content = "Stopped";
                timer.Stop();
                timer15.Stop();
                if (timer15contador > 14)
                {
                    labelTempo.Content = "It would've been funny if you'd failed";
                    tada.Play();
                    timer15contador = 0;
                }
                buttonTempo.Content = "Start";
                buttonTempo.Background = brushVerde;
                iniciado2 = false;
            }
            */
        }
        private String limpiarString(String sting)
        {
            String ssalida = "";
            for (int i = 0; i < sting.Length; i++)
            {
                if (sting[i].Equals('0') && i != 0) ssalida += '0';
                if (sting[i].Equals('1')) ssalida += '1';
                if (sting[i].Equals('2')) ssalida += '2';
                if (sting[i].Equals('3')) ssalida += '3';
                if (sting[i].Equals('4')) ssalida += '4';
                if (sting[i].Equals('5')) ssalida += '5';
                if (sting[i].Equals('6')) ssalida += '6';
                if (sting[i].Equals('7')) ssalida += '7';
                if (sting[i].Equals('8')) ssalida += '8';
                if (sting[i].Equals('9')) ssalida += '9';
                if (ssalida.Length > 6) break;
            }
            return ssalida;
        }

        private int stringAInt(String sting)
        {
            String ssalida = "";
            for (int i = 0; i < sting.Length; i++)
            {
                if (sting[i].Equals('0') && i != 0) ssalida += '0';
                if (sting[i].Equals('1')) ssalida += '1';
                if (sting[i].Equals('2')) ssalida += '2';
                if (sting[i].Equals('3')) ssalida += '3';
                if (sting[i].Equals('4')) ssalida += '4';
                if (sting[i].Equals('5')) ssalida += '5';
                if (sting[i].Equals('6')) ssalida += '6';
                if (sting[i].Equals('7')) ssalida += '7';
                if (sting[i].Equals('8')) ssalida += '8';
                if (sting[i].Equals('9')) ssalida += '9';

                if (ssalida.Length > 6) break;
            }
            int returno = 0;
            if (ssalida != "")
            {
                returno = int.Parse(ssalida, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
            }
            return returno;
        }

        private void window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        public System.Collections.Generic.List<String> net_adapters()
        {
            List<String> values = new List<String>();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                values.Add(nic.Name);
            }
            return values;
        }

        void usoDeRed()
        {
            long subidaMax = 0;
            long bajadaMax = 0;
            foreach (NetworkInterface ni in interfaces)
            {
                lectura = ni.GetIPv4Statistics().BytesSent;
                if (lectura != 0) subida = lectura;
                if ((long)subida > subidaMax) subidaMax = subida;

                lectura = ni.GetIPv4Statistics().BytesReceived;
                if (lectura != 0) bajada = lectura;
                if ((long)bajada > bajadaMax) bajadaMax = bajada;
            }
            subidadif = subidaMax - subidaPrev;
            bajadadif = bajadaMax - bajadaPrev;

            subidadif = subidadif / 1024;
            bajadadif = bajadadif / 1024;

            bajadaPrev = bajadaMax;
            subidaPrev = subidaMax;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            cbUsarPromedioChecked();
            
        }

        private void cbUsarPromedioChecked()
        {
            if ((bool)cbUsarPromedio.IsChecked)
            {
                usarPromedio = true;
            }
            else
            {
                usarPromedio = false;
            }
        }

        private void calcPromedioRed(int valorSubida, int valorBajada)
        {
            redVector[ipromRed] = valorSubida;
            redVector2[ipromRed] = valorBajada;

            int tot = 0;
            int to1 = 30;
            for (int j = ipromRed; j >= 0; j--)
            {
                tot += (redVector[j] * to1);
                to1--;
            }
            for (int j = 29; j > ipromRed; j--)
            {
                tot += (redVector[j] * to1);
                to1--;
            }
            promedioRed = (tot / 465);

            tot = 0;
            to1 = 30;
            for (int j = ipromRed; j >= 0; j--)
            {
                tot += (redVector2[j] * to1);
                to1--;
            }
            for (int j = 29; j > ipromRed; j--)
            {
                tot += (redVector2[j] * to1);
                to1--;
            }
            promedioRed2 = (tot / 465);

            /*
            int tot = 0;
            for(int j = 0; j<30; j++)
            {
                tot += redVector[j];
            }
            promedioRed = (tot / 30);

            tot = 0;
            for (int j = 0; j < 30; j++)
            {
                tot += redVector2[j];
            }
            promedioRed2 = (tot / 30);
            */
            ipromRed++;
            if (ipromRed == 30) ipromRed = 0;

        }
        private void calcPromedioCpu(int valorNuevoCPU)
        {
            cpuVector[ipromCpu] = valorNuevoCPU;

            int tot = 0;
            int to1 = 30;

            for (int j = ipromCpu; j >= 0; j--)
            {
                tot += (cpuVector[j] * to1);
                to1--;
            }
            for (int j = 29; j > ipromCpu; j--)
            {
                tot += (cpuVector[j] * to1);
                to1--;
            }
            promedioCpu = (tot / 465);

            /*
            int tot = 0;
            for (int j = 0; j < 30; j++)
            {
                tot += cpuVector[j];
            }
            promedioCpu = (tot / 30);
             */

            ipromCpu++;
            if (ipromCpu == 30) ipromCpu = 0;
        }


        /*
private void avisoRedClickfunc(object sender, RoutedEventArgs e)
{
   if ((bool)cbRedAviso.IsChecked)
   {
       redAvisoOn = true;
       avisointernet.Start();
   }
   else
   {
       redAvisoOn = false;
   }
}
*/
        /*
        private void avisoCpuClickfunc(object sender, RoutedEventArgs e)
        {
            if ((bool)cbCpuAviso.IsChecked)
            {
                cpuAvisoOn = true;
                avisointernet.Start();
            }
            else
            {
                cpuAvisoOn = false;
            }
        }
        */
        private void rbTempo1click(object sender, RoutedEventArgs e)
        {
            rbTemporizador = 1;
        }

        private void rbTempo2click(object sender, RoutedEventArgs e)
        {
            rbTemporizador = 2;
        }

        private void botonCargar(object sender, RoutedEventArgs e)
        {
            cargarSettings();
        }

        private void botonGuardar(object sender, RoutedEventArgs e)
        {
            guardarSettings();
        }

        private void cbEnlazar_Checked(object sender, RoutedEventArgs e)
        {
            cbEnlazarCheckedFunc();
        }

        private void cbEnlazarCheckedFunc()
        {
            if ((bool)cbEnlazar.IsChecked)
            {
                vincularapagados = true;
            }
            else
            {
                vincularapagados = false;
            }
        }

        private void lafuncion(object sender, RoutedEventArgs e)
        {
            rbCpu.IsChecked = true;
        }
 

        private void window_StateChanged(object sender, EventArgs e)
        {
            funcionMinimizar();
        }

        private void funcionMinimizar()
        {

            //notifyIcon.ContextMenuStrip = new forms.ContextMenuStrip();
            //notifyIcon.ContextMenuStrip.Items.Add("Exit", null, contextMenuExitClicked);
            
            if (WindowState == System.Windows.WindowState.Minimized)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Properties.Resources.Shutdown.Save(ms);
                    File.WriteAllBytes(Path.GetTempPath() + "APA AutoShutdown.ico", ms.ToArray());

                    notifyIcon.Icon = new System.Drawing.Icon(Path.GetTempPath() + "APA AutoShutdown.ico");
                    notifyIcon.Visible = true;
                    notifyIcon.Click += NotifyIcon_Click;
                    notifyIcon.Text = "APA AutoShutdown";
                    Thread.Sleep(250);
                    this.Hide();
                }
                
                /*
                if (!File.Exists(Properties.Settings.Default.iconLocationSet))
                {
                    /*
                    String mensaje = "Can't minimize to tray: @Resources/Shutdown.ico is missing, make sure you have all the files";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBox.Show(mensaje, "Error", buttons);
                    
                    labelStatus.Content = "Error: Shutdown.ico is missing";
                }
                else
                {
                    notifyIcon.Icon = new System.Drawing.Icon(Properties.Settings.Default.iconLocationSet);
                    notifyIcon.Visible = true;
                    notifyIcon.Click += NotifyIcon_Click;
                    notifyIcon.Text = "AutoShutdown";
                    this.Hide();
                }
                */
            }
            //base.OnStateChanged(e);  //esto congela el programa no se por que carajo está
        }

        private void contextMenuExitClicked(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void NotifyIcon_Click (object sender, EventArgs e)
        {
            funcionMaximizar();
        }

        private void funcionMaximizar()
        {
            this.Show();
            WindowState = WindowState.Normal;
            notifyIcon.Visible = false;
            timerMain.Stop();
            timerAnimacionInicio.Start();
            File.Delete(Path.GetTempPath() + "APA AutoShutdown.ico");
        }

        private void cbStartOnBoot_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbStartOnBoot.IsChecked)
            {
                /*
                char comilla = (char)34;
                char barrita = (char)92;
                */
                var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
                //key.SetValue("AutoShutdown", System.Reflection.Assembly.GetExecutingAssembly().Location);
                key.SetValue("AutoShutdown", '"' + System.Reflection.Assembly.GetExecutingAssembly().Location + '"' + " /minimizedputo");
                
                //key.SetValue("AutoShutdown", System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName).ToString());
            }
            else
            {
                var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
                key.DeleteValue("AutoShutdown", false);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://linktr.ee/SirDella");
        }

        private void editTextTempo_TextChanged(object sender, TextChangedEventArgs e)
        {
            leerCasillasTimer();
        }

        private void leerCasillasTimer()
        {
            minutosTimer = stringAInt(editTextTempo.Text);
            segundosTimer = minutosTimer * 60;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            timerAnimacionInicio.Start();
        }

        private void Button_Click_4()
        {

        }

        private void tbRed_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void editTextTempo_Copy_TextChanged(object sender, TextChangedEventArgs e)
        {
            horaTempo = editTextTempo_Copy.Text;
            try
            {
                if (horaTempo[0] == '0' && horaTempo.Length == 5) horaTempo = horaTempo.Remove(0,1);
            }
            catch (Exception) { }
        }

        private void window_Closing(object sender, CancelEventArgs e)
        {
            if (comandoEnviado) Process.Start("shutdown", "/a");
            guardarSettings();
            notifyIcon.Dispose();
            if (File.Exists(Path.GetTempPath() + "APA AutoShutdown.ico")) File.Delete(Path.GetTempPath() + "APA AutoShutdown.ico");
;        }

        private void tbRed_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void guardarSettings()
        {
            Properties.Settings.Default.netThSet = tbRed.Text;
            Properties.Settings.Default.cpuThSet = tbCpu.Text;
            Properties.Settings.Default.rbAutoShutSet = rbint;
            Properties.Settings.Default.HoldSet = editTextTiempoUmbral.Text;
            Properties.Settings.Default.AutoshutStartSet = iniciado;
            if (apagarnohibernar == true)
            {
                Properties.Settings.Default.shutdownTypeSet = 1;
            }
            else
            {
                Properties.Settings.Default.shutdownTypeSet = 2;
            }

            if (rbTemporizador == 1)
            {
                Properties.Settings.Default.timedRbSet = 1;
            }
            else
            {
                Properties.Settings.Default.timedRbSet = 2;
            }

            Properties.Settings.Default.useAverageSet = usarPromedio;
            Properties.Settings.Default.chainShutdownsSet = (bool)cbEnlazar.IsChecked.Value;
            Properties.Settings.Default.keepActiveStateSet = (bool)cbGuardarEstado.IsChecked.Value;
            Properties.Settings.Default.startOnBootSet = (bool)cbStartOnBoot.IsChecked.Value;
            Properties.Settings.Default.timedMinutesSet = editTextTempo.Text;
            Properties.Settings.Default.timedTimeSet = editTextTempo_Copy.Text;
            Properties.Settings.Default.timedStartSet = iniciado2;
            Properties.Settings.Default.Save();
        }

        private void cargarSettings()
        {
            tbRed.Text = Properties.Settings.Default.netThSet;
            tbCpu.Text = Properties.Settings.Default.cpuThSet;
            if (Properties.Settings.Default.rbAutoShutSet == 1) rbint = 1;
            if (Properties.Settings.Default.rbAutoShutSet == 2)
            {
                rbint = 2;
                rbCpu.IsChecked = true;
            }
            if (Properties.Settings.Default.rbAutoShutSet == 3)
            {
                rbint = 3;
                rbAmbos.IsChecked = true;
            }

            if (Properties.Settings.Default.timedRbSet == 2)
            {
                tbTempo2.IsChecked = true;
            }

            editTextTiempoUmbral.Text = Properties.Settings.Default.HoldSet;
            editTextTempo.Text = Properties.Settings.Default.timedMinutesSet;
            editTextTempo_Copy.Text = Properties.Settings.Default.timedTimeSet;

            if (Properties.Settings.Default.shutdownTypeSet == 2) rbHibernar.IsChecked = true;

            cbUsarPromedio.IsChecked = Properties.Settings.Default.useAverageSet;
            if ((bool)cbUsarPromedio.IsChecked) cbUsarPromedioChecked();

            cbGuardarEstado.IsChecked = Properties.Settings.Default.keepActiveStateSet;
            
            cbEnlazar.IsChecked = Properties.Settings.Default.chainShutdownsSet;
            if ((bool)cbEnlazar.IsChecked) cbEnlazarCheckedFunc();

            cbStartOnBoot.IsChecked = Properties.Settings.Default.startOnBootSet;

            if ((bool)cbGuardarEstado.IsChecked)
            {
                if (Properties.Settings.Default.AutoshutStartSet == true)
                {
                    button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
                if (Properties.Settings.Default.timedStartSet == true)
                {
                    buttonTempo.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }
            
            string[] args = Environment.GetCommandLineArgs();
            
            for (int index = 1; index < args.Length; index += 2)
            {
                if (args[index].Contains("minimized"))
                {
                    WindowState = WindowState.Minimized;
                    funcionMinimizar();
                    index = args.Length;
                }
            }
            //detecta bien cuando se usa el argumento de inicio -minimized, pero por alguna razon el notifyIcon no se quiere crear, se crashea, solo en este caso
        }
    }
}
