using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
//using DevExpress.XtraEditors;
using Net.Axilog.BLL;
using Net.Axilog.Model.Devis;
using Net.Axilog.Model.Base;
using System.Diagnostics;
using System.Threading;

namespace Devis2017
{
    public partial class Form1 /*: DevExpress.XtraBars.Ribbon.RibbonForm*/
    {
        private Devis dev;
        private ElementDevis elemActif;
        private List<LigneFabrication> tbList;
        private BindingSource bsDetail=new BindingSource();
        
        public Form1()
        {
            InitializeComponent();

            Trace.Listeners.Add(new ConsoleTraceListener());

            //comboBox1.DataSource = SupportBLL.GetSousSortes();
            //comboBox1.DisplayMember = "nom";
            //comboBox1.ValueMember = "code";
            

            
            //dev=DevisBLL.GetDevisById(14209, 0, 1);
            dev = DevisBLL.GetDevisById(14253, 0, 1);

            teClient.DataBindings.Add("text", dev, "ClientID");
            //EtapeProcess etape=DevisBLL.AddEtape(dev.Elements[0], 4, "DOR");
            decimal[] wrep=new Decimal[5];
            wrep[0]=0.0M;
            wrep[1]=0.0M;
            wrep[2]=1.0M;
            wrep[3]=1.0M;
            wrep[4]=0.0M;

            DevisBLL.AddRubriqueToEtape(dev.Elements[0].EtapesProcess[0], 76);
            DevisBLL.AddReponsesToRubrique(dev.Elements[0].EtapesProcess[0], 76, wrep);

            //DevisBLL.AddRubriqueToEtape(etape, 26);
            //wrep = new Decimal[5];
            //wrep[0] = 1.0M;
            //wrep[1] = 1.0M;
            //wrep[2] = 0.0M;
           // wrep[3] = 1.0M;
            //wrep[4] = 1.0M;
            //DevisBLL.AddReponsesToRubrique(etape, 26, wrep);
            DevisBLL.TraiteElement(dev.Elements[0]);


            //RubriqueChoisie[] rubs=dev.Elements[0].EtapesProcess[0].GetRubriquesChoisiesAsArray();
            //DevisBLL.AddRubriqueToElement(dev.Elements[0], 44);
            //DevisBLL.AddReponsesToRubrique(dev.Elements[0],44, new decimal[] {4.0M,0.0M,0.0M,3407.0M,0.0M});
            //DevisBLL.AddReponsesToRubrique(dev.Elements[0], 2, new String[] { "145", "", "", "", "" });
            //DevisBLL.AddReponsesToRubrique(dev.Elements[0], 9, new String[] { "88", "", "", "", "" });

            //dev.Elements[0].RubriquesChoisies.ForEach(p =>
            //{
            //    p.AValoriser = true;
            //});
            //DevisBLL.TraiteElement(dev.Elements[0]);

            //dev.Faconnage.RubriquesChoisies.ForEach(p =>
            //{
            //    p.AValoriser = true;
            //});
            //DevisBLL.TraiteElement(dev.Faconnage);

            //TableauRubrique tab=DevisBLL.GetTableauRubriqueElement(dev.Categorie.produit, dev.Categorie.Id, dev.Elements[0]);

            //ParametrageDevisBLL.GetRubrique(19);

            BindingSource SBind = new BindingSource();
            SBind.DataSource = dev.Elements;
            dgvElements.DataSource = dev.Elements;
            dgvElements.DataSource = SBind;
            dgvElements.Refresh();

            SBind = new BindingSource();
            SBind.DataSource = dev.GetTableauRecap();

            dataGridView1.DataSource = SBind;
            dataGridView1.Refresh();

            cbMachine.DataSource = null;
            cbMachine.DisplayMember = "NomCourt";
            cbMachine.ValueMember = "Id";
            List<MachineImpression> machines = MachineBLL.GetMachinesImpression();
            //machines.Add(new MachineImpression { Id = String.Empty, NomCourt = String.Empty });
            cbMachine.DataSource = machines;

            //SBind = new BindingSource();
            //SBind.DataSource = dev.Faconnage.GetDetailElement();

            

            //dataGridView3.DataSource = dev.Faconnage.GetDetailElement();

            //dataGridView3.DataSource = SBind;
            //dataGridView3.Refresh();
        }

        private void dgvElements_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.dgvElements.SelectedRows)
            {
                ElementDevis elt = row.DataBoundItem as ElementDevis;
                if (elt != null)
                {
                    tbList = elt.GetDetailElement();
                    //tbList.ListChanged += SBind_ListChanged;
                    
                    
                    bsDetail.AllowNew = true;
                    bsDetail.DataSource = tbList;

                    dgvDetail.DataSource = bsDetail;
                    bsDetail.ListChanged += SBind_ListChanged;
                    dgvDetail.Refresh();
                  
                    tbQteElt.DataBindings.Clear();
                    tbQteElt.DataBindings.Add("Text", elt, "Quantite");

                    chbSansImpression.DataBindings.Clear();
                    chbSansImpression.DataBindings.Add("CheckState", elt, "SansImpression", true, DataSourceUpdateMode.OnPropertyChanged);
                    chbSansImpression.CheckedChanged+=chbSansImpression_CheckedChanged;
                    
                    cbMachine.DataBindings.Clear();
                    //cbMachine.DataBindings.Add("SelectedValue", elt, "machineImpression");

                    Binding bnd = new Binding(
                                      "SelectedItem",
                                      elt,
                                      "machineImpression",
                                      true,
                                      DataSourceUpdateMode.OnPropertyChanged);
                    bnd.NullValue = String.Empty;
                    
                    cbMachine.DataBindings.Add(bnd);
                    cbMachine.ValueMemberChanged += cbMachine_ValueMemberChanged;

                    if (elemActif!=null) elemActif.PropertyChanged -= elt_PropertyChanged;

                    elemActif = elt;

                    elt.PropertyChanged += elt_PropertyChanged;
                }
            }
        }

        void cbMachine_ValueMemberChanged(object sender, EventArgs e)
        {
            Console.WriteLine(sender.GetType().ToString() + " " + e.GetType());
        }

        void SBind_ListChanged(object sender, ListChangedEventArgs e)
        {
            Console.WriteLine(e.ListChangedType.ToString());
            //(sender as BindingSource).
        }

        void elt_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "machineImpression")
            {
                bsDetail.DataSource = null;
                DevisBLL.TraiteElement(sender as ElementDevis);
                tbList = (sender as ElementDevis).GetDetailElement();
                bsDetail.DataSource = tbList; ;
                //tbList.ResetBindings();
                this.dgvDetail.Refresh();
            }
        }

        private void navigationPane1_Click(object sender, EventArgs e)
        {
            //if (sender == navigationPane1)
            //{
            //    if (navigationPane1.SelectedPage == npRecap)
            //    {
            //        BindingSource SBind = new BindingSource();

            //        foreach (var elm in dev.Elements)
            //        {
            //            DevisBLL.TraiteElement(elm);
            //        }
            //        DevisBLL.ValoriseDevis(dev);
            //        SBind.DataSource = dev.GetTableauRecap();
            //        dataGridView1.DataSource = SBind;
            //        dataGridView1.Refresh();
            //        DevisBLL.SaveDevis(dev);
            //    }
            //}

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chbSansImpression_CheckedChanged(object sender, EventArgs e)
        {
            if (chbSansImpression.Checked == true)
            {
                //cbMachine.DataBindings.Clear();
                cbMachine.Enabled = false;
                cbMachine.SelectedIndex = -1;
            }
            else
            {
                cbMachine.Enabled = true;
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DevisBLL.SaveDevis(dev);
        }
        
    }
}