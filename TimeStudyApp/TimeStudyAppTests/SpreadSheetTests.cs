using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private const string connString = "/Users/billytomlinson/TimeStudyProduction.db3";
        //private const string connString = "TimeStudyDBNew.db3";

        private readonly IBaseRepository<TimeStudy.Model.RatedTimeStudy> sampleRepo;
        private readonly IBaseRepository<WorkElement> activityRepo;
        private readonly IBaseRepository<LapTimeHistoric> lapTimeRepo;
        private readonly IBaseRepository<RatedTimeStudyHistoryVersion> studyVersionRepo;
        //private readonly IBaseRepository<LapTimeHistoric> lapTimeHistoricRepo;

        TimeStudy.Model.RatedTimeStudy sample;
        List<WorkElement> allStudyActivities;
        //List<LapTime> totalLapTimes;
        List<LapTimeHistoric> totalLapTimes;

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
        IStyle frequencyStyle;
        IStyle worksheetStyle;

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
            Utilities.StudyId = 2;
            Utilities.StudyVersion = 35;
            sampleRepo = new BaseRepository<TimeStudy.Model.RatedTimeStudy>(connString);
            activityRepo = new BaseRepository<WorkElement>(connString);
            //lapTimeRepo = new BaseRepository<LapTime>(connString);
            studyVersionRepo = new BaseRepository<RatedTimeStudyHistoryVersion>(connString);
            lapTimeRepo = new BaseRepository<LapTimeHistoric>(connString);

            BaseViewModel modelA = new BaseViewModel(connString);
            sample = sampleRepo.GetItem(Utilities.StudyId);

            IntervalTime = 0; //alarm.Interval / 60;
            allStudyActivities = activityRepo.GetAllWithChildren().Where(x => x.StudyId == Utilities.StudyId).ToList();

            //totalLapTimes = lapTimeRepo.GetItems().Where(x => x.StudyId == Utilities.StudyId).ToList();
            totalLapTimes = lapTimeRepo.GetItems().Where(x => x.StudyId == Utilities.StudyId).ToList();
            var totalCount = totalLapTimes.Count();
        }

        [TestMethod]
        public void Create_Excel_Spreadsheet_From_SQL()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTAwMjk5QDMxMzYyZTMzMmUzMEdpQVpZS0g1RHZMN2RGVUEreFpGOXp3UW12dmt4RW00U09OSFdnOVd6SG89;MTAwMzAwQDMxMzYyZTMzMmUzMElsS3RQMk4ycm1mUlJFa3JYY1A3cUpzMnRsYmdkSTdoTVBGcHlWa1BQU1U9");

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
                headerStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                headerStyle.EndUpdate();


                titleStyle = workbook.Styles.Add("TitleStyle");
                titleStyle.BeginUpdate();
                titleStyle.Color = Syncfusion.Drawing.Color.FromArgb(93, 173, 226);
                titleStyle.Font.Bold = true;
                titleStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                titleStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                titleStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                titleStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                titleStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                titleStyle.EndUpdate();

                totalsStyle = workbook.Styles.Add("TotalsStyle");
                totalsStyle.BeginUpdate();
                totalsStyle.Color = Syncfusion.Drawing.Color.FromArgb(255, 255, 153);
                totalsStyle.Font.Bold = true;
                totalsStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                totalsStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                totalsStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                totalsStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                totalsStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
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
                detailsStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                detailsStyle.EndUpdate();


                frequencyStyle = workbook.Styles.Add("FrequencyStyle");
                frequencyStyle.BeginUpdate();
                frequencyStyle.Color = Syncfusion.Drawing.Color.FromArgb(255, 255, 153);
                frequencyStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                frequencyStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                frequencyStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                frequencyStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                frequencyStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                frequencyStyle.EndUpdate();

                worksheetStyle = workbook.Styles.Add("WorkSheetStyle");
                worksheetStyle.BeginUpdate();
                worksheetStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheetStyle.EndUpdate();


                BuildStudyDetails();
                CreateAllLapTimesSheet();
                BuildStudyAnalysisDetails();

                workbook.Worksheets[0].Remove();

                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    workbook.Close();

                    ms.Seek(0, SeekOrigin.Begin);

                    using (FileStream fs = new FileStream("TimeStudySummary4.xlsx", FileMode.OpenOrCreate))
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

        private void BuildStudyAnalysisDetails()
        {

            int summaryCount = 0;

            var allRatedLapTimes = lapTimeRepo.GetItems()
                        .Where(x => x.StudyId == Utilities.StudyId
                        && x.Version == Utilities.StudyVersion
                        && x.Status == RunningStatus.Completed
                        && x.IsRated).ToList();

            double totalBMS = allRatedLapTimes.Sum(x => x.IndividualLapBMS);
            double totalObservedTime = allRatedLapTimes.Sum(x => x.IndividualLapTime);
            double averageRating = totalBMS / totalObservedTime * 100;


            var studyVersion = studyVersionRepo.GetItems()
                .FirstOrDefault(x => x.StudyId == Utilities.StudyId && x.Id == Utilities.StudyVersion);

            var destSheetStudyDetails = workbook.Worksheets.Create("Study Analysis");

            destSheetStudyDetails.Range[1, 1, 100, 10].CellStyle = worksheetStyle;

            destSheetStudyDetails.Range[2, 1].Text = "Study Date";
            destSheetStudyDetails.Range[3, 1].Text = "Start Time";
            destSheetStudyDetails.Range[4, 1].Text = "Finish Timer";
            destSheetStudyDetails.Range[5, 1].Text = "Elapsed Time";
            destSheetStudyDetails.Range[6, 1].Text = "Average Rating";

            destSheetStudyDetails.Range["A2:A6"].CellStyle = frequencyStyle;

            TimeSpan studyElapsedTime = studyVersion.TimeStudyFinished.Subtract(studyVersion.TimeStudyStarted);

            destSheetStudyDetails.Range[2, 2].Text = studyVersion.TimeStudyStarted.ToString("dd/MM/yyyy");
            destSheetStudyDetails.Range[3, 2].Text = studyVersion.TimeStudyStarted.ToLongTimeString();
            destSheetStudyDetails.Range[4, 2].Text = studyVersion.TimeStudyFinished.ToLongTimeString();
            destSheetStudyDetails.Range[5, 2].TimeSpan = studyElapsedTime;
            destSheetStudyDetails.Range[6, 2].Number = averageRating;
            destSheetStudyDetails.Range[6, 2].NumberFormat = "###0.00";

            startRowIndex = 10;

            //get all standard rated non forerign laps
            var allLapTimes = lapTimeRepo.GetItems()
                .Where(x => x.StudyId == Utilities.StudyId
                && x.Version == Utilities.StudyVersion
                && x.Status == RunningStatus.Completed
                && x.IsRated
                && x.IsValueAdded).ToList();


            var summary = allLapTimes.GroupBy(a => new { a.ActivityId, a.Element })
                                   .Select(g => new LapTimeSummary
                                   {
                                       ActivityId = g.Key.ActivityId,
                                       Element = g.Key.Element,
                                       NumberOfObservations = g.Count(),
                                       LapTimeTotal = g.Sum(a => a.IndividualLapBMS)

                                   }).ToList();

            summaryCount = summaryCount + 5 + summary.Count();

            destSheetStudyDetails.Range[startRowIndex + 2, 1].Text = "Element Number";
            destSheetStudyDetails.Range[startRowIndex + 2, 2].Text = "Description";
            destSheetStudyDetails.Range[startRowIndex + 2, 3].Text = "Total BMS";
            destSheetStudyDetails.Range[startRowIndex + 2, 4].Text = "Observed Occassions";
            destSheetStudyDetails.Range[startRowIndex + 2, 5].Text = "BMS Per Obs Occ";
            destSheetStudyDetails.Range[startRowIndex + 2, 6].Text = "Frequency Req";
            destSheetStudyDetails.Range[startRowIndex + 2, 7].Text = "BMS by Freq";
            destSheetStudyDetails.Range[startRowIndex + 2, 8].Text = "CA. 3%";
            destSheetStudyDetails.Range[startRowIndex + 2, 9].Text = "RA. 12%";
            destSheetStudyDetails.Range[startRowIndex + 2, 10].Text = "Element Standard Mins";


            destSheetStudyDetails.Range[startRowIndex + 4, 1].Text = "Standard Elements";
            destSheetStudyDetails.Range[startRowIndex + 4, 1].CellStyle = frequencyStyle;

            var totalCount = 0;

            foreach (var item in summary)
            {

                var bmsPerOccassion = item.LapTimeTotal / item.NumberOfObservations;
                var caAllowance = (bmsPerOccassion * 0.03) + bmsPerOccassion;
                var raAllowance = (caAllowance * 0.12) + caAllowance;

                destSheetStudyDetails.Range[startRowIndex + 6 + totalCount, 1].Number = item.ActivityId;
                destSheetStudyDetails.Range[startRowIndex + 6 + totalCount, 2].Text = item.Element;
                destSheetStudyDetails.Range[startRowIndex + 6 + totalCount, 3].Number = item.LapTimeTotal;
                destSheetStudyDetails.Range[startRowIndex + 6 + totalCount, 4].Number = item.NumberOfObservations;
                destSheetStudyDetails.Range[startRowIndex + 6 + totalCount, 5].Number = bmsPerOccassion; ;

                destSheetStudyDetails.Range[startRowIndex + 6 + totalCount, 6].CellStyle = frequencyStyle;

                var columnAddress1 = destSheetStudyDetails.Range[startRowIndex + 6 + totalCount, 5].AddressLocal;
                var columnAddress2 = destSheetStudyDetails.Range[startRowIndex + 6 + totalCount, 6].AddressLocal;
                var columnAddress3 = destSheetStudyDetails.Range[startRowIndex + 6 + totalCount, 8].AddressLocal;

                var formula1 = $"={columnAddress1}*{columnAddress2}";
                var formula2 = $"=({columnAddress1}* 0.03) + {columnAddress1}*{columnAddress2}";
                var formula3 = $"=({columnAddress3} * 0.12) + {columnAddress3}";

                destSheetStudyDetails.Range[startRowIndex + 6 + totalCount, 7].Formula = formula1;
                destSheetStudyDetails.Range[startRowIndex + 6 + totalCount, 8].Formula = formula2; //caAllowance;
                destSheetStudyDetails.Range[startRowIndex + 6 + totalCount, 9].Formula = formula3;  //raAllowance;
                destSheetStudyDetails.Range[startRowIndex + 6 + totalCount, 10].Formula = formula3;  //raAllowance;

                totalCount = totalCount + 2;
            }

            destSheetStudyDetails.Range[startRowIndex + 6 + totalCount, 1].Text = "Occassional Elements";
            destSheetStudyDetails.Range[startRowIndex + 6 + totalCount, 1].CellStyle = frequencyStyle;

            summaryCount = summaryCount + 5 + summary.Count();

            allLapTimes = lapTimeRepo.GetItems()
               .Where(x => x.StudyId == Utilities.StudyId
               && x.Version == Utilities.StudyVersion
               && x.Status == RunningStatus.Completed
               && x.IsRated
               && !x.IsValueAdded).ToList();

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

                destSheetStudyDetails.Range[startRowIndex + 8 + totalCount, 1].Number = item.ActivityId;
                destSheetStudyDetails.Range[startRowIndex + 8 + totalCount, 2].Text = item.Element;
                destSheetStudyDetails.Range[startRowIndex + 8 + totalCount, 3].Number = item.LapTimeTotal;
                destSheetStudyDetails.Range[startRowIndex + 8 + totalCount, 4].Number = item.NumberOfObservations;
                destSheetStudyDetails.Range[startRowIndex + 8 + totalCount, 5].Number = bmsPerOccassion;
                destSheetStudyDetails.Range[startRowIndex + 8 + totalCount, 6].CellStyle = frequencyStyle;

                var columnAddress1 = destSheetStudyDetails.Range[startRowIndex + 8 + totalCount, 5].AddressLocal;
                var columnAddress2 = destSheetStudyDetails.Range[startRowIndex + 8 + totalCount, 6].AddressLocal;
                var columnAddress3 = destSheetStudyDetails.Range[startRowIndex + 8 + totalCount, 8].AddressLocal;

                var formula1 = $"={columnAddress1}*{columnAddress2}";
                var formula2 = $"=({columnAddress1}* 0.03) + {columnAddress1}*{columnAddress2}";
                var formula3 = $"=({columnAddress3} * 0.12) + {columnAddress3}";

                destSheetStudyDetails.Range[startRowIndex + 8 + totalCount, 7].Formula = formula1;
                destSheetStudyDetails.Range[startRowIndex + 8 + totalCount, 8].Formula = formula2; //caAllowance;
                destSheetStudyDetails.Range[startRowIndex + 8 + totalCount, 9].Formula = formula3;  //raAllowance;
                destSheetStudyDetails.Range[startRowIndex + 8 + totalCount, 10].Formula = formula3;  //raAllowance;

                totalCount = totalCount + 2;
            }

            summaryCount = summaryCount + 5 + summary.Count();

            destSheetStudyDetails.Range[startRowIndex + 8 + totalCount, 1].Text = "Ineffective Elements";
            destSheetStudyDetails.Range[startRowIndex + 8 + totalCount, 1].CellStyle = frequencyStyle;

            allLapTimes = lapTimeRepo.GetItems()
                .Where(x => x.StudyId == Utilities.StudyId
                && x.Version == Utilities.StudyVersion
                && x.Status == RunningStatus.Completed
                && !x.IsRated
                && !x.IsValueAdded).ToList();

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

                destSheetStudyDetails.Range[startRowIndex + 10 + totalCount, 1].Number = item.ActivityId;
                destSheetStudyDetails.Range[startRowIndex + 10 + totalCount, 2].Text = item.Element;
                destSheetStudyDetails.Range[startRowIndex + 10 + totalCount, 3].Number = item.LapTimeTotal;
                destSheetStudyDetails.Range[startRowIndex + 10 + totalCount, 4].Number = item.NumberOfObservations;
                destSheetStudyDetails.Range[startRowIndex + 10 + totalCount, 5].Number = bmsPerOccassion;

                destSheetStudyDetails.Range[startRowIndex + 10 + totalCount, 6].CellStyle = frequencyStyle;

                var columnAddress1 = destSheetStudyDetails.Range[startRowIndex + 10 + totalCount, 5].AddressLocal;
                var columnAddress2 = destSheetStudyDetails.Range[startRowIndex + 10 + totalCount, 6].AddressLocal;
                var columnAddress3 = destSheetStudyDetails.Range[startRowIndex + 10 + totalCount, 8].AddressLocal;

                var formula1 = $"={columnAddress1}*{columnAddress2}";
                var formula2 = $"=({columnAddress1}* 0.03) + {columnAddress1}*{columnAddress2}";
                var formula3 = $"=({columnAddress3} * 0.12) + {columnAddress3}";

                destSheetStudyDetails.Range[startRowIndex + 12 + totalCount, 7].Formula = formula1;
                destSheetStudyDetails.Range[startRowIndex + 12 + totalCount, 8].Formula = formula2; //caAllowance;
                destSheetStudyDetails.Range[startRowIndex + 12 + totalCount, 9].Formula = formula3;  //raAllowance;
                destSheetStudyDetails.Range[startRowIndex + 12 + totalCount, 10].Formula = formula3;  //raAllowance;

                totalCount = totalCount + 2;
            }

            destSheetStudyDetails.Range[startRowIndex + 12 + totalCount, 9].Text = "Total SMS";
            var formula4 = $"=SUM(J16:J{startRowIndex + totalCount + 5})";
            destSheetStudyDetails.Range[$"J{totalCount + startRowIndex + 12}"].Formula = formula4;
            destSheetStudyDetails.Range[$"J{totalCount + startRowIndex + 12}"].CellStyle = frequencyStyle;
            destSheetStudyDetails.Range[startRowIndex + 12 + totalCount, 9].CellStyle = frequencyStyle;

            summaryCount = summaryCount + 5 + summary.Count();

            destSheetStudyDetails.Range["A12:J12"].CellStyle = headerStyle;
            destSheetStudyDetails.Range[1, 1, summaryCount + 10, 11].AutofitColumns();
            destSheetStudyDetails.Range[1, 3, summaryCount + 10, 3].NumberFormat = "###0.000";
            destSheetStudyDetails.Range[1, 5, summaryCount + 10, 5].NumberFormat = "###0.000";
            destSheetStudyDetails.Range[1, 7, summaryCount + 10, 7].NumberFormat = "###0.000";
            destSheetStudyDetails.Range[1, 8, summaryCount + 10, 8].NumberFormat = "###0.000";
            destSheetStudyDetails.Range[1, 9, summaryCount + 10, 9].NumberFormat = "###0.000";
            destSheetStudyDetails.Range[1, 10, summaryCount + 10, 10].NumberFormat = "###0.000";

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
                    IsForeignElement = !lap.IsValueAdded,
                    Element = lap.Element,
                    Rating = lap.Rating,
                    ElementId = lap.ActivityId,
                    IndividualLapTimeNormalised = lap.IndividualLapBMS
                });
            }

            var destSheet = workbook.Worksheets.Create("Complete Study Details");
            //destSheet.Range[1, 1, 200, 1000].CellStyle = worksheetStyle;

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
            destSheet.Range[1, 1, totalLaptimes + 10, 10].AutofitColumns();
            destSheet.Range[1, 4, totalLaptimes + 10, 4].NumberFormat = "###0.000";
            destSheet.Range[1, 5, totalLaptimes + 10, 5].NumberFormat = "###0.000";
            destSheet.Range[1, 8, totalLaptimes + 10, 8].NumberFormat = "###0.000";

            var formula4 = $"=SUM(D3:D{totalLaptimes + 3})";
            var formula5 = $"=SUM(E3:E{totalLaptimes + 3})";
            var formula6 = $"=SUM(H3:H{totalLaptimes + 3})";

            destSheet.Range[$"E{totalLaptimes + 4}"].Formula = formula5;
            destSheet.Range[$"H{totalLaptimes + 4}"].Formula = formula6;
        }

        //[TestMethod]
        //public void Move_LapTimes_To_Historic()
        //{

        //    var sqlCommand = "INSERT INTO LapTimeHistoric SELECT * FROM LapTime";
        //    lapTimeHistoricRepo.ExecuteSQLCommand(sqlCommand);
        //    sqlCommand = "DELETE FROM LapTime";
        //    lapTimeRepo.ExecuteSQLCommand(sqlCommand);
        //}

    }
}
