using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NonFreezingApp
{
    public partial class Form1 : Form
    {
        //attributes used to refresh UI
        private readonly SynchronizationContext synchronizationContext;
        private DateTime previousTime = DateTime.Now;

        public Form1()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current; //context from UI thread
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (var i = 0; i <= 1000000; i++)
            {
                label1.Text = @"Count : " + i;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            var count = 0;

            await Task.Run(() =>
            {
                for (var i = 0; i <= 1000000; i++)
                {
                    UpdateUI(i);
                    count = i;
                }
            });
            label1.Text = @"Count : " + count;
            button1.Enabled = true;
            button1.Enabled = false;
        }

        public void UpdateUI(int value)
        {
            var timeNow = DateTime.Now;

            //Here we only refresh our UI each 50 ms
            if ((DateTime.Now - previousTime).Milliseconds <= 50) return;

            //Send the update to our UI thread
            synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                label1.Text = @"Count : " + (int)o;
            }), value);

            previousTime = timeNow;
        }
    }
}
