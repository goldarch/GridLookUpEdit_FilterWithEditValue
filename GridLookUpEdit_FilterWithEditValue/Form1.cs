using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace GridLookUpEdit_FilterWithEditValue
{
    public partial class Form1 : Form
    {
        private BindingList<SelectPerson> _selectPersons;

        public Form1()
        {
            InitializeComponent();
            //实体测试
            //bindingSourcePersons.DataSource =new BindingList<Person>(GetList());
            //database测试
            bindingSourcePersons.DataSource = GetDataTable();

            _selectPersons = new BindingList<SelectPerson>()
            {
                new SelectPerson() {Id = 2}
            };

            gridLookUpEdit1.EditValueChanging += gridLookUpEdit1_EditValueChanging;
            gridLookUpEdit1View.CustomDrawRowIndicator += gridLookUpEdit1View_CustomDrawRowIndicator;

            InitLookUp();

            //gridLookUpEdit1.EditValue = 2;
        }


        //显示行号
        private void gridLookUpEdit1View_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        //设置根据多列筛选功能
        //GridLoolUpEdit 默认 是根据 DisplayMember 绑定的字段 进行模糊筛选。
        private void gridLookUpEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            BeginInvoke(new MethodInvoker(delegate ()
            {
                GridLookUpEdit edit = sender as GridLookUpEdit;
                GridView gridView = edit.Properties.View as GridView;
                //获取GriView私有变量
                FieldInfo fi = gridView.GetType().GetField("extraFilter", BindingFlags.NonPublic | BindingFlags.Instance);

                List<FunctionOperator> columnsOperators = new List<FunctionOperator>();
                foreach (GridColumn col in gridView.VisibleColumns)
                {
                    if (col.Visible && col.ColumnType == typeof(string))
                        columnsOperators.Add(new DevExpress.Data.Filtering.FunctionOperator(DevExpress.Data.Filtering.FunctionOperatorType.Contains,
                            new DevExpress.Data.Filtering.OperandProperty(col.FieldName), new DevExpress.Data.Filtering.OperandValue(edit.Text)));
                }
                string filterCondition = new GroupOperator(GroupOperatorType.Or, columnsOperators).ToString();

                //或者明确指定列
                //BinaryOperator op1 = new BinaryOperator("Id", "%" + edit.AutoSearchText + "%", BinaryOperatorType.Like);
                //BinaryOperator op2 = new BinaryOperator("Model", "%" + edit.AutoSearchText + "%", BinaryOperatorType.Like);
                //string filterCondition = new GroupOperator(GroupOperatorType.Or, new CriteriaOperator[] { op1, op2, op3 }).ToString();

                fi.SetValue(gridView, filterCondition);
                //获取GriView中处理列过滤的私有方法
                MethodInfo mi = gridView.GetType().GetMethod("ApplyColumnsFilterEx", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(gridView, null);
            }));
        }

        List<Person> GetList()
        {
            var list = new List<Person>();
            for (int i = 0; i < 20; i++)
            {
                //list.Add(new Person() {Id = i, Address = "Address" + i, Code = "Code0" + i, Name = "Name" + i});
                list.Add(new Person() { Id = i, Address = Util.Str(10,true), Code = Util.Str(10, true), Name = "Name"+ Util.Str(5, true) });
            }

            return list;
        }

        DataTable GetDataTable()
        {
            return Util.GetDataTable(GetList(), typeof(Person));
        }

        private void InitLookUp()
        {
            // Bind the edit value to the Id field of the "Order Details" table;
            // the edit value matches the value of the ValueMember field.

            //Id务必是属性，而不是字段，否则会报以下错误：
            //System.ArgumentException: 'Cannot bind to the property or column Id on the DataSource.
            //Parameter name: dataMember'
            gridLookUpEdit1.DataBindings.Add("EditValue", _selectPersons, "Id");

            // Prevent columns from being automatically created when a data source is assigned.
            gridLookUpEdit1.Properties.PopupView.OptionsBehavior.AutoPopulateColumns = false;
            // The data source for the dropdown rows
            gridLookUpEdit1.Properties.DataSource = bindingSourcePersons;
            // The field for the editor's display text.
            gridLookUpEdit1.Properties.DisplayMember = "Name";
            // The field matching the edit value.
            gridLookUpEdit1.Properties.ValueMember = "Id";


            //双击显示下拉列表
            gridLookUpEdit1.Properties.ShowDropDown = ShowDropDown.DoubleClick;//双击显示下拉列表
            gridLookUpEdit1.Properties.ImmediatePopup = true;//显示下拉列表
            gridLookUpEdit1.Properties.AutoComplete = false;
            gridLookUpEdit1.Properties.TextEditStyle = TextEditStyles.Standard;//允许输入
            gridLookUpEdit1.Properties.NullText = "";//清空默认值
            gridLookUpEdit1.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            //显示不显示grid上第一个空行,也是用于检索的应用
            gridLookUpEdit1.Properties.View.OptionsView.ShowAutoFilterRow = true;
            gridLookUpEdit1.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            gridLookUpEdit1.Properties.ShowFooter = false;

            gridLookUpEdit1View.IndicatorWidth = 50;


            //以下更简洁：添加列： 
            //gridLookUpEdit1.Properties.View.Columns.Add(new GridColumn { FieldName = "ProductClassID", VisibleIndex = 1, Caption = "类别代码", });
            // Add columns in the dropdown:
            // A column to display the values of the Id field;
            GridColumn col1 = gridLookUpEdit1.Properties.PopupView.Columns.AddField("Id");
            col1.VisibleIndex = 0;
            col1.Caption = "Id";
            col1.MinWidth = 80;

            // A column
            //【故意制造不显示】
            GridColumn col2 = gridLookUpEdit1.Properties.PopupView.Columns.AddField("Address");
            col2.VisibleIndex = 1;
            col2.Caption = "Address";
            //
            GridColumn col3 = gridLookUpEdit1.Properties.PopupView.Columns.AddField("Code");
            col3.VisibleIndex = 2;
            col3.Caption = "Code";
            //
            GridColumn col4 = gridLookUpEdit1.Properties.PopupView.Columns.AddField("Name");
            col4.VisibleIndex = 3;
            col4.Caption = "Name";

            // Set column widths according to their contents.
            //gridLookUpEdit1.Properties.PopupView.BestFitColumns();
            // Specify the total dropdown width.
            //或者设置总宽度
            //gridLookUpEdit1.Properties.PopupFormWidth = 500;//或者设置总宽度

            //下拉框自适应宽度
            gridLookUpEdit1.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            gridLookUpEdit1.Properties.View.BestFitColumns();
        }

    }
}