﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using AiPrompts.DataAccess;
using AiPrompts.stableDb;

using DevExpress.Data.Async.Helpers;
using DevExpress.Data.Browsing;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Data.PLinq;
using DevExpress.Xpo;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;

namespace AiPrompts
{
    public partial class Form1 : RibbonForm
    {
        private readonly CopyModel _CopyData;

        public Form1()
        {
            InitializeComponent();
            // This line of code is generated by Data Source Configuration Wizard
            //this.pLinqInstantFeedbackSource1.GetEnumerable += pLinqInstantFeedbackSource1_GetEnumerable;
            //// This line of code is generated by Data Source Configuration Wizard
            //this.pLinqInstantFeedbackSource1.DismissEnumerable += pLinqInstantFeedbackSource1_DismissEnumerable;
            //This line of code is generated by Data Source Configuration Wizard
            // This line of code is generated by Data Source Configuration Wizard
            //Instantiate a new DBContext
            _CopyData = new CopyModel();
            // Call the LoadAsync method to asynchronously get the data for the given DbSet from the database.
            _CopyData.CopyEntities.LoadAsync().ContinueWith(loadTask =>
            {
                // Bind data to control when loading complete
                gridControl1.DataSource = _CopyData.CopyEntities.Local.ToBindingList();
            }, TaskScheduler.FromCurrentSynchronizationContext());
            // This line of code is generated by Data Source Configuration Wizard
            this.xpInstantFeedbackSource1.ResolveSession += xpInstantFeedbackSource1_ResolveSession;
            // This line of code is generated by Data Source Configuration Wizard
            this.xpInstantFeedbackSource1.DismissSession += xpInstantFeedbackSource1_DismissSession;
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            var form2 = new Form2();
            form2.ShowDialog();
        }

        // This event is generated by Data Source Configuration Wizard
        private void pLinqInstantFeedbackSource1_GetEnumerable(object sender, GetEnumerableEventArgs e)
        {
            // Instantiate a new DataContext
            var dataContext = new Model1();
            // Assign a queryable source to the PLinqInstantFeedbackSource
            e.Source = dataContext
                .DataTable; //.Where(@class => @class.prompt.Contains(SearchEditItem1.EditValue .ToString()));
            // Assign the DataContext to the Tag property,
            // to dispose of it in the DismissEnumerable event handler
            e.Tag = dataContext;
        }

        // This event is generated by Data Source Configuration Wizard
        private void pLinqInstantFeedbackSource1_DismissEnumerable(object sender, GetEnumerableEventArgs e)
        {
            // Dispose of the DataContext
            ((Model1)e.Tag).Dispose();
        }

        // This event is generated by Data Source Configuration Wizard
        private void entityInstantFeedbackSource1_GetQueryable(object sender, GetQueryableEventArgs e)
        {
            // Instantiate a new DataContext
            var dataContext = new Model1();
            // Assign a queryable source to the EntityInstantFeedbackSource
            e.QueryableSource = dataContext.DataTable;
            // Assign the DataContext to the Tag property,
            // to dispose of it in the DismissQueryable event handler
            e.Tag = dataContext;
        }

        // This event is generated by Data Source Configuration Wizard
        private void entityInstantFeedbackSource1_DismissQueryable(object sender, GetQueryableEventArgs e)
        {
            // Dispose of the DataContext
            ((Model1)e.Tag).Dispose();
        }

        private void filterControl1_FilterChanged(object sender, FilterChangedEventArgs e)
        {
         
        }

        private void NavButton4_ElementClick(object sender, NavElementEventArgs e)
        {
            _CopyData.CopyEntities.Add(new CopyEntity
            {
                AddFirst = addFirsTextEdit.Text,
                AddLast = addLastTextEdit.Text,
                Replace = replaceTextEdit.Text,
                ReplaceWith = repleceWithTextEdit.Text
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                var fs = new FileStream($@"{Application.CommonAppDataPath}\XmlCopyModel.xml",
                    FileMode.Open);
                var reader =
                    XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                var ser = new DataContractSerializer(_CopyData.CopyEntities.Local.GetType());

                // Deserialize the data and read it from the instance.
                _CopyData.CopyEntities.AddRange((IEnumerable<CopyEntity>)ser.ReadObject(reader, true));
                reader.Close();
                fs.Close();
            }
            catch
            {
                // ignored
            }


            try
            {
                var reader2 = new XmlSerializer(typeof(string));
                var file = new StreamReader($@"{Application.CommonAppDataPath}\XmlFilterSettings.xml");

                filterControl1.FilterString = (string)reader2.Deserialize(file);
                file.Close();
                filterControl1.ApplyFilter();
            }
            catch
            {
                // ignored
            }
        }

        private async void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            var txt = "";
            var linesCount = 0;
            foreach (var item in gridView.GetSelectedRows())
            {
                //var prompt1 = (ReadonlyThreadSafeProxyForObjectFromAnotherThread)gridView.GetRow(item);
                var prompt =
                    ((ReadonlyThreadSafeProxyForObjectFromAnotherThread)gridView.GetRow(item)).OriginalRow as DataClass;

                if (prompt == null) continue;
                var copyList = _CopyData.CopyEntities.Local.ToBindingList();
                if (copyList.Count > 0)
                {
                    foreach (var copyData in copyList)
                    {
                        txt += AddToCopy(copyData, prompt);
                        linesCount++;
                    }
                }
                else
                {
                    txt += AddToCopy(new CopyEntity(), prompt);
                    linesCount++;
                }
            }

            Clipboard.SetText(txt);

            XtraMessageBox.Show($"{linesCount} Promrtes copyed");
        }

        private static string AddToCopy(CopyEntity copyData, DataClass prompt)
        {
            if (string.IsNullOrWhiteSpace(copyData.Replace))
                return copyData.AddFirst + " " + prompt.prompt + " " + copyData.AddLast + "\n";

            if (prompt.prompt != null)
                return copyData.AddFirst + " " + prompt.prompt.Replace(copyData.Replace, copyData.ReplaceWith) +
                       " " + copyData.AddLast + "\n";

            return "";
        }

        private void navButton5_ElementClick(object sender, NavElementEventArgs e)
        {
            using var writer = XmlWriter.Create($@"{Application.CommonAppDataPath}\XmlCopyModel.xml");
            var serializer = new DataContractSerializer(_CopyData.CopyEntities.Local.GetType());
            serializer.WriteObject(writer, _CopyData.CopyEntities.Local);
        }

        private void dockPanel2_Click(object sender, EventArgs e)
        {
        }

        private void dockPanel2_CustomButtonClick(object sender, ButtonEventArgs e)
        {
            try
            {
                if (e.Button.Properties.Tag.ToString() == "Save")
                {
                    var theSerializer = new XmlSerializer(typeof(string));

                    var sw = new StreamWriter($@"{Application.CommonAppDataPath}\XmlFilterSettings.xml");
                    theSerializer.Serialize(sw, filterControl1.FilterCriteria.ToString());


                    sw.Close();
                }
            }
            catch
            {
                // ignored
            }
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            var getImagesInfo = new GetImagesInfo();
            getImagesInfo.ShowDialog();
        }

        private void NavButton3_ElementClick(object sender, NavElementEventArgs e)
        {
            gridView1.DeleteSelectedRows();
        }

        private void navButton2_ElementClick(object sender, NavElementEventArgs e)
        {
            if (gridView1.GetSelectedRows().Length <= 0) return;
            var selectR = gridView1.GetSelectedRows()[0];

            var selectedOb = (CopyEntity)gridView1.GetRow(selectR);

            selectedOb.AddFirst = addFirsTextEdit.Text;
            selectedOb.AddLast = addLastTextEdit.Text;
            selectedOb.Replace = replaceTextEdit.Text;
            selectedOb.ReplaceWith = repleceWithTextEdit.Text;
            gridView1.RefreshRow(selectR);
        }

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (gridView1.GetSelectedRows().Length <= 0) return;
            var selectR = gridView1.GetSelectedRows()[0];

            var selectedOb = (CopyEntity)gridView1.GetRow(selectR);

            addFirsTextEdit.Text = selectedOb.AddFirst;
              addLastTextEdit.Text= selectedOb.AddLast;
             replaceTextEdit.Text= selectedOb.Replace;
             repleceWithTextEdit.Text= selectedOb.ReplaceWith;
        
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            var getImagesInfo = new LoadTxt();
            getImagesInfo.ShowDialog();
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var dataContext = new Model1();
            var allData = dataContext.DataTable.ToList();
            using UnitOfWork uow = new UnitOfWork();
        
            foreach (var pro in allData)
            {
                if(pro.Id >= 1999944) continue;
                var newPro = new Prompt(uow)
                {
                    cfg = pro.cfg,
                    height = pro.height,
                    width = pro.width,
                    image_name = pro.image_name,
                    image_nsfw = pro.image_nsfw,
                    negativePrompt = pro.negativePrompt,
                    part_id = pro.part_id,
                    prompt = pro.prompt,
                    prompt_nsfw = pro.prompt_nsfw,
                    sampler = pro.sampler,
                    samplerName = pro.samplerName,
                    seed = pro.seed,
                    step = pro.step,
                    timestamp = pro.timestamp,
                    user_name = pro.user_name
                };
                Console.WriteLine(pro.Id);
            }
            uow.CommitChanges();
            //Customer customer = new Customer(uow);
            //customer.FirstName = "Mohammad";
            //customer.LastName = "Jaber";
            //uow.CommitChanges();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("don");
        }

        // This event is generated by Data Source Configuration Wizard
        void xpInstantFeedbackSource1_ResolveSession(object sender, ResolveSessionEventArgs e)
        {
            // Assign a session to the Session property,
            e.Session = new DevExpress.Xpo.UnitOfWork();
        }

        // This event is generated by Data Source Configuration Wizard
        void xpInstantFeedbackSource1_DismissSession(object sender, ResolveSessionEventArgs e)
        {
            // Here you can dismiss the session instance you have assigned to the ResolveSessionEventArgs.Session property in the ResolveSession event handler.
            IDisposable session = e.Session as IDisposable;
            if (session != null)
                session.Dispose();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            //xpInstantFeedbackSource1.FixedFilterCriteria = CriteriaOperator.Parse("[Oid] < -10");
            filterControl1.ApplyFilter();
            //xpInstantFeedbackSource1.FixedFilterCriteria = filterControl1.FilterCriteria;
          
        }
    }
}