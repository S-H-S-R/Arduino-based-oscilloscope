using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
//!!!!!!!***************speed smpling/send arduio ba refresh  speed in yeki nabashe ya bishtare ya kamter mmokene aan aghab bemone ya dir neshn bedeh
//*************age buffer in sabet bashe toolesh yani speed ha barabare va harche toole buffer kamtar bashe yani realtime tare(real time be speed  sample/send arduio ham bastegi dare va be refresh rate in)

//********idea multi channel+baraye multi channel bayad protocol design beshe ke baham chand value berfese va be ge male kodom zaman
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        diagram dig; 
        SerialPort port;
        
       // int queuelengh = 100; //????////vertial must be divisible by queuelengh and must queuelengh<=vertical
        
        System.Timers.Timer aTimer = new System.Timers.Timer();


         


        bool connected = false;
        //-------------------


        int VCC = 5;

         


        class channel
        {
            public Color color;

            public string name;

            //int queuelenght=100;

            public bool active = false;

            public bool invert = false;

            public  Queue<double> data = new Queue<double>();

          

            public channel(Color mycolor,string myname)
            {
                color = mycolor;
                name = myname;
              
                //queuelenght = myqueuelenght;

                //create empty queues of cannels of lengh setting
                //for (int i = 0; i < queuelenght; i++)
                //{                    
                    
                //        data.Enqueue(0);                    

                //}


            }

        }
        class diagram
        {
            public diagram(Graphics myg, int myVcc)
            {
                g = myg;
                Vcc = myVcc;
            }
            public int Vcc;
            public  Graphics g;
            List<channel> allchannels_data = new List<channel>();
            int horizal = 500;
            int vertical = 500;


            int block_pixels = 100;


            public bool addchannel(channel ch)
            {
                //TO DO name mut be uniqe
                if (ch != null)
                {
                    allchannels_data.Add(ch);
                    return true;
                }

                return false;

            }


            public void removechannel(string myname)
            {

                allchannels_data.RemoveAll(ch => ch.name == myname);
            }


            void clear()
            {

                //horizal line
                // e.Graphics.DrawLine(Pens.Black, new Point(0, vertical), new Point(horizal, vertical));

                //vertical line
                // e.Graphics.DrawLine(Pens.Black, new Point(horizal, 0), new Point(horizal, vertical));

                //background color

                g.FillRectangle(Brushes.Black, 0, 0, horizal, vertical);
                for (int i = 0; i < vertical / block_pixels; i++) //???kasri shod chi?????
                {
                    g.DrawLine(Pens.Gray, 0, i * block_pixels, horizal, i * block_pixels);

                }


                for (int i = 0; i < horizal / block_pixels; i++)
                {
                    g.DrawLine(Pens.Gray, i * block_pixels, 0, i * block_pixels, vertical);

                }





                // e.Graphics.DrawString("Current: "+ channel1data.Last().ToString(), new Font(FontFamily.GenericSerif, 20), Brushes.Green, new Point(0, vertical));
                g.DrawString(Vcc.ToString(), new Font(FontFamily.GenericSerif, 20), Brushes.Green, new Point(horizal, 0));
                g.DrawString("0".ToString(), new Font(FontFamily.GenericSerif, 20), Brushes.Green, new Point(horizal, vertical - 30));







            }





            Point recentpoint = new Point();

            public  void refresh()
            {

                clear();

                //firt convert discrete data to point, in order to draw continuous of lines
                int xdensity=100;




                int xspace = horizal / xdensity; //graphics between times
                xdensity++;


                for (int ch = 0; ch < allchannels_data.Count; ch++)
                {


                            channel curent_ch = allchannels_data[ch];
                            if (!curent_ch.active|| curent_ch.data.Count()<=1) continue;
                            int x = 0;
                            if (curent_ch.data.Count > xdensity+1) { curent_ch.data.Dequeue(); }


                            List<double> first100 = new List<Double>();
                            for (int i = 0; i< curent_ch.data.Count() && i < xdensity+1; i++) 
                            {
                               first100.Add(curent_ch.data.ElementAt(i));
                            }

                            //List<Point> graphic_points =new List<Point>();
                            Point[] graphic_points = new Point[first100.Count];

                            for (int i =0; i< first100.Count(); i++)
                            {
                                //jadid tare aval miad

                                int graphicvalue = (int)first100[i]; //

                                if (curent_ch.invert)
                                {
                                    //graphicvalue = y;    

                                }
                                else
                                {
                                    graphicvalue = vertical - graphicvalue;

                                }

                               // int kk = first100.Count() - i-1;
                                graphic_points[i] = new Point(horizal - (xspace * (first100.Count()-i-1)), graphicvalue);
                                
                            }                          


                            Pen pen = new Pen(curent_ch.color);
                            //Point newpoint = new Point(horizal, graphicvalues);
                            g.DrawLines(pen, graphic_points);
                    //g.FillEllipse(pen.Brush, new Rectangle(newpoint.Y, newpoint.X, 7, 7));
                    //recentpoint = newpoint;

                    ////int temp = vertical / curent_ch.data.Count();
                    ////int scaledvalue = graphicvalue;// * ????;
                    ////graphic_point[x] = new Point(x * temp, scaledvalue);
                    ////x += 1;





                    //curent_ch.data.Dequeue();



                    //int temp = vertical / curent_ch.data.Count();
                    //int scaledvalue = graphicvalue;// * ????;
                    //graphic_point[x] = new Point(x * temp, scaledvalue);
                    //x += 1;
                    //if (curent_ch.data.Count()==1) graphic_points

                    //            Pen pen = new Pen(curent_ch.color);
                    //            g.DrawLines(pen, graphic_points);
                    //            g.FillEllipse(pen.Brush, new Rectangle(graphic_points[x - 1].X - 5, graphic_points[x - 1].Y - 3, 7, 7));
                    //            curent_ch.data.Dequeue();

                    //}

                



                }

                //int count = allchannels_data[0].Count() - 1;

                //if (allchannels_data[1].Count > 1 && (allchannels_data[1].ElementAt(count) - allchannels_data[1].ElementAt(count - 1) != 0))
                //{


                //    double gain = (allchannels_data[0].ElementAt(count) - allchannels_data[0].ElementAt(count - 1)) / (float)(allchannels_data[1].ElementAt(count) - allchannels_data[1].ElementAt(count - 1));
                //    label1.Text = gain.ToString();
                //}


            }

            

            internal channel findchannel(string selectedValue)
            {
                channel[] res = allchannels_data.FindAll(ch => ch.name == selectedValue).ToArray();
                if(res.Count()>0) return res[0];
                return null;
            }
             
            internal void adddata(string channelname, float value)
            {
                channel res = findchannel(channelname);
                if (res!=null) res.data.Enqueue(value);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {                      
            

            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 50;
           

            port = new SerialPort("COM3", 9600); 
            

            this.DoubleBuffered = true;


          
        }



      
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            //rahe behtar ine ke arduio be buffer inja befrestad va ma az buffer chon timer windows taghribi ast
            // while (true)  


            //khode class port buffer darad dar c#
            //Random r = new Random();
            //channel1data.Enqueue(r.Next(50, 70));
            //channel1data.Dequeue();
            //Invalidate();
            //return;




            //string data = "";  
            //bool hasdata = false;

            //if (port.BytesToRead > 0)
            //{
            //    data = port.ReadLine(); //blocks until new line arrive 
            //    hasdata = true;
            //}
            //else
            //{
            //    hasdata = false;
            //}





            Random r = new Random();


            string data = "a="+r.Next(0,100) +"#b="+r.Next(0,100)+"#c=300#d=120";
            bool hasdata = true;



            //--

          
            // hasdata = true;
            //--


            if (hasdata) //age data nadashtim   ZERO BEZARIM  ya haman ghabli redraw???  in ravehs pull ast, payad push basahd az samte arduio
                {

                // Convert.ToInt16(TextBoxD1.Text);
                // try{
                   
                string[] channlesdata = data.Split("#".ToCharArray());
                //age bishtar available bood chi?           

                for (int i = 0; i < channlesdata.Count(); i++)
                {

                    string[] name_value = channlesdata[i].Split("=".ToCharArray());


                    float value;
                    float.TryParse(name_value[1], out value);
                    dig.adddata(name_value[0], value); 

                     
                }

                Invalidate();




                //int index = d.IndexOf('#');
                //if (index > 0)
                //{
                //    string sect1 = d.Substring(0, index - 1);
                //    d = d.Remove(0, index); //baghieash bezarim bashe bara frame baad
                //    data.Enqueue(Convert.ToInt32(sect1));
                //    data.Dequeue();
                //    Invalidate();
                //}
                //if (index == 0) { d = d.Remove(0, 1); }


            }
            else
            {
                //yani sorat arduio kmatare ...
                //////??

            }








        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (dig==null) dig = new diagram(e.Graphics,5);
            dig.g = e.Graphics;
            if (connected)
             dig.refresh();

        }
        
      
      
      



       


        private void button1_Click(object sender, EventArgs e)
        {
         //   //aTimer.Enabled = connected = true;
         //   button1.Enabled = false;
         ////   port.Open();
         // //  port.Write("1");
         //   aTimer.Enabled = connected = true;
         //   button1.Text = "stop";
         //   button1.Enabled = true;
         //   return;


            try
            {

                if (!connected)
                {
                    button1.Enabled = false;
                    port.Open();
                    port.Write("1");
                    aTimer.Enabled = connected = true;
                    button1.Text = "stop";
                    button1.Enabled = true;
                }
                else
                {
                    button1.Enabled = false;
                    aTimer.Enabled = connected = false;
                    port.Close();
                    button1.Text = "start";
                    button1.Enabled = true;


                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Restart();

            }


        }

       
       
        
         
         






       
       
        
        
        //add new channel
        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox1.Text=="") return;
            channel ch = new channel(Color.Red, textBox1.Text);
            ch.active = true;
            comboBox1.Items.Add(ch.name);
            comboBox1.SelectedIndex = 0;
            dig.addchannel(ch);

             

        }




        //get selected channel
        channel selectedchannel() {

            if (comboBox1.SelectedIndex <= 0) return null;

            return dig.findchannel(comboBox1.SelectedValue.ToString());
             


        }
         

        //hide/show channel
        private void button9_Click(object sender, EventArgs e)
        {

            channel selectedCH = selectedchannel();
            if(selectedCH != null) selectedCH.active = !selectedCH.active;









        }


        //invert channel
        private void button8_Click(object sender, EventArgs e)
        {
            channel selectedCH = selectedchannel();
            if (selectedCH != null) selectedCH.invert = !selectedCH.invert;
        }



         


        private void button6_Click(object sender, EventArgs e)
        {
            int voltage = 5;
            int internalres = 100;

            //  float current = voltage / ((float)numericUpDown1.Value + (float)numericUpDown2.Value  + (float)internalres);


            float zarb = (float)numericUpDown1.Value * (float)numericUpDown2.Value;
            float jam = (float)numericUpDown1.Value + (float)numericUpDown2.Value;
            float finalR = ((zarb / jam) + (float)internalres);

            float current = voltage / finalR;






            current *= 500;
            float drop1 = voltage / (float)numericUpDown1.Value * 10;
            float drop2 = voltage / (float)numericUpDown2.Value * 10;
            //virtualdata = (drop1.ToString() + "#" + drop2.ToString() + "#" + current + "#" + finalR);
        }


        //selected channel changed
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }





}
