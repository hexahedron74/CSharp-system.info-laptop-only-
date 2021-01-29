﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task_Manager
{
    public partial class CPU : UserControl
    {
        private Thread cpuThread;
        private double[] cpuArray = new double[30];

        public CPU()
        {
            InitializeComponent();
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            float fcpu = pCPU.NextValue();
            cpuUsagePb.Value = (int)fcpu;
            cpuUsageLabel.Text = string.Format("{0:0.00}%", fcpu);
            chartCPU.Series["CPU"].Points.AddY(fcpu);
        }

        private void CPU_Load(object sender, EventArgs e)
        {
            timer1.Start();
            cpu_core_number();
            cpu_name();
            cpu_processor_number();
            logical_processor_number();
            startCpuThread();
        }

        private void getPerformanceCounters()
        {
            var cpuPerfCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            while(true)
            {
                cpuArray[cpuArray.Length - 1] = Math.Round(cpuPerfCounter.NextValue(), 0);
                Array.Copy(cpuArray, 1, cpuArray, 0, cpuArray.Length - 1);
                if(chartCPU.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { UpdateCpuChart(); });
                }
                else
                {

                }
                Thread.Sleep(1000);
            }
        }

        private void UpdateCpuChart()
        {
            chartCPU.Series["CPU"].Points.Clear();
            for(int i = 0; i < cpuArray.Length -1; ++i)
            {
                chartCPU.Series["CPU"].Points.AddY(cpuArray[i]);
            }
        }

        private void startCpuThread()
        {
            cpuThread = new Thread(new ThreadStart(this.getPerformanceCounters));
            cpuThread.IsBackground= true;
            cpuThread.Start();
        }

        private void cpu_name()
        {
            ManagementObjectSearcher win32Proc = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (ManagementObject obj in win32Proc.Get())
            {
                string procName = obj["name"].ToString();
                cpu_main_name.Text = procName;
            }
        }

        private void cpu_core_number()
        {
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }
            core_number.Text = Convert.ToString(coreCount) + " Core";
        }

        private void cpu_processor_number()
        {
            Process[] allProc = Process.GetProcesses();
            processor_number.Text = Convert.ToString(allProc.Length);
        }

        private void logical_processor_number()
        {
            logic_pro_num.Text = Convert.ToString(Environment.ProcessorCount);
        }

    }
}
