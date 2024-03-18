using ParquetSharp;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AiPrompts
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            backgroundWorker2.RunWorkerAsync();
        }

       
        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Text =e.ProgressPercentage.ToString();
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Done");
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {


            //var dataBase = new List<DataClass>();

            MessageBox.Show("dss");
         
            //return;
            // Specify the path to your Parquet file
            var fileReader = new ParquetFileReader("metadata.parquet");
            return;
            var numColumns = fileReader.FileMetaData.NumColumns;
            var numRows = fileReader.FileMetaData.NumRows;
            var numRowGroups = fileReader.FileMetaData.NumRowGroups;
            var metadata = fileReader.FileMetaData.KeyValueMetadata;
         
            //var schema = fileReader.FileMetaData.Schema;
            //for (var columnIndex = 0; columnIndex < schema.NumColumns; ++columnIndex)
            //{
            //    var column = schema.Column(columnIndex);
            //    var columnName = column.Name;
            //    //listBox1.Items.Add(columnName);
            //    //textBox2.Text += "   " + columnName;
            //}


            for (var rowGroup = 0; rowGroup < fileReader.FileMetaData.NumRowGroups; ++rowGroup)
            {
                using var rowGroupReader = fileReader.RowGroup(rowGroup);
                var groupNumRows = rowGroupReader.MetaData.NumRows;

                backgroundWorker2.ReportProgress(-1);
                var image_name = rowGroupReader.Column(0).LogicalReader().Apply(new ColumnPrinter());
                var prompt = rowGroupReader.Column(1).LogicalReader().Apply(new ColumnPrinter());
                var part_id = rowGroupReader.Column(2).LogicalReader().Apply(new ColumnPrinter());
                var seed = rowGroupReader.Column(3).LogicalReader().Apply(new ColumnPrinter());
                var step = rowGroupReader.Column(4).LogicalReader().Apply(new ColumnPrinter());
                backgroundWorker2.ReportProgress(-2);
                var cfg = rowGroupReader.Column(5).LogicalReader().Apply(new ColumnPrinter());
                var sampler = rowGroupReader.Column(6).LogicalReader().Apply(new ColumnPrinter());
                var width = rowGroupReader.Column(7).LogicalReader().Apply(new ColumnPrinter());
                var height = rowGroupReader.Column(8).LogicalReader().Apply(new ColumnPrinter());
                var user_name = rowGroupReader.Column(9).LogicalReader().Apply(new ColumnPrinter());
                var timestamp = rowGroupReader.Column(10).LogicalReader().Apply(new ColumnPrinter());
                var image_nsfw = rowGroupReader.Column(11).LogicalReader().Apply(new ColumnPrinter());
                var prompt_nsfw = rowGroupReader.Column(12).LogicalReader().Apply(new ColumnPrinter());
                backgroundWorker2.ReportProgress(-3);
                int sum = 0;
                var dbContext = new Model1();
                var errorc = 0;
                for (int i = 0; i < image_name.Count - 1; i++)
                {

                    var data = new DataClass
                    {
                        image_name = image_name[i],
                        prompt = prompt[i],
                        part_id = part_id[i],
                        seed = Convert.ToDouble(seed[i]),
                        step = Convert.ToDouble(step[i]),
                        cfg = Convert.ToDouble(cfg[i]),
                        sampler = Convert.ToDouble(sampler[i]),
                        width = Convert.ToDouble(width[i]),
                        height = Convert.ToDouble(height[i]),
                        user_name = user_name[i],
                        timestamp = timestamp[i],
                        image_nsfw = Convert.ToDouble(image_nsfw[i]),
                        prompt_nsfw = Convert.ToDouble(prompt_nsfw[i])
                    };
                    if (double.IsInfinity((double)data.seed)) data.seed = 0;
                    if (double.IsInfinity((double)data.step)) data.step = 0;
                    if (double.IsInfinity((double)data.sampler)) data.sampler = 0;
                    if (double.IsInfinity((double)data.width)) data.width = 0;
                    if (double.IsInfinity((double)data.height)) data.height = 0;
                    if (double.IsInfinity((double)data.image_nsfw)) data.image_nsfw = 0;
                    if (double.IsInfinity((double)data.prompt_nsfw)) data.prompt_nsfw = 0;
                    if (double.IsInfinity((double)data.cfg)) data.cfg = 0;
                    // Add the instance to the context and save changes to the database
                    dbContext.DataTable.Add(data);
                    sum++;
                    if (sum > 10)
                    {
                        backgroundWorker2.ReportProgress(i);
                        sum = 0;
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                            errorc++;
                            //throw;
                        }

                        dbContext = new Model1();
                    }


                }

                var rrr = errorc;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //var dataBase = new List<DataClass>();

            MessageBox.Show("dss");

            //return;
            // Specify the path to your Parquet file
            var fileReader = new ParquetFileReader("metadata.parquet");
            return;
            var numColumns = fileReader.FileMetaData.NumColumns;
            var numRows = fileReader.FileMetaData.NumRows;
            var numRowGroups = fileReader.FileMetaData.NumRowGroups;
            var metadata = fileReader.FileMetaData.KeyValueMetadata;

            //var schema = fileReader.FileMetaData.Schema;
            //for (var columnIndex = 0; columnIndex < schema.NumColumns; ++columnIndex)
            //{
            //    var column = schema.Column(columnIndex);
            //    var columnName = column.Name;
            //    //listBox1.Items.Add(columnName);
            //    //textBox2.Text += "   " + columnName;
            //}


            for (var rowGroup = 0; rowGroup < fileReader.FileMetaData.NumRowGroups; ++rowGroup)
            {
                using var rowGroupReader = fileReader.RowGroup(rowGroup);
                var groupNumRows = rowGroupReader.MetaData.NumRows;

                backgroundWorker2.ReportProgress(-1);
                var image_name = rowGroupReader.Column(0).LogicalReader().Apply(new ColumnPrinter());
                var prompt = rowGroupReader.Column(1).LogicalReader().Apply(new ColumnPrinter());
                var part_id = rowGroupReader.Column(2).LogicalReader().Apply(new ColumnPrinter());
                var seed = rowGroupReader.Column(3).LogicalReader().Apply(new ColumnPrinter());
                var step = rowGroupReader.Column(4).LogicalReader().Apply(new ColumnPrinter());
                backgroundWorker2.ReportProgress(-2);
                var cfg = rowGroupReader.Column(5).LogicalReader().Apply(new ColumnPrinter());
                var sampler = rowGroupReader.Column(6).LogicalReader().Apply(new ColumnPrinter());
                var width = rowGroupReader.Column(7).LogicalReader().Apply(new ColumnPrinter());
                var height = rowGroupReader.Column(8).LogicalReader().Apply(new ColumnPrinter());
                var user_name = rowGroupReader.Column(9).LogicalReader().Apply(new ColumnPrinter());
                var timestamp = rowGroupReader.Column(10).LogicalReader().Apply(new ColumnPrinter());
                var image_nsfw = rowGroupReader.Column(11).LogicalReader().Apply(new ColumnPrinter());
                var prompt_nsfw = rowGroupReader.Column(12).LogicalReader().Apply(new ColumnPrinter());
                backgroundWorker2.ReportProgress(-3);
                int sum = 0;
                var dbContext = new Model1();
                var errorc = 0;
                for (int i = 0; i < image_name.Count - 1; i++)
                {

                    var data = new DataClass
                    {
                        image_name = image_name[i],
                        prompt = prompt[i],
                        part_id = part_id[i],
                        seed = Convert.ToDouble(seed[i]),
                        step = Convert.ToDouble(step[i]),
                        cfg = Convert.ToDouble(cfg[i]),
                        sampler = Convert.ToDouble(sampler[i]),
                        width = Convert.ToDouble(width[i]),
                        height = Convert.ToDouble(height[i]),
                        user_name = user_name[i],
                        timestamp = timestamp[i],
                        image_nsfw = Convert.ToDouble(image_nsfw[i]),
                        prompt_nsfw = Convert.ToDouble(prompt_nsfw[i])
                    };
                    if (double.IsInfinity((double)data.seed)) data.seed = 0;
                    if (double.IsInfinity((double)data.step)) data.step = 0;
                    if (double.IsInfinity((double)data.sampler)) data.sampler = 0;
                    if (double.IsInfinity((double)data.width)) data.width = 0;
                    if (double.IsInfinity((double)data.height)) data.height = 0;
                    if (double.IsInfinity((double)data.image_nsfw)) data.image_nsfw = 0;
                    if (double.IsInfinity((double)data.prompt_nsfw)) data.prompt_nsfw = 0;
                    if (double.IsInfinity((double)data.cfg)) data.cfg = 0;
                    // Add the instance to the context and save changes to the database
                    dbContext.DataTable.Add(data);
                    sum++;
                    if (sum > 10)
                    {
                        backgroundWorker2.ReportProgress(i);
                        sum = 0;
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                            errorc++;
                            //throw;
                        }

                        dbContext = new Model1();
                    }


                }

                var rrr = errorc;

            }
        
    }
    }
    internal sealed class ColumnPrinter : ILogicalColumnReaderVisitor<List<string>>
    {
        public List<string> OnLogicalColumnReader<TElement>(LogicalColumnReader<TElement> columnReader)
        {
            var list = new List<string>();

            foreach (var value in columnReader)
            {
                list.Add(value?.ToString() ?? "0");
            }
            return list;
        }
    }
}
