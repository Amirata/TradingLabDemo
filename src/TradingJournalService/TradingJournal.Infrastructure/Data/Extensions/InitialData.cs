// namespace TradingJournal.Infrastructure.Data.Extensions;
//
// internal static class InitialData
// {
//     private static ICollection<TradingTechnic> TradingTechnics
//     {
//         get
//         {
//             var pictures = new List<string>();
//
//             for (var i = 1; i <= 98; i++)
//             {
//                 pictures.Add($"pic{i}.jpg");
//             }
//
//             var tradingTechnics = new List<TradingTechnic>();
//             for (var i = 1; i <= 49; i++)
//             {
//                 var tradingTechnic = TradingTechnic.Create(
//                     TradingTechnicId.New(),
//                     $"Technic name {i}",
//                     $"Technic description {i}"
//                 );
//
//                 var pics = pictures.Skip(2 * (i - 1)).Take(2).ToList();
//
//                 tradingTechnic.AddImage(pics[0]);
//                 tradingTechnic.AddImage(pics[1]);
//
//                 tradingTechnics.Add(tradingTechnic);
//             }
//
//             return tradingTechnics;
//         }
//     }
//
//     public static IEnumerable<TradingPlan> TradingPlans
//     {
//         get
//         {
//             var values = Enum.GetValues(typeof(DayOfWeek));
//             var random = new Random();
//             var randomDayOfWeek = (DayOfWeek)values.GetValue(random.Next(values.Length))!;
//             var tradingTechnics = TradingTechnics;
//
//             var tradingPlans = new List<TradingPlan>();
//             for (var i = 1; i <= 7; i++)
//             {
//                 var tradingPlan = TradingPlan.Create(
//                     TradingPlanId.New(),
//                     $"Plan name {i}",
//                     new TimeOnly(00, 00, 00),
//                     new TimeOnly(01, 00, 00),
//                     [randomDayOfWeek.ToString()]
//                 );
//
//                 IList<TradingTechnic> technics = tradingTechnics.Skip(7 * (i - 1)).Take(7).ToList();
//
//                 tradingPlan.AddTechnic(technics[0]);
//                 tradingPlan.AddTechnic(technics[1]);
//                 tradingPlan.AddTechnic(technics[2]);
//                 tradingPlan.AddTechnic(technics[3]);
//                 tradingPlan.AddTechnic(technics[4]);
//                 tradingPlan.AddTechnic(technics[5]);
//                 tradingPlan.AddTechnic(technics[6]);
//
//                 tradingPlans.Add(tradingPlan);
//             }
//
//             return tradingPlans;
//         }
//     }
// }