using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syncfusion.Drawing;
using Syncfusion.XlsIO;
using TimeStudy.Model;
using TimeStudy.Services;
using TimeStudyApp.Model;

namespace TimeStudyApp.UnitTests
{
    [TestClass]
    public class SpreadSheetTests
    {
        private const string connString = "/Users/billytomlinson/RatedTimeStudy.db3";
        //private const string connString = "WorkStudy1.db3";

        private readonly IBaseRepository<ActivitySampleStudy> sampleRepo;
        private readonly IBaseRepository<Activity> activityRepo;
        private readonly IBaseRepository<Operator> operatorRepo;
        private readonly IBaseRepository<Observation> observationRepo;
        private readonly IBaseRepository<LapTime> lapTimeRepo;
        //private readonly IBaseRepository<AlarmDetails> alarmRepo;

        List<Operator> operators;
        ActivitySampleStudy sample;
        //AlarmDetails alarm;
        List<Activity> allStudyActivities;
        //List<Observation> totalObs;
        List<LapTime> totalLapTimes;
        List<List<ObservationSummary>> allTotals;

        double timePerObservation;
        double totalTimeMinutes;
        int IntervalTime;

        IWorkbook workbook;
        IWorksheet destSheetAll;
        IWorksheet pieAllCategoriesTotal;
        IWorksheet pieAllCategoriesIndividual;
        IWorksheet pieAllNonValueAddedIndividual;

        int startRowIndex;
        int valueAddedActivitiesTotalRowIndex;
        int nonValueAddedActivitiesTotalRowIndex;
        int unRatedActivitiesTotalRowIndex;

        IStyle headerStyle;
        IStyle titleStyle;
        IStyle totalsStyle;
        IStyle detailsStyle;
        IStyle summaryStyle;

        string valueAddedRatedActivitiesRange;
        string nonValueAddedRatedActivitiesRange;
        string unRatedActivitiesRange;
        string valueAddedRatedActivitiesTotal;
        string nonValueAddedRatedActivitiesTotal;
        string unRatedActivitiesTotal;
        int totalsColumn;

        string unratedTotals;

        public SpreadSheetTests()
        {
            Utilities.StudyId = 4;
            sampleRepo = new BaseRepository<ActivitySampleStudy>(connString);
            activityRepo = new BaseRepository<Activity>(connString);
            operatorRepo = new BaseRepository<Operator>(connString);
            observationRepo = new BaseRepository<Observation>(connString);
            lapTimeRepo = new BaseRepository<LapTime>(connString);
            //alarmRepo = new BaseRepository<AlarmDetails>(connString);

            operators = operatorRepo.GetAllWithChildren().Where(cw => cw.StudyId == Utilities.StudyId).ToList();
            sample = sampleRepo.GetItem(Utilities.StudyId);
            //alarm = alarmRepo.GetItem(Utilities.StudyId);
            IntervalTime = 0; //alarm.Interval / 60;
            allStudyActivities = activityRepo.GetAllWithChildren().Where(x => x.StudyId == Utilities.StudyId).ToList();

            //totalObs = observationRepo.GetItems().Where(x => x.StudyId == Utilities.StudyId).ToList();
            totalLapTimes = lapTimeRepo.GetItems().Where(x => x.StudyId == Utilities.StudyId).ToList();
            var totalCount = totalLapTimes.Count();
            //var firstOb = totalObs.Min(y => y.Date);
            //var lastOb = totalObs.Max(y => y.Date);

            var firstOb = totalLapTimes.Min(y => y.TotalElapsedTimeDouble);
            var lastOb = totalLapTimes.Max(y => y.TotalElapsedTimeDouble);
            //totalTimeMinutes = lastOb.Subtract(firstOb).TotalMinutes;
            totalTimeMinutes = lastOb - firstOb;
            timePerObservation = Math.Round(totalTimeMinutes / totalCount, 2);
        }

        [TestMethod]
        public void Create_Excel_Spreadsheet_From_SQL()
        {

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                //Set the default application version as Excel 2013.
                excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2016;

                //Create a workbook with a worksheet
                workbook = excelEngine.Excel.Workbooks.Create(1);

                headerStyle = workbook.Styles.Add("HeaderStyle");
                headerStyle.BeginUpdate();
                headerStyle.Color = Color.FromArgb(255, 174, 33);
                headerStyle.Font.Bold = true;
                headerStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                headerStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                headerStyle.EndUpdate();


                titleStyle = workbook.Styles.Add("TitleStyle");
                titleStyle.BeginUpdate();
                titleStyle.Color = Color.FromArgb(93, 173, 226);
                titleStyle.Font.Bold = true;
                titleStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                titleStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                titleStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                titleStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                titleStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                titleStyle.EndUpdate();

                totalsStyle = workbook.Styles.Add("TotalsStyle");
                totalsStyle.BeginUpdate();
                totalsStyle.Color = Color.FromArgb(255, 255, 153);
                totalsStyle.Font.Bold = true;
                totalsStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                totalsStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                totalsStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                totalsStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                totalsStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                totalsStyle.EndUpdate();

                summaryStyle = workbook.Styles.Add("SummaryStyle");
                summaryStyle.BeginUpdate();
                summaryStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                summaryStyle.EndUpdate();


                detailsStyle = workbook.Styles.Add("DetailsStyle");
                detailsStyle.BeginUpdate();
                detailsStyle.Color = Color.FromArgb(255, 255, 153);
                detailsStyle.Font.Bold = true;
                detailsStyle.Font.Size = 20;
                detailsStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                detailsStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                detailsStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                detailsStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                detailsStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                detailsStyle.EndUpdate();

                BuildStudyDetails();
                CreateSheetPerOperator();

                //destSheetAll = workbook.Worksheets.Create("Summary");

                //pieAllCategoriesTotal = workbook.Worksheets.Create("Totals Chart");
                //pieAllCategoriesIndividual = workbook.Worksheets.Create("Activities Chart");
                //pieAllNonValueAddedIndividual = workbook.Worksheets.Create("Non Value Added Chart");

                //BuildValueAddedRatedActivities();
                //BuildNonValueAddedRatedActivities();
                //BuildUnRatedActivities();
                //Build_ValueAdded_NonValueAdded_Ineffective_PieChart();
                //Build_All_Activities_PieChart();
                //SampleForTesting();

                workbook.Worksheets[0].Remove();

                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    workbook.Close();

                    ms.Seek(0, SeekOrigin.Begin);

                    using (FileStream fs = new FileStream("TimeStudySummary.xlsx", FileMode.OpenOrCreate))
                    {
                        ms.CopyTo(fs);
                        fs.Flush();
                    }
                }
            }
        }

        private void BuildStudyDetails()
        {
            var destSheetStudyDetails = workbook.Worksheets.Create("Study Details");

            destSheetStudyDetails.Range["A2"].CellStyle = detailsStyle;
            destSheetStudyDetails.Range["A3"].CellStyle = detailsStyle;
            destSheetStudyDetails.Range["A4"].CellStyle = detailsStyle;
            destSheetStudyDetails.Range["A5"].CellStyle = detailsStyle;
            destSheetStudyDetails.Range["A6"].CellStyle = detailsStyle;
            destSheetStudyDetails.Range["A7"].CellStyle = detailsStyle;
            destSheetStudyDetails.Range["A8"].CellStyle = detailsStyle;

            destSheetStudyDetails.Range["A2"].Text = "Study Number";
            destSheetStudyDetails.Range["A3"].Text = "Name";
            destSheetStudyDetails.Range["A4"].Text = "Department";
            destSheetStudyDetails.Range["A5"].Text = "Studied By";
            destSheetStudyDetails.Range["A6"].Text = "Date";
            destSheetStudyDetails.Range["A7"].Text = "Time";
            destSheetStudyDetails.Range["A8"].Text = "Rated";


            destSheetStudyDetails.Range["B2"].CellStyle = detailsStyle;
            destSheetStudyDetails.Range["B3"].CellStyle = detailsStyle;
            destSheetStudyDetails.Range["B4"].CellStyle = detailsStyle;
            destSheetStudyDetails.Range["B5"].CellStyle = detailsStyle;
            destSheetStudyDetails.Range["B6"].CellStyle = detailsStyle;
            destSheetStudyDetails.Range["B7"].CellStyle = detailsStyle;
            destSheetStudyDetails.Range["B8"].CellStyle = detailsStyle;

            destSheetStudyDetails.Range["B2"].Text = sample.StudyNumber.ToString();
            destSheetStudyDetails.Range["B3"].Text = sample.Name;
            destSheetStudyDetails.Range["B4"].Text = sample.Department;
            destSheetStudyDetails.Range["B5"].Text = sample.StudiedBy;
            destSheetStudyDetails.Range["B6"].Text = sample.DateFormatted;
            destSheetStudyDetails.Range["B7"].Text = sample.TimeFormatted;
            destSheetStudyDetails.Range["B8"].Text = sample.IsRated.ToString();

            destSheetStudyDetails.Range[1, 1, 8, 2].AutofitColumns();
        }

        private void BuildValueAddedRatedActivities()
        {
            startRowIndex = 5;

            allTotals = new List<List<ObservationSummary>>();

            var allActivities = allStudyActivities.Where(x => x.Rated && x.IsValueAdded)
                .Select(y => new { y.ActivityName.Name }).ToList();

            destSheetAll.Range[3, 1].Text = "Activity";
            destSheetAll.ImportData(allActivities, startRowIndex, 1, false);

            CreateSheetPerOperator();

            var columnCount = 1;

            var computedRange = $"A{startRowIndex}:A{allActivities.Count + startRowIndex}";
            valueAddedRatedActivitiesRange = computedRange;
            var range = destSheetAll[computedRange].ToList();

            foreach (var item in allTotals)
            {
                destSheetAll.Range[3, columnCount + 2].Text = "Obs Req";
                destSheetAll.Range[3, columnCount + 3].Text = "Total Obs";
                destSheetAll.Range[3, columnCount + 4].Text = "% of Total";
                destSheetAll.Range[3, columnCount + 5].Text = "Minutes Total";
                destSheetAll.Range[3, columnCount + 6].Text = "BMS Total";

                foreach (var cell in range.Where(x => x.Value != string.Empty))
                {

                    var v = cell.Value;
                    var c = cell.Row;

                    foreach (var vv in item)
                    {
                        destSheetAll.Range[1, columnCount + 2].Text = vv.OperatorName;
                        destSheetAll.Range[1, columnCount + 2].CellStyle = headerStyle;

                        if (vv.ActivityName == v)
                        {
                            var columnAddress = destSheetAll.Range[c, columnCount + 4].AddressLocal;
                            var formula = $"=SUM(4*{columnAddress})*(100-{columnAddress})/100";

                            var totalMinutes = IntervalTime * vv.NumberOfObservations;
                            var averageRating = vv.TotalRating / vv.NumberOfObservations;
                            var bmi = (double)totalMinutes * averageRating / 100;

                            destSheetAll.Range[c, columnCount + 2].NumberFormat = "###0";
                            destSheetAll.Range[c, columnCount + 2].Formula = formula;
                            destSheetAll.Range[c, columnCount + 3].Number = vv.NumberOfObservations;
                            destSheetAll.Range[c, columnCount + 4].Number = vv.Percentage;
                            destSheetAll.Range[c, columnCount + 5].Number = totalMinutes;
                            destSheetAll.Range[c, columnCount + 6].Number = bmi;
                        }
                    }
                }

                var columnAddress1 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 2].AddressLocal, @"[\d-]", string.Empty);
                var columnAddress2 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 3].AddressLocal, @"[\d-]", string.Empty);
                var columnAddress3 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 4].AddressLocal, @"[\d-]", string.Empty);

                var formula2 = $"=SUM({columnAddress2}{startRowIndex}:{columnAddress2}{allActivities.Count + startRowIndex})";
                var formula3 = $"=SUM({columnAddress3}{startRowIndex}:{columnAddress3}{allActivities.Count + startRowIndex})";

                destSheetAll.Range[allActivities.Count + 6, columnCount + 3].Formula = formula2;
                destSheetAll.Range[allActivities.Count + 6, columnCount + 4].Formula = formula3;

                valueAddedActivitiesTotalRowIndex = allActivities.Count + 6;

                columnCount = columnCount + 6;
            }

            destSheetAll.Range[3, columnCount + 2].Text = "OBS REQ";
            destSheetAll.Range[3, columnCount + 3].Text = "OBS TOTAL";
            destSheetAll.Range[3, columnCount + 4].Text = "% TOTAL";


            foreach (var item in allTotals)
            {
                foreach (var cell in range.Where(x => x.Value != string.Empty))
                {
                    var v = cell.Value;
                    var c = cell.Row;

                    foreach (var vv in item)
                    {
                        if (vv.ActivityName == v)
                        {
                            var columnAddress = destSheetAll.Range[c, columnCount + 4].AddressLocal;
                            var formula = $"=SUM(4*{columnAddress})*(100-{columnAddress})/100";

                            //var totalActivity = totalObs.Count(x => x.ActivityName == v);
                            var totalActivity = totalLapTimes.Count(x => x.Element == v);
                            var totalObsCount = totalLapTimes.Count();
                            var totalPercent = Math.Round((double)totalActivity / totalObsCount * 100, 2);

                            destSheetAll.Range[c, columnCount + 2].NumberFormat = "###0";
                            destSheetAll.Range[c, columnCount + 2].Formula = formula;
                            destSheetAll.Range[c, columnCount + 3].Number = Math.Round((double)totalActivity, 2);
                            destSheetAll.Range[c, columnCount + 4].Number = Math.Round((double)totalPercent, 2);

                            //**** THIS COPIES % TO THE PIE CHART SHEET *********************************
                            destSheetAll.EnableSheetCalculations();
                            pieAllCategoriesIndividual.Range[c, 2].Value = destSheetAll.Range[c, columnCount + 4].CalculatedValue;
                            //************************************
                        }
                    }
                }

                var columnAddress1 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 2].AddressLocal, @"[\d-]", string.Empty);
                var columnAddress2 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 3].AddressLocal, @"[\d-]", string.Empty);
                var columnAddress3 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 4].AddressLocal, @"[\d-]", string.Empty);

                var formula2 = $"=SUM({columnAddress2}{startRowIndex}:{columnAddress2}{allActivities.Count + startRowIndex})";
                var formula3 = $"=SUM({columnAddress3}{startRowIndex}:{columnAddress3}{allActivities.Count + startRowIndex})";

                destSheetAll.Range[allActivities.Count + 6, columnCount + 3].Formula = formula2;
                destSheetAll.Range[allActivities.Count + 6, columnCount + 4].Formula = formula3;

                //**** THIS COPIES % TO THE PIE CHART SHEET *********************************
                destSheetAll.EnableSheetCalculations();
                var source = destSheetAll.Range[allActivities.Count + 6, columnCount + 4].CalculatedValue;
                pieAllCategoriesTotal.Range["A1"].Text = "VALUE ADDED";
                pieAllCategoriesTotal.Range["B1"].Value = source;
                //************************************

            }

            destSheetAll.Range[allActivities.Count + 6, 1].Text = "SUB TOTAL VALUE ADDED";
            destSheetAll.Range[allActivities.Count + 6, 1, allActivities.Count + 6, columnCount + 4].CellStyle = headerStyle;
        }

        private void BuildNonValueAddedRatedActivities()
        {
            allTotals = new List<List<ObservationSummary>>();

            var allActivities = allStudyActivities.Where(x => x.Rated && !x.IsValueAdded)
             .Select(y => new { y.ActivityName.Name }).ToList();

            var startRow = valueAddedActivitiesTotalRowIndex + 2;

            destSheetAll.ImportData(allActivities, startRow, 1, false);

            foreach (var op in operators)
            {
                //var obs = totalObs.Where(x => x.OperatorId == op.Id).ToList();
                var obs = totalLapTimes.Where(x => x.Id == op.Id).ToList();
               
                var totalObsPerOperator = obs.Count();

                //var summary = obs.GroupBy(a => new { a.ActivityId, a.ActivityName, a.Rating })
                var summary = obs.GroupBy(a => new { a.Id, a.Element, a.Rating })
                   .Select(g => new ObservationSummary
                   {
                       //ActivityName = g.Key.ActivityName,
                       //NumberOfObservations = g.Count(),
                       //TotalRating = g.Sum(a => a.Rating)
                       ActivityName = g.Key.Element,
                       NumberOfObservations = g.Count(),
                       TotalRating = g.Sum(a => (int)a.Rating)

                   }).ToList();

                foreach (var item in summary)
                {
                    var totalPercentage = Math.Round((double)item.NumberOfObservations / totalObsPerOperator * 100, 2);
                    item.Percentage = totalPercentage;
                    item.TotalTime = item.NumberOfObservations * timePerObservation;
                    item.OperatorName = op.Name;
                }

                allTotals.Add(summary);
            }

            var columnCount = 1;

            var computedRange = $"A{startRow}:A{startRow + allActivities.Count}";
            nonValueAddedRatedActivitiesRange = computedRange;
            var range = destSheetAll[computedRange].ToList();

            foreach (var item in allTotals)
            {
                foreach (var cell in range.Where(x => x.Value != string.Empty))
                {

                    var v = cell.Value;
                    var c = cell.Row;

                    foreach (var vv in item)
                    {
                        if (vv.ActivityName == v)
                        {
                            var columnAddress = destSheetAll.Range[c, columnCount + 4].AddressLocal;
                            var formula = $"=SUM(4*{columnAddress})*(100-{columnAddress})/100";

                            var totalMinutes = IntervalTime * vv.NumberOfObservations;
                            var averageRating = vv.TotalRating / vv.NumberOfObservations;
                            var bmi = (double)totalMinutes * averageRating / 100;

                            destSheetAll.Range[c, columnCount + 2].NumberFormat = "###0";
                            destSheetAll.Range[c, columnCount + 2].Formula = formula;
                            destSheetAll.Range[c, columnCount + 3].Number = vv.NumberOfObservations;
                            destSheetAll.Range[c, columnCount + 4].Number = vv.Percentage;
                            destSheetAll.Range[c, columnCount + 5].Number = totalMinutes;
                            destSheetAll.Range[c, columnCount + 6].Number = bmi;
                        }
                    }
                }

                // Total All Unrated observations
                var columnAddress1 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 2].AddressLocal, @"[\d-]", string.Empty);
                var columnAddress2 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 3].AddressLocal, @"[\d-]", string.Empty);
                var columnAddress3 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 4].AddressLocal, @"[\d-]", string.Empty);

                var formula2 = $"=SUM({columnAddress2}{startRow}:{columnAddress2}{allActivities.Count + startRow})";
                var formula3 = $"=SUM({columnAddress3}{startRow}:{columnAddress3}{allActivities.Count + startRow})";

                destSheetAll.Range[allActivities.Count + startRow + 1, columnCount + 3].Formula = formula2;
                destSheetAll.Range[allActivities.Count + startRow + 1, columnCount + 4].Formula = formula3;

                nonValueAddedActivitiesTotalRowIndex = allActivities.Count + startRow + 1;

                columnCount = columnCount + 6;
            }

            foreach (var item in allTotals)
            {
                foreach (var cell in range.Where(x => x.Value != string.Empty))
                {
                    var v = cell.Value;
                    var c = cell.Row;

                    foreach (var vv in item)
                    {
                        if (vv.ActivityName == v)
                        {
                            var columnAddress = destSheetAll.Range[c, columnCount + 4].AddressLocal;
                            var formula = $"=SUM(4*{columnAddress})*(100-{columnAddress})/100";

                            //var totalActivity = totalObs.Count(x => x.ActivityName == v);
                            var totalActivity = totalLapTimes.Count(x => x.Element == v);
                            var totalObsCount = totalLapTimes.Count();
                            var totalPercent = Math.Round((double)totalActivity / totalObsCount * 100, 2);

                            destSheetAll.Range[c, columnCount + 2].NumberFormat = "###0";
                            destSheetAll.Range[c, columnCount + 2].Formula = formula;
                            destSheetAll.Range[c, columnCount + 3].Number = Math.Round((double)totalActivity, 2);
                            destSheetAll.Range[c, columnCount + 4].Number = Math.Round((double)totalPercent, 2);

                            //**** THIS COPIES % TO THE PIE CHART SHEET *********************************
                            destSheetAll.EnableSheetCalculations();
                            pieAllCategoriesIndividual.Range[c, 2].Value = destSheetAll.Range[c, columnCount + 4].CalculatedValue;
                            pieAllNonValueAddedIndividual.Range[c, 2].Value = destSheetAll.Range[c, columnCount + 4].CalculatedValue;
                            //************************************
                        }
                    }
                }

                //total all unrated totals of all operators
                var columnAddress2 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 3].AddressLocal, @"[\d-]", string.Empty);
                var columnAddress3 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 4].AddressLocal, @"[\d-]", string.Empty);

                var formula2 = $"=SUM({columnAddress2}{startRow}:{columnAddress2}{allActivities.Count + startRow})";
                var formula3 = $"=SUM({columnAddress3}{startRow}:{columnAddress3}{allActivities.Count + startRow})";

                destSheetAll.Range[allActivities.Count + startRow + 1, columnCount + 3].Formula = formula2;
                destSheetAll.Range[allActivities.Count + startRow + 1, columnCount + 4].Formula = formula3;

                //**** THIS COPIES % TO THE PIE CHART SHEET *********************************
                destSheetAll.EnableSheetCalculations();
                var source = destSheetAll.Range[allActivities.Count + startRow + 1, columnCount + 4].CalculatedValue;
                pieAllCategoriesTotal.Range["A2"].Text = "NON VALUE ADDED";
                pieAllCategoriesTotal.Range["B2"].Value = source;
                //************************************

                destSheetAll.Range[allActivities.Count + startRow + 1, 1].Text = "SUB TOTAL NON VALUE ADDED";
                destSheetAll.Range[allActivities.Count + startRow + 1, 1, allActivities.Count + startRow + 1, columnCount + 4].CellStyle = headerStyle;
            }

            destSheetAll.Range[allActivities.Count + startRow + 1, 1].Text = "SUB TOTAL NON VALUE ADDED";
            destSheetAll.Range[allActivities.Count + startRow + 1, 1, allActivities.Count + startRow + 1, columnCount + 4].CellStyle = headerStyle;
        }

        private void BuildUnRatedActivities()
        {
            allTotals = new List<List<ObservationSummary>>();

            var allActivities = allStudyActivities.Where(x => !x.Rated)
             .Select(y => new { y.ActivityName.Name }).ToList();

            var startRow = nonValueAddedActivitiesTotalRowIndex + 2;

            destSheetAll.ImportData(allActivities, startRow, 1, false);

            foreach (var op in operators)
            {
                //var obs = totalObs.Where(x => x.OperatorId == op.Id).ToList();
                var obs = totalLapTimes.Where(x => x.Id == op.Id).ToList();
                var totalObsPerOperator = obs.Count();

                //var summary = obs.GroupBy(a => new { a.ActivityId, a.ActivityName })
                    var summary = obs.GroupBy(a => new { a.Id, a.Element })
                    .Select(g => new ObservationSummary
                    {
                        //ActivityName = g.Key.ActivityName,
                        ActivityName = g.Key.Element,
                        NumberOfObservations = g.Count()
                    }).ToList();

                foreach (var item in summary)
                {
                    var totalPercentage = Math.Round((double)item.NumberOfObservations / totalObsPerOperator * 100, 2);
                    item.Percentage = totalPercentage;
                    item.TotalTime = item.NumberOfObservations * timePerObservation;
                    item.OperatorName = op.Name;
                }

                allTotals.Add(summary);
            }

            var columnCount = 1;

            var computedRange = $"A{startRow}:A{startRow + allActivities.Count}";
            unRatedActivitiesRange = computedRange;
            var range = destSheetAll[computedRange].ToList();

            foreach (var item in allTotals)
            {
                foreach (var cell in range.Where(x => x.Value != string.Empty))
                {

                    var v = cell.Value;
                    var c = cell.Row;

                    foreach (var vv in item)
                    {
                        if (vv.ActivityName == v)
                        {

                            var columnAddress = destSheetAll.Range[c, columnCount + 4].AddressLocal;
                            var formula = $"=SUM(4*{columnAddress})*(100-{columnAddress})/100";

                            destSheetAll.Range[c, columnCount + 2].NumberFormat = "###0";
                            destSheetAll.Range[c, columnCount + 2].Formula = formula;
                            destSheetAll.Range[c, columnCount + 3].Number = vv.NumberOfObservations;
                            destSheetAll.Range[c, columnCount + 4].Number = vv.Percentage;

                        }
                    }
                }

                // Total All Unrated observations
                var columnAddress1 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 2].AddressLocal, @"[\d-]", string.Empty);
                var columnAddress2 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 3].AddressLocal, @"[\d-]", string.Empty);
                var columnAddress3 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 4].AddressLocal, @"[\d-]", string.Empty);

                var formula2 = $"=SUM({columnAddress2}{startRow}:{columnAddress2}{allActivities.Count + startRow})";
                var formula3 = $"=SUM({columnAddress3}{startRow}:{columnAddress3}{allActivities.Count + startRow})";

                destSheetAll.Range[allActivities.Count + startRow + 1, columnCount + 3].Formula = formula2;
                destSheetAll.Range[allActivities.Count + startRow + 1, columnCount + 4].Formula = formula3;

                unRatedActivitiesTotalRowIndex = allActivities.Count + startRow + 1;

                // Total All observations  - Add together total value added +  total value added +  total unrated
                var formula4 = $"=TEXT(SUM({columnAddress1}{startRowIndex}:{columnAddress1}{unRatedActivitiesTotalRowIndex}), \"####\")";
                var formula5 = $"=SUM({columnAddress2}{valueAddedActivitiesTotalRowIndex}+{columnAddress2}{nonValueAddedActivitiesTotalRowIndex}+{columnAddress2}{unRatedActivitiesTotalRowIndex})";
                var formula6 = $"=TEXT(SUM({columnAddress3}{valueAddedActivitiesTotalRowIndex}+{columnAddress3}{nonValueAddedActivitiesTotalRowIndex}+{columnAddress3}{unRatedActivitiesTotalRowIndex}), \"00.0\")";

                destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, columnCount + 2].Formula = formula4;
                destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, columnCount + 3].NumberFormat = "###0";
                destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, columnCount + 3].Formula = formula5;
                destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, columnCount + 4].Formula = formula6;

                columnCount = columnCount + 6;
            }

            foreach (var item in allTotals)
            {
                foreach (var cell in range.Where(x => x.Value != string.Empty))
                {
                    var v = cell.Value;
                    var c = cell.Row;

                    foreach (var vv in item)
                    {
                        if (vv.ActivityName == v)
                        {

                            var columnAddress = destSheetAll.Range[c, columnCount + 4].AddressLocal;
                            var formula = $"=SUM(4*{columnAddress})*(100-{columnAddress})/100";

                            //var totalActivity = totalObs.Count(x => x.ActivityName == v);
                            var totalActivity = totalLapTimes.Count(x => x.Element == v);
                            var totalObsCount = totalLapTimes.Count();
                            var totalPercent = Math.Round((double)totalActivity / totalObsCount * 100, 2);
                            var totalPerActivity = vv.TotalTime * totalActivity;

                            destSheetAll.Range[c, columnCount + 2].NumberFormat = "###0";
                            destSheetAll.Range[c, columnCount + 2].Formula = formula;
                            destSheetAll.Range[c, columnCount + 3].Number = Math.Round((double)totalActivity, 2);
                            destSheetAll.Range[c, columnCount + 4].Number = Math.Round((double)totalPercent, 2);

                            //**** THIS COPIES % TO THE PIE CHART SHEET *********************************
                            destSheetAll.EnableSheetCalculations();
                            pieAllCategoriesIndividual.Range[c, 2].Value = destSheetAll.Range[c, columnCount + 4].CalculatedValue;
                            //************************************
                        }
                    }
                }

                //total all unrated totals of all operators
                var columnAddress1 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 2].AddressLocal, @"[\d-]", string.Empty);
                var columnAddress2 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 3].AddressLocal, @"[\d-]", string.Empty);
                var columnAddress3 = Regex.Replace(destSheetAll.Range[allActivities.Count + 6, columnCount + 4].AddressLocal, @"[\d-]", string.Empty);

                var formula1 = $"=SUM({columnAddress1}{startRow}:{columnAddress1}{allActivities.Count + startRow})";
                var formula2 = $"=SUM({columnAddress2}{startRow}:{columnAddress2}{allActivities.Count + startRow})";
                var formula3 = $"=SUM({columnAddress3}{startRow}:{columnAddress3}{allActivities.Count + startRow})";

                destSheetAll.Range[allActivities.Count + startRow + 1, columnCount + 3].Formula = formula2;
                destSheetAll.Range[allActivities.Count + startRow + 1, columnCount + 4].Formula = formula3;

                //**** THIS COPIES % TO THE PIE CHART SHEET *********************************
                destSheetAll.EnableSheetCalculations();
                var source = destSheetAll.Range[allActivities.Count + startRow + 1, columnCount + 4].CalculatedValue;
                pieAllCategoriesTotal.Range["A3"].Text = "INNEFECTIVE";
                pieAllCategoriesTotal.Range["B3"].Value = source;
                //************************************

                destSheetAll.Range[allActivities.Count + startRow + 1, 1].Text = "SUB TOTAL INEFFECTIVE";
                destSheetAll.Range[allActivities.Count + startRow + 1, 1, allActivities.Count + startRow + 1, columnCount + 4].CellStyle = headerStyle;


                // Total All observations  - Add together total value added +  total value added +  total unrated
                var formula4 = $"=TEXT(SUM({columnAddress1}{startRowIndex}:{columnAddress1}{unRatedActivitiesTotalRowIndex}), \"####\")";
                var formula5 = $"=SUM({columnAddress2}{valueAddedActivitiesTotalRowIndex}+{columnAddress2}{nonValueAddedActivitiesTotalRowIndex}+{columnAddress2}{unRatedActivitiesTotalRowIndex})";
                var formula6 = $"=TEXT(SUM({columnAddress3}{valueAddedActivitiesTotalRowIndex}+{columnAddress3}{nonValueAddedActivitiesTotalRowIndex}+{columnAddress3}{unRatedActivitiesTotalRowIndex}), \"00.0\")";

                valueAddedRatedActivitiesTotal = $"{columnAddress3}{valueAddedActivitiesTotalRowIndex}";
                nonValueAddedRatedActivitiesTotal = $"{columnAddress3}{nonValueAddedActivitiesTotalRowIndex}";
                unRatedActivitiesTotal = $"{columnAddress3}{unRatedActivitiesTotalRowIndex}";

                //**** THIS TOTALS ALL THE TOTALS AT THE END OF THE SHEET *********************************
                destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, columnCount + 2].Formula = formula4;
                destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, columnCount + 3].NumberFormat = "###0";
                destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, columnCount + 3].Formula = formula5;
                destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, columnCount + 4].Formula = formula6;
                totalsColumn = columnCount + 4;

                //******************************************************************************************


                //**** THIS STYLES THE TOTALS *********************************
                destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, 1].Text = "TOTAL";
                destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, 1, unRatedActivitiesTotalRowIndex + 2, columnCount + 4].CellStyle = headerStyle;

                destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, columnCount + 2].CellStyle = titleStyle;
                destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, columnCount + 3].CellStyle = titleStyle;
                destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, columnCount + 4].CellStyle = titleStyle;
                destSheetAll.Range[3, 1, 3, columnCount + 4].CellStyle = titleStyle;


                destSheetAll.Range[1, 1, unRatedActivitiesTotalRowIndex + 2, columnCount + 4].AutofitColumns();
                //******************************************************************************************
            }

            destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, 1].Text = "TOTAL";
            destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, 1, unRatedActivitiesTotalRowIndex + 2, columnCount + 4].CellStyle = headerStyle;

            destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, columnCount + 2].CellStyle = totalsStyle;
            destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, columnCount + 3].CellStyle = totalsStyle;
            destSheetAll.Range[unRatedActivitiesTotalRowIndex + 2, columnCount + 4].CellStyle = totalsStyle;

            destSheetAll.Range[3, columnCount + 2].CellStyle = totalsStyle;
            destSheetAll.Range[3, columnCount + 3].CellStyle = totalsStyle;
            destSheetAll.Range[3, columnCount + 4].CellStyle = totalsStyle;
        }

        private void CreateSheetPerOperator()
        {
            //foreach (var op in operators)
            //{
                var data = new List<SpreadSheetLapTime>();
                //var obs = totalObs.Where(x => x.OperatorId == op.Id).ToList();
                var obs = totalLapTimes.Where(x => x.StudyId == Utilities.StudyId).ToList();
                var totalLaptimes = obs.Count();
                foreach (var lap in obs)
                {
                    var formattedDate = "Date";//$"{observation.Date:d/M/yyyy HH:mm:ss}";
                    data.Add(new SpreadSheetLapTime()
                    {
                        StudyId = Utilities.StudyId,
                        TotalElapsedTime = lap.TotalElapsedTime,
                        IndividualLapTimeFormatted = lap.IndividualLapTimeFormatted,
                        IsForeignElement = lap.IsForeignElement,
                        Element = lap.Element,
                        Rating = lap.Rating
                    });
                }

                var destSheet = workbook.Worksheets.Create("Complete Study Details");

                destSheet.Range["A1"].Text = "Study";
                destSheet.Range["B1"].Text = "Element";
                destSheet.Range["C1"].Text = "Elapsed Time";
                destSheet.Range["D1"].Text = "Lap Time";
                destSheet.Range["E1"].Text = "Foreign Element";
                destSheet.Range["F1"].Text = "Rating";

            destSheet.ImportData(data, 3, 1, false);

                destSheet.Range["A1:F1"].CellStyle = headerStyle;
                destSheet.Range[1, 1, 1000, 6].AutofitColumns();

                //var summary = obs.GroupBy(a => new { a.ActivityId, a.ActivityName })
                    //.Select(g => new ObservationSummary
                    //{
                    //    ActivityName = g.Key.ActivityName,
                    //    NumberOfObservations = g.Count(),
                    //    TotalRating = g.Sum(a => a.Rating)

                    //}).ToList();


                //var summary = obs.GroupBy(a => new { a.Id, a.Element })
                //.Select(g => new ObservationSummary
                //{
                //    ActivityName = g.Key.Element,
                //    NumberOfObservations = g.Count(),
                //    TotalRating = g.Sum(a => (int)a.Rating)

                //}).ToList();

                //int counter = 1;
                //foreach (var item in summary)
                //{
                //    if (item.TotalRating > 0)
                //    {
                //        var totalMinutes = IntervalTime * item.NumberOfObservations;
                //        var averageRating = (double)item.TotalRating / item.NumberOfObservations;

                //        destSheet.Range["I1:I1"].Text = "Summary";

                //        destSheet.Range[counter + 1, 8].Text = item.ActivityName;
                //        destSheet.Range[counter + 2, 8].Text = "Total Rating";
                //        destSheet.Range[counter + 2, 9].Text = "Total Obs";
                //        destSheet.Range[counter + 2, 10].Text = "Average Rating";
                //        destSheet.Range[counter + 3, 8].Number = item.TotalRating;
                //        destSheet.Range[counter + 3, 9].Number = item.NumberOfObservations;
                //        destSheet.Range[counter + 3, 10].Number = averageRating;

                //        destSheet.Range[counter + 1, 8, counter + 3, 10].CellStyle = summaryStyle;
                //        destSheet.Range[counter + 1, 8].CellStyle = titleStyle;
                //        destSheet.Range["H1:J1"].CellStyle = headerStyle;
                //        destSheet.Range[counter + 2, 8, counter + 2, 10].CellStyle = totalsStyle;
                //        destSheet.Range[counter + 3, 10].NumberFormat = "00.0#";

                //        counter = counter + 4;
                //    }

                //    destSheet.Range[1, 7, counter, 11].AutofitColumns();

                //    var totalPercentage = Math.Round((double)item.NumberOfObservations / totalLaptimes * 100, 2);
                //    item.Percentage = totalPercentage;
                //    item.TotalTime = item.NumberOfObservations * timePerObservation;
                //    //item.OperatorName = op.Name;
                //}

                //allTotals.Add(summary);
            //}
        }

        private void SampleForTesting()
        {
            //var destSheet = workbook.Worksheets.Create("PieChart");

            destSheetAll.EnableSheetCalculations();
            pieAllCategoriesTotal.EnableSheetCalculations();

            //Assigns an object to the range of cells (90 rows) both for source and destination.
            IRange source = destSheetAll.Range["A5:A9"];
            IRange des = pieAllCategoriesTotal.Range["A5:A9"];
            source.CopyTo(des);

            source = destSheetAll.Range["Y5:Y9"];
            des = pieAllCategoriesTotal.Range["B5:B9"];

            //Copies from Source to Destination worksheet.
            source.CopyTo(des);

            IChartShape chart = pieAllCategoriesTotal.Charts.Add();

            chart.DataRange = pieAllCategoriesTotal.Range["A1:B3"];

            chart.ChartTitle = "Exploded Pie Chart";
            chart.HasLegend = true;
            chart.Legend.Position = ExcelLegendPosition.Right;

            IChartSerie serie = chart.Series[0];
            serie.DataPoints.DefaultDataPoint.DataLabels.IsPercentage = true;


            chart.TopRow = 4;
            chart.LeftColumn = 4;
            chart.BottomRow = 23;
            chart.RightColumn = 10;

            chart.ChartType = ExcelChartType.Pie_Exploded;
            chart.Elevation = 70;
            chart.DisplayBlanksAs = ExcelChartPlotEmpty.Interpolated;


            chart.IsSeriesInRows = false;

        }

        private void Build_ValueAdded_NonValueAdded_Ineffective_PieChart()
        {

            IChartShape chart = pieAllCategoriesTotal.Charts.Add();

            pieAllCategoriesTotal.InsertRow(1, 4);
            chart.DataRange = pieAllCategoriesTotal.Range["A5:B7"];
            pieAllCategoriesTotal.Range["A5:B7"].AutofitColumns();

            chart.ChartTitle = "Value Added, NonValueAdded, Ineffective";
            chart.HasLegend = true;
            chart.Legend.Position = ExcelLegendPosition.Right;

            IChartSerie serie = chart.Series[0];
            serie.DataPoints.DefaultDataPoint.DataLabels.IsPercentage = true;

            chart.TopRow = 1;
            chart.LeftColumn = 3;
            chart.BottomRow = 30;
            chart.RightColumn = 15;

            chart.ChartType = ExcelChartType.Pie_Exploded;
            chart.Elevation = 70;
            chart.DisplayBlanksAs = ExcelChartPlotEmpty.Interpolated;

            chart.IsSeriesInRows = false;

        }

        private void Build_All_Activities_PieChart()
        {
            startRowIndex = 5;

            allTotals = new List<List<ObservationSummary>>();

            var allActivities = allStudyActivities.Where(x => x.Rated && x.IsValueAdded)
                .Select(y => new { y.ActivityName.Name }).ToList();

            pieAllCategoriesIndividual.ImportData(allActivities, startRowIndex, 1, false);

            //delete empty rows under value added and move non value added up
            pieAllCategoriesIndividual.Range[startRowIndex + allActivities.Count, 1, startRowIndex + allActivities.Count + 2, 2].Clear(ExcelMoveDirection.MoveUp);

            allActivities = allStudyActivities.Where(x => x.Rated && !x.IsValueAdded)
                .Select(y => new { y.ActivityName.Name }).ToList();

            var startRow = valueAddedActivitiesTotalRowIndex - 1;

            pieAllCategoriesIndividual.ImportData(allActivities, startRow, 1, false);
            pieAllNonValueAddedIndividual.ImportData(allActivities, startRow + 3, 1, false);

            var allNonValueAddedIndividualRange = $"A{startRow + 3}:B{startRow + 2 + allActivities.Count}";

            //delete empty rows under non value added and move ineefective up
            pieAllCategoriesIndividual.Range[startRow + allActivities.Count, 1, startRow + allActivities.Count + 2, 2].Clear(ExcelMoveDirection.MoveUp);

            allActivities = allStudyActivities.Where(x => !x.Rated)
                .Select(y => new { y.ActivityName.Name }).ToList();

            startRow = nonValueAddedActivitiesTotalRowIndex;

            pieAllCategoriesIndividual.ImportData(allActivities, startRow - 4, 1, false);

            IChartShape chart = pieAllCategoriesIndividual.Charts.Add();

            var range = $"A5:B{startRow - 2}";
            chart.DataRange = pieAllCategoriesIndividual.Range[range];

            pieAllCategoriesIndividual.Range[range].AutofitColumns();

            chart.ChartTitle = "All Activities";
            chart.HasLegend = true;
            chart.DisplayBlanksAs = ExcelChartPlotEmpty.NotPlotted;
            chart.Legend.Position = ExcelLegendPosition.Right;

            IChartSerie serie = chart.Series[0];
            serie.DataPoints.DefaultDataPoint.DataLabels.IsPercentage = true;

            chart.TopRow = 1;
            chart.LeftColumn = 4;
            chart.BottomRow = 30;
            chart.RightColumn = 17;

            chart.ChartType = ExcelChartType.Pie_Exploded;
            chart.Elevation = 70;
            chart.DisplayBlanksAs = ExcelChartPlotEmpty.Interpolated;

            chart.IsSeriesInRows = false;

            IChartShape chart2 = pieAllNonValueAddedIndividual.Charts.Add();

            chart2.DataRange = pieAllNonValueAddedIndividual.Range[allNonValueAddedIndividualRange];

            pieAllNonValueAddedIndividual.Range[allNonValueAddedIndividualRange].AutofitColumns();

            chart2.ChartTitle = "All Non Value Added";
            chart2.HasLegend = true;
            chart2.DisplayBlanksAs = ExcelChartPlotEmpty.NotPlotted;
            chart2.Legend.Position = ExcelLegendPosition.Right;

            IChartSerie serie1 = chart2.Series[0];
            serie1.DataPoints.DefaultDataPoint.DataLabels.IsPercentage = true;

            chart2.TopRow = 1;
            chart2.LeftColumn = 4;
            chart2.BottomRow = 30;
            chart2.RightColumn = 17;

            chart2.ChartType = ExcelChartType.Pie_Exploded;
            chart2.Elevation = 70;
            chart2.DisplayBlanksAs = ExcelChartPlotEmpty.Interpolated;

            chart2.IsSeriesInRows = false;

        }
    }
}
