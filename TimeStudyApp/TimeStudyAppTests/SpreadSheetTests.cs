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
using TimeStudy.ViewModels;
using TimeStudyApp.Model;

namespace TimeStudyApp.UnitTests
{
    [TestClass]
    public class SpreadSheetTests
    {
        private const string connString = "/Users/billytomlinson/TimeStudyNew.db3";
        //private const string connString = "TimeStudyNew.db3";

        private readonly IBaseRepository<ActivitySampleStudy> sampleRepo;
        private readonly IBaseRepository<Activity> activityRepo;
        private readonly IBaseRepository<Operator> operatorRepo;
        private readonly IBaseRepository<Observation> observationRepo;
        private readonly IBaseRepository<LapTime> lapTimeRepo;

        List<Operator> operators;
        ActivitySampleStudy sample;
        List<Activity> allStudyActivities;
        List<LapTime> totalLapTimes;
        List<List<ObservationSummary>> allTotals;

        double timePerObservation;
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

            BaseViewModel model = new BaseViewModel(connString);
            Utilities.StudyId = 28;
            Utilities.StudyVersion = 123;
            sampleRepo = new BaseRepository<ActivitySampleStudy>(connString);
            activityRepo = new BaseRepository<Activity>(connString);
            operatorRepo = new BaseRepository<Operator>(connString);
            observationRepo = new BaseRepository<Observation>(connString);
            lapTimeRepo = new BaseRepository<LapTime>(connString);

            BaseViewModel modelA = new BaseViewModel(connString);
            operators = operatorRepo.GetAllWithChildren().Where(cw => cw.StudyId == Utilities.StudyId).ToList();
            sample = sampleRepo.GetItem(Utilities.StudyId);

            IntervalTime = 0; //alarm.Interval / 60;
            allStudyActivities = activityRepo.GetAllWithChildren().Where(x => x.StudyId == Utilities.StudyId).ToList();

            totalLapTimes = lapTimeRepo.GetItems().Where(x => x.StudyId == Utilities.StudyId).ToList();
            var totalCount = totalLapTimes.Count();
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
                headerStyle.Color = Syncfusion.Drawing.Color.FromArgb(255, 174, 33);
                headerStyle.Font.Bold = true;
                headerStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                headerStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                headerStyle.EndUpdate();


                titleStyle = workbook.Styles.Add("TitleStyle");
                titleStyle.BeginUpdate();
                titleStyle.Color = Syncfusion.Drawing.Color.FromArgb(93, 173, 226);
                titleStyle.Font.Bold = true;
                titleStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                titleStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                titleStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                titleStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                titleStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                titleStyle.EndUpdate();

                totalsStyle = workbook.Styles.Add("TotalsStyle");
                totalsStyle.BeginUpdate();
                totalsStyle.Color = Syncfusion.Drawing.Color.FromArgb(255, 255, 153);
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
                detailsStyle.Color = Syncfusion.Drawing.Color.FromArgb(255, 255, 153);
                detailsStyle.Font.Bold = true;
                detailsStyle.Font.Size = 20;
                detailsStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                detailsStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                detailsStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                detailsStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                detailsStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                detailsStyle.EndUpdate();

                BuildStudyDetails();
                CreateAllLapTimesSheet();
                BuildValueAddedRatedActivities();

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
            var destSheetStudyDetails = workbook.Worksheets.Create("Total Study Observations");

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

            var destSheetStudyDetails = workbook.Worksheets.Create("Study Analysis");

            startRowIndex = 2;

            //get all standard rated non forerign laps
            var allLapTimes = lapTimeRepo.GetItems()
                .Where(x => x.StudyId == Utilities.StudyId 
                && x.Version == Utilities.StudyVersion
                && x.Status == RunningStatus.Completed
                && x.IsRated
                && !x.IsForeignElement).ToList();


            var summary = allLapTimes.GroupBy(a => new { a.ActivityId, a.Element})
                                   .Select(g => new LapTimeSummary
                                   {    
                                       ActivityId = g.Key.ActivityId,
                                       Element = g.Key.Element,
                                       NumberOfObservations = g.Count(),
                                       LapTimeTotal = g.Sum(a => a.IndividualLapBMS)

                                   }).ToList();


            destSheetStudyDetails.Range[2, 1].Text = "Element Number";
            destSheetStudyDetails.Range[2, 2].Text = "Description";
            destSheetStudyDetails.Range[2, 3].Text = "Total BMS";
            destSheetStudyDetails.Range[2, 4].Text = "Observed Occassions";
            destSheetStudyDetails.Range[2, 5].Text = "BMS Per Obs Occ";
            destSheetStudyDetails.Range[2, 6].Text = "Frequency Req";
            destSheetStudyDetails.Range[2, 7].Text = "BMS by Freq";
            destSheetStudyDetails.Range[2, 8].Text = "CA. 3%";
            destSheetStudyDetails.Range[2, 9].Text = "RA. 12%";
            destSheetStudyDetails.Range[2, 10].Text = "Element Standard Mins";


            destSheetStudyDetails.Range[4, 1].Text = "Standard Elements";

            var totalCount = 0;

            foreach (var item in summary)
            {

                var bmsPerOccassion = item.LapTimeTotal / item.NumberOfObservations;
                var caAllowance = (bmsPerOccassion * 0.03) + bmsPerOccassion;
                var raAllowance = (caAllowance * 0.12) + caAllowance;

                destSheetStudyDetails.Range[6 + totalCount, 1].Number = item.ActivityId;
                destSheetStudyDetails.Range[6 + totalCount, 2].Text = item.Element;
                destSheetStudyDetails.Range[6 + totalCount, 3].Number = item.LapTimeTotal;
                destSheetStudyDetails.Range[6 + totalCount, 4].Number = item.NumberOfObservations;
                destSheetStudyDetails.Range[6 + totalCount, 5].Number = bmsPerOccassion; ;
                destSheetStudyDetails.Range[6 + totalCount, 6].Number = 1;
                destSheetStudyDetails.Range[6 + totalCount, 7].Number = bmsPerOccassion;
                destSheetStudyDetails.Range[6 + totalCount, 8].Number = caAllowance;
                destSheetStudyDetails.Range[6 + totalCount, 9].Number = raAllowance;
                destSheetStudyDetails.Range[6 + totalCount, 10].Number = raAllowance;

                totalCount = totalCount + 2;
            }

            destSheetStudyDetails.Range[8 + totalCount, 1].Text = "Occassional Elements";

            allLapTimes = lapTimeRepo.GetItems()
               .Where(x => x.StudyId == Utilities.StudyId
               && x.Version == Utilities.StudyVersion
               && x.Status == RunningStatus.Completed
               && x.IsRated
               && x.IsForeignElement).ToList();

            summary = allLapTimes.GroupBy(a => new { a.ActivityId, a.Element })
                                   .Select(g => new LapTimeSummary
                                   {
                                       ActivityId = g.Key.ActivityId,
                                       Element = g.Key.Element,
                                       NumberOfObservations = g.Count(),
                                       LapTimeTotal = g.Sum(a => a.IndividualLapBMS)

                                   }).ToList();

            foreach (var item in summary)
            {
                var bmsPerOccassion = item.LapTimeTotal / item.NumberOfObservations;
                var caAllowance = (bmsPerOccassion * 0.03) + bmsPerOccassion;
                var raAllowance = (caAllowance * 0.12) + caAllowance;

                destSheetStudyDetails.Range[10 + totalCount, 1].Number = item.ActivityId;
                destSheetStudyDetails.Range[10 + totalCount, 2].Text = item.Element;
                destSheetStudyDetails.Range[10 + totalCount, 3].Number = item.LapTimeTotal;
                destSheetStudyDetails.Range[10 + totalCount, 4].Number = item.NumberOfObservations;
                destSheetStudyDetails.Range[10 + totalCount, 5].Number = bmsPerOccassion;
                //destSheetStudyDetails.Range[10 + totalCount, 6].Number = 1;
                //destSheetStudyDetails.Range[10 + totalCount, 7].Number = bmsPerOccassion;
                destSheetStudyDetails.Range[10 + totalCount, 8].Number = caAllowance;
                destSheetStudyDetails.Range[10 + totalCount, 9].Number = raAllowance;
                destSheetStudyDetails.Range[10 + totalCount, 10].Number = raAllowance;

                var columnAddress1 = destSheetStudyDetails.Range[10 + totalCount, 5].AddressLocal;
                var columnAddress2 = destSheetStudyDetails.Range[10 + totalCount, 6].AddressLocal;
                var formula1 = $"={columnAddress1}/{columnAddress2}";
                destSheetStudyDetails.Range[10 + totalCount, 7].Formula = formula1;

                totalCount = totalCount + 2;
            }

            destSheetStudyDetails.Range[10 + totalCount, 1].Text = "Ineffective Elements";

            allLapTimes = lapTimeRepo.GetItems()
                .Where(x => x.StudyId == Utilities.StudyId
                && x.Version == Utilities.StudyVersion
                && x.Status == RunningStatus.Completed
                && !x.IsRated
                && !x.IsForeignElement).ToList();

            summary = allLapTimes.GroupBy(a => new { a.ActivityId, a.Element })
                                   .Select(g => new LapTimeSummary
                                   {
                                       ActivityId = g.Key.ActivityId,
                                       Element = g.Key.Element,
                                       NumberOfObservations = g.Count(),
                                       LapTimeTotal = g.Sum(a => a.IndividualLapBMS)

                                   }).ToList();

            foreach (var item in summary)
            {
                destSheetStudyDetails.Range[12 + totalCount, 1].Number = item.ActivityId;
                destSheetStudyDetails.Range[12 + totalCount, 2].Text = item.Element;
                destSheetStudyDetails.Range[12 + totalCount, 3].Number = item.LapTimeTotal;
                destSheetStudyDetails.Range[12 + totalCount, 4].Number = item.NumberOfObservations;

                totalCount = totalCount + 2;
            }

            destSheetStudyDetails.Range["A2:J2"].CellStyle = headerStyle;
            destSheetStudyDetails.Range[1, 1, 10000, 100].AutofitColumns();
            destSheetStudyDetails.Range["C1:C10000"].NumberFormat = "###0.000";
            destSheetStudyDetails.Range["E1:E10000"].NumberFormat = "###0.000";
            destSheetStudyDetails.Range["G1:G10000"].NumberFormat = "###0.000";
            destSheetStudyDetails.Range["H1:H10000"].NumberFormat = "###0.000";
            destSheetStudyDetails.Range["I1:I10000"].NumberFormat = "###0.000";
            destSheetStudyDetails.Range["J1:J10000"].NumberFormat = "###0.000";
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

        private void CreateAllLapTimesSheet()
        {
                var data = new List<SpreadSheetLapTime>();
                var obs = totalLapTimes
                            .Where(x => x.StudyId == Utilities.StudyId 
                            && x.Version == Utilities.StudyVersion
                            && x.Status == RunningStatus.Completed)
                            .OrderBy(x => x.TotalElapsedTimeDouble).ToList();
                var totalLaptimes = obs.Count();
                foreach (var lap in obs)
                {
                    double individualLapTimeNormalised = 0;
                    if (lap.Rating != null && lap.Rating != 0)
                        individualLapTimeNormalised = lap.IndividualLapTime * (int)lap.Rating / 100;
                    else
                    individualLapTimeNormalised = lap.IndividualLapTime;

                    data.Add(new SpreadSheetLapTime()
                    {
                        StudyId = Utilities.StudyId,
                        TotalElapsedTime = lap.TotalElapsedTimeDouble,
                        IndividualLapTime = lap.IndividualLapTime,
                        IsForeignElement = lap.IsForeignElement,
                        Element = lap.Element,
                        Rating = lap.Rating,
                        ElementId = lap.ActivityId,
                        IndividualLapTimeNormalised = lap.IndividualLapBMS
                     });
                }

                var destSheet = workbook.Worksheets.Create("Complete Study Details");

                destSheet.Range["A1"].Text = "Study";
                destSheet.Range["B1"].Text = "Element ID";
                destSheet.Range["C1"].Text = "Element";
                destSheet.Range["D1"].Text = "Elapsed Time";
                destSheet.Range["E1"].Text = "Lap Time";
                destSheet.Range["F1"].Text = "Foreign Element";
                destSheet.Range["G1"].Text = "Rating";
                destSheet.Range["H1"].Text = "Normalised Lap Time";

                destSheet.ImportData(data, 3, 1, false);

                destSheet.Range["A1:H1"].CellStyle = headerStyle;
                destSheet.Range[1, 1, 1000, 10].AutofitColumns();
                destSheet.Range["D1:D10000"].NumberFormat = "###0.000";
                destSheet.Range["E1:E10000"].NumberFormat = "###0.000";
                destSheet.Range["H1:H10000"].NumberFormat = "###0.000";

                var formula4 = $"=SUM(D3:D{totalLaptimes + 3})";
                var formula5 = $"=SUM(E3:E{totalLaptimes + 3})";
                var formula6 = $"=SUM(H3:H{totalLaptimes + 3})";
                
                destSheet.Range[$"E{totalLaptimes + 4}"].Formula = formula5;
                destSheet.Range[$"H{totalLaptimes + 4}"].Formula = formula6;
        }

    }
}
