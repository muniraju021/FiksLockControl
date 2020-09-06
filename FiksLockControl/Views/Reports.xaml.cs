using FiksLockControl.Model;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FiksLockControl.Views
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : UserControl
    {
        private readonly ReportViewModel _reportViewModel;
        public Reports()
        {
            InitializeComponent();
            _reportViewModel = (DataContext as ReportViewModel);
            _reportViewModel.GetVehicleNos();
        }

        private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _reportViewModel.GetLockHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void btnExportReport_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var options = new ExcelExportingOptions();
                options.StartRowIndex = 2;
                options.ExcelVersion = ExcelVersion.Excel2007;
                options.ExportAllPages = true;
                options.ExportingEventHandler = ExportingHandler_1;
                var excelEngine = datagrid.ExportToExcel(datagrid.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                
                workBook.Worksheets[0].UsedRange.AutofitColumns();
                workBook.Worksheets[0].UsedRange.AutofitRows();

                workBook.Worksheets[0].Range["A1"].Text = "VehicleNo: " + _reportViewModel.SelectedVehNo;
                workBook.Worksheets[0].Range["A1:B1"].Merge();
                workBook.Worksheets[0].Range["A1"].HorizontalAlignment = ExcelHAlign.HAlignCenter;

                SaveFileDialog sfd = new SaveFileDialog
                {
                    FilterIndex = 1,
                    Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
                };
                if(sfd.ShowDialog() == true)
                {
                    using (Stream stream = sfd.OpenFile())
                    {
                        if (sfd.FilterIndex == 1)
                            workBook.Version = ExcelVersion.Excel97to2003;

                        else if (sfd.FilterIndex == 2)
                            workBook.Version = ExcelVersion.Excel2010;

                        else
                            workBook.Version = ExcelVersion.Excel2013;
                        workBook.SaveAs(stream);
                    }

                    //Message box confirmation to view the created workbook.
                    if (MessageBox.Show("Do you want to view the extract?", "Excel has been created",
                                        MessageBoxButton.YesNo, MessageBoxImage.Information) 
                                        == MessageBoxResult.Yes)
                    {
                        //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void ExportingHandler_1(object sender, GridExcelExportingEventArgs e)
        {
            if (e.CellType == ExportCellType.StackedHeaderCell)
            {
                e.CellStyle.BackGroundBrush = new SolidColorBrush(Colors.Orchid);
                e.CellStyle.ForeGroundBrush = new SolidColorBrush(Colors.White);
                e.Handled = true;
            }
            else if (e.CellType == ExportCellType.HeaderCell)
            {
                e.CellStyle.BackGroundBrush = new SolidColorBrush(Colors.LightPink);
                e.CellStyle.ForeGroundBrush = new SolidColorBrush(Colors.White);
                e.Handled = true;
            }
            else if (e.CellType == ExportCellType.RecordCell)
            {
                e.CellStyle.BackGroundBrush = new SolidColorBrush(Colors.LightSkyBlue);
                e.Handled = true;
            }

            else if (e.CellType == ExportCellType.GroupCaptionCell)
            {
                e.CellStyle.BackGroundBrush = new SolidColorBrush(Colors.Wheat);
                e.Handled = true;
            }
        }
    }
}
