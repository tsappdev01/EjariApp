//using DIP.MakaniNumber;
//using DotLiquid.Util;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using Quartz;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Volo.Abp;
//using Volo.Abp.BackgroundWorkers.Quartz;
//using Volo.Abp.Json;
//using Volo.Abp.ObjectMapping;
//using Volo.Abp.Settings;
//using static DIP.Permissions.DIPPermissions;
//using static DotLiquid.Variable;

//namespace DIP.Commercials
//{
//    //[DisallowConcurrentExecution]
//    public class UpdateMakaniNumberJob : QuartzBackgroundWorkerBase
//    {
      
//        public IMakaniNumberAppService MakaniNumberAppService { get; set; }
//        public ICommercialsAppService CommercialsAppService { get; set; }
//        private readonly IObjectMapper _objectMapper;

//        public UpdateMakaniNumberJob(
//           IMakaniNumberAppService makaniNumberAppService,
//            ICommercialsAppService commercialsAppService,
//            IObjectMapper objectMapper
//            )
//        {
//            JobDetail = JobBuilder.Create<UpdateMakaniNumberJob>().WithIdentity(nameof(UpdateMakaniNumberJob)).Build();

//            Trigger = TriggerBuilder.Create().WithIdentity(nameof(UpdateMakaniNumberJob))
//                //.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(00, 00))
//                .WithSimpleSchedule(s => s.WithIntervalInHours(1).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires()).StartNow()
//                .Build();

//            ScheduleJob = async scheduler =>
//            {
//                if (!await scheduler.CheckExists(JobDetail.Key))
//                {
//                    await scheduler.ScheduleJob(JobDetail, Trigger);
//                }
//            };

//            MakaniNumberAppService = makaniNumberAppService;
//            CommercialsAppService = commercialsAppService;
//            _objectMapper = objectMapper;
//        }

//        public override async Task Execute(IJobExecutionContext context)
//        {        



//            try
//            {
//                Logger.LogInformation($"Update Location Start {nameof(UpdateMakaniNumberJob)}");
//                CommercialUpdateDto editingCommercial;
//                GetCommercialsInput Filter = new GetCommercialsInput();
//                Filter.MaxResultCount = 1000;
//                Filter.SkipCount = 0;

//                var result = await CommercialsAppService.GetListWithPlotNoAsync(Filter);
//              if (result.Count() > 0)
//                {
//                    Logger.LogInformation($"Update Location Count {result.Count()}");
//                    foreach (var item in result)
//                    {
//                        Logger.LogInformation($"CALL API START");

//                        MakaniNumberPropertyDto makaniNumberPropertyDto = await MakaniNumberAppService.GetMakaniNumberAsync(item.Commercial.PlotNo);
//                        Logger.LogInformation($"CALL API END RESUTL" + JsonConvert.SerializeObject(makaniNumberPropertyDto));
//                        if (makaniNumberPropertyDto != null && makaniNumberPropertyDto.MAKANI != null && makaniNumberPropertyDto.MAKANI.Count > 0 && !makaniNumberPropertyDto.MAKANI[0].Makani.IsNullOrWhiteSpace())
//                        {
//                            Logger.LogInformation($"Update Location new makani {makaniNumberPropertyDto.MAKANI[0].Makani.Replace(" ", "")}");
//                            // editingCommercial.MakaniNo = makaniNumberPropertyDto.MAKANI[0].Makani.Replace(" ", "");
//                            await CommercialsAppService.UpdateMakaniAsync(item.Commercial.Id, makaniNumberPropertyDto.MAKANI[0].Makani.Replace(" ", ""));
//                            Logger.LogInformation($"Update Location END new makani {makaniNumberPropertyDto.MAKANI[0].Makani.Replace(" ", "")}");
//                        }
                      
//                        await Task.Delay(5000);
//                    }
//                }
//                Logger.LogInformation($"Update Location End {nameof(UpdateMakaniNumberJob)}");

//            }
//            catch (Exception ex)
//            {
//                //  _dmDataCashAppService.ClearDmBulkInProgress();
//                Logger.LogError(ex, $"Update Location Error {nameof(UpdateMakaniNumberJob)}");
//            }
//        }
//    }
//}
