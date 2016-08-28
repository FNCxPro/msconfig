using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.ServiceProcess;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace msconfig
{
    public partial class Form1 : Form
    {
        public bool serviceDone = false;
        public bool startupDone = false;

        public Form1()
        {
            InitializeComponent();
        }

        /*
         * 
         * Most of getOSString() is from:
         * https://code.msdn.microsoft.com/windowsapps/Sample-to-demonstrate-how-495e69db
         * THANKS! :D
         * 
         */
        private string getOSString()
        {
            Version vs = Environment.OSVersion.Version;
            switch (vs.Major)
            {
                case 5:
                    if (vs.Minor == 0)
                        return "Vindows 2000";
                    else if (vs.Minor == 1)
                        return "Vindows XP";
                    else
                    {
                        return "Vindows XP";
                    }
                    break;
                case 6:
                    if (vs.Minor == 0)
                    {
                        return "Vindows 2000";
                    }
                    else if (vs.Minor == 1)
                    {
                        return "Vindows XP";
                    }
                    else if (vs.Minor == 2)
                        return "Vindows 9";
                    else
                    {
                        return "Vindows 12";
                    }
                    break;
                case 10:
                    return "Vindows Vhistler";
                    break;
            }
            return "Vindows 7";
        }

        private string getStartPath(String serviceName)
        {
            WqlObjectQuery wqlObjectQuery = new WqlObjectQuery(string.Format("SELECT * FROM Win32_Service WHERE Name = '{0}'", serviceName));
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(wqlObjectQuery);
            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();

            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                return managementObject.GetPropertyValue("PathName").ToString();
            }
            return "";
        }

        private ListViewItem tool(String name, String desc, String cmd)
        {
            ListViewItem item = new ListViewItem(new[] { name, desc });
            item.Tag = cmd;
            listView2.Items.Add(item);
            return item;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lstOS.Items.Add(getOSString() +" ("+Environment.SystemDirectory.Replace("\\system32", "")+") : Current OS; Default OS");
            //listView1.Items.Add(new ListViewItem(new[] { "Service", "Manufacturer", "Status", "Date Disabled" }));

            tool("About Windows", "Display Windows version information.", "C:\\Windows\\system32\\winver.exe");
            tool("Change UAC Settings", "Change User Account Control settings.", "C:\\Windows\\System32\\UserAccountControlSettings.exe");
            tool("Action Center", "Open the Action Center.", "C:\\Windows\\System32\\wscui.cpl");
            tool("Windows Troubleshooting", "Troubleshoot problems with your computer.", "C:\\Windows\\System32\\control.exe /name Microsoft.Troubleshooting");
            tool("Computer Management", "View and configure system settings and components.", "C:\\Windows\\system32\\compmgmt.msc");
            tool("System Information", "View advanced information about hardware and software settings.", "C:\\Windows\\system32\\msinfo32.exe");
            tool("Event Viewer", "View monitoring and troubleshooting messages.", "C:\\Windows\\system32\\eventvwr.exe");
            tool("Programs", "Launch, add or remove programs and Windows components.", "C:\\Windows\\system32\\appwiz.cpl");
            tool("System Properties", "View basic information about your computer system settings.", "C:\\Windows\\system32\\control.exe system");
            tool("Internet Options", "View Internet Properties.", "C:\\Windows\\system32\\inetcpl.cpl");
            tool("Internet Protocol Configuration", "View and configure network address settings.", "C:\\Windows\\system32\\cmd.exe /k %windir%\\system32\\ipconfig.exe");
            tool("Performance Monitor", "Monitor the performance of local or remote computers.", "C:\\Windows\\system32\\perfmon.exe");
            tool("Resource Monitor", "Monitor the performance and resource usage of the local computer.", "C:\\Windows\\system32\\resmon.exe");
            tool("Task Manager", "View details about programs and processes running on your computer.", "C:\\Windows\\system32\\taskmgr.exe");
            tool("Command Prompt", "Open a command prompt window.", "C:\\Windows\\system32\\cmd.exe");
            tool("Registry Editor", "Make changes to the Windows registry.", "C:\\Windows\\system32\\regedit32.exe");
            tool("Remote Assistance", "Receive help from (or offer help to) a friend over the Internet.", "C:\\Windows\\system32\\msra.exe");
            tool("System Restore", "restore your computer system to an earlier state.", "C:\\Windows\\system32\\rstrui.exe");
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedTab.Text == "Services")
            {
                if (serviceDone)
                {
                    return;
                }
                serviceDone = true;
                try
                {
                    ServiceController[] services = ServiceController.GetServices();
                    for (int i = 0; i < services.Length; i++)
                    {
                        ServiceController svc = services[i];
                        /*string startPath = getStartPath(svc.ServiceName);
                        String mfr = "Microsoft Corporation";
                        if (File.Exists(startPath.Replace("\"", "")))
                        {
                            FileVersionInfo file = FileVersionInfo.GetVersionInfo(startPath.Replace("\"", ""));
                            mfr = file.CompanyName;
                        }*/


                        ListViewItem item = new ListViewItem(new[] { svc.DisplayName, "Microsoft Corporation", "Running", "" });
                        listView1.Items.Add(item);

                    }
                }
                catch (Exception ex)
                {

                }
            }else if(tabControl1.SelectedTab.Text == "Startup")
            {
                if (startupDone)
                {
                    return;
                }
                startupDone = true;
                RegistryKey x86 = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
                RegistryKey HKCU = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
                RegistryKey x64;
                foreach (string startupPrgrm in x86.GetValueNames())
                {
                    ListViewItem item = new ListViewItem(new[] { startupPrgrm, "Unknown", startupPrgrm + ".exe", "" });
                    lstStartup.Items.Add(item);
                }
                foreach (string startupPrgrm in HKCU.GetValueNames())
                {
                    ListViewItem item = new ListViewItem(new[] { startupPrgrm, "Unknown", startupPrgrm + ".exe", "" });
                    lstStartup.Items.Add(item);
                }
                try
                {
                    x64 = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run");
                    foreach (string startupPrgrm in x64.GetValueNames())
                    {
                        ListViewItem item = new ListViewItem(new[] { startupPrgrm, "Microsoft Corporation", startupPrgrm + ".exe", "" });
                        lstStartup.Items.Add(item);
                    }
                }
                catch(Exception ex)
                {

                }
                
            }
        }

        private void listView2_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                txtCmd.Text = listView2.SelectedItems[0].Tag.ToString();
            }
            else
            {
                txtCmd.Clear();
            }   
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Process.Start(txtCmd.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
