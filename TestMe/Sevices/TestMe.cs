using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Sevices.Interfaces;

namespace TestMe.Sevices
{
    public class TestMe : ITestingPlatform
    {
        public ITestManager TestManager { get; }
        public ITestQuestionManager TestQuestionManager { get; }
        public ITestAnswerManager TestAnswerManager { get; }
        public ITestResultManager TestResultManager { get; }
        public IRandomStringGenerator RandomStringGenerator { get; }
        public IAnswerImageManager AnswerImageManager { get; }
        public ITestReportManager TestReportManager { get; }
        public ITestMarkManager TestMarkManager { get; }
        public TestMe(ITestManager testManager,
            ITestQuestionManager testQuestionManager,
            ITestAnswerManager testAnswerManager,
            ITestResultManager testResultManager, 
            IRandomStringGenerator randomStringGenerator,
            IAnswerImageManager answerImageManager,
            ITestReportManager testReportManager,
            ITestReportManager testReportManage)
        {
            TestManager = testManager;
            TestQuestionManager = testQuestionManager;
            TestAnswerManager = testAnswerManager;
            TestResultManager = testResultManager;
            RandomStringGenerator = randomStringGenerator;
            AnswerImageManager = answerImageManager;
            TestReportManager = testReportManager;
            TestReportManager = testReportManage;
        }
    }
}
