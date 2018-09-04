using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Data;
using TestMe.Data.Interfaces;
using TestMe.Data.Repositories;
using TestMe.Models;
using TestMe.Sevices.Interfaces;

namespace TestMe.Sevices.Extentions
{
    public static class TestingPlatformExtention
    {
        public static IServiceCollection AddTestMe(this IServiceCollection service) =>
            service.AddFreelancingPlatformDbContext()
                .AddRepositories().AddModelManagers()
                .AddRandomStringGenerator()
                .AddAnswerImageManager()
                .AddScoped<ITestingPlatform, TestMe>();

        #region Managers

        private static IServiceCollection AddModelManagers(this IServiceCollection service) =>
            service.AddTestManager().AddTestQuestionManager()
            .AddTestAnswerManager().AddTestResultManager()
            .AddTestReportManager();

        private static IServiceCollection AddTestManager(this IServiceCollection service) =>
            service.AddScoped<ITestManager, TestManager>();

        private static IServiceCollection AddTestQuestionManager(this IServiceCollection service) =>
            service.AddScoped<ITestQuestionManager, TestQuestionManager>();

        private static IServiceCollection AddTestAnswerManager(this IServiceCollection service) =>
            service.AddScoped<ITestAnswerManager, TestAnswerManager>();

        private static IServiceCollection AddTestResultManager(this IServiceCollection service) =>
            service.AddScoped<ITestResultManager, TestResultManager>();
        private static IServiceCollection AddTestReportManager(this IServiceCollection service) =>
            service.AddScoped<ITestReportManager, TestReportManager>();

        #endregion

        #region Repositories

        private static IServiceCollection AddRepositories(this IServiceCollection service) =>
            service.AddTestRepository().AddTestAnswerRepository()
                .AddTestQuestionRepository().AddTestResultRepository()
            .AddTestReportRepository();

        private static IServiceCollection AddTestRepository(this IServiceCollection service) =>
            service.AddScoped<IRepository<Test>, TestRepository>();

        private static IServiceCollection AddTestAnswerRepository(this IServiceCollection service) =>
            service.AddScoped<IRepository<TestAnswer>, TestAnswerRepository>();

        private static IServiceCollection AddTestQuestionRepository(this IServiceCollection service) =>
            service.AddScoped<IRepository<TestQuestion>, TestQuestionRepository>();

        private static IServiceCollection AddTestResultRepository(this IServiceCollection service) =>
            service.AddScoped<IRepository<TestResult>, TestResultRepository>();
        private static IServiceCollection AddTestReportRepository(this IServiceCollection service) =>
            service.AddScoped<IRepository<TestReport>, TestReportRepository>();
        #endregion

        private static IServiceCollection AddRandomStringGenerator(this IServiceCollection service) =>
            service.AddSingleton<IRandomStringGenerator, RandomStringGenerator>();
        private static IServiceCollection AddAnswerImageManager(this IServiceCollection service) =>
            service.AddScoped<IAnswerImageManager, AnswerImageManager>();

        private static IServiceCollection AddFreelancingPlatformDbContext(this IServiceCollection service) =>
            service.AddScoped<ITestingPlatformDbContext, ApplicationDbContext>();
    }
}
