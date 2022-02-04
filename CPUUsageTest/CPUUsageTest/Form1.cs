using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace CPUUsageTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private int GetPID(string processtoBeMonitored)
        {
            var processes = Process.GetProcessesByName(processtoBeMonitored);
            int pid = 0;
            foreach (var p in processes)
            {
                pid = p.Id;

            }
            return pid;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //// grab all Chrome process instances

            //int cnt = 0;
            //float maxCPU = 0;
            //float cpuValue = 0;

            //while (true)
            //{
            //    try
            //    {
            //        var pid = GetPID();
            //        if (pid <= 0)
            //        {
            //            Console.WriteLine("Process is not found!!!");
            //            Thread.Sleep(2000);
            //        }
            //        var counter = ProcessCpuCounter.GetPerfCounterForProcessId(pid);
            //        // start capturing
            //        counter.NextValue();
            //        Thread.Sleep(200);
            //        var cpu = counter.NextValue() / (float)Environment.ProcessorCount;


            //        if (cpu > 0)
            //        {
            //            if (cpu > maxCPU)
            //                maxCPU = cpu;
            //            Console.WriteLine($"{DateTime.Now:hh:mm:ss.ffff} - {counter.InstanceName}  -  Cpu:{cpu}");
            //            cpuValue += cpu;
            //            cnt++;
            //        }
            //        if (cnt >= 10)
            //        {
            //            Console.WriteLine($"{DateTime.Now:hh:mm:ss.ffff} - {counter.InstanceName}  - Average Cpu after 2-3 sec :{cpuValue / 10} {maxCPU}");
            //            cnt = 0;
            //            cpuValue = 0;
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        Console.WriteLine("Process is not found!!! Exception occurred");
            //    }
            //}
        }

        public void SetTextBox(String text)
        {
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { SetTextBox(text); });
                return;
            }
            txtProcessName.AppendText(text);
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            int cnt = 0;
            float maxCPU = 0;
            float cpuValue = 0;

            while (true)
            {
                try
                {
                    var pid = GetPID(txtProcessName.Text);
                    if (pid <= 0)
                    {
                        Console.WriteLine("Process is not found!!!");
                        Thread.Sleep(2000);
                    }
                    var counter = ProcessCpuCounter.GetPerfCounterForProcessId(pid);
                    // start capturing
                    counter.NextValue();
                    Thread.Sleep(200);
                    var cpu = counter.NextValue() / (float)Environment.ProcessorCount;
                    
                    if (cpu > 0)
                    {
                        if (cpu > maxCPU)
                            maxCPU = cpu;
                        //SetTextBox($"{DateTime.Now:hh:mm:ss.ffff} - {counter.InstanceName}  -  Cpu:{cpu}");
                        Console.WriteLine($"{DateTime.Now:hh:mm:ss.ffff} - {counter.InstanceName}  -  Cpu:{cpu}");
                        cpuValue += cpu;
                        cnt++;
                    }
                    if (cnt >= 10)
                    {
                        // SetTextBox($"{DateTime.Now:hh:mm:ss.ffff} - {counter.InstanceName}  - Average Cpu after 2-3 sec :{cpuValue / 10} {maxCPU}");
                        Console.WriteLine($"{DateTime.Now:hh:mm:ss.ffff} - {counter.InstanceName}  - Average Cpu after 2-3 sec :{cpuValue / 10} {maxCPU}");
                        cnt = 0;
                        cpuValue = 0;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Process is not found!!! Exception occurred");
                }
            }
        }
    }

    public class ProcessCpuCounter
    {
        public static PerformanceCounter GetPerfCounterForProcessId(int processId, string processCounterName = "% Processor Time")
        {
            string instance = GetInstanceNameForProcessId(processId);
            if (string.IsNullOrEmpty(instance))
                return null;

            return new PerformanceCounter("Process", processCounterName, instance);
        }

        public static string GetInstanceNameForProcessId(int processId)
        {
            var process = Process.GetProcessById(processId);
            string processName = Path.GetFileNameWithoutExtension(process.ProcessName);

            PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");
            string[] instances = cat.GetInstanceNames()
                .Where(inst => inst.StartsWith(processName))
                .ToArray();

            foreach (string instance in instances)
            {
                using (PerformanceCounter cnt = new PerformanceCounter("Process",
                    "ID Process", instance, true))
                {
                    int val = (int)cnt.RawValue;
                    if (val == processId)
                    {
                        return instance;
                    }
                }
            }
            return null;
        }
    }
}
